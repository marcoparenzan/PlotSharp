using System.Security.Cryptography;
using System.Xml.Linq;
using PlotSharp.Inventory;

var root = Path.GetFullPath(args.Length > 0 ? args[0] : ".");
var imports = Path.Combine(root, "src", "imports");
if (!Directory.Exists(imports)) throw new DirectoryNotFoundException($"Missing imports: {imports}");
var output = Path.Combine(root, "ops", "inventory");
Directory.CreateDirectory(output);
var files = Directory.GetFiles(imports, "*", SearchOption.AllDirectories)
    .Where(p => !p.Split(Path.DirectorySeparatorChar).Any(s => s is "bin" or "obj" or ".git" or "node_modules"))
    .Order(StringComparer.Ordinal).ToArray();
var fileRows = new List<string> { InventoryScanner.CsvRow("Origin", "Path", "Kind", "SHA256") };
var apiRows = new List<string> { InventoryScanner.CsvRow("Path", "Line", "PublicDeclaration", "ProposedDestination", "State") };
var calls = new List<string> { InventoryScanner.CsvRow("Path", "Cell", "SourceLine", "Call", "State") };
var refs = new List<string> { InventoryScanner.CsvRow("Project", "Kind", "Reference", "ResolvedImportedPath", "State") };
int missing = 0;
foreach (var file in files)
{
    string path = Path.GetRelativePath(root, file).Replace('\\', '/');
    string origin = Path.GetRelativePath(imports, file).Split(Path.DirectorySeparatorChar)[0];
    string ext = Path.GetExtension(file).ToLowerInvariant();
    string kind = ext switch { ".cs" or ".csx" => "source", ".csproj" => "project", ".ipynb" => "notebook", ".dll" or ".pdb" => "runtime-binary", ".ttf" => "third-party-font", _ => Path.GetFileName(file).StartsWith("LICENSE") ? "license" : "reference/asset" };
    fileRows.Add(InventoryScanner.CsvRow(origin, path, kind, Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(file)))));
    // Imported undo/configuration snapshots are provenance, not consumer projects.
    if (path.Contains("/.ralfai/", StringComparison.Ordinal)) continue;
    if (ext is ".cs" or ".csx")
        foreach (var (line, declaration) in InventoryScanner.PublicDeclarations(File.ReadAllText(file)))
        {
            var map = InventoryScanner.Map(path, declaration);
            apiRows.Add(InventoryScanner.CsvRow(path, line.ToString(), declaration, map.Destination, map.Status));
        }
    if (ext == ".ipynb")
        foreach (var call in InventoryScanner.NotebookCalls(File.ReadAllText(file)))
            calls.Add(InventoryScanner.CsvRow(path, call.Cell.ToString(), call.Line.ToString(), call.Call, "Non migrato; uso lessicale da verificare"));
    if (ext == ".csproj")
        foreach (var reference in XDocument.Load(file).Descendants().Where(e => e.Name.LocalName is "ProjectReference" or "PackageReference"))
        {
            string include = reference.Attribute("Include")?.Value ?? "";
            string resolved = "", state;
            if (reference.Name.LocalName == "ProjectReference")
            {
                var target = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(file)!, include.Replace('\\', Path.DirectorySeparatorChar).Replace('/', Path.DirectorySeparatorChar)));
                resolved = Path.GetRelativePath(root, target).Replace('\\', '/');
                bool exists = File.Exists(target); if (!exists) missing++;
                state = exists ? "csproj presente; sorgenti/build non verificati" : "csproj mancante nello snapshot";
            }
            else state = "package " + (reference.Attribute("Version")?.Value ?? "versione esterna/centralizzata non raccolta");
            refs.Add(InventoryScanner.CsvRow(path, reference.Name.LocalName, include, resolved, state));
        }
}
foreach (var (name, rows) in new[] { ("files.csv", fileRows), ("public-declarations.csv", apiRows), ("notebook-calls.csv", calls), ("references.csv", refs) })
    File.WriteAllLines(Path.Combine(output, name), rows);
Console.WriteLine($"Inventario lessicale: {files.Length} file, {apiRows.Count - 1} dichiarazioni public, {calls.Count - 1} usi notebook, {refs.Count - 1} riferimenti, {missing} ProjectReference mancanti. Output: {output}");
