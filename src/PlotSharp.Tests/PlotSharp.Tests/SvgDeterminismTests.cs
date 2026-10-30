using System.Text.RegularExpressions;

namespace PlotSharp.Tests;

/// <summary>
/// The charts used to name clip paths and gradients with random GUIDs, so the same chart gave a different SVG on every run and every saved notebook
/// changed on every execution. The ids are now derived from the definition they name.
/// </summary>
public class SvgDeterminismTests
{
    private static readonly double[] X = [0, 1, 2, 3];
    private static readonly double[] Y = [0, 1, 4, 9];

    private static string Line(int width = 560) =>
        Charts.LineChart([new Charts.Series("s", X, Y, Charts.Series1)], new Charts.LineChartOptions { Title = "t", Width = width });

    private static string[] Ids(string svg, string element) =>
        Regex.Matches(svg, $"<{element} id='([^']+)'").Select(m => m.Groups[1].Value).ToArray();

    [Fact]
    public void TheSameLineChartIsTheSameSvgEveryTime() => Assert.Equal(Line(), Line());

    [Fact]
    public void ALineChartClipPathHasAStableIdOfTheExpectedShape()
    {
        var id = Assert.Single(Ids(Line(), "clipPath"));
        Assert.Matches("^clip[0-9a-f]{12}$", id);
        Assert.Equal(id, Assert.Single(Ids(Line(), "clipPath")));
    }

    [Fact]
    public void ChartsWithDifferentGeometryGetDifferentClipIds() =>
        Assert.NotEqual(Ids(Line(560), "clipPath")[0], Ids(Line(800), "clipPath")[0]);

    [Fact]
    public void DifferentSeedsGiveDifferentIdsToChartsOfTheSameGeometry()
    {
        string Seeded(string seed) => Charts.LineChart([new Charts.Series("s", X, Y, Charts.Series1)], new Charts.LineChartOptions { IdSeed = seed });
        Assert.NotEqual(Ids(Seeded("a"), "clipPath")[0], Ids(Seeded("b"), "clipPath")[0]);
        Assert.Equal(Seeded("a"), Seeded("a"));
    }

    [Fact]
    public void PanelsKeepTheirClipIdsUniqueAndStable()
    {
        string Html() => IndexedCharts.Panels("p", new("A", [0, 1]), new("B", [0, 1]), new("C", [0, 1]));
        var ids = Ids(Html(), "clipPath");
        Assert.Equal(3, ids.Distinct().Count());
        Assert.Equal(ids, Ids(Html(), "clipPath"));
    }

    [Fact]
    public void EveryReferencedIdIsDefined()
    {
        var svg = Line();
        var id = Ids(svg, "clipPath")[0];
        Assert.Contains($"clip-path='url(#{id})'", svg);
    }

    [Fact]
    public void AHeatmapWithContoursIsReproducible()
    {
        double[,] field = { { 0, 1, 2 }, { 1, 2, 3 }, { 2, 3, 4 } };
        string Make() => Charts.Heatmap(field, new Charts.HeatmapOptions { ContourLevels = [1.5, 2.5], ShowColorbar = true, ColorbarLabel = "v" });
        var svg = Make();
        Assert.Equal(svg, Make());
        Assert.Matches("^field[0-9a-f]{12}$", Assert.Single(Ids(svg, "clipPath")));
    }

    [Fact]
    public void AGradientColorbarIsReproducibleAndItsIdFollowsItsColors()
    {
        string Bar(Func<double, string> color) => GradientColorbar.Render(0, 1, color, 500, 40, 200, "value");
        string Grey(double t) => $"#{(int)(t * 255):x2}{(int)(t * 255):x2}{(int)(t * 255):x2}";
        string Red(double t) => $"#{(int)(t * 255):x2}0000";

        Assert.Equal(Bar(Grey), Bar(Grey));
        var grey = Assert.Single(Ids(Bar(Grey), "linearGradient"));
        var red = Assert.Single(Ids(Bar(Red), "linearGradient"));
        Assert.Matches("^cb[0-9a-f]{8}$", grey);
        Assert.NotEqual(grey, red);
    }
}
