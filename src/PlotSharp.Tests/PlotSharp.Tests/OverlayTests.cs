using System.Xml.Linq;
using PlotSharp;
namespace PlotSharp.Tests;
public class OverlayTests
{
    private static readonly XNamespace Ns = "http://www.w3.org/2000/svg";
    [Fact] public void LinearFieldInterpolatesInIndexCoordinates()
    {
        var s = Assert.Single(Charts.MarchingSquares(new double[,] { { 0, 1 }, { 0, 1 } }, 0.5));
        Assert.Equal(new Charts.Segment(0.5, 0, 0.5, 1), s);
    }
    [Fact] public void SaddleHasStableEdgeOrderPairing()
    {
        var s = Charts.MarchingSquares(new double[,] { { 0, 1 }, { 1, 0 } }, 0.5);
        Assert.Equal(2, s.Count);
        Assert.Equal(new Charts.Segment(0.5, 0, 1, 0.5), s[0]);
        Assert.Equal(new Charts.Segment(0.5, 1, 0, 0.5), s[1]);
    }
    [Fact] public void ConstantAndSingletonHaveNoContours()
    {
        Assert.Empty(Charts.MarchingSquares(new double[2, 2], 0));
        Assert.Empty(Charts.MarchingSquares(new double[,] { { 1 } }, 0.5));
    }
    [Fact] public void EqualityDoesNotEmitZeroLengthSegments() => Assert.Empty(Charts.MarchingSquares(new double[,] { { 1, 0 }, { 0, 0 } }, 1));
    [Fact] public void MaskedCornerSkipsCell() => Assert.Empty(Charts.MarchingSquares(new double[,] { { double.NaN, 1 }, { 0, 1 } }, 0.5, new bool[,] { { true, false }, { false, false } }));
    [Fact] public void InvalidLevelRejected() => Assert.Throws<ArgumentOutOfRangeException>(() => Charts.MarchingSquares(new double[2, 2], double.NaN));
    [Fact] public void UnmaskedNaNRejected() => Assert.Throws<ArgumentException>(() => Charts.MarchingSquares(new double[,] { { double.NaN } }, 0));
    [Theory] [InlineData(false, "M300 229.5L300 108.5")] [InlineData(true, "M300 108.5L300 229.5")]
    public void ContoursMapBetweenCellCenters(bool upper, string path)
    {
        var svg = XDocument.Parse(Charts.Heatmap(new double[,] { { 0, 1 }, { 0, 1 } }, new() { OriginUpper = upper, ContourLevels = [0.5] }));
        var contour = svg.Descendants(Ns + "g").Single(e => e.Attribute("class")?.Value == "contours");
        Assert.Equal(path, contour.Element(Ns + "path")!.Attribute("d")!.Value);
        Assert.Single(svg.Descendants(Ns + "clipPath"));
    }
    [Theory] [InlineData(false, "M300 144.8")] [InlineData(true, "M300 193.2")]
    public void VectorDirectionUsesPhysicalExtentAndOrigin(bool upper, string prefix)
    {
        var svg = XDocument.Parse(Charts.Heatmap(new double[1, 1], new() { OriginUpper = upper, Extent = new(0, 10, 0, 10), Vectors = new(new double[,] { { 0 } }, new double[,] { { 1 } }) }));
        var arrow = svg.Descendants(Ns + "g").Single(e => e.Attribute("class")?.Value == "quiver");
        Assert.StartsWith(prefix, arrow.Element(Ns + "path")!.Attribute("d")!.Value);
    }
    [Fact] public void ZeroVectorsOmitted()
    {
        var svg = XDocument.Parse(Charts.Heatmap(new double[1, 1], new() { Vectors = new(new double[1, 1], new double[1, 1]) }));
        Assert.Empty(svg.Descendants(Ns + "g").Single(e => e.Attribute("class")?.Value == "quiver").Elements());
    }
    [Fact] public void ShapeMismatchRejected() => Assert.Throws<ArgumentException>(() => Charts.Heatmap(new double[2, 2], new() { Vectors = new(new double[1, 1], new double[1, 1]) }));
    [Fact] public void InvalidStrideRejected() => Assert.Throws<ArgumentException>(() => Charts.Heatmap(new double[1, 1], new() { Vectors = new(new double[1, 1], new double[1, 1], Stride: 0) }));
    [Fact] public void ColorInjectionRejected() => Assert.Throws<ArgumentException>(() => Charts.Heatmap(new double[1, 1], new() { ContourLevels = [0], ContourColor = "red' onload='evil" }));
    [Fact] public void StrideAndMaskReduceArrows()
    {
        var u = new double[,] { { 1, 1, 1 }, { 1, 1, 1 }, { 1, 1, 1 } };
        var svg = XDocument.Parse(Charts.Heatmap(new double[3, 3], new() { Vectors = new(u, new double[3, 3], Scale: 0.1, Stride: 2), Mask = new bool[,] { { true, false, false }, { false, false, false }, { false, false, false } } }));
        Assert.Equal(3, svg.Descendants(Ns + "g").Single(e => e.Attribute("class")?.Value == "quiver").Elements(Ns + "path").Count());
    }
}
