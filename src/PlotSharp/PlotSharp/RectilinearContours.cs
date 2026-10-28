namespace PlotSharp;

/// <summary>NDSharp-compatible line-contour geometry in physical rectilinear coordinates. Does not select levels or render styles/bands.</summary>
public static class RectilinearContours
{
    public static IReadOnlyList<Charts.Segment> Trace(double[] xs, double[] ys, double[] rowMajorValues, double level)
    {
        ArgumentNullException.ThrowIfNull(xs); ArgumentNullException.ThrowIfNull(ys); ArgumentNullException.ThrowIfNull(rowMajorValues);
        if (xs.Length == 0 || ys.Length == 0 || (long)xs.Length * ys.Length != rowMajorValues.LongLength)
            throw new ArgumentException("Nonempty axes and matching row-major values required.");
        if (xs.Any(v => !double.IsFinite(v)) || ys.Any(v => !double.IsFinite(v)))
            throw new ArgumentException("Physical coordinates must be finite.");
        if (!double.IsFinite(level)) throw new ArgumentOutOfRangeException(nameof(level));
        // No node-field ±1e12/100k cap: the ND consumer retains its own raster/data scope.
        return ContourKernel.Trace(ys.Length, xs.Length, (r, c) => rowMajorValues[r * xs.Length + c],
            c => xs[c], r => ys[r], level, mask: null,
            forwardOppositeEdges: true, preserveDegenerate: true, skipNonFiniteCornerSum: true);
    }
}
