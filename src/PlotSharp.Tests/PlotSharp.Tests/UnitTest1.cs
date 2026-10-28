using System.Globalization;
using System.Xml.Linq;
using PlotSharp;
namespace PlotSharp.Tests;
public class ChartsTests
{
    private static Charts.Series Sample(bool line = true) => new("A<&", [0, 1], [2, 3], "#123456", true, line);
    [Fact] public void SvgIsXmlAndEscapesText()
    {
        var doc = XDocument.Parse(Charts.LineChart([Sample(), Sample(false)], new() { Title = "<script>&'", XLabel = "x<&" }));
        XNamespace ns = "http://www.w3.org/2000/svg";
        Assert.Equal("<script>&'", doc.Descendants(ns + "text").First().Value);
        Assert.Equal(4, doc.Descendants(ns + "g").Single().Descendants(ns + "circle").Count());
        Assert.Equal(6, doc.Descendants(ns + "circle").Count()); // Includes two legend markers.
        Assert.Single(doc.Descendants(ns + "path"));
    }
    [Fact] public void RejectsEmpty() => Assert.Throws<ArgumentException>(() => Charts.LineChart([], new()));
    [Fact] public void RejectsMismatched() => Assert.Throws<ArgumentException>(() => Charts.LineChart([new("", [1], [], "#123456")], new()));
    [Theory] [InlineData(double.NaN)] [InlineData(double.PositiveInfinity)]
    public void RejectsNonFinite(double v) => Assert.Throws<ArgumentException>(() => Charts.LineChart([new("", [v], [1], "#123456")], new()));
    [Fact] public void RejectsInjection() => Assert.Throws<ArgumentException>(() => Charts.LineChart([Sample() with { Color = "red' onload='evil" }], new()));
    [Fact] public void ConstantSeriesWorks() => XDocument.Parse(Charts.LineChart([new("", [1, 1], [2, 2], "#123456")], new()));
    [Fact] public void BoundsValidated() => Assert.Throws<ArgumentException>(() => Charts.LineChart([Sample()], new() { XMin = 5, XMax = 1 }));
    [Fact] public void DimensionsValidated() => Assert.Throws<ArgumentOutOfRangeException>(() => Charts.LineChart([Sample()], new() { Width = 10 }));
    [Fact] public void TicksAreOrderedAndCoverRange()
    {
        var ticks = Charts.NiceTicks(-3, 7);
        Assert.True(ticks[0] <= -3 && ticks[^1] >= 7);
        Assert.True(ticks.Zip(ticks.Skip(1)).All(p => p.First < p.Second));
    }
    [Fact] public void InvalidTicksRejected() => Assert.Throws<ArgumentOutOfRangeException>(() => Charts.NiceTicks(0, 1, 0));
    [Fact] public void CultureIndependent()
    {
        var saved = CultureInfo.CurrentCulture;
        try { CultureInfo.CurrentCulture = new("it-IT"); Assert.Equal("1.25", Charts.Num(1.25)); XDocument.Parse(Charts.LineChart([Sample()], new())); }
        finally { CultureInfo.CurrentCulture = saved; }
    }
}
