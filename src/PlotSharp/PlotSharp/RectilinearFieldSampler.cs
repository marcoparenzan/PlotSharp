namespace PlotSharp;

/// <summary>ND-compatible physical-grid bilinear sampling; four-term arithmetic retained. Not Navier's uniform RGB node resampler.</summary>
public sealed class RectilinearFieldSampler
{
    private readonly double[] xs, ys, values;

    public RectilinearFieldSampler(double[] xs, double[] ys, double[] rowMajorValues)
    {
        ArgumentNullException.ThrowIfNull(xs); ArgumentNullException.ThrowIfNull(ys); ArgumentNullException.ThrowIfNull(rowMajorValues);
        if (xs.Length < 2 || ys.Length < 2 || (long)xs.Length * ys.Length != rowMajorValues.LongLength)
            throw new ArgumentException("At least 2x2 axes and matching row-major data required.");
        ValidateAxis(xs); ValidateAxis(ys);
        this.xs = xs.ToArray(); this.ys = ys.ToArray(); values = rowMajorValues.ToArray();
    }

    public double Sample(double x, double y)
    {
        if (!double.IsFinite(x) || !double.IsFinite(y)) throw new ArgumentOutOfRangeException(nameof(x));
        int col = Math.Clamp(Array.BinarySearch(xs, x) is var i && i >= 0 ? i : ~i - 1, 0, xs.Length - 2);
        int row = Math.Clamp(Array.BinarySearch(ys, y) is var j && j >= 0 ? j : ~j - 1, 0, ys.Length - 2);
        double tx = (x - xs[col]) / (xs[col + 1] - xs[col]), ty = (y - ys[row]) / (ys[row + 1] - ys[row]);
        return At(row, col) * (1 - tx) * (1 - ty) + At(row, col + 1) * tx * (1 - ty)
             + At(row + 1, col) * (1 - tx) * ty + At(row + 1, col + 1) * tx * ty;
    }

    private double At(int row, int col) => values[row * xs.Length + col];

    private static void ValidateAxis(double[] axis)
    {
        for (int i = 0; i < axis.Length; i++)
            if (!double.IsFinite(axis[i]) || i > 0 && axis[i] <= axis[i - 1])
                throw new ArgumentException("Sampling axes must be finite and strictly increasing.");
    }
}
