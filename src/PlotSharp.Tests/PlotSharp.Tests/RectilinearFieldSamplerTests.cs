using System.Text.Json;

namespace PlotSharp.Tests;

public class RectilinearFieldSamplerTests
{
    [Fact]
    public void MatchesAllFrozenLegacyQueriesExactly()
    {
        using var doc = JsonDocument.Parse(File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "Fixtures", "rectilinear-sample-baseline.json")));
        static double[] Values(JsonElement e) => e.EnumerateArray().Select(v => v.GetDouble()).ToArray();
        var root = doc.RootElement;
        var sampler = new RectilinearFieldSampler(Values(root.GetProperty("Xs")), Values(root.GetProperty("Ys")), Values(root.GetProperty("Values")));
        var samples = root.GetProperty("Samples").EnumerateArray().ToArray(); Assert.Equal(81, samples.Length);
        foreach (var sample in samples)
            Assert.Equal(sample.GetProperty("Value").GetDouble(), sampler.Sample(sample.GetProperty("X").GetDouble(), sample.GetProperty("Y").GetDouble()));
    }

    [Fact]
    public void NonUniformPlaneAndEndpointAreInterpolatedInPhysicalSpace()
    {
        var sampler = new RectilinearFieldSampler([0, 1, 10], [0, 1, 4], [0, 1, 10, 2, 3, 12, 8, 9, 18]);
        Assert.Equal(10.5, sampler.Sample(5.5, 2.5)); Assert.Equal(18, sampler.Sample(10, 4));
        Assert.Equal(-1, sampler.Sample(-1, 0)); // old cell clamping still allows weight extrapolation
    }

    [Fact]
    public void SnapshotIsUnaffectedByCallerArrayMutation()
    {
        double[] xs = [0, 1], ys = [0, 1], values = [0, 1, 2, 3];
        var sampler = new RectilinearFieldSampler(xs, ys, values);
        xs[1] = 100; ys[1] = 100; values[3] = 100;
        Assert.Equal(1.5, sampler.Sample(0.5, 0.5));
    }

    [Fact]
    public void MissingValuesPropagateToCallerBandPolicy()
    {
        var sampler = new RectilinearFieldSampler([0, 1], [0, 1], [double.NaN, 0, 0, 0]);
        Assert.True(double.IsNaN(sampler.Sample(0.5, 0.5)));
    }

    [Theory]
    [InlineData(double.NaN)]
    [InlineData(double.PositiveInfinity)]
    public void InvalidQueryCoordinatesAreRejected(double query)
        => Assert.Throws<ArgumentOutOfRangeException>(() => new RectilinearFieldSampler([0, 1], [0, 1], [0, 1, 2, 3]).Sample(query, 0));

    [Fact]
    public void InvalidGridShapeOrAxisOrderIsRejected()
    {
        Assert.Throws<ArgumentException>(() => new RectilinearFieldSampler([0], [0, 1], [0, 1]));
        Assert.Throws<ArgumentException>(() => new RectilinearFieldSampler([0, 0], [0, 1], [0, 1, 2, 3]));
        Assert.Throws<ArgumentException>(() => new RectilinearFieldSampler([1, 0], [0, 1], [0, 1, 2, 3]));
        Assert.Throws<ArgumentException>(() => new RectilinearFieldSampler([0, 1], [0, 1], [0]));
    }
}
