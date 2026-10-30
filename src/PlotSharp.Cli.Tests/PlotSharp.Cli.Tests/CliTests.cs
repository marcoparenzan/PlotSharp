using System.Xml.Linq;

namespace PlotSharp.Cli.Tests;

public class CliTests : IDisposable
{
    private readonly string _dir = Directory.CreateTempSubdirectory("plotsharp-cli-").FullName;
    private string Csv(string text) { var p = Path.Combine(_dir, Guid.NewGuid().ToString("N") + ".csv"); File.WriteAllText(p, text); return p; }
    public void Dispose() => Directory.Delete(_dir, true);

    private static (int Code, string Out, string Err) Run(params string[] args)
    {
        var o = new StringWriter(); var e = new StringWriter();
        return (PlotCli.Run(args, o, e), o.ToString(), e.ToString());
    }

    private const string Data = "day,a,b\n0,1,2\n1,3,\n2,2,5\n3,4,6\n";

    [Fact]
    public void NoArgumentsPrintsUsage()
    {
        var (code, output, _) = Run();
        Assert.Equal(0, code);
        Assert.Contains("plotsharp line", output);
    }

    [Fact]
    public void UnknownCommandExitsOne() => Assert.Equal(1, Run("pie").Code);

    [Fact]
    public void LineChartIsWellFormedSvgWithTitleAndBothSeries()
    {
        var (code, svg, _) = Run("line", Csv(Data), "--x", "day", "--y", "a,b", "--title", "Demo");
        Assert.Equal(0, code);
        var root = XElement.Parse(svg.Trim());
        Assert.Equal("svg", root.Name.LocalName);
        Assert.Contains("Demo", svg);
        Assert.Contains(">a<", svg);
        Assert.Contains(">b<", svg);
    }

    [Fact]
    public void LineChartWritesFileWhenOutIsGiven()
    {
        var path = Path.Combine(_dir, "o.svg");
        var (code, output, _) = Run("line", Csv(Data), "--y", "a", "--out", path);
        Assert.Equal(0, code);
        Assert.Contains("1 series, 4 rows", output);
        Assert.StartsWith("<svg", File.ReadAllText(path));
    }

    [Fact]
    public void ScatterDropsRowsWithMissingValues()
    {
        var (code, svg, _) = Run("scatter", Csv(Data), "--x", "a", "--y", "b");
        Assert.Equal(0, code);
        XElement.Parse(svg.Trim());
    }

    [Theory]
    [InlineData("line", "--y", "missing")]
    [InlineData("scatter", "--y", "a")]
    [InlineData("line", "--x", "day")]
    public void BadArgumentsExitTwo(string command, string option, string value)
    {
        var args = new List<string> { command, Csv(Data), option, value };
        Assert.Equal(2, Run(args.ToArray()).Code);
    }

    [Fact]
    public void MissingFileAndBadSizeExitTwo()
    {
        Assert.Equal(2, Run("line", Path.Combine(_dir, "none.csv"), "--y", "a").Code);
        Assert.Equal(2, Run("line", Csv(Data), "--y", "a", "--width", "5").Code);
        Assert.Equal(2, Run("line", Csv("only,header\n"), "--y", "only").Code);
    }

    [Fact]
    public void SelfTestPasses() => Assert.Equal(0, Run("selftest").Code);
}
