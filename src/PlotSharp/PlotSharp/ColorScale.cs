namespace PlotSharp;

/// <summary>Immutable linear normalization shared by fields and colorbars.</summary>
public sealed class ColorScale
{
    public double Min { get; }
    public double Max { get; }
    public ColorScale(double min, double max)
    {
        if (!double.IsFinite(min) || !double.IsFinite(max) || Math.Abs(min) > 1e12 || Math.Abs(max) > 1e12 || min > max)
            throw new ArgumentException("Color limits must be finite, ordered and within ±1e12.");
        Min = min; Max = max;
    }
    public double Normalize(double value)
    {
        if (!double.IsFinite(value)) throw new ArgumentOutOfRangeException(nameof(value));
        if (Min == Max) return 0.5;
        // Clamp first: avoids overflow when the caller passes a very large finite value.
        if (value <= Min) return 0;
        if (value >= Max) return 1;
        return (value - Min) / (Max - Min);
    }
    public string Color(double value) => Charts.SequentialColor(Normalize(value));
}

/// <summary>Outer cell edges, not sample centers. Y increases upwards when OriginUpper is false.</summary>
public sealed record FieldExtent(double XMin, double XMax, double YMin, double YMax)
{
    internal void Validate()
    {
        if (new[] { XMin, XMax, YMin, YMax }.Any(v => !double.IsFinite(v) || Math.Abs(v) > 1e12) || XMin >= XMax || YMin >= YMax)
            throw new ArgumentException("Extent must have finite, strictly increasing edges within ±1e12.");
    }
}
