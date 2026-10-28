using System.Globalization;
using System.Text;

namespace PlotSharp;

/// <summary>Navier-compatible node vector paths: global-max normalization and open heads. Not the physical/cell-centered Heatmap quiver.</summary>
public static class NodeVectorOverlay
{
    public static string PathData(double[,] uField, double[,] vField, double plotWidth, double plotHeight, int stride,
        double left = 16, double top = 44)
    {
        ArgumentNullException.ThrowIfNull(uField); ArgumentNullException.ThrowIfNull(vField);
        int rows = uField.GetLength(0), cols = uField.GetLength(1);
        if (rows < 2 || cols < 2 || uField.LongLength > 100000 || vField.GetLength(0) != rows || vField.GetLength(1) != cols)
            throw new ArgumentException("Matching node fields of 2x2 to 100000 samples required.");
        if (stride < 1 || stride > 100000) throw new ArgumentOutOfRangeException(nameof(stride));
        if (!double.IsFinite(plotWidth) || !double.IsFinite(plotHeight) || plotWidth <= 0 || plotHeight <= 0 || plotWidth > 10000 || plotHeight > 10000
            || !double.IsFinite(left) || !double.IsFinite(top) || Math.Abs(left) > 1e7 || Math.Abs(top) > 1e7)
            throw new ArgumentException("Invalid plot projection.");
        double maximum = 1e-12;
        for (int row = 0; row < rows; row++) for (int col = 0; col < cols; col++)
        {
            double u = uField[row, col], v = vField[row, col];
            if (!double.IsFinite(u) || !double.IsFinite(v) || Math.Abs(u) > 1e12 || Math.Abs(v) > 1e12)
                throw new ArgumentException("Vector samples must be finite and within ±1e12.");
            maximum = Math.Max(maximum, Math.Sqrt(u * u + v * v));
        }
        double sx = plotWidth / (cols - 1), sy = plotHeight / (rows - 1);
        double length = stride * sx * 0.9;
        var path = new StringBuilder();
        for (int row = stride / 2; row < rows; row += stride)
            for (int col = stride / 2; col < cols; col += stride)
            {
                double u = uField[row, col] / maximum, v = vField[row, col] / maximum;
                double magnitude = Math.Sqrt(u * u + v * v);
                if (magnitude < 0.02) continue;
                double x0 = left + col * sx, y0 = top + plotHeight - row * sy;
                double dx = u * length, dy = -v * length;
                double x1 = x0 + dx, y1 = y0 + dy;
                double angle = Math.Atan2(dy, dx), head = Math.Max(2.5, 4 * Math.Sqrt(magnitude));
                path.Append($"M{F(x0)} {F(y0)}L{F(x1)} {F(y1)}");
                path.Append($"M{F(x1 - head * Math.Cos(angle - 0.45))} {F(y1 - head * Math.Sin(angle - 0.45))}L{F(x1)} {F(y1)}L{F(x1 - head * Math.Cos(angle + 0.45))} {F(y1 - head * Math.Sin(angle + 0.45))}");
            }
        return path.ToString();
    }

    private static string F(double value) => value.ToString("0.##", CultureInfo.InvariantCulture);
}
