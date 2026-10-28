using System.Xml.Linq;
using PlotSharp;
namespace PlotSharp.Tests;
public class ColorScaleTests
{
    private static readonly XNamespace Ns = "http://www.w3.org/2000/svg";
    [Theory] [InlineData(-10, 0)] [InlineData(0, 0)] [InlineData(5, 0.5)] [InlineData(10, 1)] [InlineData(100, 1)]
    public void NormalizesAndSaturates(double value, double expected) => Assert.Equal(expected, new ColorScale(0, 10).Normalize(value));
    [Fact] public void ConstantScaleIsCentral() => Assert.Equal(0.5, new ColorScale(7, 7).Normalize(10));
    [Fact] public void HugeFiniteInputDoesNotOverflow() => Assert.Equal(1, new ColorScale(-1, 1).Normalize(double.MaxValue));
    [Fact] public void NonFiniteInputRejected() => Assert.Throws<ArgumentOutOfRangeException>(() => new ColorScale(0, 1).Normalize(double.NaN));
    [Fact] public void InvalidScaleRejected() => Assert.Throws<ArgumentException>(() => new ColorScale(2, 1));
    [Fact] public void MaskExcludesNaNAndOutlierFromLimits()
    {
        var svg = XDocument.Parse(Charts.Heatmap(new double[,] { { double.NaN, 7 }, { 999, 3 } },
            new() { Mask = new bool[,] { { true, false }, { true, false } } }));
        var cells = svg.Descendants(Ns + "rect").Where(e => e.Attribute("data-row") != null).ToArray();
        Assert.Equal(2, cells.Length);
        Assert.Equal("#0d366b", cells[0].Attribute("fill")!.Value);
        Assert.Equal("#f4f8fd", cells[1].Attribute("fill")!.Value);
    }
    [Fact] public void MaskDimensionsValidated() => Assert.Throws<ArgumentException>(() => Charts.Heatmap(new double[2, 2], new() { Mask = new bool[1, 1] }));
    [Fact] public void FullyMaskedRequiresExplicitScale() => Assert.Throws<ArgumentException>(() => Charts.Heatmap(new double[1, 1], new() { Mask = new bool[,] { { true } } }));
    [Fact] public void FullyMaskedWithScaleProducesLegend()
    {
        var svg = XDocument.Parse(Charts.Heatmap(new double[,] { { double.NaN } }, new() { Mask = new bool[,] { { true } }, Min = 0, Max = 1, ShowColorbar = true }));
        Assert.DoesNotContain(svg.Descendants(Ns + "rect"), e => e.Attribute("data-row") != null);
        Assert.Single(svg.Descendants(Ns + "g"), e => e.Attribute("class")?.Value == "colorbar");
    }
    [Fact] public void ColorbarUsesFieldEndpointColorsAndEscapesLabel()
    {
        var svg = XDocument.Parse(Charts.Heatmap(new double[,] { { 0, 10 } }, new() { ShowColorbar = true, ColorbarLabel = "<°C>&" }));
        var cells = svg.Descendants(Ns + "rect").Where(e => e.Attribute("data-row") != null).ToArray();
        var legend = svg.Descendants(Ns + "g").Single(e => e.Attribute("class")?.Value == "colorbar");
        var strips = legend.Elements(Ns + "rect").ToArray(); Assert.Equal(128, strips.Length);
        Assert.Equal(cells[1].Attribute("fill")!.Value, strips[0].Attribute("fill")!.Value);
        Assert.Equal(cells[0].Attribute("fill")!.Value, strips[^1].Attribute("fill")!.Value);
        Assert.Equal("<°C>&", legend.Elements(Ns + "text").Last().Value);
        Assert.True(double.Parse(cells[1].Attribute("x")!.Value, System.Globalization.CultureInfo.InvariantCulture) < double.Parse(strips[0].Attribute("x")!.Value, System.Globalization.CultureInfo.InvariantCulture));
    }
    [Fact] public void ConstantColorbarUsesSingleColor()
    {
        var svg = XDocument.Parse(Charts.Heatmap(new double[,] { { 7 } }, new() { ShowColorbar = true }));
        var legend = svg.Descendants(Ns + "g").Single(e => e.Attribute("class")?.Value == "colorbar");
        Assert.Single(legend.Elements(Ns + "rect").Select(e => e.Attribute("fill")!.Value).Distinct());
        Assert.Equal("7", legend.Elements(Ns + "text").First().Value);
    }
    [Theory] [InlineData(false, "294")] [InlineData(true, "52")]
    public void ExtentTicksFollowOrientation(bool upper, string y)
    {
        var svg = XDocument.Parse(Charts.Heatmap(new double[2, 2], new() { Extent = new(-2, 2, 10, 20), OriginUpper = upper }));
        var tick = svg.Descendants(Ns + "text").Single(e => e.Value == "10");
        Assert.Equal(y, tick.Attribute("y")!.Value);
    }
    [Fact] public void InvalidExtentRejected() => Assert.Throws<ArgumentException>(() => Charts.Heatmap(new double[1, 1], new() { Extent = new(2, 1, 0, 1) }));
    [Fact] public void NarrowColorbarRejected() => Assert.Throws<ArgumentOutOfRangeException>(() => Charts.Heatmap(new double[1, 1], new() { Width = 200, ShowColorbar = true }));
}
