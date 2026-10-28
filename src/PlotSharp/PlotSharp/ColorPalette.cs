namespace PlotSharp;

/// <summary>Immutable shared RGB interpolation. Presets retain historical interval arithmetic and byte truncation.</summary>
public sealed class ColorPalette
{
    private readonly (double Position, byte R, byte G, byte B)[] stops;
    private readonly bool scaledIndex;

    /// <summary>Eight-stop sequential palette used by NavierStokes fields and colorbars.</summary>
    public static ColorPalette Sequential { get; } = new(false,
        "#f4f8fd", "#cde2fb", "#9ec5f4", "#6da7ec", "#3987e5", "#256abf", "#184f95", "#0d366b");

    /// <summary>Nine-stop blue/gray/red palette used by NavierStokes.</summary>
    public static ColorPalette Diverging { get; } = new(false,
        "#0d366b", "#256abf", "#6da7ec", "#b7d3f6", "#f0efec", "#f3b9b3", "#e66767", "#c23b3a", "#7f1f1f");

    /// <summary>Original four-stop PlotSharp palette; colors selected from Sequential, intervals rescaled uniformly.</summary>
    public static ColorPalette CompactSequential { get; } = new(true,
        Sequential.Hex(0), Sequential.Hex(2d / 7), Sequential.Hex(4d / 7), Sequential.Hex(1));

    private ColorPalette(bool scaledIndex, params string[] colors)
    {
        this.scaledIndex = scaledIndex;
        if (colors.Length < 2) throw new ArgumentException("At least two palette stops are required.", nameof(colors));
        stops = colors.Select((hex, index) => ((double)index / (colors.Length - 1),
            Convert.ToByte(hex.Substring(1, 2), 16), Convert.ToByte(hex.Substring(3, 2), 16), Convert.ToByte(hex.Substring(5, 2), 16))).ToArray();
    }

    /// <summary>Finite normalized input; saturates to endpoints. NaN/infinity policy belongs to compatibility callers.</summary>
    public (byte R, byte G, byte B) Sample(double normalized)
    {
        if (!double.IsFinite(normalized)) throw new ArgumentOutOfRangeException(nameof(normalized));
        double t = Math.Clamp(normalized, 0, 1), weight;
        int index;
        if (scaledIndex)
        {
            double position = t * (stops.Length - 1);
            index = Math.Min((int)position, stops.Length - 2);
            weight = position - index;
        }
        else
        {
            index = 0;
            while (index + 1 < stops.Length - 1 && t > stops[index + 1].Position) index++;
            weight = (t - stops[index].Position) / (stops[index + 1].Position - stops[index].Position);
        }
        var a = stops[index]; var b = stops[index + 1];
        // Truncate after the original expression; no rounding or algebraic reassociation.
        return ((byte)(a.R + weight * (b.R - a.R)), (byte)(a.G + weight * (b.G - a.G)), (byte)(a.B + weight * (b.B - a.B)));
    }

    public string Hex(double normalized)
    {
        var (r, g, b) = Sample(normalized);
        return $"#{r:x2}{g:x2}{b:x2}";
    }
}
