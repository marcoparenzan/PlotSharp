using System.Xml.Linq;

namespace PlotSharp.Tests;

public class BoundaryMarkerTests
{
    static readonly XNamespace Svg = "http://www.w3.org/2000/svg";
    static Charts.Series Sample() => new("sample", [-0.01, 0, 1, 1.01], [0.5, 0, 1, 0.5], "#123456", Markers: true);
    static Charts.LineChartOptions Options(bool preserve) => new() { XMin = 0, XMax = 1, YMin = 0, YMax = 1, PreserveBoundaryMarkers = preserve, PointTooltips = true };

    [Fact]
    public void OptInKeepsFullBoundaryPointsButGeometryClipUnchanged()
    {
        var svg = XElement.Parse(Charts.LineChart([Sample()], Options(true)));
        var geometry = svg.Elements(Svg + "g").Single(g => g.Attribute("clip-path") is not null);
        var overlay = svg.Elements(Svg + "g").Single(g => g.Attribute("class")?.Value == "point-overlays");
        Assert.Null(overlay.Attribute("clip-path"));
        Assert.Empty(geometry.Descendants(Svg + "circle"));
        Assert.Single(geometry.Elements(Svg + "path"));
        Assert.Equal(4, overlay.Elements(Svg + "circle").Count()); // two markers and two tooltip hit targets
        var visible = overlay.Elements(Svg + "circle").Where(c => c.Attribute("fill")?.Value != "transparent").ToArray();
        Assert.Equal("64", visible[0].Attribute("cx")!.Value);
        Assert.Equal("290", visible[0].Attribute("cy")!.Value);
        Assert.Equal("536", visible[1].Attribute("cx")!.Value);
        Assert.Equal("48", visible[1].Attribute("cy")!.Value);
        var clip = Assert.Single(svg.Elements(Svg + "clipPath")).Element(Svg + "rect")!;
        Assert.Equal("64", clip.Attribute("x")!.Value);
        Assert.Equal("48", clip.Attribute("y")!.Value);
        Assert.Equal("472", clip.Attribute("width")!.Value);
        Assert.Equal("242", clip.Attribute("height")!.Value);
    }

    [Fact]
    public void LegacyDefaultPreservesClipping()
    {
        var svg = XElement.Parse(Charts.LineChart([Sample()], Options(false)));
        Assert.DoesNotContain(svg.Elements(Svg + "g"), g => g.Attribute("class")?.Value == "point-overlays");
        Assert.Equal(8, Assert.Single(svg.Elements(Svg + "g")).Elements(Svg + "circle").Count());
    }

    [Fact]
    public void PointsJustOutsideYBoundsAreOmittedNotProjectedOntoBoundary()
    {
        var series = new Charts.Series("s", [0.5, 0.5, 0.5], [-0.0001, 0.5, 1.0001], "#123456", Markers: true);
        var svg = XElement.Parse(Charts.LineChart([series], Options(true)));
        var overlay = svg.Elements(Svg + "g").Single(g => g.Attribute("class")?.Value == "point-overlays");
        Assert.Equal(2, overlay.Elements(Svg + "circle").Count());
        Assert.Single(overlay.Descendants(Svg + "title"));
    }

    [Fact]
    public void IndexedStemsUseOverlayForPositiveNegativeAndZeroPoints()
    {
        var svg = XElement.Parse(IndexedCharts.Stems("s", [0, 1, -1]));
        var overlay = svg.Elements(Svg + "g").Single(g => g.Attribute("class")?.Value == "point-overlays");
        Assert.Equal(3, overlay.Elements(Svg + "circle").Count(e => e.Attribute("fill")?.Value != "transparent"));
        var geometry = svg.Elements(Svg + "g").Single(g => g.Attribute("clip-path") is not null);
        Assert.Equal(3, geometry.Elements(Svg + "line").Count());
    }
}
