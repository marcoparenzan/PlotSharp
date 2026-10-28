using System.Text.RegularExpressions;
using System.Xml.Linq;
using NavierStokes.Core;
using NC = NavierStokes.Core.Charts;

namespace PlotSharp.Tests;

public class NavierAdoptionTests
{
    static string Normalize(string svg) => Regex.Replace(svg, "clip[0-9a-f]+", "clip-id");

    [Fact]
    public void FacadeMapsEveryOptionWithoutMutatingInputs()
    {
        double[] x = [0, 1, 2], y = [1, 3, 2];
        var options = new NC.LineChartOptions { Title="Title<&", Subtitle="Sub", XLabel="time", YLabel="value", Width=720, Height=380,
            XMin=-1, XMax=3, YMin=0, YMax=4, Highlight=(1,3,"peak<&") };
        var actual=NC.LineChart([new("data",x,y,"#123456",true)],options);
        var expected=Charts.LineChart([new("data",x,y,"#123456",true)],new() { Title=options.Title,Subtitle=options.Subtitle,XLabel=options.XLabel,YLabel=options.YLabel,
            Width=720,Height=380,XMin=-1,XMax=3,YMin=0,YMax=4,Highlight=options.Highlight,
            PreserveBoundaryMarkers=true,SeparateLegendRow=true,PointTooltips=true });
        Assert.Equal(Normalize(expected),Normalize(actual));
        Assert.Equal(new double[]{0,1,2},x);Assert.Equal(new double[]{1,3,2},y);
    }

    [Theory]
    [InlineData(double.NaN)]
    [InlineData(double.PositiveInfinity)]
    public void StrictSharedValidationRejectsNonFinite(double value)
        => Assert.Throws<ArgumentException>(()=>NC.LineChart([new("s",[0,1],[1,value],"#123456")],new()));

    [Fact]
    public void ReversedBoundsAreRejected()
        => Assert.Throws<ArgumentException>(()=>NC.LineChart([new("s",[0,1],[1,2],"#123456")],new() { XMin=2,XMax=1 }));

    [Fact]
    public void CavityReportsUseSharedPointAndLegendPolicies()
    {
        var cavity=new CavitySolver(17,100);cavity.Run(40);
        var ghia=XElement.Parse(Reports.CavityGhia(cavity));
        XNamespace ns="http://www.w3.org/2000/svg";
        Assert.Equal("720",ghia.Attribute("width")!.Value);
        Assert.Contains(ghia.Elements(ns+"g"),g=>g.Attribute("class")?.Value=="point-overlays");
        Assert.NotEmpty(ghia.Descendants(ns+"title"));
        XElement.Parse(Reports.CavityEnergy(cavity));
    }

    [Fact]
    public void CylinderProbeRemainsAnExecutableReport()
    {
        var cylinder=new CylinderLbm(100,80,40,8);cylinder.Run(40);
        Assert.NotEmpty(cylinder.ProbeV);
        var svg=XElement.Parse(Reports.CylinderProbe(cylinder));
        Assert.Contains(svg.Descendants(),e=>e.Name.LocalName=="title");
    }

    [Fact]
    public void PreCancelledCavityRunDoesNotAdvance()
    {
        var solver = new CavitySolver(17, 100);
        using var cancellation = new CancellationTokenSource();
        cancellation.Cancel();
        var error = Assert.Throws<OperationCanceledException>(() => solver.Run(10, cancellation: cancellation.Token));
        Assert.Equal(cancellation.Token, error.CancellationToken);
        Assert.Equal(0, solver.Step);
    }

    [Fact]
    public void CavityCanResumeAfterCancellationWithFreshToken()
    {
        var solver = new CavitySolver(17, 100);
        using var cancellation = new CancellationTokenSource();
        Assert.Throws<OperationCanceledException>(() => solver.Run(10, progress: s =>
        {
            if (s.Step == 3) cancellation.Cancel();
        }, progressEvery: 1, cancellation: cancellation.Token));
        Assert.Equal(3, solver.Step);
        using var freshCancellation = new CancellationTokenSource();
        solver.Run(2, cancellation: freshCancellation.Token);
        Assert.Equal(5, solver.Step);
    }

    [Fact]
    public void PreCancelledCylinderRunDoesNotAdvance()
    {
        var solver = new CylinderLbm(100, 80, 40, 8);
        using var cancellation = new CancellationTokenSource();
        cancellation.Cancel();
        var error = Assert.Throws<OperationCanceledException>(() => solver.Run(10, cancellation: cancellation.Token));
        Assert.Equal(cancellation.Token, error.CancellationToken);
        Assert.Equal(0, solver.Step);
    }

    [Fact]
    public void CylinderCanResumeAfterCancellationWithFreshToken()
    {
        var solver = new CylinderLbm(100, 80, 40, 8);
        using var cancellation = new CancellationTokenSource();
        Assert.Throws<OperationCanceledException>(() => solver.Run(10, progress: s =>
        {
            if (s.Step == 3) cancellation.Cancel();
        }, progressEvery: 1, cancellation: cancellation.Token));
        Assert.Equal(3, solver.Step);
        using var freshCancellation = new CancellationTokenSource();
        solver.Run(2, cancellation: freshCancellation.Token);
        Assert.Equal(5, solver.Step);
    }

    [Fact]
    public void LocalPngRendererRemainsAvailable()
    {
        var png=NC.FieldToPng(new double[,]{{0,1},{1,0}},NC.ColorMap.Sequential,0,1,8,8);
        Assert.Equal(new byte[]{137,80,78,71,13,10,26,10},png.Take(8).ToArray());
        Assert.Contains("IHDR",System.Text.Encoding.ASCII.GetString(png));
    }
}
