namespace PlotSharp;

/// <summary>NDSharp-compatible inclusive band lookup and midpoint normalization; no palette, alpha or raster rendering.</summary>
public sealed class ContourBands
{
    private readonly double[] levels;

    public ContourBands(double[] levels)
    {
        ArgumentNullException.ThrowIfNull(levels);
        if (levels.Length == 0) throw new ArgumentException("At least one level required.", nameof(levels));
        for (int i = 0; i < levels.Length; i++)
            if (!double.IsFinite(levels[i]) || i > 0 && levels[i] < levels[i - 1])
                throw new ArgumentException("Levels must be finite and nondecreasing.", nameof(levels));
        this.levels = levels.ToArray();
    }

    public int Find(double value)
    {
        int band = -1;
        for (int k = 0; k + 1 < levels.Length; k++)
            if (value >= levels[k] && value <= levels[k + 1])
            {
                band = k;
                if (value < levels[k + 1]) break;
            }
        return band;
    }

    public double NormalizedMidpoint(int band)
    {
        if (band < 0 || band >= levels.Length - 1) throw new ArgumentOutOfRangeException(nameof(band));
        double mid = (levels[band] + levels[band + 1]) / 2, lo = levels[0], hi = levels[^1];
        // Original arithmetic retained, including overflow/NaN propagation to the caller's color policy.
        return hi == lo ? 0 : (mid - lo) / (hi - lo);
    }
}
