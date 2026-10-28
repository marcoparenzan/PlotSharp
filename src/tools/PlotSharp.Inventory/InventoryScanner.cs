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
            return ("P05: facade Figure/Axes + backend raster/adattatore dati", "Non consolidato; confrontare API");
        if (path.EndsWith("Charts.csx", StringComparison.Ordinal))
        {
            if (Regex.IsMatch(declaration, @"\b(Lines|Panels|Stems)\s*\(")) return ("IndexedCharts", "Parziale: firma/display/NaN/scale differenti");
            if (declaration.Contains("record Line")) return ("IndexedCharts.Line", "Parziale: adattamento namespace e contratto");
            return ("P01: presentazione host / da classificare", "Non consolidato; esclusione da approvare");
        }
        if (path.EndsWith("Charts.cs", StringComparison.Ordinal) && path.Contains("NavierStokes"))
        {
            if (Regex.IsMatch(declaration, @"\b(LineChart|NiceTicks|Num|Series|LineChartOptions)\b")) return ("PlotSharp.Charts", "Estratto/adattato; equivalenza completa non verificata");
            if (declaration.Contains("MarchingSquares")) return ("Charts.MarchingSquares", "Adattato: Segment eager, mask, degeneri");
            if (declaration.Contains("FieldPlot") || declaration.Contains("FieldToPng")) return ("Heatmap/overlay + P05 raster", "Parziale SVG; API e PNG originali non sostituiti");
            return ("P01: confronto palette/opzioni", "Da classificare");
        }
        return ("P01: classificazione puntuale", "Da classificare");
    }
}
