using PlotSharp;
using System.Xml.Linq;

var output = Path.GetFullPath(args.Length > 0 ? args[0] : "artifacts/gallery");
Directory.CreateDirectory(output);
var line = IndexedCharts.Lines("Osservazioni e previsione",
    new("Osservazioni", [2, 4, 3, 5]), new("Previsione", [5, 6, 5.5], Start: 3, Dashed: true));
var scatter = Charts.LineChart([new("Misure", [0, 1, 2, 3], [2, 4, 3, 5], Charts.Series2, Markers: true, Line: false)],
    new() { Title = "Scatter", XMin = 0, XMax = 3, YMin = 0, YMax = 6 });
var panels = IndexedCharts.Panels("Componenti", new("Trend", [100, 110, 120]), new("Residuo", [-0.1, 0.2, -0.1]));
var stems = IndexedCharts.Stems("Correlogramma", [0.7, 0.3, -0.2, 0.1, -0.05], band: 0.2);
var bars = Charts.LineChart([new("Bilancio", [0, 1, 2, 3], [4, -2, 3, -1], Charts.Series1, Line: false, BarWidth: 0.8)], new() { Title = "Barre positive e negative" });
var heatmap = Charts.Heatmap(new double[,] { { 0, 1, 2 }, { 2, 3, 4 }, { 4, 5, 6 } }, new() { Title = "Campo [riga,colonna] · origine in basso" });
var physical = Charts.Heatmap(new double[,] { { double.NaN, 10, 20 }, { 5, 15, 25 } },
    new() { Title = "Temperatura · campo mascherato", ShowColorbar = true, ColorbarLabel = "°C", Extent = new(0, 3, -1, 1), Mask = new bool[,] { { true, false, false }, { false, false, false } } });
var overlays = Charts.Heatmap(new double[,] { { 0, 1, 2 }, { 1, 2, 3 }, { 2, 3, 4 } },
    new() { Title = "Contour e vettori", ShowColorbar = true, ContourLevels = [1.5, 2.5],
        Vectors = new(new double[,] { { 1, 1, 1 }, { 1, 1, 1 }, { 1, 1, 1 } }, new double[,] { { 0, 0.5, 1 }, { 0, 0.5, 1 }, { 0, 0.5, 1 } }, Scale: 0.3) });
foreach (var (name, content) in new[] { ("lines.svg", line), ("scatter.svg", scatter), ("panels.html", panels), ("stems.svg", stems), ("bars.svg", bars), ("heatmap.svg", heatmap), ("physical-field.svg", physical), ("overlays.svg", overlays) })
{
    XDocument.Parse(content);
    File.WriteAllText(Path.Combine(output, name), content);
}
File.WriteAllText(Path.Combine(output, "index.html"), "<!doctype html><html lang='it'><meta charset='utf-8'><title>PlotSharp Gallery</title><body><h1>PlotSharp Gallery</h1>" + line + scatter + panels + stems + bars + heatmap + physical + overlays + "</body></html>");
Console.WriteLine($"Gallery generated and XML validated: {output}");
