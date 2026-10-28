using System.Xml.Linq;

namespace PlotSharp.Tests;

public class LegendRowTests
{
    static readonly XNamespace Svg = "http://www.w3.org/2000/svg";
    static Charts.Series[] Series() => [new("long legend", [0,1], [1,2], "#123456"),new("other", [0,1], [2,3], "#654321")];

    [Theory]
    [InlineData("", 66, 56)]
    [InlineData("subtitle", 84, 74)]
    public void OptInAddsSeparateRow(string subtitle, int plotTop, int legendBaseline)
    {
        var svg = XElement.Parse(Charts.LineChart(Series(),new() { Title="Long title", Subtitle=subtitle, SeparateLegendRow=true }));
        Assert.Equal(plotTop.ToString(),Assert.Single(svg.Descendants(Svg+"clipPath")).Element(Svg+"rect")!.Attribute("y")!.Value);
        Assert.Equal(legendBaseline.ToString(),svg.Elements(Svg+"text").Single(t=>t.Value=="long legend").Attribute("y")!.Value);
    }

    [Fact]
    public void DefaultAndSingleSeriesPreserveLayout()
    {
        foreach(var pair in new[]{(Series(),false),(new[]{Series()[0]},true)})
        {
            var svg=XElement.Parse(Charts.LineChart(pair.Item1,new() { SeparateLegendRow=pair.Item2 }));
            Assert.Equal("48",Assert.Single(svg.Descendants(Svg+"clipPath")).Element(Svg+"rect")!.Attribute("y")!.Value);
        }
    }

    [Fact]
    public void IndexedStemsEnableTooltipsAndSeparateRow()
    {
        var svg=XElement.Parse(IndexedCharts.Stems("Long ACF title", [0.2, -0.3], 0.1));
        Assert.Equal("66",Assert.Single(svg.Descendants(Svg+"clipPath")).Element(Svg+"rect")!.Attribute("y")!.Value);
        Assert.Equal(6,svg.Descendants(Svg+"title").Count()); // two stems and four band endpoints
    }
}
