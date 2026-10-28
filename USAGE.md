# Usage

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
    [new Charts.Series("Temperatura", [0, 1, 2], [3, 5, 4], "#2a78d6"),
     new Charts.Series("Misure", [0, 1, 2], [3, 4, 5], "#eb6834", Markers: true, Line: false)],
    new Charts.LineChartOptions { Title = "Confronto", XLabel = "Tempo", YLabel = "°C" });
File.WriteAllText("chart.svg", svg);
```

Aggiungere il riferimento dal progetto consumatore:

```sh
dotnet add <consumer.csproj> reference <path>/src/PlotSharp/PlotSharp/PlotSharp.csproj
```

Nei notebook passare `svg` al display HTML/SVG del kernel. PlotSharp restituisce testo: non invoca `Display.Html`. In Blazor un wrapper può visualizzare l'output con `MarkupString`; inserire solo SVG prodotto dalla libreria, non markup arbitrario dell'utente. Gli host non sono stati migrati o verificati in questa tranche.

## Reference — API attiva

Namespace `PlotSharp`, classe statica `Charts`.

| API | Contratto |
|---|---|
| `Series(Name, X, Y, Color, Markers=false, Line=true, Dashed=false, Stems=false, BarWidth=null)` | Array double accoppiati, non vuoti; colore esclusivamente `#RRGGBB`. Line e Markers possono coesistere. Gli array non sono copiati: non mutarli durante il rendering. |
| `LineChart(series, options)` | Restituisce SVG completo con sfondo, titolo, assi, griglia e legenda quando ci sono più serie. Scatter: `Line=false, Markers=true`. |
| `LineChartOptions` | Title, Subtitle, XLabel, YLabel; Width=560, Height=340; XMin/XMax/YMin/YMax opzionali; Highlight opzionale `(X,Y,Label)`. |
| `NiceTicks(min,max,target=5)` | Tick ordinati, scala 1/2/2.5/5/10. target da 1 a 100, estremi finiti ordinati. |
| `Num(value)` | Formattazione numerica invariant; notazione scientifica per valori piccoli/grandi. |
| Costanti colori | Surface, Ink, InkSecondary, InkMuted, Grid, Series1, Series2. |

Dimensioni ammesse: Width 200–10000, Height 160–10000. Coordinate ed estremi: double finiti entro ±1e12. Dati vuoti, mismatch, colori non validi e limiti invertiti sono rifiutati con ArgumentException (o sottoclasse). NaN/Infinity sono rifiutati, non saltati. Serie costanti hanno intervallo espanso. I testi sono escaped; i colori validati. Non c'è serializzazione del modello né export PNG nella API attiva.

Il renderer usa coordinate double e formattazione a due decimali per i pixel: non garantisce conservazione di differenze Decimal32/64/128 o grandi offset. Gli ID di clipping sono casuali: output semanticamente equivalente, non byte-identico. La gestione tipografica delle legende lunghe è ancora euristica.

## Serie indicizzate e pannelli

```csharp
var svg = IndexedCharts.Lines("Previsioni",
    new IndexedCharts.Line("Storico", [2, 4, 3]),
    new IndexedCharts.Line("Forecast", [3, 5, 6], Start: 2, Dashed: true));
var html = IndexedCharts.Panels("Componenti",
    new IndexedCharts.Line("Trend", [100, 110, 120]),
    new IndexedCharts.Line("Residuo", [-0.1, 0.2, -0.1]));
```

`IndexedCharts.Line(Name,Y,Start=0,Dashed=false)` descrive X=Start+indice. `ToSeries(line,paletteIndex=0)` copia Y, genera X in double senza overflow int e applica una palette ciclica di otto colori. `Lines(title,params Line[])` restituisce SVG 720×260 con scala Y condivisa; `Panels(title,params Line[])` restituisce HTML con SVG 720×180, scale Y indipendenti. Nessuna funzione chiama Display.Html. Input vuoti e non finiti sono rifiutati come nel renderer base; la facade non replica la vecchia policy MathemaSharp dei gap NaN.

`Dashed=true` applica il tratteggio 6/4 al path e alla legenda. Il clipping ora coincide esattamente con l'area del plot; gli ID usano GUID completi.

## Stems, barre e heatmap

```csharp
var acf = IndexedCharts.Stems("ACF", [0.7, 0.3, -0.2], band: 0.2, firstLag: 1);
var bars = Charts.LineChart(
    [new Charts.Series("Bilancio", [0, 1, 2], [4, -2, 3], Charts.Series1,
        Line: false, BarWidth: 0.8)], new() { Title = "Bilancio" });
var map = Charts.Heatmap(new double[,] { { 0, 1 }, { 2, 3 } },
    new() { Title = "Campo", Min = 0, Max = 3, OriginUpper = false });
```

- `Series.Stems=true`: segmenti verticali da zero al valore; impostare `Line=false` per non collegare i punti. `Markers=true` aggiunge cerchi. Gli assi automatici includono zero.
- `Series.BarWidth`: larghezza positiva in unità X, non pixel. Le barre vanno da zero a Y; gli assi automatici includono i bordi e zero. Limiti espliciti possono tagliare barre/aste tramite clip. Non ci sono ancora categorie, stacking o raggruppamento automatico; barre sovrapposte sono disegnate in ordine di serie. Se BarWidth e Stems coesistono prevalgono le barre.
- `IndexedCharts.Stems(title,values,band=null,firstLag=1)`: correlogramma 720×260. Banda finita non negativa, due linee a ±band; null significa nessuna banda (NaN non è un sentinel ammesso). Y non è forzato simmetrico.
- `Heatmap(field,options=null)`: SVG a celle, field[row,column]. `HeatmapOptions`: Title, Width=560, Height=340, Min/Max opzionali, OriginUpper=false. Riga zero in basso per default, in alto con OriginUpper=true. Sono ammesse anche matrici 1×1; da 1 a 100000 celle. Non mutare il campo durante rendering.
- Normalizzazione heatmap: `(value-Min)/(Max-Min)` saturata in [0,1]; campo/range costante usa il colore centrale. Dati/limiti finiti entro ±1e12, limiti ordinati; NaN/Infinity rifiutati. La palette è sequenziale a quattro stop derivati da NavierStokes, non l'intera palette originale.
- `SequentialColor(normalized)`: accetta double finiti, satura ai colori estremi, restituisce `#RRGGBB`.

Heatmap include tooltip di cella e testo min/max; colorbar, tick spaziali e maschere sono opzionali (vedi sotto). Non include ancora interpolazione bilineare o PNG. Le celle sono rettangoli SVG: per campi grandi servirà il backend raster. La galleria ora genera anche `stems.svg`, `bars.svg`, `heatmap.svg`.

## Campi fisici, maschere e colorbar

```csharp
var svg = Charts.Heatmap(new double[,] { { double.NaN, 10 }, { 15, 25 } },
    new() {
        Title = "Temperatura", ShowColorbar = true, ColorbarLabel = "°C",
        Extent = new FieldExtent(0, 2, -1, 1),
        Mask = new bool[,] { { true, false }, { false, false } }
    });
var scale = new ColorScale(10, 25);
var normalized = scale.Normalize(15);
var color = scale.Color(15);
```

`ColorScale(Min,Max)` è immutabile: estremi finiti ordinati entro ±1e12; `Normalize(value)` satura valori finiti in [0,1], range costante → 0.5; `Color(value)` applica SequentialColor. La normalizzazione è condivisa fra campo e legenda, senza cambio di policy per le chiamate esistenti.

Nuove proprietà `HeatmapOptions`:

| Proprietà | Contratto |
|---|---|
| `ShowColorbar=false` | Legenda verticale opzionale, larghezza minima SVG 320. Riduce l'area del campo per non sovrapporsi; 128 strisce cromatiche, tick min/medio/max. Range costante: un solo colore e tick. |
| `ColorbarLabel=""` | Testo escaped, nessun markup arbitrario. Etichette lunghe non hanno ancora layout automatico. |
| `Extent=null` | `FieldExtent(XMin,XMax,YMin,YMax)` definisce **bordi esterni** del campo, non centri campionati. Estremi finiti entro ±1e12, strettamente crescenti. Attiva tick spaziali. X cresce verso destra; Y cresce verso l'alto con OriginUpper=false, verso il basso con true. |
| `Mask=null` | Matrice bool della stessa forma del campo: true omette la cella ed esclude il valore dai limiti automatici. Valori NaN/Infinity sono ammessi solo se mascherati. Le celle mancanti mostrano lo sfondo, non un colore speciale. |

Un campo interamente mascherato richiede entrambi Min e Max espliciti; può mostrare una colorbar senza celle. La maschera non è copiata: non mutare field/Mask durante rendering. Il limite di 100000 riguarda tutte le celle, non solo quelle visibili. I tooltip restano indicizzati `[row,column]`; Extent cambia i tick, non la matrice né il numero delle celle. La galleria aggiunge `physical-field.svg`.

## Contour e quiver

```csharp
var svg = Charts.Heatmap(new double[,] { { 0, 1 }, { 1, 2 } },
    new() {
        Title = "Campo", ContourLevels = [0.5, 1.5], ContourColor = "#ffffff",
        Vectors = new Charts.VectorField(
            new double[,] { { 1, 1 }, { 1, 1 } },
            new double[,] { { 0, 0.5 }, { 0, 0.5 } }, Scale: 0.25)
    });
var segments = Charts.MarchingSquares(new double[,] { { 0, 1 }, { 0, 1 } }, 0.5);
```

- `MarchingSquares(field,level,mask=null)` restituisce `IReadOnlyList<Charts.Segment>` con X1/Y1/X2/Y2 in **indici campione**, non coordinate SVG/fisiche. Interpolazione lineare degli attraversamenti lungo i bordi; equality appartiene al lato alto. Campo almeno 1×1: senza quadrilateri il risultato è vuoto. Validazione immediata, stessi limiti di heatmap.
- `HeatmapOptions.ContourLevels`: null disabilita; massimo 32 livelli finiti entro ±1e12, duplicati rimossi. ContourColor esclusivamente #RRGGBB. Il campo è interpretato come campionato ai **centri** delle celle: contour non arriva ai bordi esterni e non extrapola. Non ci sono isolinee etichettate o riempimenti.
- Le celle marching squares con almeno un vertice mascherato sono escluse. Segmenti di lunghezza zero omessi; celle costanti al livello non generano contour. Nei casi a sella con quattro attraversamenti si conserva la coppia fissa per ordine dei bordi dell'originale NavierStokes: comportamento deterministico, **non un asymptotic decider**, né una garanzia topologica del campo bilineare.
- `VectorField(U,V,Scale=1,Stride=1,Color="#0b0b0b")`: matrici della stessa forma della heatmap, valori finiti entro ±1e12 salvo campioni mascherati. Frecce dai centri delle celle; Scale positivo finito ≤1e6, Stride intero da 1 a 100000, colore #RRGGBB. Stride sottocampiona righe e colonne partendo da indice 0.
- Senza Extent, U/V sono spostamenti in unità indice; con Extent sono spostamenti nelle sue unità fisiche. V positivo segue l'asse Y: verso alto con OriginUpper=false, verso basso con true. Scale non è autoscale. Proiezioni non finite o oltre ±1e7 pixel rifiutate.
- Frecce proiettate più corte di 0.5 pixel omesse, testa ridotta sulle frecce corte. Contour e quiver sono sovrapposti in quest'ordine, ritagliati all'area del campo; la maschera sopprime i campioni, non ritaglia geometricamente frecce che attraversano una cella mascherata.

Queste API non sono compatibili automaticamente con NDSharp.Quiver o NavierStokes.FieldPlot. Non includono streamlines, contour stitching o coordinate non uniformi. La galleria aggiunge `overlays.svg`.

## Rigenerare il catalogo degli import

Dalla root del repository:

```sh
dotnet run --project src/tools/PlotSharp.Inventory/PlotSharp.Inventory.csproj -- .
```

Il tool scrive esclusivamente `ops/inventory/files.csv`, `public-declarations.csv`, `notebook-calls.csv`, `references.csv`; non cambia gli snapshot o il manifest. È uno scanner lessicale: righe public e chiamate Chart/Charts/IndexedCharts/plt nelle code cells, non una verifica di compatibilità semantica. Non include i backup `.ralfai/undo` nell'analisi dei consumatori correnti. Consultare ARCHITECTURE per limiti e matrice di destinazione.

## Migrazione futura

- NavierStokes line/scatter: sostituire namespace `NavierStokes.Core` con `PlotSharp` nei punti che usano queste API; non eliminare ancora FieldPlot/FieldToPng.
- MathemaSharp: usare `IndexedCharts.Line` e `IndexedCharts.Lines/Panels`, passando il risultato al display HTML del kernel. La facade supporta Start, Dashed e Stems con banda opzionale. Table/Tiles/Quote sono presentazione, non Charts.
- NDSharp: mantenere Figure/Axes esistenti fino a disponibilità di una facade compatibile; l'adattatore NDArray deve stare fuori dal nucleo.
- JupyterSharp: host di display e distribuzione assembly, non fonte di un nuovo renderer. Non sostituire DLL negli snapshot.

Queste sono istruzioni di pianificazione, non migrazioni già eseguite.
