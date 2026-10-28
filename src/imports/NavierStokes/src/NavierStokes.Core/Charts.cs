using System.Globalization;
using System.IO.Compression;
using System.Text;

namespace NavierStokes.Core;

/// <summary>
/// Grafici in SVG scritti a mano, senza librerie: heatmap (PNG incorporato), linee di flusso
/// (curve di livello con marching squares), frecce, grafici a linee. Data science in C#, a mani nude.
/// </summary>
public static class Charts
{
    // Palette (tema chiaro)
    public const string Surface = "#fcfcfb";
    public const string Ink = "#0b0b0b";
    public const string InkSecondary = "#52514e";
    public const string InkMuted = "#8a8984";
    public const string Grid = "#e4e3de";
    public const string Series1 = "#2a78d6";
    public const string Series2 = "#eb6834";

    private static readonly CultureInfo Inv = CultureInfo.InvariantCulture;
    private static string F(double v) => v.ToString("0.##", Inv);

    // ───────────────────────── mappe di colore ─────────────────────────

    public sealed class ColorMap
    {
        private readonly (double t, byte r, byte g, byte b)[] _stops;
        private ColorMap(params string[] hex)
        {
            _stops = hex.Select((h, k) => ((double)k / (hex.Length - 1),
                Convert.ToByte(h.Substring(1, 2), 16), Convert.ToByte(h.Substring(3, 2), 16), Convert.ToByte(h.Substring(5, 2), 16))).ToArray();
        }

        /// <summary>Sequenziale: un solo colore, da chiaro (poco) a scuro (tanto).</summary>
        public static ColorMap Sequential { get; } = new("#f4f8fd", "#cde2fb", "#9ec5f4", "#6da7ec", "#3987e5", "#256abf", "#184f95", "#0d366b");

        /// <summary>Divergente: blu (negativo) – grigio (zero) – rosso (positivo).</summary>
        public static ColorMap Diverging { get; } = new("#0d366b", "#256abf", "#6da7ec", "#b7d3f6", "#f0efec", "#f3b9b3", "#e66767", "#c23b3a", "#7f1f1f");

        public (byte r, byte g, byte b) At(double t)
        {
            t = Math.Clamp(double.IsNaN(t) ? 0 : t, 0, 1);
            for (int k = 1; k < _stops.Length; k++)
                if (t <= _stops[k].t)
                {
                    var a = _stops[k - 1]; var b = _stops[k];
                    double w = (t - a.t) / (b.t - a.t);
                    return ((byte)(a.r + w * (b.r - a.r)), (byte)(a.g + w * (b.g - a.g)), (byte)(a.b + w * (b.b - a.b)));
                }
            var l = _stops[^1];
            return (l.r, l.g, l.b);
        }

        public string Hex(double t) { var (r, g, b) = At(t); return $"#{r:x2}{g:x2}{b:x2}"; }
    }

    // ───────────────────────── PNG minimale ─────────────────────────

    /// <summary>Campo [j, i] (j = 0 in basso) → PNG RGB ingrandito con interpolazione bilineare.</summary>
    public static byte[] FieldToPng(double[,] field, ColorMap map, double vmin, double vmax, int width, int height,
                                    Func<double, double, bool>? mask = null)
    {
        int ny = field.GetLength(0), nx = field.GetLength(1);
        var raw = new byte[height * (1 + width * 3)];
        int p = 0;
        for (int py = 0; py < height; py++)
        {
            raw[p++] = 0; // filtro PNG "none"
            double fy = (1 - (py + 0.5) / height) * (ny - 1);
            int j0 = Math.Min((int)fy, ny - 2); double wy = fy - j0;
            for (int px = 0; px < width; px++)
            {
                double fx = (px + 0.5) / width * (nx - 1);
                int i0 = Math.Min((int)fx, nx - 2); double wx = fx - i0;
                double v = (1 - wy) * ((1 - wx) * field[j0, i0] + wx * field[j0, i0 + 1])
                         + wy * ((1 - wx) * field[j0 + 1, i0] + wx * field[j0 + 1, i0 + 1]);
                var (r, g, b) = mask != null && mask(fx, fy) ? ((byte)0x52, (byte)0x51, (byte)0x4e) : map.At((v - vmin) / (vmax - vmin));
                raw[p++] = r; raw[p++] = g; raw[p++] = b;
            }
        }
        using var ms = new MemoryStream();
        ms.Write(new byte[] { 137, 80, 78, 71, 13, 10, 26, 10 });
        var ihdr = new byte[13];
        WriteBE(ihdr, 0, (uint)width); WriteBE(ihdr, 4, (uint)height);
        ihdr[8] = 8; ihdr[9] = 2; // 8 bit, RGB
        WriteChunk(ms, "IHDR", ihdr);
        using (var z = new MemoryStream())
        {
            using (var zs = new ZLibStream(z, CompressionLevel.Optimal, leaveOpen: true)) zs.Write(raw);
            WriteChunk(ms, "IDAT", z.ToArray());
        }
        WriteChunk(ms, "IEND", Array.Empty<byte>());
        return ms.ToArray();
    }

    private static void WriteBE(byte[] b, int o, uint v) { b[o] = (byte)(v >> 24); b[o + 1] = (byte)(v >> 16); b[o + 2] = (byte)(v >> 8); b[o + 3] = (byte)v; }

    private static void WriteChunk(Stream s, string type, byte[] data)
    {
        var len = new byte[4]; WriteBE(len, 0, (uint)data.Length); s.Write(len);
        var td = Encoding.ASCII.GetBytes(type).Concat(data).ToArray();
        s.Write(td);
        var crc = new byte[4]; WriteBE(crc, 0, Crc32(td)); s.Write(crc);
    }

    private static uint[]? _crcTable;
    private static uint Crc32(byte[] data)
    {
        _crcTable ??= Enumerable.Range(0, 256).Select(n =>
        {
            uint c = (uint)n;
            for (int k = 0; k < 8; k++) c = (c & 1) != 0 ? 0xEDB88320u ^ (c >> 1) : c >> 1;
            return c;
        }).ToArray();
        uint crc = 0xFFFFFFFFu;
        foreach (var b in data) crc = _crcTable[(crc ^ b) & 0xFF] ^ (crc >> 8);
        return crc ^ 0xFFFFFFFFu;
    }

    // ───────────────────────── mappe di campo ─────────────────────────

    public sealed class FieldPlotOptions
    {
        public string Title { get; init; } = "";
        public string Subtitle { get; init; } = "";
        public string ColorbarLabel { get; init; } = "";
        public ColorMap Map { get; init; } = ColorMap.Sequential;
        public double? Min { get; init; }
        public double? Max { get; init; }
        public bool Symmetric { get; init; }               // per mappe divergenti: centra lo zero
        public double ClipPercentile { get; init; } = 0;   // es. 0.02 per ignorare gli estremi
        public double[,]? Contours { get; init; }          // es. funzione di corrente → linee di flusso
        public int ContourLevels { get; init; } = 14;
        public (double[,] U, double[,] V)? Arrows { get; init; }
        public int ArrowStride { get; init; } = 6;
        public (double Cx, double Cy, double R)? Obstacle { get; init; } // in coordinate di griglia
        public int PlotWidth { get; init; } = 440;
    }

    public static string FieldPlot(double[,] field, FieldPlotOptions o)
    {
        int ny = field.GetLength(0), nx = field.GetLength(1);
        int pw = o.PlotWidth;
        int ph = (int)Math.Round(pw * (double)(ny - 1) / (nx - 1));
        int left = 16, top = o.Subtitle.Length > 0 ? 62 : 44, cbw = 14, right = 80;
        int width = left + pw + 20 + cbw + right, height = top + ph + 24;

        var values = new List<double>(nx * ny);
        foreach (var v in field) if (!double.IsNaN(v)) values.Add(v);
        values.Sort();
        double lo = o.Min ?? values[(int)(o.ClipPercentile * (values.Count - 1))];
        double hi = o.Max ?? values[(int)((1 - o.ClipPercentile) * (values.Count - 1))];
        if (o.Symmetric) { double m = Math.Max(Math.Abs(lo), Math.Abs(hi)); lo = -m; hi = m; }
        if (hi - lo < 1e-12) hi = lo + 1;

        Func<double, double, bool>? mask = o.Obstacle is { } ob
            ? (fx, fy) => (fx - ob.Cx) * (fx - ob.Cx) + (fy - ob.Cy) * (fy - ob.Cy) <= ob.R * ob.R
            : null;
        var png = FieldToPng(field, o.Map, lo, hi, pw * 2, ph * 2, mask);

        var sb = new StringBuilder();
        Header(sb, width, height, o.Title, o.Subtitle);
        sb.Append($"<image x='{left}' y='{top}' width='{pw}' height='{ph}' preserveAspectRatio='none' href='data:image/png;base64,{Convert.ToBase64String(png)}'/>");

        double sx = (double)pw / (nx - 1), sy = (double)ph / (ny - 1);
        string X(double i) => F(left + i * sx);
        string Y(double j) => F(top + ph - j * sy);

        if (o.Contours is { } psi)
        {
            double pmin = double.MaxValue, pmax = double.MinValue;
            foreach (var v in psi) { pmin = Math.Min(pmin, v); pmax = Math.Max(pmax, v); }
            var path = new StringBuilder();
            for (int k = 1; k <= o.ContourLevels; k++)
            {
                double level = pmin + (pmax - pmin) * k / (o.ContourLevels + 1);
                foreach (var (x1, y1, x2, y2) in MarchingSquares(psi, level))
                    path.Append($"M{X(x1)} {Y(y1)}L{X(x2)} {Y(y2)}");
            }
            sb.Append($"<path d='{path}' fill='none' stroke='#ffffff' stroke-opacity='0.9' stroke-width='1.2'/>");
        }

        if (o.Arrows is { } ar)
        {
            double vmax = 1e-12;
            for (int j = 0; j < ny; j++) for (int i = 0; i < nx; i++)
                vmax = Math.Max(vmax, Math.Sqrt(ar.U[j, i] * ar.U[j, i] + ar.V[j, i] * ar.V[j, i]));
            double len = o.ArrowStride * sx * 0.9;
            var path = new StringBuilder();
            for (int j = o.ArrowStride / 2; j < ny; j += o.ArrowStride)
            for (int i = o.ArrowStride / 2; i < nx; i += o.ArrowStride)
            {
                double u = ar.U[j, i] / vmax, v = ar.V[j, i] / vmax;
                double mag = Math.Sqrt(u * u + v * v);
                if (mag < 0.02) continue;
                double x0 = left + i * sx, y0 = top + ph - j * sy;
                double dx = u * len, dy = -v * len;
                double x1 = x0 + dx, y1 = y0 + dy;
                double a = Math.Atan2(dy, dx), h = Math.Max(2.5, 4 * Math.Sqrt(mag));
                path.Append($"M{F(x0)} {F(y0)}L{F(x1)} {F(y1)}");
                path.Append($"M{F(x1 - h * Math.Cos(a - 0.45))} {F(y1 - h * Math.Sin(a - 0.45))}L{F(x1)} {F(y1)}L{F(x1 - h * Math.Cos(a + 0.45))} {F(y1 - h * Math.Sin(a + 0.45))}");
            }
            sb.Append($"<path d='{path}' fill='none' stroke='{Ink}' stroke-opacity='0.75' stroke-width='1.1' stroke-linecap='round'/>");
        }

        sb.Append($"<rect x='{left}' y='{top}' width='{pw}' height='{ph}' fill='none' stroke='{InkMuted}' stroke-width='1'/>");

        // Barra dei colori
        int cbx = left + pw + 20;
        string gid = "cb" + Guid.NewGuid().ToString("N")[..8];
        sb.Append($"<defs><linearGradient id='{gid}' x1='0' y1='1' x2='0' y2='0'>");
        for (int k = 0; k <= 10; k++) sb.Append($"<stop offset='{k * 10}%' stop-color='{o.Map.Hex(k / 10.0)}'/>");
        sb.Append("</linearGradient></defs>");
        sb.Append($"<rect x='{cbx}' y='{top}' width='{cbw}' height='{ph}' fill='url(#{gid})' rx='2'/>");
        foreach (var t in new[] { 0.0, 0.5, 1.0 })
        {
            double val = lo + t * (hi - lo);
            sb.Append($"<text x='{cbx + cbw + 6}' y='{F(top + ph - t * ph + 4)}' font-size='11' fill='{InkSecondary}'>{Num(val)}</text>");
        }
        sb.Append($"<text transform='translate({cbx + cbw + 62},{top + ph / 2}) rotate(90)' text-anchor='middle' font-size='11' fill='{InkSecondary}'>{Esc(o.ColorbarLabel)}</text>");
        sb.Append("</svg>");
        return sb.ToString();
    }

    /// <summary>Curve di livello: per ogni cella della griglia, i segmenti dove il campo vale "level".</summary>
    public static IEnumerable<(double, double, double, double)> MarchingSquares(double[,] f, double level)
    {
        int ny = f.GetLength(0), nx = f.GetLength(1);
        for (int j = 0; j < ny - 1; j++)
        for (int i = 0; i < nx - 1; i++)
        {
            double a = f[j, i], b = f[j, i + 1], c = f[j + 1, i + 1], d = f[j + 1, i];
            var pts = new List<(double, double)>(4);
            void Edge(double v1, double v2, double x1, double y1, double x2, double y2)
            {
                if ((v1 < level) != (v2 < level))
                {
                    double t = (level - v1) / (v2 - v1);
                    pts.Add((x1 + t * (x2 - x1), y1 + t * (y2 - y1)));
                }
            }
            Edge(a, b, i, j, i + 1, j);
            Edge(b, c, i + 1, j, i + 1, j + 1);
            Edge(c, d, i + 1, j + 1, i, j + 1);
            Edge(d, a, i, j + 1, i, j);
            if (pts.Count >= 2) yield return (pts[0].Item1, pts[0].Item2, pts[1].Item1, pts[1].Item2);
            if (pts.Count == 4) yield return (pts[2].Item1, pts[2].Item2, pts[3].Item1, pts[3].Item2);
        }
    }

    // ───────────────────────── grafici a linee ─────────────────────────

    public sealed record Series(string Name, double[] X, double[] Y, string Color, bool Markers = false, bool Line = true);

    public sealed class LineChartOptions
    {
        public string Title { get; init; } = "";
        public string Subtitle { get; init; } = "";
        public string XLabel { get; init; } = "";
        public string YLabel { get; init; } = "";
        public int Width { get; init; } = 560;
        public int Height { get; init; } = 340;
        public double? XMin { get; init; }
        public double? XMax { get; init; }
        public double? YMin { get; init; }
        public double? YMax { get; init; }
        public (double X, double Y, string Label)? Highlight { get; init; }
    }

    public static string LineChart(IReadOnlyList<Series> series, LineChartOptions o)
    {
        int top = o.Subtitle.Length > 0 ? 66 : 48, left = 64, right = 24, bottom = 50;
        int pw = o.Width - left - right, ph = o.Height - top - bottom;
        double xmin = o.XMin ?? series.SelectMany(s => s.X).Min(), xmax = o.XMax ?? series.SelectMany(s => s.X).Max();
        double ymin = o.YMin ?? series.SelectMany(s => s.Y).Min(), ymax = o.YMax ?? series.SelectMany(s => s.Y).Max();
        var xt = NiceTicks(xmin, xmax); var yt = NiceTicks(ymin, ymax);
        if (o.XMin is null) xmin = Math.Min(xmin, xt[0]);
        if (o.XMax is null) xmax = Math.Max(xmax, xt[^1]);
        if (o.YMin is null) ymin = Math.Min(ymin, yt[0]);
        if (o.YMax is null) ymax = Math.Max(ymax, yt[^1]);
        if (xmax - xmin < 1e-12) xmax = xmin + 1;
        if (ymax - ymin < 1e-12) ymax = ymin + 1;
        double X(double v) => left + (v - xmin) / (xmax - xmin) * pw;
        double Y(double v) => top + ph - (v - ymin) / (ymax - ymin) * ph;

        var sb = new StringBuilder();
        Header(sb, o.Width, o.Height, o.Title, o.Subtitle);

        foreach (var t in yt.Where(t => t >= ymin - 1e-9 && t <= ymax + 1e-9))
        {
            sb.Append($"<line x1='{left}' x2='{left + pw}' y1='{F(Y(t))}' y2='{F(Y(t))}' stroke='{Grid}' stroke-width='1'/>");
            sb.Append($"<text x='{left - 8}' y='{F(Y(t) + 4)}' text-anchor='end' font-size='11' fill='{InkSecondary}'>{Num(t)}</text>");
        }
        foreach (var t in xt.Where(t => t >= xmin - 1e-9 && t <= xmax + 1e-9))
            sb.Append($"<text x='{F(X(t))}' y='{top + ph + 18}' text-anchor='middle' font-size='11' fill='{InkSecondary}'>{Num(t)}</text>");
        sb.Append($"<line x1='{left}' x2='{left + pw}' y1='{top + ph}' y2='{top + ph}' stroke='{InkMuted}' stroke-width='1'/>");
        sb.Append($"<text x='{left + pw / 2}' y='{o.Height - 10}' text-anchor='middle' font-size='12' fill='{InkSecondary}'>{Esc(o.XLabel)}</text>");
        sb.Append($"<text transform='translate(16,{top + ph / 2}) rotate(-90)' text-anchor='middle' font-size='12' fill='{InkSecondary}'>{Esc(o.YLabel)}</text>");

        string cid = "clip" + Guid.NewGuid().ToString("N")[..8];
        sb.Append($"<clipPath id='{cid}'><rect x='{left}' y='{top - 6}' width='{pw}' height='{ph + 12}'/></clipPath><g clip-path='url(#{cid})'>");
        foreach (var s in series)
        {
            if (s.Line)
            {
                var d = new StringBuilder();
                for (int k = 0; k < s.X.Length; k++) d.Append(k == 0 ? 'M' : 'L').Append($"{F(X(s.X[k]))} {F(Y(s.Y[k]))}");
                sb.Append($"<path d='{d}' fill='none' stroke='{s.Color}' stroke-width='2' stroke-linejoin='round' stroke-linecap='round'/>");
            }
            if (s.Markers)
                for (int k = 0; k < s.X.Length; k++)
                    sb.Append($"<circle cx='{F(X(s.X[k]))}' cy='{F(Y(s.Y[k]))}' r='4.5' fill='{s.Color}' stroke='{Surface}' stroke-width='2'/>");
        }
        if (o.Highlight is { } h)
        {
            sb.Append($"<line x1='{F(X(h.X))}' x2='{F(X(h.X))}' y1='{top}' y2='{top + ph}' stroke='{InkMuted}' stroke-dasharray='3 3'/>");
            sb.Append($"<circle cx='{F(X(h.X))}' cy='{F(Y(h.Y))}' r='6' fill='{Series2}' stroke='{Surface}' stroke-width='2'/>");
            bool flip = X(h.X) > left + pw * 0.6;
            sb.Append($"<text x='{F(X(h.X) + (flip ? -10 : 10))}' y='{F(Y(h.Y) - 10)}' text-anchor='{(flip ? "end" : "start")}' font-size='12' font-weight='600' fill='{Ink}'>{Esc(h.Label)}</text>");
        }
        sb.Append("</g>");

        // Legenda (solo con più serie)
        if (series.Count > 1)
        {
            double lx = left + pw;
            for (int k = series.Count - 1; k >= 0; k--)
            {
                var s = series[k];
                double w = 7 * s.Name.Length + 28;
                lx -= w;
                double ly = top - 14;
                if (s.Line) sb.Append($"<line x1='{F(lx)}' x2='{F(lx + 16)}' y1='{F(ly)}' y2='{F(ly)}' stroke='{s.Color}' stroke-width='2'/>");
                if (s.Markers) sb.Append($"<circle cx='{F(lx + 8)}' cy='{F(ly)}' r='4' fill='{s.Color}'/>");
                sb.Append($"<text x='{F(lx + 22)}' y='{F(ly + 4)}' font-size='11' fill='{InkSecondary}'>{Esc(s.Name)}</text>");
            }
        }
        sb.Append("</svg>");
        return sb.ToString();
    }

    // ───────────────────────── utilità ─────────────────────────

    private static void Header(StringBuilder sb, int w, int h, string title, string subtitle)
    {
        sb.Append($"<svg xmlns='http://www.w3.org/2000/svg' viewBox='0 0 {w} {h}' width='{w}' height='{h}' font-family='Segoe UI, Inter, system-ui, sans-serif'>");
        sb.Append($"<rect width='{w}' height='{h}' fill='{Surface}'/>");
        sb.Append($"<text x='16' y='26' font-size='15' font-weight='600' fill='{Ink}'>{Esc(title)}</text>");
        if (subtitle.Length > 0) sb.Append($"<text x='16' y='45' font-size='12' fill='{InkSecondary}'>{Esc(subtitle)}</text>");
    }

    public static double[] NiceTicks(double min, double max, int target = 5)
    {
        if (max - min < 1e-12) { max = min + 1; }
        double raw = (max - min) / target;
        double mag = Math.Pow(10, Math.Floor(Math.Log10(raw)));
        double step = new[] { 1, 2, 2.5, 5, 10 }.Select(m => m * mag).First(s => s >= raw);
        double start = Math.Floor(min / step) * step, end = Math.Ceiling(max / step) * step;
        var ticks = new List<double>();
        for (double t = start; t <= end + step * 0.5; t += step) ticks.Add(Math.Abs(t) < step * 1e-9 ? 0 : t);
        return ticks.ToArray();
    }

    public static string Num(double v)
    {
        double a = Math.Abs(v);
        if (a == 0) return "0";
        if (a >= 1e5 || a < 1e-3) return v.ToString("0.0e0", Inv);
        if (a >= 100) return v.ToString("#,0", Inv);
        if (a >= 10) return v.ToString("0.#", Inv);
        return v.ToString("0.###", Inv);
    }

    private static string Esc(string s) => s.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;").Replace("'", "&#39;");
}
