using System.Xml.Linq;
using NC = NavierStokes.Core.Charts;

namespace PlotSharp.Tests;

public class NavierContourTests
{
    [Fact]
    public void ExactVertexRetainsLegacyZeroLengthSegment()
    {
        double[,] field = { { 0, -1 }, { -1, -1 } };
        Assert.Empty(Charts.MarchingSquares(field, 0)); // default policy unchanged
        Assert.Equal(new[] { (0d, 0d, 0d, 0d) }, NC.MarchingSquares(field, 0).ToArray());
        Assert.Single(Charts.MarchingSquares(field, 0, null, preserveDegenerateSegments: true));
    }

    [Fact]
    public void AllSixteenBinaryCellsPreserveFixedEdgePairing()
    {
        (double X, double Y)[] edgeMidpoints = [(0.5, 0), (1, 0.5), (0.5, 1), (0, 0.5)];
        for (int mask = 0; mask < 16; mask++)
        {
            var values = Enumerable.Range(0, 4).Select(i => (mask & (1 << i)) != 0 ? 1d : -1d).ToArray();
            double[,] field = { { values[0], values[1] }, { values[3], values[2] } };
            var crossings = new List<(double X, double Y)>();
            for (int edge = 0; edge < 4; edge++)
                if (values[edge] * values[(edge + 1) % 4] < 0) crossings.Add(edgeMidpoints[edge]);
            var expected = new List<(double, double, double, double)>();
            for (int edge = 0; edge < crossings.Count; edge += 2)
                expected.Add((crossings[edge].X, crossings[edge].Y, crossings[edge + 1].X, crossings[edge + 1].Y));
            Assert.Equal(expected.ToArray(), NC.MarchingSquares(field, 0).ToArray());
        }
    }

    [Fact]
    public void RectangularGridKeepsSampleCoordinatesAndRowOrder()
    {
        double[,] field = { { 0, 1, 2, 3 }, { 0, 1, 2, 3 }, { 0, 1, 2, 3 } };
        Assert.Equal(new[] { (1.5, 0d, 1.5, 1d), (1.5, 1d, 1.5, 2d) }, NC.MarchingSquares(field, 1.5).ToArray());
    }

    [Fact]
    public void InputIsNotModified()
    {
        double[,] field = { { 1, -1 }, { -1, 1 } };
        var before = field.Cast<double>().ToArray();
        _ = NC.MarchingSquares(field, 0).ToArray();
        Assert.Equal(before, field.Cast<double>().ToArray());
    }

    [Theory]
    [InlineData(double.NaN)]
    [InlineData(double.PositiveInfinity)]
    public void SharedFiniteValidationAppliesAtEnumeration(double value)
        => Assert.Throws<ArgumentException>(() => NC.MarchingSquares(new double[,] { { 0, value } }, 0).ToArray());

    [Theory]
    [InlineData(0, 0)]
    [InlineData(1, 100001)]
    public void SharedBoundedContractRejectsUnsupportedShapes(int rows, int columns)
        => Assert.Throws<ArgumentException>(() => NC.MarchingSquares(new double[rows, columns], 0).ToArray());

    [Fact]
    public void InvalidLevelIsRejected()
        => Assert.Throws<ArgumentOutOfRangeException>(() => NC.MarchingSquares(new double[,] { { 0, 1 }, { 0, 1 } }, double.NaN).ToArray());

    [Fact]
    public void ConstantAndSingletonFieldsHaveNoContour()
    {
        Assert.Empty(NC.MarchingSquares(new double[,] { { 1 } }, 1));
        Assert.Empty(NC.MarchingSquares(new double[,] { { 1, 1 }, { 1, 1 } }, 1));
    }

    [Fact]
    public void FieldReportRetainsPngAndOriginalIndexToSvgProjection()
    {
        double[,] field = { { 0, 1, 2 }, { 0, 1, 2 }, { 0, 1, 2 } };
        var svg = XElement.Parse(NC.FieldPlot(field, new() { Min = 0, Max = 2, Contours = field, ContourLevels = 1, PlotWidth = 60 }));
        XNamespace ns = "http://www.w3.org/2000/svg";
        Assert.StartsWith("data:image/png;base64,", Assert.Single(svg.Elements(ns + "image")).Attribute("href")!.Value);
        var contour = Assert.Single(svg.Elements(ns + "path"), p => p.Attribute("stroke")?.Value == "#ffffff");
        Assert.Equal("M46 104L46 74M46 74L46 44", contour.Attribute("d")!.Value);
    }
}
