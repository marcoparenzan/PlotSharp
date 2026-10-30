using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace PlotSharp;

/// <summary>Navier-compatible gradient colorbar fragment: 11 sampled stops and 3 numeric labels, separate from Heatmap's strips.</summary>
public static partial class GradientColorbar
{
    [GeneratedRegex("^#[0-9a-fA-F]{6}$")]
    private static partial Regex HexColor();

    public static string Render(double min, double max, Func<double, string> color,
        double x, double y, double height, string label, double width = 14)
    {
        ArgumentNullException.ThrowIfNull(color); ArgumentNullException.ThrowIfNull(label);
        if (!double.IsFinite(min) || !double.IsFinite(max) || min > max || !double.IsFinite(max - min))
            throw new ArgumentException("Finite ordered colorbar limits required.");
        if (!double.IsFinite(x) || !double.IsFinite(y) || Math.Abs(x) > 1e7 || Math.Abs(y) > 1e7
            || !double.IsFinite(height) || !double.IsFinite(width) || height <= 0 || width <= 0 || height > 10000 || width > 10000)
            throw new ArgumentException("Invalid colorbar projection.");
        var stops = new StringBuilder();
        for (int k = 0; k <= 10; k++)
        {
            string shade = color(k / 10.0);
            if (shade is null || !HexColor().IsMatch(shade)) throw new ArgumentException("Color callback must return #RRGGBB.", nameof(color));
            stops.Append($"<stop offset='{k * 10}%' stop-color='{shade}'/>");
        }
        string id = SvgId.For("cb", stops.ToString(), 8);
        var svg = new StringBuilder($"<defs><linearGradient id='{id}' x1='0' y1='1' x2='0' y2='0'>");
        svg.Append(stops);
        svg.Append("</linearGradient></defs>");
        svg.Append($"<rect x='{F(x)}' y='{F(y)}' width='{F(width)}' height='{F(height)}' fill='url(#{id})' rx='2'/>");
        foreach (double t in new[] { 0.0, 0.5, 1.0 })
        {
            double value = min + t * (max - min);
            svg.Append($"<text x='{F(x + width + 6)}' y='{F(y + height - t * height + 4)}' font-size='11' fill='{Charts.InkSecondary}'>{Charts.Num(value)}</text>");
        }
        svg.Append($"<text transform='translate({F(x + width + 62)},{F(y + Math.Floor(height / 2))}) rotate(90)' text-anchor='middle' font-size='11' fill='{Charts.InkSecondary}'>{Escape(label)}</text>");
        return svg.ToString();
    }

    private static string F(double value) => value.ToString("0.##", CultureInfo.InvariantCulture);
    private static string Escape(string value) => value.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;").Replace("'", "&#39;");
}
