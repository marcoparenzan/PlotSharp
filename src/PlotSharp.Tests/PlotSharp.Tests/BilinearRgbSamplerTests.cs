namespace PlotSharp.Tests;

public class BilinearRgbSamplerTests
{
    static (byte R, byte G, byte B) Gray(double value) => ((byte)value, (byte)value, (byte)value);

    [Fact]
    public void SamplesTheCellCenterWithoutChangingArithmetic()
    {
        double[,] field = { { 0, 10 }, { 20, 30 } };
        Assert.Equal(new byte[] { 0, 15, 15, 15 }, BilinearRgbSampler.SampleScanlines(field, 1, 1, Gray));
    }

    [Fact]
    public void OutputIsTopDownWhileFieldRowZeroIsBottom()
    {
        double[,] field = { { 0, 0 }, { 100, 100 } };
        Assert.Equal(new byte[] { 0, 75, 75, 75, 0, 25, 25, 25 }, BilinearRgbSampler.SampleScanlines(field, 1, 2, Gray));
    }

    [Fact]
    public void MaskUsesSampleIndexCoordinatesAndSkipsColorCallback()
    {
        double[,] field = { { 0, 1, 2 }, { 2, 1, 0 } };
        var positions = new List<(double, double)>(); int colorCalls = 0;
        var raw = BilinearRgbSampler.SampleScanlines(field, 2, 2, _ => { colorCalls++; return ((byte)0, (byte)0, (byte)0); },
            (x, y) => { positions.Add((x, y)); return x < 1; }, maskedColor: ((byte)9, (byte)8, (byte)7));
        Assert.Equal(new[] { (0.5, 0.75), (1.5, 0.75), (0.5, 0.25), (1.5, 0.25) }, positions.ToArray());
        Assert.Equal(2, colorCalls);
        Assert.Equal(new byte[] { 0, 9, 8, 7, 0, 0, 0, 0, 9, 8, 7, 0, 0, 0 }, raw);
    }

    [Fact]
    public void NonFiniteValuePolicyIsDelegatedToColorCallback()
    {
        double[,] field = { { double.NaN, 0 }, { 0, 0 } }; bool sawNaN = false;
        var raw = BilinearRgbSampler.SampleScanlines(field, 1, 1, value =>
        {
            sawNaN = double.IsNaN(value); return ((byte)1, (byte)2, (byte)3);
        });
        Assert.True(sawNaN); Assert.Equal(new byte[] { 0, 1, 2, 3 }, raw);
    }

    [Fact]
    public void DoesNotModifyInput()
    {
        double[,] field = { { 0, 10 }, { 20, 30 } }; var before = field.Cast<double>().ToArray();
        _ = BilinearRgbSampler.SampleScanlines(field, 3, 3, Gray);
        Assert.Equal(before, field.Cast<double>().ToArray());
    }

    [Theory]
    [InlineData(0, 2)]
    [InlineData(1, 1)]
    [InlineData(1, 2)]
    [InlineData(2, 1)]
    public void RejectsFieldsTooSmallForBilinearSampling(int rows, int columns)
        => Assert.Throws<ArgumentException>(() => BilinearRgbSampler.SampleScanlines(new double[rows, columns], 1, 1, Gray));

    [Fact]
    public void RequiresExplicitMaskedColor()
        => Assert.Throws<ArgumentException>(() => BilinearRgbSampler.SampleScanlines(new double[2, 2], 1, 1, Gray, (_, _) => true));

    [Fact]
    public void RequiresFieldAndColorCallback()
    {
        Assert.Throws<ArgumentNullException>(() => BilinearRgbSampler.SampleScanlines(null!, 1, 1, Gray));
        Assert.Throws<ArgumentNullException>(() => BilinearRgbSampler.SampleScanlines(new double[2, 2], 1, 1, null!));
    }
}
