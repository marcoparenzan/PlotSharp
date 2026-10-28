using System.Text;

namespace PlotSharp;

/// <summary>Node/raster field chart variant, deliberately separate from cell-based Heatmap.</summary>
public sealed class NodeFieldOptions
{
    public string Title { get; init; } = "";
    public string Subtitle { get; init; } = "";
    public string ColorbarLabel { get; init; } = "";
    public Func<double, (byte R, byte G, byte B)> NormalizedColor { get; init; }
        = t => ColorPalette.Sequential.Sample(Math.Clamp(double.IsNaN(t) ? 0 : t, 0, 1));
    public double? Min { get; init; }
    public double? Max { get; init; }
    public bool Symmetric { get; init; }
    public double ClipPercentile { get; init; }
    public double[,]? Contours { get; init; }
    public int ContourLevels { get; init; } = 14;
    public (double[,] U, double[,] V)? Arrows { get; init; }
    public int ArrowStride { get; init; } = 6;
    public (double Cx, double Cy, double R)? Obstacle { get; init; }
    public (byte R, byte G, byte B) MaskedColor { get; init; } = (0x52, 0x51, 0x4e);
    public int PlotWidth { get; init; } = 440;
}

public static partial class Charts
{
    public static string NodeFieldPlot(double[,] field, NodeFieldOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(field);
        var o = options ?? new();
        ArgumentNullException.ThrowIfNull(o.Title); ArgumentNullException.ThrowIfNull(o.Subtitle);
        ArgumentNullException.ThrowIfNull(o.ColorbarLabel); ArgumentNullException.ThrowIfNull(o.NormalizedColor);
        var layout = NodeFieldLayout.Create(field.GetLength(0), field.GetLength(1), o.PlotWidth, o.Subtitle.Length > 0);
        if (o.ContourLevels < 0 || o.ContourLevels > 32) throw new ArgumentOutOfRangeException(nameof(options), "Contour levels must be 0 to 32.");
        void MatchShape(double[,] layer)
        {
            if (layer is null || layer.GetLength(0) != layout.Rows || layer.GetLength(1) != layout.Columns)
                throw new ArgumentException("Overlay shape must match the node field.", nameof(options));
        }
        if (o.Contours is { } contour) MatchShape(contour);
        if (o.Arrows is { } vectors) { MatchShape(vectors.U); MatchShape(vectors.V); }
        if (o.Obstacle is { } obstacle && (!double.IsFinite(obstacle.Cx) || !double.IsFinite(obstacle.Cy) || !double.IsFinite(obstacle.R)
            || Math.Abs(obstacle.Cx) > 1e12 || Math.Abs(obstacle.Cy) > 1e12 || obstacle.R < 0 || obstacle.R > 1e12))
            throw new ArgumentException("Invalid circular obstacle.", nameof(options));
        var limits = FieldColorLimits.Resolve(field, o.Min, o.Max, o.Symmetric, o.ClipPercentile);
        Func<double, double, bool>? mask = o.Obstacle is { } ob
            ? (x, y) => (x - ob.Cx) * (x - ob.Cx) + (y - ob.Cy) * (y - ob.Cy) <= ob.R * ob.R : null;
        var rows = BilinearRgbSampler.SampleScanlines(field, layout.PlotWidth * 2, layout.PlotHeight * 2,
            value => o.NormalizedColor(limits.NormalizeLegacy(value)), mask, o.MaskedColor);
        var png = RgbPngEncoder.EncodeScanlines(layout.PlotWidth * 2, layout.PlotHeight * 2, rows);
        var svg = new StringBuilder();
        Header(svg, layout.Width, layout.Height, o.Title, o.Subtitle);
        svg.Append($"<image x='{layout.Left}' y='{layout.Top}' width='{layout.PlotWidth}' height='{layout.PlotHeight}' preserveAspectRatio='none' href='data:image/png;base64,{Convert.ToBase64String(png)}'/>");
        if (o.Contours is { } psi)
        {
            double min = double.MaxValue, max = double.MinValue;
            foreach (double value in psi) { min = Math.Min(min, value); max = Math.Max(max, value); }
            var path = new StringBuilder();
            for (int k = 1; k <= o.ContourLevels; k++)
            {
                double level = min + (max - min) * k / (o.ContourLevels + 1);
                foreach (var s in MarchingSquares(psi, level, null, preserveDegenerateSegments: true))
                    path.Append($"M{F(layout.X(s.X1))} {F(layout.Y(s.Y1))}L{F(layout.X(s.X2))} {F(layout.Y(s.Y2))}");
            }
            svg.Append($"<path d='{path}' fill='none' stroke='#ffffff' stroke-opacity='0.9' stroke-width='1.2'/>");
        }
        if (o.Arrows is { } ar)
        {
            string path = NodeVectorOverlay.PathData(ar.U, ar.V, layout.PlotWidth, layout.PlotHeight, o.ArrowStride, layout.Left, layout.Top);
            svg.Append($"<path d='{path}' fill='none' stroke='{Ink}' stroke-opacity='0.75' stroke-width='1.1' stroke-linecap='round'/>");
        }
        svg.Append($"<rect x='{layout.Left}' y='{layout.Top}' width='{layout.PlotWidth}' height='{layout.PlotHeight}' fill='none' stroke='{InkMuted}' stroke-width='1'/>");
        string Color(double t) { var (r, g, b) = o.NormalizedColor(t); return $"#{r:x2}{g:x2}{b:x2}"; }
        svg.Append(GradientColorbar.Render(limits.Min, limits.Max, Color, layout.ColorbarX, layout.Top, layout.PlotHeight, o.ColorbarLabel, layout.ColorbarWidth));
        return svg.Append("</svg>").ToString();
    }
}
