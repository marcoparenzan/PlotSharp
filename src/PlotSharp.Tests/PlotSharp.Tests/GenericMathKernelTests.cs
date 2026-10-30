namespace PlotSharp.Tests;

/// <summary>Generic-math forms of the shared kernels (R06): the number type of the caller is preserved end to end.</summary>
public class GenericMathKernelTests
{
    // 0.1 and 0.3 are exact in decimal; in double (0.3 - 0.1) is 0.19999999999999998, which moves the crossing off 0.15.
    static readonly decimal[] DecX = [0m, 0.3m], DecY = [0m, 1m], DecZ = [0.1m, 0.3m, 0.1m, 0.3m];

    [Fact]
    public void DecimalKeepsTheCrossingExactWhereDoubleDoesNot()
    {
        var exact = Assert.Single(RectilinearContours<decimal>.Trace(DecX, DecY, DecZ, 0.2m));
        Assert.Equal(0.15m, exact.X1); Assert.Equal(0.15m, exact.X2);
        Assert.Equal(0m, exact.Y1); Assert.Equal(1m, exact.Y2);

        var approx = Assert.Single(RectilinearContours.Trace([0, 0.3], [0, 1], [0.1, 0.3, 0.1, 0.3], 0.2));
        Assert.NotEqual(0.15, approx.X1);                      // the double result is off by an ulp
        Assert.Equal(0.15, approx.X1, 12);
    }

    [Fact]
    public void DoubleInstantiationMatchesTheNonGenericApiExactly()
    {
        double[] xs = [0, 1, 10], ys = [0, 1, 4], z = [0, 1, 2, 0, 1, 2, 0, 1, 2];
        var plain = RectilinearContours.Trace(xs, ys, z, 1.5);
        var generic = RectilinearContours<double>.Trace(xs, ys, z, 1.5);
        Assert.Equal(plain.Count, generic.Count);
        for (int i = 0; i < plain.Count; i++)
            Assert.Equal((plain[i].X1, plain[i].Y1, plain[i].X2, plain[i].Y2), (generic[i].X1, generic[i].Y1, generic[i].X2, generic[i].Y2));
    }

    [Fact]
    public void SinglePrecisionAndHalfTraceTheSameGeometry()
    {
        var reference = Assert.Single(RectilinearContours<double>.Trace([0, 2], [0, 2], [0, 4, 0, 4], 1));
        var single = Assert.Single(RectilinearContours<float>.Trace([0f, 2f], [0f, 2f], [0f, 4f, 0f, 4f], 1f));
        var half = Assert.Single(RectilinearContours<Half>.Trace([(Half)0, (Half)2], [(Half)0, (Half)2], [(Half)0, (Half)4, (Half)0, (Half)4], (Half)1));
        Assert.Equal(reference.X1, single.X1, 6); Assert.Equal(reference.X1, (double)half.X1, 3);
        Assert.Equal(reference.Y2, single.Y2, 6); Assert.Equal(reference.Y2, (double)half.Y2, 3);
    }

    [Fact]
    public void DecimalSamplerIsExactOnTheSameGrid()
    {
        var sampler = new RectilinearFieldSampler<decimal>(DecX, DecY, DecZ);
        Assert.Equal(0.2m, sampler.Sample(0.15m, 0.5m));
        Assert.Equal(0.3m, sampler.Sample(0.3m, 1m));
    }

    [Fact]
    public void GenericValidationMatchesTheDoubleApi()
    {
        Assert.Throws<ArgumentException>(() => new RectilinearFieldSampler<decimal>([0m, 0m], [0m, 1m], [0m, 1m, 2m, 3m]));
        Assert.Throws<ArgumentException>(() => new ContourBands<decimal>([1m, 0m]));
        Assert.Throws<ArgumentException>(() => RectilinearContours<float>.Trace([0f, float.NaN], [0f, 1f], [0f, 1f, 0f, 1f], 0f));
        Assert.Throws<ArgumentOutOfRangeException>(() => RectilinearContours<float>.Trace([0f, 1f], [0f, 1f], [0f, 1f, 0f, 1f], float.PositiveInfinity));
    }

    [Fact]
    public void DecimalBandsUseInclusiveBoundariesAndNormalizedMidpoints()
    {
        var bands = new ContourBands<decimal>([0m, 0.1m, 0.2m, 0.3m]);
        Assert.Equal(-1, bands.Find(-0.1m));
        Assert.Equal(1, bands.Find(0.1m));                     // interior level belongs to the upper band
        Assert.Equal(2, bands.Find(0.3m));                     // last level belongs to the last band
        Assert.Equal(0.5m, bands.NormalizedMidpoint(1));       // (0.1+0.2)/2 = 0.15; 0.15/0.3
    }

    [Fact]
    public void DecimalHasNoInfinityAndOverflowThrowsInsteadOfSkippingTheCell()
    {
        // double: the summed-corner test skips the cell. decimal cannot represent the sum, so the overflow surfaces.
        Assert.Throws<OverflowException>(() => RectilinearContours<decimal>.Trace([0m, 1m], [0m, 1m],
            [decimal.MaxValue, decimal.MaxValue, decimal.MaxValue, decimal.MaxValue], 1m));
        Assert.Empty(RectilinearContours.Trace([0, 1], [0, 1], [double.MaxValue, double.MaxValue, double.MaxValue, double.MaxValue], 1));
    }
}
