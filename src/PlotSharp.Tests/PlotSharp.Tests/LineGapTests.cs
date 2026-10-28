using System.Xml.Linq;

namespace PlotSharp.Tests;

public class LineGapTests
{
    static Charts.Series Series(double[] y) => new("A<&", Enumerable.Range(0, y.Length).Select(i => (double)i).ToArray(), y, "#123456");
    static readonly XNamespace Svg = "http://www.w3.org/2000/svg";

    [Fact]
    public void SpectrumCanNameItsIndexAxis()
    {
        var doc = XDocument.Parse(IndexedCharts.Stems("spectrum", [1, 2], band: null, firstLag: 0, xLabel: "k"));
        Assert.Contains(doc.Descendants(Svg + "text"), e => e.Value == "k");
        Assert.DoesNotContain(doc.Descendants(Svg + "text"), e => e.Value == "Lag");
    }

    [Fact]
    public void DefaultStillRejectsNaNY()
        => Assert.Throws<ArgumentException>(() => Charts.LineChart([Series([1, double.NaN, 2])], new()));

    [Fact]
    public void GapRestartsPathAndSkipsMarkersAndTooltips()
    {
        var doc = XDocument.Parse(Charts.LineChart([Series([double.NaN, 1, 2, double.NaN, 3, double.NaN]) with { Markers = true, Dashed = true }],
            new() { AllowLineGaps = true, PointTooltips = true }));
        var path = Assert.Single(doc.Descendants(Svg + "path"));
        var data = path.Attribute("d")!.Value;
        Assert.Equal(2, data.Count(c => c == 'M'));
        Assert.Equal(1, data.Count(c => c == 'L'));
        Assert.Equal("6 4", path.Attribute("stroke-dasharray")!.Value);
        Assert.Equal(6, doc.Descendants(Svg + "circle").Count());
        Assert.Equal(3, doc.Descendants(Svg + "title").Count());
        Assert.Contains("A<&", doc.Descendants(Svg + "title").First().Value);
        Assert.DoesNotContain("NaN", doc.ToString());
        Assert.DoesNotContain("Infinity", doc.ToString());
    }

    [Theory]
    [InlineData(double.PositiveInfinity)]
    [InlineData(double.NegativeInfinity)]
    public void InfinityIsNeverAGap(double infinity)
        => Assert.Throws<ArgumentException>(() => Charts.LineChart([Series([1, infinity])], new() { AllowLineGaps = true }));

    [Fact]
    public void AllMissingSeriesIsRejectedEvenWithExplicitBounds()
        => Assert.Throws<ArgumentException>(() => Charts.LineChart([Series([double.NaN])], new() { AllowLineGaps = true, YMin = 0, YMax = 1 }));

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void StemsAndBarsRemainStrict(bool stems)
        => Assert.Throws<ArgumentException>(() => Charts.LineChart([Series([1, double.NaN]) with { Stems = stems, BarWidth = stems ? null : 0.5 }], new() { AllowLineGaps = true }));

    [Fact]
    public void IndexedFacadePreservesMissingIndexAndDoesNotMutateInput()
    {
        double[] y = [1, double.NaN, 2];
        var doc = XDocument.Parse(IndexedCharts.Lines("gaps", new IndexedCharts.Line("s", y, Start: 10)));
        Assert.Equal(2, Assert.Single(doc.Descendants(Svg + "path")).Attribute("d")!.Value.Count(c => c == 'M'));
        var titles = doc.Descendants(Svg + "title").Select(e => e.Value).ToArray();
        Assert.Contains(titles, t => t.Contains("x=10"));
        Assert.Contains(titles, t => t.Contains("x=12"));
        Assert.True(double.IsNaN(y[1]));
        XDocument.Parse(IndexedCharts.Panels("panels", new IndexedCharts.Line("p", y)));
    }
}
