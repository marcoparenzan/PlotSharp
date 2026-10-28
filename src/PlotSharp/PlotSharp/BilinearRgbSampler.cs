namespace PlotSharp;

/// <summary>Sample a node-based field into RGB8 filter-0 scanlines. Row zero in the field is at the bottom; output is top-down.</summary>
public static class BilinearRgbSampler
{
    public static byte[] SampleScanlines(double[,] field, int width, int height,
        Func<double, (byte R, byte G, byte B)> colorize,
        Func<double, double, bool>? mask = null, (byte R, byte G, byte B)? maskedColor = null)
    {
        ArgumentNullException.ThrowIfNull(field);
        ArgumentNullException.ThrowIfNull(colorize);
        int rows = field.GetLength(0), columns = field.GetLength(1);
        if (rows < 2 || columns < 2) throw new ArgumentException("Bilinear node sampling requires at least 2x2 field samples.", nameof(field));
        if (mask is not null && maskedColor is null) throw new ArgumentException("Provide an explicit color for a mask.", nameof(maskedColor));
        var masked = maskedColor.GetValueOrDefault();
        var scanlines = new byte[RgbPngEncoder.GetScanlineByteCount(width, height)];
        int offset = 0;
        for (int py = 0; py < height; py++)
        {
            scanlines[offset++] = 0;
            double fy = (1 - (py + 0.5) / height) * (rows - 1);
            int row = Math.Min((int)fy, rows - 2); double wy = fy - row;
            for (int px = 0; px < width; px++)
            {
                double fx = (px + 0.5) / width * (columns - 1);
                int column = Math.Min((int)fx, columns - 2); double wx = fx - column;
                // Preserve the legacy operation order, including non-finite propagation to the color callback.
                double value = (1 - wy) * ((1 - wx) * field[row, column] + wx * field[row, column + 1])
                             + wy * ((1 - wx) * field[row + 1, column] + wx * field[row + 1, column + 1]);
                var (r, g, b) = mask?.Invoke(fx, fy) == true ? masked : colorize(value);
                scanlines[offset++] = r; scanlines[offset++] = g; scanlines[offset++] = b;
            }
        }
        return scanlines;
    }
}
