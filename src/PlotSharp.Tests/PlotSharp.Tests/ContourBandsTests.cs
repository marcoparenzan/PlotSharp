namespace PlotSharp.Tests;

public class ContourBandsTests
{
    [Theory]
    [InlineData(-0.1, -1)]
    [InlineData(0, 0)]
    [InlineData(0.5, 0)]
    [InlineData(1, 1)]
    [InlineData(1.5, 1)]
    [InlineData(2, 1)]
    [InlineData(2.1, -1)]
    [InlineData(double.NaN, -1)]
    [InlineData(double.PositiveInfinity, -1)]
    public void ExactBoundariesAndOutsideValuesMatchLegacy(double value, int expected)
        => Assert.Equal(expected, new ContourBands([0, 1, 2]).Find(value));

    [Fact]
    public void RepeatedLevelsSelectLastMatchingBoundary()
    {
        var bands = new ContourBands([0, 1, 1, 2]);
        Assert.Equal(0, bands.Find(double.BitDecrement(1)));
        Assert.Equal(2, bands.Find(1));
        Assert.Equal(2, bands.Find(double.BitIncrement(1)));
        Assert.Equal(1, new ContourBands([0, 1, 1]).Find(1));
        var allSame = new ContourBands([1, 1, 1]);
        Assert.Equal(1, allSame.Find(1)); Assert.Equal(0, allSame.NormalizedMidpoint(1));
    }

    [Fact]
    public void MidpointNormalizationUsesTheOriginalOperationOrder()
    {
        var bands = new ContourBands([-3, 1, 7]);
        Assert.Equal(0.2, bands.NormalizedMidpoint(0)); Assert.Equal(0.7, bands.NormalizedMidpoint(1));
    }

    [Fact]
    public void FloatingOverflowIsNotSilentlyReassociated()
        => Assert.True(double.IsPositiveInfinity(new ContourBands([1e308, 1.1e308]).NormalizedMidpoint(0)));

    [Fact]
    public void LevelsAreSnapshotted()
    {
        double[] levels = [0, 1, 2]; var bands = new ContourBands(levels); levels[1] = 100;
        Assert.Equal(1, bands.Find(1));
    }

    [Fact]
    public void SingleLevelContainsNoBand()
    {
        var bands = new ContourBands([1]); Assert.Equal(-1, bands.Find(1));
        Assert.Throws<ArgumentOutOfRangeException>(() => bands.NormalizedMidpoint(0));
    }

    [Fact]
    public void InvalidLevelsAreRejected()
    {
        Assert.Throws<ArgumentNullException>(() => new ContourBands(null!));
        Assert.Throws<ArgumentException>(() => new ContourBands([]));
        Assert.Throws<ArgumentException>(() => new ContourBands([1, 0]));
        Assert.Throws<ArgumentException>(() => new ContourBands([0, double.NaN]));
    }

    [Fact]
    public void InvalidBandIndicesAreRejected()
    {
        var bands = new ContourBands([0, 1, 2]);
        Assert.Throws<ArgumentOutOfRangeException>(() => bands.NormalizedMidpoint(-1));
        Assert.Throws<ArgumentOutOfRangeException>(() => bands.NormalizedMidpoint(2));
    }
}
