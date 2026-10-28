using System.Xml.Linq;
using PlotSharp;
namespace PlotSharp.Tests;
public class FieldAndStemTests
{
    private static readonly XNamespace Ns = "http://www.w3.org/2000/svg";
    [Fact] public void StemsUseZeroBaselineAndBands()
    {
        var svg = XDocument.Parse(IndexedCharts.Stems("ACF", [0.5, -0.5], 0.2));
        var g = svg.Descendants(Ns + "g").Single(e => e.Attribute("clip-path") is not null);
        var stems = g.Elements(Ns + "line").ToArray(); Assert.Equal(2, stems.Length);
        Assert.Equal(stems[0].Attribute("y1")!.Value, stems[1].Attribute("y1")!.Value);
        Assert.Empty(g.Elements(Ns + "circle"));
        Assert.Equal(2, svg.Descendants(Ns + "g").Single(e => e.Attribute("class")?.Value == "point-overlays").Elements(Ns + "circle").Count(e => e.Attribute("fill")?.Value != "transparent"));
        Assert.Equal(2, g.Elements(Ns + "path").Count(e => e.Attribute("stroke-dasharray")?.Value == "6 4"));
    }
    [Fact] public void InvalidBandRejected() => Assert.Throws<ArgumentOutOfRangeException>(() => IndexedCharts.Stems("", [1], -1));
    [Fact] public void BarsMapPositiveAndNegativeValues()
    {
        var svg = XDocument.Parse(Charts.LineChart([new("B", [0, 1], [2, -2], "#123456", Line: false, BarWidth: 0.8)],
            new() { XMin = -0.5, XMax = 1.5, YMin = -2, YMax = 2 }));
        var bars = svg.Descendants(Ns + "g").Single().Elements(Ns + "rect").ToArray();
        Assert.Equal(2, bars.Length);
        Assert.Equal("121", bars[0].Attribute("height")?.Value);
        Assert.Equal("121", bars[1].Attribute("height")?.Value);
        Assert.Equal("48", bars[0].Attribute("y")?.Value);
        Assert.Equal("169", bars[1].Attribute("y")?.Value);
    }
    [Fact] public void InvalidBarWidthRejected() => Assert.Throws<ArgumentException>(() => Charts.LineChart([new("", [0], [1], "#123456", BarWidth: 0)], new()));
    [Theory] [InlineData(false, "169")] [InlineData(true, "48")]
    public void HeatmapOrientation(bool upper, string expected)
    {
        var svg = XDocument.Parse(Charts.Heatmap(new double[,] { { 0, 1 }, { 2, 3 } }, new() { OriginUpper = upper, Title = "<&" }));
        var cells = svg.Descendants(Ns + "g").Single().Elements(Ns + "rect").ToArray();
        Assert.Equal(4, cells.Length); Assert.Equal(expected, cells[0].Attribute("y")?.Value);
        Assert.Equal("#f4f8fd", cells[0].Attribute("fill")?.Value);
        Assert.Equal("#0d366b", cells[^1].Attribute("fill")?.Value);
        Assert.Equal("<&", svg.Descendants(Ns + "text").First().Value);
    }
    [Fact] public void SingletonConstantHeatmapWorks()
    {
        var svg = XDocument.Parse(Charts.Heatmap(new double[,] { { 7 } }));
        Assert.Equal(Charts.SequentialColor(0.5), svg.Descendants(Ns + "g").Single().Element(Ns + "rect")!.Attribute("fill")!.Value);
    }
    [Fact] public void HeatmapClipsColorsToExplicitLimits()
    {
        var svg = XDocument.Parse(Charts.Heatmap(new double[,] { { -10, 10 } }, new() { Min = 0, Max = 1 }));
        Assert.Equal(new[] { "#f4f8fd", "#0d366b" }, svg.Descendants(Ns + "g").Single().Elements(Ns + "rect").Select(e => e.Attribute("fill")!.Value));
    }
    [Fact] public void EmptyHeatmapRejected() => Assert.Throws<ArgumentException>(() => Charts.Heatmap(new double[0, 1]));
    [Fact] public void NonFiniteHeatmapRejected() => Assert.Throws<ArgumentException>(() => Charts.Heatmap(new double[,] { { double.NaN } }));
    [Fact] public void ReversedColorLimitsRejected() => Assert.Throws<ArgumentException>(() => Charts.Heatmap(new double[,] { { 1 } }, new() { Min = 2, Max = 1 }));
    [Fact] public void OversizedHeatmapRejected() => Assert.Throws<ArgumentException>(() => Charts.Heatmap(new double[400, 400]));
}
