using System.IO.Compression;
using System.Text;

namespace PlotSharp;

/// <summary>BCL-only PNG encoding for RGB8, top-down scanlines with filter byte 0. No resampling, palette or alpha policy.</summary>
public static class RgbPngEncoder
{
    private static readonly uint[] CrcTable = CreateCrcTable();

    public static int GetScanlineByteCount(int width, int height)
    {
        if (width <= 0 || width > (int.MaxValue - 1) / 3) throw new ArgumentOutOfRangeException(nameof(width));
        if (height <= 0) throw new ArgumentOutOfRangeException(nameof(height));
        int rowBytes = 1 + width * 3;
        if (height > int.MaxValue / rowBytes) throw new ArgumentOutOfRangeException(nameof(height));
        return rowBytes * height;
    }

    public static byte[] EncodeScanlines(int width, int height, ReadOnlySpan<byte> scanlines)
    {
        int length = GetScanlineByteCount(width, height);
        if (scanlines.Length != length) throw new ArgumentException("RGB scanline length does not match dimensions.", nameof(scanlines));
        int rowBytes = 1 + width * 3;
        for (int offset = 0; offset < length; offset += rowBytes)
            if (scanlines[offset] != 0) throw new ArgumentException("Only filter-0 scanlines are supported.", nameof(scanlines));

        using var output = new MemoryStream();
        output.Write(new byte[] { 137, 80, 78, 71, 13, 10, 26, 10 });
        var header = new byte[13];
        WriteBE(header, 0, (uint)width); WriteBE(header, 4, (uint)height);
        header[8] = 8; header[9] = 2;
        WriteChunk(output, "IHDR", header);
        using (var compressed = new MemoryStream())
        {
            using (var zlib = new ZLibStream(compressed, CompressionLevel.Optimal, leaveOpen: true)) zlib.Write(scanlines);
            WriteChunk(output, "IDAT", compressed.ToArray());
        }
        WriteChunk(output, "IEND", []);
        return output.ToArray();
    }

    private static void WriteBE(byte[] bytes, int offset, uint value)
    {
        bytes[offset] = (byte)(value >> 24); bytes[offset + 1] = (byte)(value >> 16);
        bytes[offset + 2] = (byte)(value >> 8); bytes[offset + 3] = (byte)value;
    }

    private static void WriteChunk(Stream stream, string type, ReadOnlySpan<byte> data)
    {
        var length = new byte[4]; WriteBE(length, 0, (uint)data.Length); stream.Write(length);
        var typeBytes = Encoding.ASCII.GetBytes(type); stream.Write(typeBytes); stream.Write(data);
        uint crc = 0xFFFFFFFFu;
        foreach (byte value in typeBytes) crc = CrcTable[(crc ^ value) & 0xFF] ^ (crc >> 8);
        foreach (byte value in data) crc = CrcTable[(crc ^ value) & 0xFF] ^ (crc >> 8);
        var checksum = new byte[4]; WriteBE(checksum, 0, crc ^ 0xFFFFFFFFu); stream.Write(checksum);
    }

    private static uint[] CreateCrcTable() => Enumerable.Range(0, 256).Select(index =>
    {
        uint crc = (uint)index;
        for (int bit = 0; bit < 8; bit++) crc = (crc & 1) != 0 ? 0xEDB88320u ^ (crc >> 1) : crc >> 1;
        return crc;
    }).ToArray();
}
