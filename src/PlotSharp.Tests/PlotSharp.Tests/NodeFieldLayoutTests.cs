using System.Xml.Linq;
using NC = NavierStokes.Core.Charts;

namespace PlotSharp.Tests;

public class NodeFieldLayoutTests
{
    [Theory]
    [InlineData(2, 2, 40, false, 40, 44, 170, 108)]
    [InlineData(3, 5, 41, true, 20, 62, 171, 106)]
    [InlineData(3, 5, 43, false, 22, 44, 173, 90)]
    [InlineData(5, 3, 30, false, 60, 44, 160, 128)]
    public void DimensionsMatchLegacyRoundingAndMargins(int rows, int cols, int width, bool subtitle, int height, int top, int totalWidth, int totalHeight)
    {
        var layout = NodeFieldLayout.Create(rows, cols, width, subtitle);
        Assert.Equal(height, layout.PlotHeight); Assert.Equal(top, layout.Top);
        Assert.Equal(totalWidth, layout.Width); Assert.Equal(totalHeight, layout.Height);
        var svg = XElement.Parse(NC.FieldPlot(new double[rows, cols], new() { PlotWidth = width, Subtitle = subtitle ? "sub" : "", Min = 0, Max = 1 }));
        Assert.Equal(totalWidth.ToString(), svg.Attribute("width")!.Value);
        Assert.Equal(totalHeight.ToString(), svg.Attribute("height")!.Value);
        var image = Assert.Single(svg.Elements(), e => e.Name.LocalName == "image");
        Assert.Equal("16", image.Attribute("x")!.Value); Assert.Equal(top.ToString(), image.Attribute("y")!.Value);
        Assert.Equal(width.ToString(), image.Attribute("width")!.Value); Assert.Equal(height.ToString(), image.Attribute("height")!.Value);
    }

    [Fact]
    public void CornersAndFractionalNodesKeepBottomOrigin()
    {
        var layout = NodeFieldLayout.Create(3, 4, 60);
        Assert.Equal(16, layout.X(0)); Assert.Equal(76, layout.X(3));
        Assert.Equal(84, layout.Y(0)); Assert.Equal(44, layout.Y(2));
        Assert.Equal(46, layout.X(1.5)); Assert.Equal(74, layout.Y(0.5));
        Assert.Equal(96, layout.ColorbarX); Assert.Equal(14, layout.ColorbarWidth);
    }

    [Fact]
    public void FiniteOutsideIndicesAreNotSilentlyClamped()
    {
        var layout = NodeFieldLayout.Create(3, 4, 60);
        Assert.Equal(-4, layout.X(-1)); Assert.Equal(104, layout.Y(-1));
    }

    [Theory]
    [InlineData(1, 2, 40)]
    [InlineData(2, 1, 40)]
    [InlineData(100001, 2, 40)]
    [InlineData(2, 2, 0)]
    [InlineData(2, 2, 10001)]
    [InlineData(2, 1000, 1)]
    [InlineData(10000, 2, 40)]
    public void UnsupportedShapesWidthsAndCollapsedProjectionsAreRejected(int rows, int cols, int width)
        => Assert.ThrowsAny<ArgumentException>(() => NodeFieldLayout.Create(rows, cols, width));

    [Fact]
    public void NonFiniteProjectedIndicesAreRejected()
    {
        var layout = NodeFieldLayout.Create(2, 2, 40);
        Assert.Throws<ArgumentOutOfRangeException>(() => layout.X(double.NaN));
        Assert.Throws<ArgumentOutOfRangeException>(() => layout.Y(double.PositiveInfinity));
        Assert.Throws<ArgumentOutOfRangeException>(() => layout.X(double.MaxValue));
    }
}
