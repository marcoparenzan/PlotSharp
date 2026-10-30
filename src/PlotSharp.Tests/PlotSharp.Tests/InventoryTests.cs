using PlotSharp.Inventory;
namespace PlotSharp.Tests;
public class InventoryTests
{
    [Fact] public void CsvEscapesQuotesCommasAndNewlines() => Assert.Equal("\"a,b\",\"say \"\"hi\"\"\",\"x\ny\"", InventoryScanner.CsvRow("a,b", "say \"hi\"", "x\ny"));
    [Fact] public void DeclarationsHaveOriginalLineNumbers()
    {
        var result = InventoryScanner.PublicDeclarations("// public fake\r\n public class A\r\n private int x;\r\n public int Y;").ToArray();
        Assert.Equal(2, result.Length); Assert.Equal(2, result[0].Line); Assert.Equal(4, result[1].Line);
    }
    [Fact] public void NotebookDecodesUnicodeAndIgnoresOutputAndMarkdown()
    {
        var json = """{"cells":[{"cell_type":"markdown","source":["Chart.Lines()"]},{"cell_type":"code","source":["Chart.\u004cines(\"x\");\n","plt.show()"],"outputs":[{"text":"Charts.Fake()"}]}]}""";
        var result = InventoryScanner.NotebookCalls(json).ToArray();
        Assert.Equal(2, result.Length); Assert.Equal((2, 1, "Chart.Lines"), result[0]); Assert.Equal((2, 2, "plt.show"), result[1]);
    }
    [Fact] public void MappingDoesNotClaimFullCompatibility()
    {
        var map = InventoryScanner.Map("src/imports/MathemaSharp/notebooks/shared/Charts.csx", "public static void Lines(string title)");
        Assert.Equal("IndexedCharts", map.Destination); Assert.StartsWith("Partial", map.Status);
    }
}
