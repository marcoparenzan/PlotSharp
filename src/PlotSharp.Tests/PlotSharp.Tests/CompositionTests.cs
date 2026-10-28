using System.Xml.Linq;
using PlotSharp;
namespace PlotSharp.Tests;
public class CompositionTests
{
    private static readonly XNamespace Ns = "http://www.w3.org/2000/svg";
    [Fact] public void IndexedConversionPreservesOffsetAndCopiesValues()
    {
        double[] values = [3, 4];
        var series = IndexedCharts.ToSeries(new("A", values, -2, true));
        values[0] = 99;
        Assert.Equal(new double[] { -2, -1 }, series.X);
        Assert.Equal(3, series.Y[0]); Assert.True(series.Dashed);
    }
    [Fact] public void DashedAppliesToPathAndLegend()
    {
        var svg = XDocument.Parse(IndexedCharts.Lines("T", new("A", [0, 1], Dashed: true), new("B", [2, 3])));
        Assert.Equal("6 4", svg.Descendants(Ns + "path").First().Attribute("stroke-dasharray")?.Value);
        Assert.Contains(svg.Descendants(Ns + "line"), e => e.Attribute("stroke-dasharray")?.Value == "6 4");
    }
    [Fact] public void PanelsHaveIndependentScalesAndUniqueClips()
    {
        var html = XDocument.Parse(IndexedCharts.Panels("<T>&", new("A", [0, 1]), new("B", [100, 101])));
        Assert.Equal("<T>&", html.Descendants("h2").Single().Value);
        var svgs = html.Descendants(Ns + "svg").ToArray(); Assert.Equal(2, svgs.Length);
        Assert.Equal(svgs[0].Descendants(Ns + "path").Single().Attribute("d")?.Value,
            svgs[1].Descendants(Ns + "path").Single().Attribute("d")?.Value);
        Assert.Equal(2, html.Descendants(Ns + "clipPath").Select(e => e.Attribute("id")!.Value).Distinct().Count());
    }
    [Fact] public void MappingAndClipMatchExplicitPlotArea()
    {
        var svg = XDocument.Parse(Charts.LineChart([new("A", [0, 10], [0, 10], "#123456", true)],
            new() { XMin = 0, XMax = 10, YMin = 0, YMax = 10 }));
        var circles = svg.Descendants(Ns + "circle").ToArray();
        Assert.Equal("64", circles[0].Attribute("cx")?.Value); Assert.Equal("290", circles[0].Attribute("cy")?.Value);
        Assert.Equal("536", circles[1].Attribute("cx")?.Value); Assert.Equal("48", circles[1].Attribute("cy")?.Value);
        var rect = svg.Descendants(Ns + "clipPath").Single().Element(Ns + "rect")!;
        Assert.Equal("48", rect.Attribute("y")?.Value); Assert.Equal("242", rect.Attribute("height")?.Value);
    }
    [Fact] public void EmptyPanelsRejected() => Assert.Throws<ArgumentException>(() => IndexedCharts.Panels("T"));
    [Fact] public void NonFiniteFacadeDataRejected() => Assert.Throws<ArgumentException>(() => IndexedCharts.Lines("T", new IndexedCharts.Line("A", [double.NaN])));
    [Fact] public void NullTextRejected() => Assert.Throws<ArgumentException>(() => Charts.LineChart([new("A", [0], [0], "#123456")], new() { Title = null! }));
}
