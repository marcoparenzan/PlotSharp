using System.Globalization;
using System.Text;

namespace PlotSharp;

/// <summary>
/// Renderer SVG line/scatter estratto dallo snapshot NavierStokes.Core/Charts.cs.
/// Uso sperimentale interno: provenienza e vincoli di distribuzione in ARCHITECTURE.md.
/// </summary>
public static partial class Charts
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

    public sealed record Series(string Name, double[] X, double[] Y, string Color, bool Markers = false, bool Line = true, bool Dashed = false, bool Stems = false, double? BarWidth = null);

    public sealed class LineChartOptions
    {
        public string Title { get; init; } = "";
        public string Subtitle { get; init; } = "";
        public string XLabel { get; init; } = "";
        /// <summary>Mixed into the generated element ids. Ids are derived from the chart, so charts with the same geometry get the same ids; give each chart that shares one SVG or HTML page a different seed to keep ids unique there (IndexedCharts.Panels does).</summary>
        public string IdSeed { get; init; } = "";
        public string YLabel { get; init; } = "";
        /// <summary>Allow NaN Y samples as line gaps; X, infinity, stems and bars remain strict. Each series needs a finite Y.</summary>
        public bool AllowLineGaps { get; init; }
        /// <summary>Emit native SVG title tooltips at finite samples.</summary>
        public bool PointTooltips { get; init; }
        /// <summary>Draw full sample markers/tooltips whose centers are within the closed axis domain, outside the geometry clip. Out-of-domain centers are omitted. Default preserves legacy clipping.</summary>
        public bool PreserveBoundaryMarkers { get; init; }
        /// <summary>Reserve a separate legend row below title/subtitle when there are multiple series. Default retains legacy layout.</summary>
        public bool SeparateLegendRow { get; init; }
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
        ArgumentNullException.ThrowIfNull(series);
        ArgumentNullException.ThrowIfNull(o);
        if (o.Title is null || o.Subtitle is null || o.XLabel is null || o.YLabel is null || o.Highlight is { Label: null })
            throw new ArgumentException("Text cannot be null.", nameof(o));
        if (o.Width < 200 || o.Height < 160 || o.Width > 10000 || o.Height > 10000)
            throw new ArgumentOutOfRangeException(nameof(o));
        if (series.Count == 0) throw new ArgumentException("At least one series is required.", nameof(series));
        foreach (var s in series)
        {
            if (s is null || s.X is null || s.Y is null || s.X.Length == 0 || s.X.Length != s.Y.Length)
                throw new ArgumentException("Series must contain paired coordinates.", nameof(series));
            if (s.X.Any(v => !double.IsFinite(v) || Math.Abs(v) > 1e12)
                || s.Y.Any(v => (!double.IsFinite(v) && !(o.AllowLineGaps && s.Line && !s.Stems && s.BarWidth is null && double.IsNaN(v))) || (double.IsFinite(v) && Math.Abs(v) > 1e12))
                || !s.Y.Any(double.IsFinite))
                throw new ArgumentException("Coordinates must be within ±1e12; NaN Y is allowed only as opted-in line gaps with at least one finite sample per series.", nameof(series));
            if (s.BarWidth is { } width && (!double.IsFinite(width) || width <= 0 || width > 1e12))
                throw new ArgumentException("Bar width must be positive and finite.", nameof(series));
            if (s.Name is null) throw new ArgumentException("Series names cannot be null.", nameof(series));
            if (s.Color is null || !System.Text.RegularExpressions.Regex.IsMatch(s.Color, "^#[0-9a-fA-F]{6}$"))
                throw new ArgumentException("Colors must use #RRGGBB.", nameof(series));
        }
        foreach (var bound in new[] { o.XMin, o.XMax, o.YMin, o.YMax })
            if (bound is { } v && (!double.IsFinite(v) || Math.Abs(v) > 1e12)) throw new ArgumentOutOfRangeException(nameof(o));
        if (o.Highlight is { } highlight && (!double.IsFinite(highlight.X) || !double.IsFinite(highlight.Y) || Math.Abs(highlight.X) > 1e12 || Math.Abs(highlight.Y) > 1e12))
            throw new ArgumentOutOfRangeException(nameof(o));
        int top = o.Subtitle.Length > 0 ? 66 : 48, left = 64, right = 24, bottom = 50;
        if (o.SeparateLegendRow && series.Count > 1) top += 18;
        int pw = o.Width - left - right, ph = o.Height - top - bottom;
        double xmin = o.XMin ?? series.SelectMany(s => s.X).Min(), xmax = o.XMax ?? series.SelectMany(s => s.X).Max();
        double ymin = o.YMin ?? series.SelectMany(s => s.Y).Where(double.IsFinite).Min(), ymax = o.YMax ?? series.SelectMany(s => s.Y).Where(double.IsFinite).Max();
        if (o.XMin is null) xmin = series.Min(s => s.X.Min() - (s.BarWidth ?? 0) / 2);
        if (o.XMax is null) xmax = series.Max(s => s.X.Max() + (s.BarWidth ?? 0) / 2);
        if (series.Any(s => s.Stems || s.BarWidth is not null))
        {
            if (o.YMin is null) ymin = Math.Min(ymin, 0);
            if (o.YMax is null) ymax = Math.Max(ymax, 0);
        }
        if (xmin > xmax || ymin > ymax) throw new ArgumentException("Axis limits are reversed.", nameof(o));
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

        var pointOverlays = new StringBuilder();
        bool CenterInDomain(double x, double y) => x >= xmin && x <= xmax && y >= ymin && y <= ymax;
        string cid = SvgId.For("clip", $"{o.IdSeed}|{left}|{top}|{pw}|{ph}");
        sb.Append($"<clipPath id='{cid}'><rect x='{left}' y='{top}' width='{pw}' height='{ph}'/></clipPath><g clip-path='url(#{cid})'>");
        foreach (var s in series)
        {
            if (s.Stems || s.BarWidth is not null)
                for (int k = 0; k < s.X.Length; k++)
                {
                    if (s.BarWidth is { } barWidth)
                        sb.Append($"<rect x='{F(X(s.X[k] - barWidth / 2))}' y='{F(Math.Min(Y(0), Y(s.Y[k])))}' width='{F(barWidth / (xmax - xmin) * pw)}' height='{F(Math.Abs(Y(s.Y[k]) - Y(0)))}' fill='{s.Color}'><title>{Esc(s.Name)}: {Num(s.Y[k])}</title></rect>");
                    else
                        sb.Append($"<line x1='{F(X(s.X[k]))}' x2='{F(X(s.X[k]))}' y1='{F(Y(0))}' y2='{F(Y(s.Y[k]))}' stroke='{s.Color}' stroke-width='2'/>");
                }
            if (s.Line)
            {
                var d = new StringBuilder();
                bool pen = false;
                for (int k = 0; k < s.X.Length; k++)
                {
                    if (!double.IsFinite(s.Y[k])) { pen = false; continue; }
                    d.Append(pen ? 'L' : 'M').Append($"{F(X(s.X[k]))} {F(Y(s.Y[k]))}");
                    pen = true;
                }
                sb.Append($"<path d='{d}' fill='none' stroke='{s.Color}' stroke-width='2'{(s.Dashed ? " stroke-dasharray='6 4'" : "")} stroke-linejoin='round' stroke-linecap='round'/>");
            }
            if (s.Markers)
                for (int k = 0; k < s.X.Length; k++)
                {
                    if (!double.IsFinite(s.Y[k])) continue;
                    if (o.PreserveBoundaryMarkers && !CenterInDomain(s.X[k], s.Y[k])) continue;
                    var target = o.PreserveBoundaryMarkers ? pointOverlays : sb;
                    target.Append($"<circle cx='{F(X(s.X[k]))}' cy='{F(Y(s.Y[k]))}' r='4.5' fill='{s.Color}' stroke='{Surface}' stroke-width='2'/>");
                }
            if (o.PointTooltips)
                for (int k = 0; k < s.X.Length; k++)
                {
                    if (!double.IsFinite(s.Y[k])) continue;
                    if (o.PreserveBoundaryMarkers && !CenterInDomain(s.X[k], s.Y[k])) continue;
                    var target = o.PreserveBoundaryMarkers ? pointOverlays : sb;
                    target.Append($"<circle cx='{F(X(s.X[k]))}' cy='{F(Y(s.Y[k]))}' r='6' fill='transparent'><title>{Esc(s.Name)} · x={Num(s.X[k])}: {Num(s.Y[k])}</title></circle>");
                }
        }
        if (o.Highlight is { } h)
        {
            sb.Append($"<line x1='{F(X(h.X))}' x2='{F(X(h.X))}' y1='{top}' y2='{top + ph}' stroke='{InkMuted}' stroke-dasharray='3 3'/>");
            sb.Append($"<circle cx='{F(X(h.X))}' cy='{F(Y(h.Y))}' r='6' fill='{Series2}' stroke='{Surface}' stroke-width='2'/>");
            bool flip = X(h.X) > left + pw * 0.6;
            sb.Append($"<text x='{F(X(h.X) + (flip ? -10 : 10))}' y='{F(Y(h.Y) - 10)}' text-anchor='{(flip ? "end" : "start")}' font-size='12' font-weight='600' fill='{Ink}'>{Esc(h.Label)}</text>");
        }
        sb.Append("</g>");
        if (pointOverlays.Length > 0)
            sb.Append("<g class='point-overlays'>").Append(pointOverlays).Append("</g>");

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
                if (s.Line) sb.Append($"<line x1='{F(lx)}' x2='{F(lx + 16)}' y1='{F(ly)}' y2='{F(ly)}' stroke='{s.Color}' stroke-width='2'{(s.Dashed ? " stroke-dasharray='6 4'" : "")}/>");
                if (s.BarWidth is not null) sb.Append($"<rect x='{F(lx)}' y='{F(ly - 5)}' width='16' height='10' fill='{s.Color}'/>");
                else if (s.Stems && !s.Line) sb.Append($"<line x1='{F(lx + 8)}' x2='{F(lx + 8)}' y1='{F(ly - 6)}' y2='{F(ly + 6)}' stroke='{s.Color}' stroke-width='2'/>");
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
        if (!double.IsFinite(min) || !double.IsFinite(max) || Math.Abs(min) > 1e12 || Math.Abs(max) > 1e12 || min > max || target < 1 || target > 100)
            throw new ArgumentOutOfRangeException(nameof(min));
        if (max - min < Math.Max(1e-12, Math.Abs(min) * 1e-12)) { max = min + 1; }
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
