using System.Text.Json;
using System.Xml.Linq;
using NavierStokes.Core;
using NC = NavierStokes.Core.Charts;

namespace PlotSharp.Tests;

public class NodeVectorOverlayTests
{
    [Fact]
    public void PathsMatchFrozenProductionFixtures()
    {
        using var doc = JsonDocument.Parse(File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "Fixtures", "vector-baseline.json")));
        var samples = doc.RootElement.GetProperty("Samples").EnumerateArray().ToArray(); Assert.Equal(4, samples.Length);
        foreach (var sample in samples)
        {
            int rows = sample.GetProperty("Rows").GetInt32(), cols = sample.GetProperty("Columns").GetInt32();
            var u = new double[rows, cols]; var v = new double[rows, cols];
            var uv = sample.GetProperty("U").EnumerateArray().Select(e => e.GetDouble()).ToArray();
            var vv = sample.GetProperty("V").EnumerateArray().Select(e => e.GetDouble()).ToArray();
            for (int r = 0; r < rows; r++) for (int c = 0; c < cols; c++) { u[r, c] = uv[r * cols + c]; v[r, c] = vv[r * cols + c]; }
            string expected = sample.GetProperty("Path").GetString()!;
            int width = sample.GetProperty("PlotWidth").GetInt32(), height = sample.GetProperty("PlotHeight").GetInt32(), stride = sample.GetProperty("Stride").GetInt32();
            Assert.Equal(expected, NodeVectorOverlay.PathData(u, v, width, height, stride, sample.GetProperty("Left").GetDouble(), sample.GetProperty("Top").GetDouble()));
            var svg = XElement.Parse(NC.FieldPlot(new double[rows, cols], new() { Arrows = (u, v), ArrowStride = stride, PlotWidth = width }));
            var path = svg.Elements().Single(e => e.Name.LocalName == "path" && e.Attribute("stroke-opacity")?.Value == "0.75");
            Assert.Equal(expected, path.Attribute("d")!.Value);
        }
    }

    [Fact]
    public void ZeroVectorsProduceNoPath() => Assert.Empty(NodeVectorOverlay.PathData(new double[2, 2], new double[2, 2], 20, 20, 1));

    [Theory]
    [InlineData(1, 0, "M16 64L34 64")]
    [InlineData(0, 1, "M16 64L16 46")]
    [InlineData(-1, 0, "M16 64L-2 64")]
    [InlineData(0, -1, "M16 64L16 82")]
    public void DirectionAndVerticalSignArePreserved(double uValue, double vValue, string start)
    {
        double[,] u = { { uValue, uValue }, { uValue, uValue } }, v = { { vValue, vValue }, { vValue, vValue } };
        Assert.StartsWith(start, NodeVectorOverlay.PathData(u, v, 20, 20, 1));
    }

    [Fact]
    public void NormalizationIncludesUnsampledMaximum()
    {
        double[,] u = { { 10, 1 }, { 1, 1 } };
        Assert.StartsWith("M36 44L39.6 44", NodeVectorOverlay.PathData(u, new double[2, 2], 20, 20, 2));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void InvalidStrideIsRejected(int stride)
        => Assert.Throws<ArgumentOutOfRangeException>(() => NodeVectorOverlay.PathData(new double[2, 2], new double[2, 2], 20, 20, stride));

    [Theory]
    [InlineData(double.NaN)]
    [InlineData(double.PositiveInfinity)]
    public void InvalidSamplesAreRejected(double value)
        => Assert.Throws<ArgumentException>(() => NodeVectorOverlay.PathData(new double[,] { { value, 0 }, { 0, 0 } }, new double[2, 2], 20, 20, 1));

    [Fact]
    public void ShapeAndProjectionAreValidated()
    {
        Assert.Throws<ArgumentException>(() => NodeVectorOverlay.PathData(new double[2, 2], new double[2, 3], 20, 20, 1));
        Assert.Throws<ArgumentException>(() => NodeVectorOverlay.PathData(new double[1, 1], new double[1, 1], 20, 20, 1));
        Assert.Throws<ArgumentException>(() => NodeVectorOverlay.PathData(new double[2, 2], new double[2, 2], 0, 20, 1));
    }

    [Fact]
    public void InputsAreNotModified()
    {
        double[,] u = { { 1, 2 }, { 3, 4 } }; var before = u.Cast<double>().ToArray();
        _ = NodeVectorOverlay.PathData(u, new double[2, 2], 20, 20, 1);
        Assert.Equal(before, u.Cast<double>().ToArray());
    }

    [Fact]
    public void ProductionPressureReportStillContainsVectorGeometry()
    {
        var cavity = new CavitySolver(17, 100); cavity.Run(40);
        var svg = XElement.Parse(Reports.CavityPressure(cavity));
        var path = svg.Elements().Single(e => e.Name.LocalName == "path" && e.Attribute("stroke-opacity")?.Value == "0.75");
        Assert.Contains("M", path.Attribute("d")!.Value);
    }
}
