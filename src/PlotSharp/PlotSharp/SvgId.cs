using System.Security.Cryptography;
using System.Text;

namespace PlotSharp;

/// <summary>
/// Element ids for the SVG that the charts write (clip paths, gradients). An id is derived from the definition it names, so the same chart
/// always produces the same SVG (reproducible output, no churn in saved notebooks) while charts that differ get different ids. Charts with
/// the same geometry get the same ids; where several of them share one SVG or HTML document (a panel stack), give each a different
/// <see cref="Charts.LineChartOptions.IdSeed"/> so that ids stay unique there.
/// </summary>
internal static class SvgId
{
    /// <summary>The prefix followed by <paramref name="hexDigits"/> (default 12) hex digits of the SHA-256 of <paramref name="definition"/>.</summary>
    public static string For(string prefix, string definition, int hexDigits = 12) =>
        prefix + Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(definition)))[..hexDigits].ToLowerInvariant();
}
