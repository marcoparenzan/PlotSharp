# Architecture and inventory

## Layout

- `src/PlotSharp.slnx`: active .NET 11 solution.
- `src/PlotSharp/PlotSharp/`: library, Charts API and SVG renderer.
- `src/PlotSharp.Tests/PlotSharp.Tests/`: xUnit tests.
- `src/imports/<origin>/`: unchanged archive, excluded from the solution.
- `import-manifest.csv`: provenance and hashes of the original imports; it does not describe the hashes of extracted/modified files.
- `notebooks/`: space reserved for the future gallery.
- `ops/`: operations space; no pipeline activated.

## Regenerable inventory (P01)

Active tool `src/tools/PlotSharp.Inventory`, with no dependency on PlotSharp. Tests references it to test CSVs, declarations, notebook decoding and mapping. Output in `ops/inventory/`: files.csv (provenance/classification/hash), public-declarations.csv (declarations and proposed destination), notebook-calls.csv (cell/line/call), references.csv (ProjectReference/PackageReference and csproj presence).

Last run: 148 files, 478 lines with public declarations, 68 chart/display occurrences in the notebooks, 86 project/package references, 0 missing ProjectReference in the current perimeter. **Zero missing csproj does not prove complete sources, restore or build of the consumers.** `.ralfai/undo` imports were found: they stay in the file/hash catalogue as provenance, but are excluded from the current API/uses/references. No snapshot was modified.

Limits: lexical scanner, not Roslyn/semantic API; it does not collect non-public declarations, does not reconstruct multi-line signatures nor the full scope and may include public lines in multi-line comments. Notebooks analysed via JSON (Unicode escapes decoded), only the source of code cells, not output/markdown; the Chart/Charts/IndexedCharts/plt regex does not cover aliases or dynamic calls and may include strings/comments. The generated CSVs are review baselines, not approved compatibility decisions. Do not edit them manually: change the tool's rules and regenerate; curated decisions are in this matrix/roadmap.

### Destination matrix for NDSharp.Plot material

All rows are **proposals to verify**, not completed extractions. The individual declarations and line numbers are in the CSV.

| Source | Surface collected | Proposed destination / status |
|---|---|---|
| Artists.cs | Artist/DrawContext, Line2D, Scatter, Patch/Rect/Ellipse/Polygon, TextArtist, ImageArtist, ArrowProps/CoordSpace | P03 model + P05 backend; no complete equivalence with the current Series |
| Axes.cs | GridCell, Legend, Axes, limits/inversions, linear/log ticks, coordinate conversion, Colorbar | P05 Figure/Axes facade; log ticks/inversions/advanced legends missing |
| Figure.cs | Figure/SubplotParams, Renderer, Layout/tight_layout, RenderPng | P05 figure model + raster renderer; not consolidated |
| FigureExtensions.cs | Figure extensions | P05 compatibility, to be brought along with the model |
| Colormap.cs / Colormaps.g.cs | Colormap/Colors and generated palettes | P03 scale/colormap + P05 compatibility; complete palette not extracted |
| Contour.cs | ContourArtist/IMappable and algorithm | P05 contour/fill compatibility; the current overlay does not replace this API |
| Quiver.cs | Quiver Artist, bounds and shaft/head geometry | Geometry adapted in FieldOverlays, Artist/bounds/legend API not consolidated |
| Plot3D.cs | Artist3D, Line3D/Scatter3D/Poly3D/Text3D, projection | P05 model/projection/backend; neither excluded nor implemented |
| Text.cs | Fonts, TextMetrics, MathText | P05 typography/backend; fonts and licences to be preserved |
| Ticks.cs | Locator/formatter, offset, log | P03/P05 axes; the current NiceTicks is not equivalent |
| Fonts/*.ttf and LICENSE_DEJAVU | Third-party assets | P02/P05 notice and backend resources; not embedded |
| HighLevelTests.cs | Original plotting API tests | P05 compatibility tests; archived, not run |
| tools/oracle/gen_colormaps.py | Palette generation | Source tool to keep as provenance/generation; not core runtime |

NavierStokes: LineChart/Series/options/Num/NiceTicks adapted; MarchingSquares adapted with a different contract; FieldPlot/FieldToPng and the complete ColorMap not replaced. MathemaSharp: Lines/Panels/Stems and Line have a partial facade; display, NaN-gap, tooltip and scales differ; Table/Tiles/Quote remain presentation to be classified. JupyterSharp: host/assembly/notebook are consumers, not a new renderer; PySharpLib.Matplotlib is available only as a binary, no source API reconstructed.

## Functional inventory and decisions

The initial import declares 148 files (MathemaSharp 54, NDSharp 50, JupyterSharp 29, NavierStokes 15); the manifest remains the precise source of the heuristic selection. It is not a complete inventory of the original repositories.

| Source | Material / dependencies | Decision |
|---|---|---|
| NavierStokes/Core/Charts.cs | SVG lines, scatter through a flag, legend/ticks; field PNG written in C# with ZLib, colour map, marching squares and arrows | Extract line/scatter and helpers in the first tranche; fields remain archive until dedicated validation. |
| NavierStokes/Web/Shared/Chart.razor | Displays an SVG string through MarkupString | Keep as an example of a host adapter, not a core dependency. |
| NavierStokes/Reports.cs, pages and notebook | Consumers of simulation data | Do not transfer application logic into the library. |
| MathemaSharp/notebooks/shared/Charts.csx | Lines, Panels, Stems; Display.Html; tables, tiles and quotes | Reuse composition/presentation later; do not import the kernel. Conceptual duplicate of the NavierStokes lines, not a second active renderer. |
| NDSharp.Plot | Figure/Axes/Artists, contour, quiver, 3D, colormap, ticks, layout and PNG | Richest asset to preserve; separate the matplotlib-shaped model, the Skia backend and data adaptation before extracting it. |
| NDSharp.Plot.Tests/HighLevelTests.cs | High-level tests of the previous API | Reference for future compatibility; not run by the active solution. |
| NDSharp.Plot/Fonts | DejaVu Sans, three TTFs and LICENSE_DEJAVU | Third-party assets, not embedded in the current library. |
| JupyterSharp | Notebooks, kernel/host csproj and NDSharp.Plot / PySharpLib.Matplotlib DLLs | Display/runtime integration; the DLLs are not an authoritative source and must not be recompiled or redistributed blindly. |

NDSharp.Plot requires NDSharp, SkiaSharp 3.119.0 and Linux/macOS native assets; the dependency snapshots are not necessarily complete. The application csprojs collected do not imply the presence of all the required classes. No snapshot was compiled in this tranche.

## Provenance of the extraction

`src/PlotSharp/PlotSharp/Charts.cs` derives from `src/imports/NavierStokes/src/NavierStokes.Core/Charts.cs`: Series, LineChartOptions, LineChart, Header, NiceTicks, Num, Esc and the colours were kept; fields/PNG/contour were excluded. Namespace changed; validation of coordinates, bounds, sizes and colours added; the guard on the ticks strengthened. Another graphics library was not implemented from scratch.

Licences: NDSharp includes LICENSE and MIT headers; the fonts have a separate licence. JupyterSharp includes LICENSE. NavierStokes, MathemaSharp, NDSharp and JupyterSharp are the author's own code, and since the decision of 2026-10-29 the whole suite, this repository included, is MIT, Copyright (c) 2026 Marco Parenzan. The snapshots in `src/imports` do not all carry their own LICENSE file; their origin is not audited (suite L3). The DejaVu fonts keep their separate licence and notice. Publishing a package is a separate decision, no longer blocked for rights.

## Boundaries

For now a single small assembly: separate backends/adapters only when a second one is implemented. The core does not depend on Microsoft.Extensions because it has no services, configuration or logging; such components belong to the future apps/integrations. Do not modify the originals nor insert the imports indiscriminately into the solution.

Active tests: XML/escaping, line/scatter, empty and inconsistent inputs, colours/injection, non-finite data, constant series, limits, sizes, ticks and Italian culture. They are not visual regression tests, nor PNG or full-compatibility tests. No visual verification or executed notebook is claimed.

## Fifth tranche

`FieldOverlays.cs`: MarchingSquares derives from the algorithm in NavierStokes.Core/Charts.cs (edge order and pairing kept). Eager validation, masks, Segment results and removal of degenerate segments added. The saddle cases keep a fixed pairing: an asymptotic decider remains a future option to compare, not a declared property of the current algorithm.

The SVG quiver geometry adapts the shaft/triangle construction of `src/imports/NDSharp/src/NDSharp.Plot/Quiver.cs`, origin NDSharp, copyright Marco Parenzan, repository with MIT licence in `src/imports/NDSharp/LICENSE`. Distribution has to keep the complete MIT notice. No SkiaSharp dependency introduced: head in pixels (8 maximum) instead of points/DPI, explicit Scale.

Heatmap interprets values and vectors at the cell centres; FieldExtent continues to describe the outer edges. Overlays share plotW/plotH and the field orientation and use unique GUID clips. The mask excludes a contour quadrilateral if one of the samples is excluded; quiver removes only the masked samples, not the parts of the arrow over other excluded cells. The vector mapping can distort angles if the physical aspect ratio is not preserved.

66 passing tests, including linear field, saddle, equality, singleton, masks, vector direction with both origins, invalid shape/stride/colour. Gallery run with eight validated XML examples. No visual verification or executed notebook is claimed.

## Fourth tranche

`ColorScale.cs` separates normalisation from the renderer. Immutable Min/Max, values saturated before the subtraction, colours obtained from the existing palette. Heatmap and colorbar share the contract; the colorbar's 128 stripes sample the same palette, with ticks at the extremes and in the centre. The legend is optional and the default layout of the previous heatmaps is preserved.

`FieldExtent` describes physical outer edges; the SVG grid is still uniform. It does not preserve the physical aspect ratio and does not represent non-uniform coordinates. Y ticks consistent with OriginUpper, tooltip centres still expressed as indices. Mask true omits the cell before analysing its value: masked non-finite values are valid, unmasked ones are rejected. A fully masked field is admitted only with an explicit range. This does not change the reject policy of the lines.

50 active passing tests; gallery with seven examples run and XML validated. No visual browser/notebook verification or migration of the original repositories was performed.

## Third tranche

Stems and BarWidth extend Series in the same renderer, keeping shared axes, validations and clipping. `IndexedCharts.Stems` translates the MathemaSharp correlogram into a series of stems plus two dashed series for the band: no second SVG engine.

`Heatmap.cs` extends Charts through partial and uses the common Header/F/Num. New cell-based SVG path: default orientation [j,i] with j=0 at the bottom like NavierStokes, unlike imshow which may use an upper origin. A deliberate choice for a first API without native dependencies: it is neither a port of the NavierStokes bilinear PNG nor a compatible replacement of NDSharp.Axes.imshow. Four stops of the NavierStokes palette are reused; linear colour interpolation, normalisation and saturation separated from the spatial mapping. Limit of 100000 cells to contain the output; raster backend and colorbar still to be extracted.

The executed gallery now includes six examples: lines, scatter, panels, stems, bars and heatmap. 31 passing tests, including positive/negative baselines, band, orientation, extreme/constant colours, limits and empty/non-finite/oversized matrices. The snapshots remain unchanged; licensing constraints and the absence of visual checks remain valid.

## Second tranche

`IndexedCharts.cs` is a host-independent facade inspired by Line/Lines/Panels of the MathemaSharp material: it always delegates to Charts.LineChart, it does not embed a second renderer. It does not replicate the original NaN-gap policy: the contract remains reject. Dashed applies to path and legend, the clipping IDs use full GUIDs and the clip coincides with the axes rectangle.

`src/PlotSharp.Gallery/PlotSharp.Gallery/` is an executable without services/configuration; it generates `artifacts/gallery/index.html`, lines.svg, scatter.svg and panels.html. Run successfully, with XML parsing of the examples before writing. The outputs are not reference snapshots and can be regenerated. Total active tests: 19, including mapping of explicit extremes, independent scales, data copy, dashing and null text. No browser or notebook verification was performed.
