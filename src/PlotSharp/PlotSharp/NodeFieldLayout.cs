namespace PlotSharp;

/// <summary>Navier-compatible node viewport and margins, not a cell-centered heatmap layout.</summary>
public sealed class NodeFieldLayout
{
    public int Rows { get; }
    public int Columns { get; }
    public int PlotWidth { get; }
    public int PlotHeight { get; }
    public int Left => 16;
    public int Top { get; }
    public int ColorbarWidth => 14;
    public int ColorbarX => Left + PlotWidth + 20;
    public int Width => Left + PlotWidth + 20 + ColorbarWidth + 80;
    public int Height => Top + PlotHeight + 24;
    private readonly double sx, sy;

    private NodeFieldLayout(int rows, int columns, int plotWidth, bool hasSubtitle)
    {
        Rows = rows; Columns = columns; PlotWidth = plotWidth;
        PlotHeight = (int)Math.Round(plotWidth * (double)(rows - 1) / (columns - 1));
        if (PlotHeight < 1 || PlotHeight > 10000) throw new ArgumentOutOfRangeException(nameof(plotWidth), "Projected height must be 1 to 10000 pixels.");
        Top = hasSubtitle ? 62 : 44;
        sx = (double)PlotWidth / (Columns - 1); sy = (double)PlotHeight / (Rows - 1);
    }

    public static NodeFieldLayout Create(int rows, int columns, int plotWidth, bool hasSubtitle = false)
    {
        if (rows < 2 || columns < 2 || (long)rows * columns > 100000)
            throw new ArgumentException("Node fields require at least 2x2 and at most 100000 samples.");
        if (plotWidth < 1 || plotWidth > 10000) throw new ArgumentOutOfRangeException(nameof(plotWidth));
        return new(rows, columns, plotWidth, hasSubtitle);
    }

    public double X(double column)
    {
        double x = Left + column * sx;
        if (!double.IsFinite(column) || !double.IsFinite(x)) throw new ArgumentOutOfRangeException(nameof(column));
        return x;
    }

    public double Y(double row)
    {
        double y = Top + PlotHeight - row * sy;
        if (!double.IsFinite(row) || !double.IsFinite(y)) throw new ArgumentOutOfRangeException(nameof(row));
        return y;
    }
}
