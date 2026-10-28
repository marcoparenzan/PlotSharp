using System.Text;

namespace PlotSharp;

public static partial class Charts
{
    public sealed class HeatmapOptions
    {
        public string Title { get; init; } = "";
        public int Width { get; init; } = 560;
        public int Height { get; init; } = 340;
        public double? Min { get; init; }
        public double? Max { get; init; }
        /// <summary>False: row zero is at the bottom, as in NavierStokes. True: at the top.</summary>
        public bool OriginUpper { get; init; }
        public bool ShowColorbar { get; init; }
        public string ColorbarLabel { get; init; } = "";
        public FieldExtent? Extent { get; init; }
        /// <summary>True cells are omitted and excluded from automatic color limits.</summary>
        public bool[,]? Mask { get; init; }
        public double[]? ContourLevels { get; init; }
        public string ContourColor { get; init; } = "#0b0b0b";
        public VectorField? Vectors { get; init; }
    }

    /// <summary>Interpolated sequential palette; stops selected from NavierStokes ColorMap.Sequential.</summary>
    public static string SequentialColor(double normalized) => ColorPalette.CompactSequential.Hex(normalized);

    /// <summary>Cell-based SVG heatmap of field[row,column], without resampling or native dependencies.</summary>
    public static string Heatmap(double[,] field, HeatmapOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(field);
        var o = options ?? new();
        ArgumentNullException.ThrowIfNull(o.Title);
        ArgumentNullException.ThrowIfNull(o.ColorbarLabel);
        o.Extent?.Validate();
        int rows = field.GetLength(0), cols = field.GetLength(1);
        if (rows == 0 || cols == 0 || field.LongLength > 100000)
            throw new ArgumentException("Heatmaps require 1 to 100000 cells.", nameof(field));
        if (o.Width < 200 || o.Width > 10000 || o.Height < 160 || o.Height > 10000)
            throw new ArgumentOutOfRangeException(nameof(options));
        if (o.Mask is { } mask && (mask.GetLength(0) != rows || mask.GetLength(1) != cols))
            throw new ArgumentException("Mask dimensions must match the field.", nameof(options));
        if (o.ShowColorbar && o.Width < 320) throw new ArgumentOutOfRangeException(nameof(options), "Colorbar requires width >= 320.");
        double min = double.PositiveInfinity, max = double.NegativeInfinity;
        int visible = 0;
        for (int row = 0; row < rows; row++)
            for (int col = 0; col < cols; col++)
            {
                if (o.Mask?[row, col] == true) continue;
                double v = field[row, col];
                if (!double.IsFinite(v) || Math.Abs(v) > 1e12) throw new ArgumentException("Unmasked values must be finite and within ±1e12.", nameof(field));
                min = Math.Min(min, v); max = Math.Max(max, v); visible++;
            }
        if (visible == 0 && (o.Min is null || o.Max is null))
            throw new ArgumentException("Fully masked fields require explicit Min and Max.", nameof(options));
        var scale = new ColorScale(o.Min ?? min, o.Max ?? max);
        min = scale.Min; max = scale.Max;
        double plotW = o.Width - (o.ShowColorbar ? 160.0 : 88.0), plotH = o.Height - 98.0;
        double cellW = plotW / cols, cellH = plotH / rows;
        var sb = new StringBuilder();
        Header(sb, o.Width, o.Height, o.Title, "");
        sb.Append("<g shape-rendering='crispEdges'>");
        for (int row = 0; row < rows; row++)
            for (int col = 0; col < cols; col++)
            {
                if (o.Mask?[row, col] == true) continue;
                double value = field[row, col];
                int displayRow = o.OriginUpper ? row : rows - row - 1;
                sb.Append($"<rect data-row='{row}' data-column='{col}' x='{F(64 + col * cellW)}' y='{F(48 + displayRow * cellH)}' width='{F(cellW)}' height='{F(cellH)}' fill='{scale.Color(value)}'><title>[{row},{col}]: {Num(value)}</title></rect>");
            }
        sb.Append("</g>");
        if (o.ContourLevels is not null || o.Vectors is not null)
        {
            string clip = "field" + Guid.NewGuid().ToString("N");
            sb.Append($"<clipPath id='{clip}'><rect x='64' y='48' width='{F(plotW)}' height='{F(plotH)}'/></clipPath><g clip-path='url(#{clip})'>");
            AppendOverlays(sb, field, o, plotW, plotH);
            sb.Append("</g>");
        }
        if (o.Extent is { } extent)
        {
            foreach (var tick in NiceTicks(extent.XMin, extent.XMax).Where(v => v >= extent.XMin && v <= extent.XMax))
                sb.Append($"<text x='{F(64 + (tick - extent.XMin) / (extent.XMax - extent.XMin) * plotW)}' y='{o.Height - 32}' text-anchor='middle' font-size='11' fill='{InkSecondary}'>{Num(tick)}</text>");
            foreach (var tick in NiceTicks(extent.YMin, extent.YMax).Where(v => v >= extent.YMin && v <= extent.YMax))
            {
                double t = (tick - extent.YMin) / (extent.YMax - extent.YMin);
                double y = 48 + (o.OriginUpper ? t : 1 - t) * plotH;
                sb.Append($"<text x='56' y='{F(y + 4)}' text-anchor='end' font-size='11' fill='{InkSecondary}'>{Num(tick)}</text>");
            }
        }
        if (o.ShowColorbar) AppendColorbar(sb, scale, 64 + plotW + 20, 48, plotH, o.ColorbarLabel);
        sb.Append($"<text x='64' y='{o.Height - 18}' font-size='12' fill='{InkSecondary}'>min {Num(min)} · max {Num(max)}</text></svg>");
        return sb.ToString();
    }

    private static void AppendColorbar(StringBuilder sb, ColorScale scale, double x, double y, double height, string label)
    {
        sb.Append("<g class='colorbar' shape-rendering='crispEdges'>");
        // Discrete strips sample exactly the same palette/normalization as the field.
        const int strips = 128;
        for (int i = 0; i < strips; i++)
        {
            double t = 1 - i / (double)(strips - 1);
            string color = scale.Min == scale.Max ? scale.Color(scale.Min) : SequentialColor(t);
            sb.Append($"<rect x='{F(x)}' y='{F(y + i * height / strips)}' width='16' height='{F(height / strips + 0.01)}' fill='{color}'/>");
        }
        var ticks = scale.Min == scale.Max ? new[] { scale.Min } : new[] { scale.Min, scale.Min + (scale.Max - scale.Min) / 2, scale.Max };
        foreach (double tick in ticks)
        {
            double ty = y + (1 - scale.Normalize(tick)) * height;
            sb.Append($"<text x='{F(x + 22)}' y='{F(ty + 4)}' font-size='10' fill='{InkSecondary}'>{Num(tick)}</text>");
        }
        sb.Append($"<text x='{F(x)}' y='{F(y + height + 18)}' font-size='11' fill='{InkSecondary}'>{Esc(label)}</text></g>");
    }
}
