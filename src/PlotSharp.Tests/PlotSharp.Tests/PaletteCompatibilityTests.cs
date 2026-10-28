using System.Text.Json;
using NC = NavierStokes.Core.Charts;

namespace PlotSharp.Tests;

public class PaletteCompatibilityTests
{
    static string Baseline => Path.Combine(AppContext.BaseDirectory, "Fixtures", "palette-baseline.json");

    [Fact]
    public void ThreePalettesMatchFrozenPreMigrationBytes()
    {
        using var document = JsonDocument.Parse(File.ReadAllText(Baseline));
        var samples = document.RootElement.GetProperty("Samples").EnumerateArray().ToArray();
        Assert.Equal(323, samples.Length);
        foreach (var sample in samples)
        {
            double t = sample.GetProperty("T").GetDouble();
            string sequential = sample.GetProperty("Sequential8").GetString()!;
            string diverging = sample.GetProperty("Diverging9").GetString()!;
            string compact = sample.GetProperty("Compact4").GetString()!;
            Assert.Equal(sequential, NC.ColorMap.Sequential.Hex(t));
            Assert.Equal(diverging, NC.ColorMap.Diverging.Hex(t));
            Assert.Equal(compact, Charts.SequentialColor(t));
            Assert.Equal(sequential, ColorPalette.Sequential.Hex(t));
            Assert.Equal(diverging, ColorPalette.Diverging.Hex(t));
            Assert.Equal(compact, ColorPalette.CompactSequential.Hex(t));
            var (r, g, b) = NC.ColorMap.Sequential.At(t);
            Assert.Equal(sequential, $"#{r:x2}{g:x2}{b:x2}");
        }
    }

    [Fact]
    public void BaselineRecordsLegacySourceProvenance()
    {
        using var document = JsonDocument.Parse(File.ReadAllText(Baseline));
        var hashes = document.RootElement.GetProperty("SourceHashes").EnumerateObject().ToArray();
        Assert.Equal(2, hashes.Length);
        Assert.All(hashes, h => Assert.Matches("^[0-9A-F]{64}$", h.Value.GetString()!));
        Assert.Contains("Pre-palette-migration", document.RootElement.GetProperty("Notes").GetString());
    }

    [Fact]
    public void LegacyNavierNonFinitePolicyRemainsExplicit()
    {
        foreach (var map in new[] { NC.ColorMap.Sequential, NC.ColorMap.Diverging })
        {
            Assert.Equal(map.Hex(0), map.Hex(double.NaN));
            Assert.Equal(map.Hex(0), map.Hex(double.NegativeInfinity));
            Assert.Equal(map.Hex(1), map.Hex(double.PositiveInfinity));
        }
    }

    [Theory]
    [InlineData(double.NaN)]
    [InlineData(double.PositiveInfinity)]
    [InlineData(double.NegativeInfinity)]
    public void SharedAndPlotSharpPoliciesRejectNonFinite(double value)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => ColorPalette.Sequential.Hex(value));
        Assert.Throws<ArgumentOutOfRangeException>(() => Charts.SequentialColor(value));
    }

    [Fact]
    public void EndpointsAndDivergingCenterArePreserved()
    {
        Assert.Equal("#f4f8fd", ColorPalette.Sequential.Hex(0));
        Assert.Equal("#0d366b", ColorPalette.Sequential.Hex(1));
        Assert.Equal("#f0efec", ColorPalette.Diverging.Hex(0.5));
    }

    [Fact]
    public void FourAndEightStopPalettesAreNotSilentlyCollapsed()
        => Assert.NotEqual(ColorPalette.CompactSequential.Hex(0.5), ColorPalette.Sequential.Hex(0.5));
}
