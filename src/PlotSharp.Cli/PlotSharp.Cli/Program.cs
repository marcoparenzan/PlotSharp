using System.Globalization;
using System.Reflection;
using PlotSharp;

return PlotCli.Run(args, Console.Out, Console.Error);

/// <summary>The plotsharp command line. A class so tests call it without spawning a process.</summary>
public static class PlotCli
{
    public const string Usage = """
        plotsharp — PlotSharp command line

          plotsharp line    <data.csv> --y a[,b,…] [--x col] [--out chart.svg] [options]
          plotsharp scatter <data.csv> --x col --y a[,b,…] [--out chart.svg] [options]
          plotsharp selftest
          plotsharp --version | --help

        Options: --title T  --xlabel T  --ylabel T  --width N (default 720)  --height N (default 340)  --sep <c>
        Without --x the row number is used. Without --out the SVG is written to standard output.
        A cell that is empty or not a number becomes a gap in a line chart; scatter drops such rows.
        Exit codes: 0 ok, 1 unknown command, 2 usage or input error.
        """;

    private static readonly string[] Palette = ["#2a78d6", "#eb6834", "#1baf7a", "#eda100", "#e87ba4", "#008300", "#6250d6", "#e34948"];

    public static int Run(string[] args, TextWriter output, TextWriter error)
    {
        if (args.Length == 0 || args[0] is "help" or "--help" or "-h") { output.WriteLine(Usage); return 0; }
        try
        {
            switch (args[0])
            {
                case "--version" or "-v" or "version":
                    output.WriteLine(typeof(PlotCli).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion.Split('+')[0] ?? "unknown");
                    return 0;
                case "line": return Render(args, scatter: false, output);
                case "scatter": return Render(args, scatter: true, output);
                case "selftest": return SelfTest(output, error);
                default:
                    error.WriteLine($"Unknown command '{args[0]}'.");
                    error.WriteLine(Usage);
                    return 1;
            }
        }
        catch (Exception ex) when (ex is ArgumentException or IOException or FormatException or UnauthorizedAccessException)
        {
            error.WriteLine($"Error: {ex.Message}");
            return 2;
        }
    }

    private static string? Option(string[] args, string name)
    {
        var i = Array.IndexOf(args, name);
        if (i < 0) return null;
        return i + 1 < args.Length ? args[i + 1] : throw new ArgumentException($"{name} needs a value.");
    }

    private static int Size(string[] args, string name, int fallback)
    {
        var text = Option(args, name);
        if (text is null) return fallback;
        return int.TryParse(text, out var n) && n is >= 120 and <= 4000 ? n : throw new ArgumentException($"{name} must be an integer from 120 to 4000, got '{text}'.");
    }

    private static int Render(string[] args, bool scatter, TextWriter output)
    {
        if (args.Length < 2 || args[1].StartsWith('-')) throw new ArgumentException("A CSV file is required.");
        if (!File.Exists(args[1])) throw new IOException($"File not found: {args[1]}");
        var ySpec = Option(args, "--y") ?? throw new ArgumentException("--y is required (one or more comma-separated column names).");
        var xName = Option(args, "--x");
        if (scatter && xName is null) throw new ArgumentException("scatter needs --x.");
        var sep = Option(args, "--sep");
        if (sep is { Length: not 1 }) throw new ArgumentException("--sep must be a single character.");

        var table = ReadCsv(File.ReadAllText(args[1]), sep is null ? ',' : sep[0]);
        var x = xName is null ? Enumerable.Range(0, table.Rows).Select(i => (double)i).ToArray() : Column(table, xName);
        var names = ySpec.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (names.Length == 0) throw new ArgumentException("--y names no column.");

        var series = new List<Charts.Series>();
        for (var i = 0; i < names.Length; i++)
        {
            var y = Column(table, names[i]);
            series.Add(scatter
                ? Finite(names[i], x, y, Palette[i % Palette.Length])
                : new Charts.Series(names[i], x, y, Palette[i % Palette.Length]));
        }

        var svg = Charts.LineChart(series, new Charts.LineChartOptions
        {
            Title = Option(args, "--title") ?? "",
            XLabel = Option(args, "--xlabel") ?? xName ?? "",
            YLabel = Option(args, "--ylabel") ?? "",
            Width = Size(args, "--width", 720),
            Height = Size(args, "--height", 340),
            AllowLineGaps = !scatter,
            PointTooltips = true,
            SeparateLegendRow = names.Length > 1,
        });

        var outPath = Option(args, "--out");
        if (outPath is null) output.WriteLine(svg);
        else { File.WriteAllText(outPath, svg); output.WriteLine($"Wrote {outPath} ({series.Count} series, {table.Rows} rows)"); }
        return 0;
    }

    private static Charts.Series Finite(string name, double[] x, double[] y, string color)
    {
        var keep = Enumerable.Range(0, x.Length).Where(i => double.IsFinite(x[i]) && double.IsFinite(y[i])).ToArray();
        if (keep.Length == 0) throw new ArgumentException($"Column '{name}' has no finite (x, y) pair to plot.");
        return new Charts.Series(name, keep.Select(i => x[i]).ToArray(), keep.Select(i => y[i]).ToArray(), color, Markers: true, Line: false);
    }

    private sealed record Table(string[] Header, List<string[]> Body) { public int Rows => Body.Count; }

    private static double[] Column(Table t, string name)
    {
        var at = Array.FindIndex(t.Header, h => string.Equals(h, name, StringComparison.OrdinalIgnoreCase));
        if (at < 0) throw new ArgumentException($"No column '{name}'. Columns: {string.Join(", ", t.Header)}.");
        return t.Body.Select(r => at < r.Length && double.TryParse(r[at], NumberStyles.Float, CultureInfo.InvariantCulture, out var v) && double.IsFinite(v) ? v : double.NaN).ToArray();
    }

    /// <summary>Minimal CSV reader: header row, double-quoted cells with doubled quotes, no multi-line cells.</summary>
    private static Table ReadCsv(string text, char sep)
    {
        var lines = text.Split('\n').Select(l => l.TrimEnd('\r')).Where(l => l.Length > 0).ToList();
        if (lines.Count < 2) throw new ArgumentException("The CSV needs a header row and at least one data row.");
        return new Table(Split(lines[0], sep), lines.Skip(1).Select(l => Split(l, sep)).ToList());
    }

    private static string[] Split(string line, char sep)
    {
        var cells = new List<string>();
        var sb = new System.Text.StringBuilder();
        var quoted = false;
        for (var i = 0; i < line.Length; i++)
        {
            var c = line[i];
            if (quoted)
            {
                if (c != '"') sb.Append(c);
                else if (i + 1 < line.Length && line[i + 1] == '"') { sb.Append('"'); i++; }
                else quoted = false;
            }
            else if (c == '"') quoted = true;
            else if (c == sep) { cells.Add(sb.ToString().Trim()); sb.Clear(); }
            else sb.Append(c);
        }
        cells.Add(sb.ToString().Trim());
        return cells.ToArray();
    }

    private static int SelfTest(TextWriter output, TextWriter error)
    {
        var svg = Charts.LineChart([new Charts.Series("s", [0, 1, 2], [0, 1, 4], Palette[0])], new Charts.LineChartOptions { Title = "selftest" });
        var ok = svg.StartsWith("<svg") && svg.Contains("selftest");
        output.WriteLine(ok ? "selftest: ok" : "selftest: FAILED");
        if (!ok) error.WriteLine("PlotSharp did not return an SVG for a three-point series.");
        return ok ? 0 : 1;
    }
}
