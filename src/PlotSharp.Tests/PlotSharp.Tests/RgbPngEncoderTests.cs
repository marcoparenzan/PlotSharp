using System.Buffers.Binary;
using System.IO.Compression;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using NC = NavierStokes.Core.Charts;

namespace PlotSharp.Tests;

public class RgbPngEncoderTests
{
    static string Baseline => Path.Combine(AppContext.BaseDirectory, "Fixtures", "png-baseline.json");

    [Fact]
    public void FieldPngMatchesFrozenScanlinesAndSameRuntimeEncodedHash()
    {
        using var doc = JsonDocument.Parse(File.ReadAllText(Baseline));
        var fixtures = doc.RootElement.GetProperty("Samples").EnumerateArray().ToArray();
        Assert.Equal(3, fixtures.Length);
        bool sameEnvironment = doc.RootElement.GetProperty("Framework").GetString() == RuntimeInformation.FrameworkDescription
            && doc.RootElement.GetProperty("OS").GetString() == RuntimeInformation.OSDescription;
        foreach (var fixture in fixtures)
        {
            int rows = fixture.GetProperty("Rows").GetInt32(), cols = fixture.GetProperty("Columns").GetInt32();
            var values = fixture.GetProperty("Values").EnumerateArray().Select(v => v.GetDouble()).ToArray();
            var field = new double[rows, cols];
            for (int r = 0; r < rows; r++) for (int c = 0; c < cols; c++) field[r, c] = values[r * cols + c];
            Func<double, double, bool>? mask = fixture.GetProperty("MaskCircle").GetBoolean()
                ? (x, y) => (x - 1) * (x - 1) + (y - 0.5) * (y - 0.5) <= 0.16 : null;
            int width = fixture.GetProperty("Width").GetInt32(), height = fixture.GetProperty("Height").GetInt32();
            var png = NC.FieldToPng(field, fixture.GetProperty("Diverging").GetBoolean() ? NC.ColorMap.Diverging : NC.ColorMap.Sequential,
                fixture.GetProperty("Min").GetDouble(), fixture.GetProperty("Max").GetDouble(), width, height, mask);
            var decoded = Decode(png);
            Assert.Equal(width, decoded.Width); Assert.Equal(height, decoded.Height);
            Assert.Equal(Convert.FromHexString(fixture.GetProperty("ScanlinesHex").GetString()!), decoded.Raw);
            if (sameEnvironment)
                Assert.Equal(fixture.GetProperty("EncodedSha256").GetString(), Convert.ToHexString(SHA256.HashData(png)));
        }
    }

    [Fact]
    public void EncoderPreservesInputAndChunkOrder()
    {
        byte[] raw = [0, 255, 0, 0, 0, 255, 0]; // two RGB pixels
        var before = raw.ToArray();
        var decoded = Decode(RgbPngEncoder.EncodeScanlines(2, 1, raw));
        Assert.Equal(raw, decoded.Raw); Assert.Equal(before, raw);
        Assert.Equal(new[] { "IHDR", "IDAT", "IEND" }, decoded.Chunks);
    }

    [Theory]
    [InlineData(0, 1)]
    [InlineData(-1, 1)]
    [InlineData(1, 0)]
    [InlineData(int.MaxValue, 1)]
    [InlineData(1, int.MaxValue)]
    public void InvalidDimensionsDoNotOverflowOrAllocate(int width, int height)
        => Assert.Throws<ArgumentOutOfRangeException>(() => RgbPngEncoder.EncodeScanlines(width, height, []));

    [Fact]
    public void InvalidLengthIsRejected()
        => Assert.Throws<ArgumentException>(() => RgbPngEncoder.EncodeScanlines(2, 2, new byte[13]));

    [Fact]
    public void UnsupportedFiltersAreRejected()
        => Assert.Throws<ArgumentException>(() => RgbPngEncoder.EncodeScanlines(1, 1, new byte[] { 1, 0, 0, 0 }));

    [Fact]
    public void ConcurrentEncodesAreDeterministic()
    {
        byte[] raw = [0, 1, 2, 3];
        var expected = RgbPngEncoder.EncodeScanlines(1, 1, raw);
        Parallel.For(0, 32, _ => Assert.Equal(expected, RgbPngEncoder.EncodeScanlines(1, 1, raw)));
    }

    private static (int Width, int Height, byte[] Raw, string[] Chunks) Decode(byte[] png)
    {
        Assert.Equal(new byte[] { 137, 80, 78, 71, 13, 10, 26, 10 }, png.Take(8).ToArray());
        int offset = 8, width = 0, height = 0; var chunks = new List<string>();
        using var compressed = new MemoryStream();
        while (offset < png.Length)
        {
            Assert.True(offset + 12 <= png.Length);
            int length = checked((int)BinaryPrimitives.ReadUInt32BigEndian(png.AsSpan(offset, 4)));
            Assert.InRange(length, 0, png.Length - offset - 12);
            string type = Encoding.ASCII.GetString(png, offset + 4, 4); chunks.Add(type);
            uint crc = 0xFFFFFFFFu;
            foreach (byte value in png.AsSpan(offset + 4, length + 4))
            {
                crc ^= value;
                for (int bit = 0; bit < 8; bit++) crc = (crc & 1) != 0 ? 0xEDB88320u ^ (crc >> 1) : crc >> 1;
            }
            Assert.Equal(BinaryPrimitives.ReadUInt32BigEndian(png.AsSpan(offset + 8 + length, 4)), crc ^ 0xFFFFFFFFu);
            if (type == "IHDR")
            {
                Assert.Equal(13, length);
                width = checked((int)BinaryPrimitives.ReadUInt32BigEndian(png.AsSpan(offset + 8, 4)));
                height = checked((int)BinaryPrimitives.ReadUInt32BigEndian(png.AsSpan(offset + 12, 4)));
                Assert.Equal((byte)8, png[offset + 16]); Assert.Equal((byte)2, png[offset + 17]);
            }
            if (type == "IDAT") compressed.Write(png, offset + 8, length);
            offset += length + 12;
        }
        Assert.Equal(png.Length, offset);
        compressed.Position = 0;
        using var zlib = new ZLibStream(compressed, CompressionMode.Decompress); using var raw = new MemoryStream(); zlib.CopyTo(raw);
        Assert.Equal(RgbPngEncoder.GetScanlineByteCount(width, height), raw.Length);
        return (width, height, raw.ToArray(), chunks.ToArray());
    }
}
