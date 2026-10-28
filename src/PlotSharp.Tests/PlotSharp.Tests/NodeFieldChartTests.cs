using System.Buffers.Binary;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using NC = NavierStokes.Core.Charts;

namespace PlotSharp.Tests;

public class NodeFieldChartTests
{
    [Fact]
    public void CompleteFieldAssemblyMatchesFrozenMarkupAndPixels()
    {
        using var doc = JsonDocument.Parse(File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "Fixtures", "node-field-baseline.json")));
        var samples = doc.RootElement.GetProperty("Samples").EnumerateArray().ToArray(); Assert.Equal(4, samples.Length);
        foreach (var sample in samples)
        {
            int rows = sample.GetProperty("Rows").GetInt32(), cols = sample.GetProperty("Columns").GetInt32();
            var field = new double[rows, cols]; var u = new double[rows, cols]; var v = new double[rows, cols];
            var values = sample.GetProperty("Values").EnumerateArray().Select(e => e.GetDouble()).ToArray();
            for (int r = 0; r < rows; r++) for (int c = 0; c < cols; c++) { field[r, c] = values[r * cols + c]; u[r, c] = 1; }
            bool diverging = sample.GetProperty("Diverging").GetBoolean();
            var map = diverging ? NC.ColorMap.Diverging : NC.ColorMap.Sequential;
            var contours = sample.GetProperty("Contours").GetBoolean() ? field : null;
            (double[,] U, double[,] V)? arrows = sample.GetProperty("Vectors").GetBoolean() ? (u, v) : null;
            (double Cx, double Cy, double R)? obstacle = sample.GetProperty("Obstacle").GetBoolean() ? (1, 1, 0.4) : null;
            var legacy = new NC.FieldPlotOptions { Title = "Field <&'", Subtitle = sample.GetProperty("Subtitle").GetString()!, ColorbarLabel = "Scale <&'",
                Min = diverging ? -2 : 0, Max = 2, Map = map, Symmetric = diverging, Contours = contours, ContourLevels = 1,
                Arrows = arrows, ArrowStride = 1, Obstacle = obstacle, PlotWidth = sample.GetProperty("PlotWidth").GetInt32() };
            var shared = new NodeFieldOptions { Title = legacy.Title, Subtitle = legacy.Subtitle, ColorbarLabel = legacy.ColorbarLabel,
                Min = legacy.Min, Max = legacy.Max, NormalizedColor = map.At, Symmetric = legacy.Symmetric, Contours = contours, ContourLevels = 1,
                Arrows = arrows, ArrowStride = 1, Obstacle = obstacle, PlotWidth = legacy.PlotWidth };
            foreach (var svg in new[] { NC.FieldPlot(field, legacy), Charts.NodeFieldPlot(field, shared) })
            {
                var (markup, pixels) = Normalize(svg);
                Assert.Equal(sample.GetProperty("Markup").GetString(), markup);
                Assert.Equal(sample.GetProperty("RawPixelSha256").GetString(), pixels);
            }
        }
    }

    [Fact]
    public void OverlayShapesMustMatchTheField()
    {
        Assert.Throws<ArgumentException>(() => Charts.NodeFieldPlot(new double[2, 2], new() { Contours = new double[3, 3] }));
        Assert.Throws<ArgumentException>(() => Charts.NodeFieldPlot(new double[2, 2], new() { Arrows = (new double[3, 3], new double[3, 3]) }));
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(33)]
    public void UnsupportedContourCountsAreRejected(int count)
        => Assert.Throws<ArgumentOutOfRangeException>(() => Charts.NodeFieldPlot(new double[2, 2], new() { ContourLevels = count }));

    [Fact]
    public void InvalidObstaclesAndCallbacksAreRejected()
    {
        Assert.Throws<ArgumentException>(() => Charts.NodeFieldPlot(new double[2, 2], new() { Obstacle = (1, 1, -1) }));
        Assert.Throws<ArgumentNullException>(() => Charts.NodeFieldPlot(new double[2, 2], new() { NormalizedColor = null! }));
    }

    [Fact]
    public void HeatmapRemainsCellBasedAndZeroContourFieldIsSupported()
    {
        XNamespace ns = "http://www.w3.org/2000/svg";
        var heatmap = XElement.Parse(Charts.Heatmap(new double[,] { { 1 } }));
        Assert.Empty(heatmap.Descendants(ns + "image"));
        var node = XElement.Parse(Charts.NodeFieldPlot(new double[2, 2], new() { Contours = new double[2, 2], ContourLevels = 0 }));
        Assert.Single(node.Elements(ns + "image"));
        Assert.Empty(node.Elements(ns + "path").Single().Attribute("d")!.Value);
    }

    private static (string Markup, string Pixels) Normalize(string svg)
    {
        XNamespace ns = "http://www.w3.org/2000/svg";
        var root = XElement.Parse(svg); var image = Assert.Single(root.Elements(ns + "image"));
        var png = Convert.FromBase64String(image.Attribute("href")!.Value["data:image/png;base64,".Length..]);
        using var compressed = new MemoryStream(); int offset = 8;
        while (offset < png.Length)
        {
            int length = checked((int)BinaryPrimitives.ReadUInt32BigEndian(png.AsSpan(offset, 4)));
            if (Encoding.ASCII.GetString(png, offset + 4, 4) == "IDAT") compressed.Write(png, offset + 8, length);
            offset += length + 12;
        }
        compressed.Position = 0; using var zlib = new ZLibStream(compressed, CompressionMode.Decompress); using var raw = new MemoryStream(); zlib.CopyTo(raw);
        image.SetAttributeValue("href", "RGB-PIXELS-CHECKED-SEPARATELY");
        return (Regex.Replace(root.ToString(SaveOptions.DisableFormatting), "cb[0-9a-f]{8}", "cb-fixed"), Convert.ToHexString(SHA256.HashData(raw.ToArray())));
    }
}
