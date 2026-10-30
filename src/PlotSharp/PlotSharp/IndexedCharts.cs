using System.Net;
using System.Text;

namespace PlotSharp;

/// <summary>Host-independent facade for indexed time series, inspired by MathemaSharp Charts.csx.</summary>
public static class IndexedCharts
{
    public sealed record Line(string Name, double[] Y, int Start = 0, bool Dashed = false);
    private static readonly string[] Palette = ["#2a78d6", "#eb6834", "#1baf7a", "#eda100", "#e87ba4", "#008300", "#6250d6", "#e34948"];

    public static Charts.Series ToSeries(Line line, int paletteIndex = 0)
    {
        ArgumentNullException.ThrowIfNull(line);
        ArgumentNullException.ThrowIfNull(line.Y);
        if (paletteIndex < 0) throw new ArgumentOutOfRangeException(nameof(paletteIndex));
        return new(line.Name, Enumerable.Range(0, line.Y.Length).Select(i => (double)line.Start + i).ToArray(),
            (double[])line.Y.Clone(), Palette[paletteIndex % Palette.Length], Dashed: line.Dashed);
    }

    /// <summary>Returns one SVG with a shared y scale. Does not call a notebook display API.</summary>
    public static string Lines(string title, params Line[] lines)
    {
        ArgumentNullException.ThrowIfNull(lines);
        return Charts.LineChart(lines.Select((line, i) => ToSeries(line, i)).ToArray(),
            new() { Title = title, Width = 720, Height = 260, AllowLineGaps = true, PointTooltips = true, PreserveBoundaryMarkers = true, SeparateLegendRow = true });
    }

    /// <summary>Correlogram with zero-based stems and optional symmetric reference lines.</summary>
    public static string Stems(string title, double[] values, double? band = null, int firstLag = 1)
        => Stems(title, values, band, firstLag, "Lag");

    /// <summary>Correlogram or spectral stems with an explicit index-axis label. Original overload remains binary compatible.</summary>
    public static string Stems(string title, double[] values, double? band, int firstLag, string xLabel)
    {
        ArgumentNullException.ThrowIfNull(values);
        if (values.Length == 0) throw new ArgumentException("Values cannot be empty.", nameof(values));
        if (band is { } b && (!double.IsFinite(b) || b < 0 || b > 1e12)) throw new ArgumentOutOfRangeException(nameof(band));
        var data = ToSeries(new(title, values, firstLag)) with { Line = false, Markers = true, Stems = true };
        var series = new List<Charts.Series> { data };
        if (band is { } reference)
        {
            double left = firstLag - 0.5, right = (double)firstLag + values.Length - 0.5;
            series.Add(new("+band", [left, right], [reference, reference], Charts.Series2, Dashed: true));
            series.Add(new("-band", [left, right], [-reference, -reference], Charts.Series2, Dashed: true));
        }
        return Charts.LineChart(series, new() { Title = title, XLabel = xLabel, Width = 720, Height = 260, PreserveBoundaryMarkers = true, SeparateLegendRow = true, PointTooltips = true,
            XMin = firstLag - 0.5, XMax = (double)firstLag + values.Length - 0.5 });
    }

    /// <summary>Returns HTML containing one SVG per series, each with its own y scale.</summary>
    public static string Panels(string title, params Line[] lines)
    {
        ArgumentNullException.ThrowIfNull(title);
        ArgumentNullException.ThrowIfNull(lines);
        if (lines.Length == 0) throw new ArgumentException("At least one panel is required.", nameof(lines));
        var html = new StringBuilder("<section><h2>").Append(WebUtility.HtmlEncode(title)).Append("</h2>");
        for (int i = 0; i < lines.Length; i++)
            html.Append(Charts.LineChart([ToSeries(lines[i], i)], new() { Title = lines[i].Name, IdSeed = "panel" + i, Width = 720, Height = 180, AllowLineGaps = true, PointTooltips = true, PreserveBoundaryMarkers = true, SeparateLegendRow = true }));
        return html.Append("</section>").ToString();
    }
}
