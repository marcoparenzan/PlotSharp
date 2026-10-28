namespace PlotSharp;

/// <summary>Legacy node-field limit selection: floor-index percentiles, optional symmetry, and a unit expansion for tiny ranges.</summary>
public sealed record FieldColorLimits(double Min, double Max)
{
    public static FieldColorLimits Resolve(double[,] field, double? min = null, double? max = null,
        bool symmetric = false, double clipPercentile = 0)
    {
        ArgumentNullException.ThrowIfNull(field);
        if (field.LongLength == 0 || field.LongLength > 100000) throw new ArgumentException("Require 1 to 100000 field samples.", nameof(field));
        if (!double.IsFinite(clipPercentile) || clipPercentile < 0 || clipPercentile > 0.5)
            throw new ArgumentOutOfRangeException(nameof(clipPercentile));
        if (min is { } minimum && (!double.IsFinite(minimum) || Math.Abs(minimum) > 1e12)
            || max is { } maximum && (!double.IsFinite(maximum) || Math.Abs(maximum) > 1e12))
            throw new ArgumentException("Explicit limits must be finite and within ±1e12.");
        var values = new List<double>();
        foreach (double value in field)
        {
            if (double.IsNaN(value)) continue;
            if (!double.IsFinite(value) || Math.Abs(value) > 1e12) throw new ArgumentException("Samples must be finite within ±1e12 or NaN.", nameof(field));
            values.Add(value);
        }
        if (values.Count == 0 && (min is null || max is null))
            throw new ArgumentException("All-missing fields require both explicit limits.", nameof(field));
        values.Sort();
        double lo = min ?? values[(int)(clipPercentile * (values.Count - 1))];
        double hi = max ?? values[(int)((1 - clipPercentile) * (values.Count - 1))];
        if (lo > hi) throw new ArgumentException("Selected color limits are reversed.");
        if (symmetric) { double magnitude = Math.Max(Math.Abs(lo), Math.Abs(hi)); lo = -magnitude; hi = magnitude; }
        // Deliberately retain legacy expansion, not ColorScale's constant-range center policy.
        if (hi - lo < 1e-12) hi = lo + 1;
        return new(lo, hi);
    }

    /// <summary>Preserves caller-owned normalization; NaN/infinity propagate to the palette policy.</summary>
    public double NormalizeLegacy(double value) => (value - Min) / (Max - Min);
}
