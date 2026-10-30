using System.Text.Json;
using System.Text.RegularExpressions;
namespace PlotSharp.Inventory;

public static class InventoryScanner
{
    public static string CsvRow(params string[] values) => string.Join(",", values.Select(v => "\"" + v.Replace("\"", "\"\"") + "\""));
    // Lexical inventory only: includes public declarations, not an API compatibility parser.
    public static IEnumerable<(int Line, string Declaration)> PublicDeclarations(string source)
    {
        var lines = source.Replace("\r\n", "\n").Split('\n');
        for (int i = 0; i < lines.Length; i++)
            if (Regex.IsMatch(lines[i], @"^\s*public\s+")) yield return (i + 1, lines[i].Trim());
    }
    public static IEnumerable<(int Cell, int Line, string Call)> NotebookCalls(string json)
    {
        using var document = JsonDocument.Parse(json);
        if (!document.RootElement.TryGetProperty("cells", out var cells)) yield break;
        int cell = 0;
        foreach (var entry in cells.EnumerateArray())
        {
            cell++;
            if (!entry.TryGetProperty("cell_type", out var type) || type.GetString() != "code" || !entry.TryGetProperty("source", out var source)) continue;
            var text = source.ValueKind == JsonValueKind.Array ? string.Concat(source.EnumerateArray().Select(e => e.GetString())) : source.GetString() ?? "";
            var lines = text.Replace("\r\n", "\n").Split('\n');
            for (int line = 0; line < lines.Length; line++)
                foreach (Match match in Regex.Matches(lines[line], @"\b(?:Chart|Charts|plt|IndexedCharts)\.\w+\s*\("))
                    yield return (cell, line + 1, match.Value.TrimEnd(' ', '('));
        }
    }
    public static (string Destination, string Status) Map(string path, string declaration)
    {
        if (path.Contains("NDSharp.Plot", StringComparison.Ordinal))
            return ("P05: Figure/Axes facade + raster backend/data adapter", "Not consolidated; compare APIs");
        if (path.EndsWith("Charts.csx", StringComparison.Ordinal))
        {
            if (Regex.IsMatch(declaration, @"\b(Lines|Panels|Stems)\s*\(")) return ("IndexedCharts", "Partial: signature/display/NaN/scale differ");
            if (declaration.Contains("record Line")) return ("IndexedCharts.Line", "Partial: namespace and contract adaptation");
            return ("P01: host presentation / to be classified", "Not consolidated; exclusion to be approved");
        }
        if (path.EndsWith("Charts.cs", StringComparison.Ordinal) && path.Contains("NavierStokes"))
        {
            if (Regex.IsMatch(declaration, @"\b(LineChart|NiceTicks|Num|Series|LineChartOptions)\b")) return ("PlotSharp.Charts", "Extracted/adapted; full equivalence not verified");
            if (declaration.Contains("MarchingSquares")) return ("Charts.MarchingSquares", "Adapted: eager Segment, mask, degenerate cases");
            if (declaration.Contains("FieldPlot") || declaration.Contains("FieldToPng")) return ("Heatmap/overlay + P05 raster", "Partial SVG; original APIs and PNG not replaced");
            return ("P01: palette/options comparison", "To be classified");
        }
        return ("P01: point-by-point classification", "To be classified");
    }
}
