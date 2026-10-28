using System.Text.Json;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using NC = NavierStokes.Core.Charts;

namespace PlotSharp.Tests;

public class GradientColorbarTests
{
    static readonly XNamespace Ns = "http://www.w3.org/2000/svg";
    static string Normalize(XElement root) => Regex.Replace(string.Concat(root.Elements().SkipWhile(e => e.Name != Ns + "defs")
        .Select(e => e.ToString(SaveOptions.DisableFormatting))), "cb[0-9a-f]{8}", "cb-fixed");

    [Fact]
    public void MatchesFrozenProductionMarkup()
    {
        using var doc = JsonDocument.Parse(File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "Fixtures", "colorbar-baseline.json")));
        var samples = doc.RootElement.GetProperty("Samples").EnumerateArray().ToArray(); Assert.Equal(3, samples.Length);
        foreach (var sample in samples)
        {
            var map = sample.GetProperty("Diverging").GetBoolean() ? NC.ColorMap.Diverging : NC.ColorMap.Sequential;
            int width = sample.GetProperty("PlotWidth").GetInt32(); string label = sample.GetProperty("Label").GetString()!;
            string subtitle = sample.GetProperty("Subtitle").GetString()!;
            var actual = XElement.Parse(NC.FieldPlot(new double[2, 2], new() { Min = sample.GetProperty("Min").GetDouble(), Max = sample.GetProperty("Max").GetDouble(),
                Map = map, PlotWidth = width, ColorbarLabel = label, Subtitle = subtitle }));
            Assert.Equal(sample.GetProperty("Markup").GetString(), Normalize(actual));
            var fragment = GradientColorbar.Render(sample.GetProperty("ResolvedMin").GetDouble(), sample.GetProperty("ResolvedMax").GetDouble(), map.Hex,
                16 + width + 20, subtitle.Length > 0 ? 62 : 44, width, label);
            var root = XElement.Parse("<svg xmlns='http://www.w3.org/2000/svg'>" + fragment + "</svg>");
            Assert.Equal(sample.GetProperty("Markup").GetString(), Normalize(root));
        }
    }

    [Fact]
    public void StopCountLabelsEscapingAndOddHeightAnchorArePreserved()
    {
        var positions = new List<double>();
        var svg = XElement.Parse("<svg xmlns='http://www.w3.org/2000/svg'>" + GradientColorbar.Render(-3, 7,
            t => { positions.Add(t); return ColorPalette.Diverging.Hex(t); }, 20, 44, 37, "<&'") + "</svg>");
        Assert.Equal(Enumerable.Range(0, 11).Select(i => i / 10.0).ToArray(), positions.ToArray());
        Assert.Equal(11, svg.Descendants(Ns + "stop").Count());
        Assert.Equal(new[] { "-3", "2", "7", "<&'" }, svg.Elements(Ns + "text").Select(e => e.Value));
        Assert.Equal("translate(96,62) rotate(90)", svg.Elements(Ns + "text").Last().Attribute("transform")!.Value);
    }

    [Theory]
    [InlineData("red")]
    [InlineData("#abc")]
    [InlineData("#ffffff' onload='bad")]
    public void UnsafeOrUnsupportedColorsAreRejected(string color)
        => Assert.Throws<ArgumentException>(() => GradientColorbar.Render(0, 1, _ => color, 0, 0, 20, ""));

    [Theory]
    [InlineData(double.NaN, 1)]
    [InlineData(2, 1)]
    [InlineData(0, double.PositiveInfinity)]
    public void InvalidLimitsAreRejected(double min, double max)
        => Assert.Throws<ArgumentException>(() => GradientColorbar.Render(min, max, ColorPalette.Sequential.Hex, 0, 0, 20, ""));

    [Fact]
    public void InvalidProjectionAndNullCallbacksAreRejected()
    {
        Assert.Throws<ArgumentException>(() => GradientColorbar.Render(0, 1, ColorPalette.Sequential.Hex, 0, 0, 0, ""));
        Assert.Throws<ArgumentException>(() => GradientColorbar.Render(0, 1, ColorPalette.Sequential.Hex, double.NaN, 0, 20, ""));
        Assert.Throws<ArgumentNullException>(() => GradientColorbar.Render(0, 1, null!, 0, 0, 20, ""));
        Assert.Throws<ArgumentNullException>(() => GradientColorbar.Render(0, 1, ColorPalette.Sequential.Hex, 0, 0, 20, null!));
    }

    [Fact]
    public void ExistingHeatmapStripColorbarIsNotReplaced()
    {
        var root = XElement.Parse(Charts.Heatmap(new double[,] { { 1 } }, new() { ShowColorbar = true }));
        var bar = root.Descendants(Ns + "g").Single(e => e.Attribute("class")?.Value == "colorbar");
        Assert.Equal(128, bar.Elements(Ns + "rect").Count());
    }
}
