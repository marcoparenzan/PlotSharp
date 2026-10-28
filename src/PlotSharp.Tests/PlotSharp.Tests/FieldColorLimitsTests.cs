using System.Text.Json;

namespace PlotSharp.Tests;

public class FieldColorLimitsTests
{
    [Fact]
    public void MatchesFrozenProductionColorbarEndpoints()
    {
        using var doc = JsonDocument.Parse(File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "Fixtures", "field-limits-baseline.json")));
        var samples = doc.RootElement.GetProperty("Samples").EnumerateArray().ToArray();
        Assert.Equal(8, samples.Length);
        foreach (var sample in samples)
        {
            int rows = sample.GetProperty("Rows").GetInt32(), cols = sample.GetProperty("Columns").GetInt32();
            var values = sample.GetProperty("Values").EnumerateArray().Select(v => v.ValueKind == JsonValueKind.Null ? double.NaN : v.GetDouble()).ToArray();
            var field = new double[rows, cols];
            for (int r = 0; r < rows; r++) for (int c = 0; c < cols; c++) field[r, c] = values[r * cols + c];
            double? min = sample.GetProperty("Min").ValueKind == JsonValueKind.Null ? null : sample.GetProperty("Min").GetDouble();
            double? max = sample.GetProperty("Max").ValueKind == JsonValueKind.Null ? null : sample.GetProperty("Max").GetDouble();
            var limits = FieldColorLimits.Resolve(field, min, max, sample.GetProperty("Symmetric").GetBoolean(), sample.GetProperty("Clip").GetDouble());
            Assert.Equal(sample.GetProperty("ExpectedMin").GetDouble(), limits.Min);
            Assert.Equal(sample.GetProperty("ExpectedMax").GetDouble(), limits.Max);
        }
    }

    [Fact]
    public void ConstantFieldDoesNotChangeColorScaleDefault()
    {
        var limits = FieldColorLimits.Resolve(new double[,] { { 5 } });
        Assert.Equal(new FieldColorLimits(5, 6), limits);
        Assert.Equal(0, limits.NormalizeLegacy(5));
        Assert.Equal(0.5, new ColorScale(5, 5).Normalize(5));
    }

    [Fact]
    public void TinyRangeRetainsLegacyUnitExpansion()
        => Assert.Equal(new FieldColorLimits(5, 6), FieldColorLimits.Resolve(new double[,] { { 5, 5 + 1e-13 } }));

    [Fact]
    public void ZeroSymmetryRetainsNegativeZeroAndUnitFallback()
    {
        var limits = FieldColorLimits.Resolve(new double[1, 1], symmetric: true);
        Assert.Equal(long.MinValue, BitConverter.DoubleToInt64Bits(limits.Min));
        Assert.Equal(1, limits.Max);
    }

    [Fact]
    public void ExpansionAtInputBoundaryIsNotSilentlyClamped()
        => Assert.Equal(1e12 + 1, FieldColorLimits.Resolve(new double[,] { { 1e12 } }).Max);

    [Fact]
    public void HalfClippingUsesFloorMedianNotInterpolatedQuantile()
        => Assert.Equal(new FieldColorLimits(4, 5), FieldColorLimits.Resolve(new double[,] { { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 } }, clipPercentile: 0.5));

    [Fact]
    public void ReversedBoundsAreRejected()
        => Assert.Throws<ArgumentException>(() => FieldColorLimits.Resolve(new double[,] { { 0, 1 } }, 2, 1));

    [Fact]
    public void AllMissingDataRequiresBothExplicitBounds()
    {
        double[,] field = { { double.NaN } };
        Assert.Throws<ArgumentException>(() => FieldColorLimits.Resolve(field));
        Assert.Throws<ArgumentException>(() => FieldColorLimits.Resolve(field, min: 0));
        Assert.Equal(new FieldColorLimits(0, 1), FieldColorLimits.Resolve(field, 0, 1));
    }

    [Theory]
    [InlineData(-0.1)]
    [InlineData(0.6)]
    [InlineData(double.NaN)]
    [InlineData(double.PositiveInfinity)]
    public void InvalidClippingIsRejected(double clip)
        => Assert.Throws<ArgumentOutOfRangeException>(() => FieldColorLimits.Resolve(new double[,] { { 0, 1 } }, clipPercentile: clip));

    [Theory]
    [InlineData(double.PositiveInfinity)]
    [InlineData(1000000000001d)]
    public void UnsupportedSamplesAreRejected(double value)
        => Assert.Throws<ArgumentException>(() => FieldColorLimits.Resolve(new double[,] { { 0, value } }));

    [Fact]
    public void NullEmptyAndOversizedFieldsAreRejected()
    {
        Assert.Throws<ArgumentNullException>(() => FieldColorLimits.Resolve(null!));
        Assert.Throws<ArgumentException>(() => FieldColorLimits.Resolve(new double[0, 0]));
        Assert.Throws<ArgumentException>(() => FieldColorLimits.Resolve(new double[1, 100001]));
    }

    [Fact]
    public void RawNormalizationKeepsDirectCallerNonFinitePolicy()
    {
        Assert.True(double.IsNaN(new FieldColorLimits(1, 1).NormalizeLegacy(1)));
        Assert.True(double.IsPositiveInfinity(new FieldColorLimits(0, 1).NormalizeLegacy(double.PositiveInfinity)));
    }
}
