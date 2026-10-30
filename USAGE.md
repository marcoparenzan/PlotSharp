# Usage

## Installation

PlotSharp is a **library** plus the global tool **`plotsharp`**. Requires the **.NET 11 SDK**.

```powershell
dotnet add package PlotSharp --source D:\Dev\NuGetLocalFeed --prerelease
```

### The `plotsharp` tool

```powershell
dotnet tool install --global PlotSharp.Cli --add-source D:\Dev\NuGetLocalFeed --prerelease
plotsharp line data.csv --x day --y a,b --title Demo --out chart.svg
plotsharp scatter data.csv --x a --y b --out scatter.svg
plotsharp selftest
```

Options: `--title`, `--xlabel`, `--ylabel`, `--width`, `--height`, `--sep`. Without `--out` the SVG goes to standard output. In a line
chart an empty or non-numeric cell becomes a gap; scatter drops such rows. Exit codes: 0 ok, 1 unknown command, 2 usage or input error.

Build and test from source: `dotnet build src/PlotSharp.slnx` and `dotnet test src/PlotSharp.slnx`. Most users get PlotSharp
indirectly through `NDSharp.Plot`. The suite installer can download the package: `D:\Dev\Science\ScienceSharp\Install-Suite.ps1 -Download`.

## Reproducible SVG

The SVG a chart writes is the same on every run: clip paths and gradients get ids that are a hash of the chart's own geometry or colour stops (`SvgId`), not a
random GUID, so saved notebooks do not change when they are re-executed. Charts with the same geometry therefore share ids. When several charts end up in
one SVG or HTML document, give each a different `LineChartOptions.IdSeed` to keep the ids unique there; `IndexedCharts.Panels` does it for you (panel index).
Package `0.1.1-net11-preview.2` is the first with this behaviour.

## Current complete baseline after ND filled sampling/bands

Science gate --no-images D:\Dev\BuildArtifacts\ecosystem-validation\20261028-215324 passed31/31 after the latest ND sampler/band changes. All build/test/audit/kernel/notebook/UI/browser/numerical checks ran; no new review image files or screenshots, encoded notebook outputs and in-memory QA retained.
Supersedes earlier no-full-gate notes. Not a complete ND family migration, publication, all-input pixel oracle or universal accessibility claim; known warnings/suspended tests remain visible.

## ContourBands — source-build compatibility helper

ContourBands(levels) snapshots finite nondecreasing levels, including duplicates. Find(value) retains ND inclusive selection/endpoints; outside and non-finite values yield -1. NormalizedMidpoint(band) preserves the original sum/division and constant-range fallback; invalid band indices reject. It does not decide palette/alpha or render.
NDSharp DrawFilled now delegates this selection. Shared227/227 andND19/19 tests passed, source kernel notebook outputs rechecked without review image files. No full gate rerun or publication.

## Physical rectilinear sampler — source-build only

RectilinearFieldSampler(xs, ys, rowMajorValues).Sample(x,y) retains NDSharp's original binary-search/four-term bilinear interpolation. Snapshots arrays, requires finite strictly increasing ≥2 axes and matching data, leaves NaN/Infinity value policy to the caller. Finite outside queries extrapolate, non-finite queries reject. It does not replace uniform RGB sampling or descending-axis line contours.
81 frozen legacy queries match exactly; shared211/211 andND19/19 tests passed plus rebuilt source kernel notebook outputs. Filled-band styling/raster stay native; no full gate/publication.

## Rectilinear contour geometry for NDSharp — source-build only

RectilinearContours.Trace(xs, ys, rowMajorValues, level) uses shared ContourKernel with physical edge coordinates and ND's original direction/pairing/degenerate/non-finite corner-sum policy. No global sample-index transformation and no Navier node-field size/value cap. Requires matching nonempty axes/data and finite coordinates/level.
NDSharp.Plot DrawLines now references this geometry; level pruning, style, SKPath/Axes and contourf remain native. Previous Charts.MarchingSquares APIs retain their mask/bounds/default-degenerate convention. Targeted tests204/204 shared and18/18 ND passed, no saved review images or full gate/package validation. New source DLL dependency must be included in deployments.

## Current full baseline after FieldPlot consolidation

From MathemaSharp: `dotnet run --file ops/ValidateEcosystem.cs -- --no-images`. Latest report D:\Dev\BuildArtifacts\ecosystem-validation\20261028-185732: 29/29 steps exit 0 after the node renderer extraction, including full numerical selftest and UI/browser/raster validation. No new review image files/screenshots; JSON evidence stays separate and fresh reports are archived.
This supersedes earlier notes of no full gate rerun. It is not certification of every consumer/input, all accessibility or redistribution rights; no release was published.

## Node-field SVG assembly — source-build only

Charts.NodeFieldPlot(field, NodeFieldOptions) combines shared node layout, RGB callback, legacy limits, bilinear sampling/PNG, contours, open-head vectors and gradient colorbar. Default RGB policy is the Navier eight-stop palette with legacy NaN-to-first-color behavior. Cell-based Heatmap remains separate.
Navier FieldPlot is now an options/API facade. Complete pre-assembly fixtures match markup and decoded RGB pixels, with only GUID/PNG payload normalized for structural comparison. Require overlay shapes matching field, contour count0–32 and finite nonnegative circular obstacles; previous unsupported inputs not guaranteed compatible.
198 tests passed and Web/quadro/adoption smoke passed without images. No complete gate rerun, universal RGBA/image backend or publication implied.

## Node-field layout — source-build only

NodeFieldLayout.Create(rows, columns, plotWidth, hasSubtitle) provides legacy Navier margins/viewport/colorbar position and node X/Y projection. Math.Round height (to-even), bottom-origin field rows and fractional coordinates retained; not a cell-centered Heatmap layout. Finite extrapolation is allowed without clamp, non-finite projections rejected.
Requires ≥2×2/≤100000 nodes and projected width/height1–10000. Collapsed aspect ratios are rejected before raster allocation. 192 tests passed plus Web/quadro/adoption smoke without images. Caller retains assembly/styles/header. No full gate rerun or publication.

## Gradient colorbar compatibility fragment — source-build only

GradientColorbar.Render(min, max, color, x, y, height, label, width) builds the Navier gradient/rect/ticks/rotated-label fragment. Eleven color callback samples and three ticks, finite ordered limits/projection and #RRGGBB required. Legacy integer half-height label anchor and text escaping retained. It does not replace Heatmap's 128-strip colorbar.
Three frozen production fragments match after normalizing random ids only; 178 tests passed plus Web/quadro/adoption smoke without images. Field frame placement and contour projection remain caller-owned. No full gate rerun or publication.

## Node-vector compatibility paths — source-build only

NodeVectorOverlay.PathData preserves Navier's global magnitude normalization, stride/2 node sampling, relative-length scaling, 0.02 omission threshold and open arrowheads. Sample nodes are mapped with Y increasing upwards. It does not replace Heatmap's cell/physical quiver or add clipping at field edges.
Requires matching ≥2×2/≤100000 finite fields within ±1e12, stride 1–100000 and valid positive finite projection. Four frozen production paths match; 168 tests passed, Web/quadro and pressure-report smoke passed without images. Caller retains styling/frame/colorbar. Not new full gate or publication.

## Legacy node-field color limits — source-build only

FieldColorLimits.Resolve selects sorted floor-index percentiles, optional symmetric magnitude and legacy max=min+1 for spans below 1e-12. It ignores NaN; all-NaN input needs explicit min/max. This does not change ColorScale's constant-range center policy.
Validated Resolve domain: nonempty ≤100000 samples, finite ±1e12 or NaN, finite clipping [0,0.5], finite ordered input bounds ±1e12. Unit expansion is deliberately not clamped at the boundary. Direct record construction/NormalizeLegacy preserves raw caller arithmetic, not Resolve validation.
Eight production colorbar endpoint fixtures were frozen before migration; 154 tests passed, Web/quadro smoke passed without images. Field layout/colorbar rendering/vectors stay local. Not a new full gate or package release.

## Bilinear RGB node sampler — source-build only

BilinearRgbSampler.SampleScanlines(field, width, height, colorize, mask, maskedColor) prepares RGB8/filter-0 rows for RgbPngEncoder. Requires a node grid at least 2×2, field row zero at bottom and top-down output. Color callback controls normalization/non-finite policy; optional mask uses sample-index coordinates and requires an explicit RGB color.
Pixel-center placement and legacy arithmetic order are retained, input unchanged. Navier wrapper keeps its normalization and gray mask color. Frozen PNG fixtures still match, 138 tests passed, Web/quadro smoke passed without images. Does not consolidate field projection/colorbar/arrows or support general image formats; no new full gate or publication.

## Minimal RGB PNG encoder — source-build only

RgbPngEncoder.EncodeScanlines(width, height, scanlines) accepts top-down RGB8 rows prefixed with filter byte 0. GetScanlineByteCount validates positive dimensions and bounded integer buffer sizes. Input length and row filter bytes are checked; input remains read-only. Chunk order IHDR/IDAT/IEND, ZLib Optimal compression and private immutable CRC table.
This encodes prepared pixels, not SVG/plots, alpha or Figure/Axes. Navier sampling/masking stays local and delegates only PNG encoding. Three frozen pre-migration fixtures compare scanlines/dimensions/CRC; encoded hashes compared only on the recorded runtime/OS. 127 tests passed plus Web/quadro smoke without images. No package publication or full gate rerun.

## Shared RGB presets — source-build only

ColorPalette exposes immutable Sequential (8 stops), Diverging (9) and CompactSequential (4) presets. Charts.SequentialColor and ColorScale retain CompactSequential, preserving their previous output. Navier ColorMap delegates to the 8/9 presets and retains its special-value policy; shared Sample/Hex reject NaN/infinity and clamp finite input to endpoints.
Interval arithmetic and byte truncation preserve each historical implementation. A frozen pre-migration fixture compares 323 values per preset, including exact stops, adjacent doubles and colorbar samples. 117 tests passed; Web quadro smoke passed without images. PNG/field normalization/colorbar rendering are not migrated here. Existing preview packages do not automatically expose this new API.

## Marching squares compatibility overload — source-build only

The original three-argument overload remains available and suppresses zero-length segments. The new four-argument overload accepts preserveDegenerateSegments, used by NavierStokes to retain exact-vertex legacy segments. Sample-index coordinates, equality convention, fixed edge-order saddle pairing and mask/validation rules are unchanged.
Navier's tuple facade now uses this core; its node-to-SVG mapping, PNG/colorbar/vector renderers remain local. Shared limits apply: nonempty field, at most 100000 samples, finite samples/level within ±1e12. Results are buffered per level, not the old per-cell streaming iterator. Input outside the shared contract is not promised compatible.
109 tests passed, including all 16 binary cell cases and an exact field SVG projection. Web quadro smoke passed without images. No full gate rerun after this change, no package publication.

## Full validation with no new image artifacts

From MathemaSharp: `dotnet run --file ops/ValidateEcosystem.cs -- --no-images`. All 29 steps still run. Visual tools keep XML/DOM/encoding checks active but do not save new review PNG/SVG or screenshots; PNG encoding runs in memory. JSON-only evidence is written separately from previous images, and notebook outputs remain updated.
The gate validates report timestamps and image policy before archiving JSON. This option is also supported directly by the three visual tools. Their default saving behavior remains compatible. Last full gate: D:\Dev\BuildArtifacts\ecosystem-validation\20261028-150530, all steps exit 0. No fresh manual image review or package release is implied.

## NavierStokes adoption — current source consumer

The active `articoli/2026.10 NavierStokes` Core now references PlotSharp and delegates line/scatter while preserving public Series/Options/method signatures. CLI/Web inherit that dependency. Field/PNG/contour rendering remains local; the imported snapshot is not modified.
Seven integration cases in PlotSharp.Tests exercise the facade and retained PNG API. The ecosystem gate builds CLI/Web, runs the original full numerical selftest, and checks three production line reports in Edge on small smoke fixtures. Report Ghia uses a wider 720 px viewBox; marker/layout/validation differences are deliberate, not byte compatibility. New strict validation rejects previously unchecked invalid inputs.
This is local source integration, not a package release, full Web UI validation or complete family migration.

## Application scenarios — current adoption

MathemaSharp's cold-storage and solar scenarios now use the PlotSharp bootstrap: eight notebooks and 43 SVGs overall. Shared notebook presentation preserves KPI tiles and response quotes; no presentation or kernel dependency was added to PlotSharp.
Edge checks all 43 SVGs plus two assembled output dashboards at desktop width 1280 px. Each dashboard has five tiles and two nonempty response quotes, with no horizontal document overflow; full-page screenshots were inspected. This does not certify the VS Code notebook UI, markdown, mobile layout or the scenarios' real-world business models. Old preview packages were not changed.
The current section supersedes earlier consumer counts below.

## Current consumer scope — STL and tour

MathemaSharp now uses the source-build bootstrap in six notebooks (Core/03, TimeSeries/06–10): 38 SVGs executed and XML-validated. STL keeps its NDSharp bridge separate from graphics; HTML tables remain shared presentation. The tour's numerical C#/PySharp comparison tests remain active. Eleven new Edge screenshots were inspected; browser checks cover all 38 charts.
The shared manifest supplies expected SVG counts, required gaps/tables and the ecosystem gate's migrated steps. Earlier counts below are historical. New source-build contracts are not claimed for old NuGet previews; no packages were overwritten or published.

## Browser validation and legend row — local source-build

`SeparateLegendRow` is opt-in in Charts.LineChart, enabled by IndexedCharts. With multiple series it reserves an extra 18 px below the title/subtitle for the legend; the legacy default and single-series layout are unchanged. Indexed stems now also emit native title targets.

From MathemaSharp: `dotnet run --file ops/VerifyBrowserCharts.cs`. Requires installed Microsoft Edge; Playwright 1.56.0 uses its Chromium channel headlessly. The tool checks all 27 SVGs from the shared manifest, takes browser screenshots, rejects text outside the SVG and title/text bounding-box collisions, and requires at least one reachable native-title hit target per chart. Inline documents block network requests. Results: ops/visual/browser/report.json.

This is **not** proof that every dense/overlapping sample is independently reachable, that the OS tooltip popup appears, or that VS Code's notebook renderer works identically. Initial browser validation found three title/legend collisions; after the separate-row fix all 27 passed and three corrected browser screenshots were observed. The 19 newly migrated raster charts were reviewed as well. Distribution and wider browser coverage remain separate tasks.

## Line gaps — local source-build API

`Charts.LineChartOptions.AllowLineGaps` defaults to false. When enabled, NaN Y samples split line paths; no interpolation, markers or tooltips at missing samples. X must stay finite, infinities are rejected, each series needs at least one finite Y, and stems/bars remain strict. Axis bounds ignore missing Y but preserve the X positions of all samples. `PointTooltips` emits escaped native SVG titles at finite points.

`IndexedCharts.Lines/Panels` enable line gaps and point tooltips. This is a source-build extension, not a feature of already published preview packages. The original four-argument Stems overload is preserved; the new five-argument overload accepts an explicit index-axis label (e.g. `Stems("spectrum", values, null, 0, "k")`).

Four MathemaSharp notebooks use the source-build bootstrap: Core/03 and TimeSeries/06,07,08 (27 SVGs executed and XML-validated). Table/Kpi/Tile remain notebook presentation helpers in shared/Presentation.csx, not PlotSharp dependencies. Preliminary visual evidence is in MathemaSharp/ops/visual; rasterization there is a review tool, **not a production PNG backend**. Browser tooltip interaction and visual review of 06/08 remain open.

`PreserveBoundaryMarkers` defaults to false for Charts.LineChart compatibility. With true, finite sample markers and tooltip hit targets whose centers lie inside the closed axis domain are drawn in an unclipped point-overlays group. Out-of-domain centers are omitted, not clamped; geometry, axis scales and its tight clip are unchanged. IndexedCharts enables the option for Lines/Panels/Stems. Highlights keep their original clipping policy. Two regenerated spectrum images were visually checked after the fix: zero-baseline and maximum markers are now full circles.

## C# / console

```csharp
using PlotSharp;
var svg = Charts.LineChart(
    [new Charts.Series("Temperature", [0, 1, 2], [3, 5, 4], "#2a78d6"),
     new Charts.Series("Measurements", [0, 1, 2], [3, 4, 5], "#eb6834", Markers: true, Line: false)],
    new Charts.LineChartOptions { Title = "Comparison", XLabel = "Time", YLabel = "°C" });
File.WriteAllText("chart.svg", svg);
```

Add the reference from the consumer project:

```sh
dotnet add <consumer.csproj> reference <path>/src/PlotSharp/PlotSharp/PlotSharp.csproj
```

In notebooks pass `svg` to the kernel's HTML/SVG display. PlotSharp returns text: it does not call `Display.Html`. In Blazor a wrapper can show the output with `MarkupString`; insert only SVG produced by the library, not arbitrary user markup. The hosts were not migrated or verified in this tranche.

## Reference — active API

Namespace `PlotSharp`, classe statica `Charts`.

| API | Contract |
|---|---|
| `Series(Name, X, Y, Color, Markers=false, Line=true, Dashed=false, Stems=false, BarWidth=null)` | Paired, non-empty double arrays; colour exclusively `#RRGGBB`. Line and Markers can coexist. The arrays are not copied: do not mutate them during rendering. |
| `LineChart(series, options)` | Returns a complete SVG with background, title, axes, grid and legend when there are several series. Scatter: `Line=false, Markers=true`. |
| `LineChartOptions` | Title, Subtitle, XLabel, YLabel; Width=560, Height=340; optional XMin/XMax/YMin/YMax; optional Highlight `(X,Y,Label)`. |
| `NiceTicks(min,max,target=5)` | Ordered ticks, 1/2/2.5/5/10 scale. target from 1 to 100, finite ordered extremes. |
| `Num(value)` | Invariant numeric formatting; scientific notation for small/large values. |
| Colour constants | Surface, Ink, InkSecondary, InkMuted, Grid, Series1, Series2. |

Allowed sizes: Width 200–10000, Height 160–10000. Coordinates and extremes: finite doubles within ±1e12. Empty data, mismatches, invalid colours and inverted limits are rejected with ArgumentException (or a subclass). NaN/Infinity are rejected, not skipped. Constant series have an expanded range. Texts are escaped; colours validated. There is no model serialisation nor PNG export in the active API.

The renderer uses double coordinates and two-decimal formatting for pixels: it does not guarantee preservation of Decimal32/64/128 differences or large offsets. The clipping IDs are random: semantically equivalent output, not byte-identical. The typographic handling of long legends is still heuristic.

## Indexed series and panels

```csharp
var svg = IndexedCharts.Lines("Forecasts",
    new IndexedCharts.Line("History", [2, 4, 3]),
    new IndexedCharts.Line("Forecast", [3, 5, 6], Start: 2, Dashed: true));
var html = IndexedCharts.Panels("Components",
    new IndexedCharts.Line("Trend", [100, 110, 120]),
    new IndexedCharts.Line("Residual", [-0.1, 0.2, -0.1]));
```

`IndexedCharts.Line(Name,Y,Start=0,Dashed=false)` describes X=Start+index. `ToSeries(line,paletteIndex=0)` copies Y, generates X in double without int overflow and applies a cyclic palette of eight colours. `Lines(title,params Line[])` returns a 720×260 SVG with a shared Y scale; `Panels(title,params Line[])` returns HTML with 720×180 SVGs, independent Y scales. No function calls Display.Html. Empty and non-finite inputs are rejected as in the base renderer; the facade does not replicate the old MathemaSharp NaN-gap policy.

`Dashed=true` applies the 6/4 dash to the path and to the legend. The clipping now coincides exactly with the plot area; the IDs use full GUIDs.

## Stems, bars and heatmap

```csharp
var acf = IndexedCharts.Stems("ACF", [0.7, 0.3, -0.2], band: 0.2, firstLag: 1);
var bars = Charts.LineChart(
    [new Charts.Series("Balance", [0, 1, 2], [4, -2, 3], Charts.Series1,
        Line: false, BarWidth: 0.8)], new() { Title = "Balance" });
var map = Charts.Heatmap(new double[,] { { 0, 1 }, { 2, 3 } },
    new() { Title = "Field", Min = 0, Max = 3, OriginUpper = false });
```

- `Series.Stems=true`: vertical segments from zero to the value; set `Line=false` so as not to connect the points. `Markers=true` adds circles. Automatic axes include zero.
- `Series.BarWidth`: positive width in X units, not pixels. Bars go from zero to Y; automatic axes include the edges and zero. Explicit limits can cut bars/stems through clipping. There are no categories, stacking or automatic grouping yet; overlapping bars are drawn in series order. If BarWidth and Stems coexist, bars prevail.
- `IndexedCharts.Stems(title,values,band=null,firstLag=1)`: 720×260 correlogram. Finite non-negative band, two lines at ±band; null means no band (NaN is not an allowed sentinel). Y is not forced symmetric.
- `Heatmap(field,options=null)`: cell-based SVG, field[row,column]. `HeatmapOptions`: Title, Width=560, Height=340, optional Min/Max, OriginUpper=false. Row zero at the bottom by default, at the top with OriginUpper=true. 1×1 matrices are also allowed; from 1 to 100000 cells. Do not mutate the field during rendering.
- Heatmap normalisation: `(value-Min)/(Max-Min)` saturated in [0,1]; a constant field/range uses the central colour. Finite data/limits within ±1e12, ordered limits; NaN/Infinity rejected. The palette is a sequential four-stop one derived from NavierStokes, not the whole original palette.
- `SequentialColor(normalized)`: accepts finite doubles, saturates at the extreme colours, returns `#RRGGBB`.

Heatmap includes cell tooltips and min/max text; colorbar, spatial ticks and masks are optional (see below). It does not yet include bilinear interpolation or PNG. The cells are SVG rectangles: for large fields the raster backend will be needed. The gallery now also generates `stems.svg`, `bars.svg`, `heatmap.svg`.

## Physical fields, masks and colorbar

```csharp
var svg = Charts.Heatmap(new double[,] { { double.NaN, 10 }, { 15, 25 } },
    new() {
        Title = "Temperature", ShowColorbar = true, ColorbarLabel = "°C",
        Extent = new FieldExtent(0, 2, -1, 1),
        Mask = new bool[,] { { true, false }, { false, false } }
    });
var scale = new ColorScale(10, 25);
var normalized = scale.Normalize(15);
var color = scale.Color(15);
```

`ColorScale(Min,Max)` is immutable: finite ordered extremes within ±1e12; `Normalize(value)` saturates finite values in [0,1], constant range → 0.5; `Color(value)` applies SequentialColor. The normalisation is shared between field and legend, with no change of policy for existing calls.

New `HeatmapOptions` properties:

| Property | Contract |
|---|---|
| `ShowColorbar=false` | Optional vertical legend, minimum SVG width 320. Reduces the field area so as not to overlap; 128 colour stripes, min/mid/max ticks. Constant range: a single colour and tick. |
| `ColorbarLabel=""` | Text is escaped, no arbitrary markup. Long labels do not yet have automatic layout. |
| `Extent=null` | `FieldExtent(XMin,XMax,YMin,YMax)` defines the **outer edges** of the field, not sampled centres. Finite extremes within ±1e12, strictly increasing. Enables spatial ticks. X grows to the right; Y grows upwards with OriginUpper=false, downwards with true. |
| `Mask=null` | Bool matrix of the same shape as the field: true omits the cell and excludes the value from the automatic limits. NaN/Infinity values are allowed only if masked. Missing cells show the background, not a special colour. |

A fully masked field requires both Min and Max explicit; it can show a colorbar without cells. The mask is not copied: do not mutate field/Mask during rendering. The 100000 limit applies to all cells, not only the visible ones. Tooltips remain indexed `[row,column]`; Extent changes the ticks, not the matrix nor the number of cells. The gallery adds `physical-field.svg`.

## Contour and quiver

```csharp
var svg = Charts.Heatmap(new double[,] { { 0, 1 }, { 1, 2 } },
    new() {
        Title = "Field", ContourLevels = [0.5, 1.5], ContourColor = "#ffffff",
        Vectors = new Charts.VectorField(
            new double[,] { { 1, 1 }, { 1, 1 } },
            new double[,] { { 0, 0.5 }, { 0, 0.5 } }, Scale: 0.25)
    });
var segments = Charts.MarchingSquares(new double[,] { { 0, 1 }, { 0, 1 } }, 0.5);
```

- `MarchingSquares(field,level,mask=null)` returns `IReadOnlyList<Charts.Segment>` with X1/Y1/X2/Y2 in **sample indices**, not SVG/physical coordinates. Linear interpolation of the crossings along the edges; equality belongs to the upper side. Field at least 1×1: without quadrilaterals the result is empty. Immediate validation, same limits as heatmap.
- `HeatmapOptions.ContourLevels`: null disables; at most 32 finite levels within ±1e12, duplicates removed. ContourColor exclusively #RRGGBB. The field is interpreted as sampled at the cell **centres**: contour does not reach the outer edges and does not extrapolate. There are no labelled isolines or fills.
- Marching-squares cells with at least one masked vertex are excluded. Zero-length segments omitted; cells constant at the level generate no contour. In saddle cases with four crossings the fixed pair by edge order of the original NavierStokes is kept: deterministic behaviour, **not an asymptotic decider**, nor a topological guarantee of the bilinear field.
- `VectorField(U,V,Scale=1,Stride=1,Color="#0b0b0b")`: matrices of the same shape as the heatmap, finite values within ±1e12 except masked samples. Arrows from the cell centres; Scale positive finite ≤1e6, Stride integer from 1 to 100000, colour #RRGGBB. Stride subsamples rows and columns starting from index 0.
- Without Extent, U/V are displacements in index units; with Extent they are displacements in its physical units. Positive V follows the Y axis: upwards with OriginUpper=false, downwards with true. Scale is not autoscale. Non-finite projections or beyond ±1e7 pixels rejected.
- Projected arrows shorter than 0.5 pixel omitted, head reduced on short arrows. Contour and quiver are overlaid in this order, clipped to the field area; the mask suppresses the samples, it does not geometrically clip arrows crossing a masked cell.

These APIs are not automatically compatible with NDSharp.Quiver or NavierStokes.FieldPlot. They do not include streamlines, contour stitching or non-uniform coordinates. The gallery adds `overlays.svg`.

## Regenerating the import catalogue

From the repository root:

```sh
dotnet run --project src/tools/PlotSharp.Inventory/PlotSharp.Inventory.csproj -- .
```

The tool writes exclusively `ops/inventory/files.csv`, `public-declarations.csv`, `notebook-calls.csv`, `references.csv`; it does not change the snapshots or the manifest. It is a lexical scanner: public lines and Chart/Charts/IndexedCharts/plt calls in code cells, not a semantic compatibility verification. It does not include the `.ralfai/undo` backups in the analysis of the current consumers. See ARCHITECTURE for limits and the destination matrix.

## Future migration

- NavierStokes line/scatter: replace namespace `NavierStokes.Core` with `PlotSharp` where these APIs are used; do not yet remove FieldPlot/FieldToPng.
- MathemaSharp: use `IndexedCharts.Line` and `IndexedCharts.Lines/Panels`, passing the result to the kernel's HTML display. The facade supports Start, Dashed and Stems with an optional band. Table/Tiles/Quote are presentation, not Charts.
- NDSharp: keep the existing Figure/Axes until a compatible facade is available; the NDArray adapter must stay outside the core.
- JupyterSharp: display host and assembly distribution, not the source of a new renderer. Do not replace DLLs in the snapshots.

These are planning instructions, not migrations already carried out.
