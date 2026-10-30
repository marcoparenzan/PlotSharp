# PlotSharp — roadmap and completion status

This document is the register of the product and of the consumers. It replaces the previous sequence of tranches: a feature being present does not imply that the original material is consolidated or that a consumer has been migrated.

## 1. Project objective

A shared visualisation library, independent of host and mathematics libraries, that becomes the authoritative source of the Charts distributed today among MathemaSharp, NDSharp, NavierStokes and JupyterSharp. The consumers must use PlotSharp directly or through adapters/facades, without maintaining duplicated engines. The previous APIs must have an explicit migration path; any exclusions must be deliberated, not confused with completed work.

Current target of the active projects: .NET 11 RC3 (imported snapshots excluded). Active output: static SVG/HTML.

### Current realigned state — 2026-10-29

This section prevails over the tables and the historical notes further down (R01 of the `Science` ROADMAP.md).

- **Baseline:** `--no-images` gate rerun, all 31 steps exit 0 (plus `skip-report`), report `D:\Dev\BuildArtifacts\ecosystem-validation\20261028-230541`. `PlotSharp.Tests` suite rerun separately: **227/227**. PySharp in the same gate: 1507 passed, 0 failed, 13 suspended (12 PostgreSQL unreachable, 1 opt-in pretrained download): not counted as passed.
- **Shared and already consumed:** NavierStokes line/scatter and FieldPlot (palette, RGB8 encoder, node sampler, colour limits, arrows, colorbar, layout, assembly, contour); linear contours, physical sampler and band selection of NDSharp.Plot; eight MathemaSharp notebooks.
- **Still local in the consumers:** palette/alpha/native raster and Skia renderer of NDSharp.Plot, Figure/Axes/Artists, the NavierStokes article notebook, the legacy MathemaSharp renderer kept in the paths not migrated.
- **Effective dependencies on PlotSharp** (ProjectReference in the active sources): `NDSharp.Plot`, `NavierStokes.Core`, besides `PlotSharp.Tests` and `PlotSharp.Gallery`. MathemaSharp consumes through an opt-in source-build facade.
- **Unchanged:** 0/4 families complete, no package published, rights (P02/B01) unresolved.
- **Version control:** the repository has been under git since 2026-10-29 (baseline commit `dfc7b2b`).

Update 2026-10-28: ecosystem gate run successfully, including the two NuGet pilots. Added in MathemaSharp the opt-in pilot `03-indexed-charts.ipynb` and the facade `notebooks/shared/PlotSharpCharts.csx`: Lines, Panels and Stems delegate to IndexedCharts, four SVGs run and XML-validated. It does not amount to a complete migration of MathemaSharp: legacy renderer kept, non-finite data and layout not equivalent, visual verification still open. The local counts below are PlotSharp's historical baseline; the central gate is documented in MathemaSharp/ROADMAP.md. PNG/Skia and Figure/Axes compatibility are to be consolidated, not already available.

### Updated complete baseline — D03 sampler/bands

Gate --no-images D:\Dev\BuildArtifacts\ecosystem-validation\20261028-215324, 31/31 exit0 after shared physical sampler and band lookup. Build/test/audit and real kernel/notebook/UI/browser/complete selftest integration reconfirmed. No new PNG/SVG/screenshot review, embedded notebook outputs and in-memory raster. Warnings/suspended not hidden; recent JSON evidence archived.
It prevails over the previous non-full-gate notes. D03 not completely migrated: palette/alpha/native raster/other categories/consumer/licences/versions remain separate. No publication.

### Previous targeted verification D03 — band lookup and midpoint

Inclusive selection/duplicates/final bound and midpoint normalisation shared in ContourBands, preserving the legacy operation order. External/non-finite data without a band, finite/non-decreasing level snapshots. Colour/alpha/Skia renderer remain ND. Shared227/227 eND19/19 with green build, real kernel/notebook rechecked, zero review images. Not a new full gate or release.

### Previous targeted verification D03 — physical bilinear sampler of filled contours

NDSharp Sample delegated to RectilinearFieldSampler with snapshots and legacy arithmetic; 81 frozen queries coincide. Increasing/finite axes and consistent shape, finite queries/extrapolation preserved, non-finite values left to the caller. Bands/colours/alpha/Skia remain ND; line geometry and Navier sampler unaltered.
Shared211/211 eND19/19, rebuilt kernel source and real lines/bands320×240 notebook verified, zero review images. Known warnings not suppressed. Not a new full gate/release; D03 partial.

### Previous D03 integration in the real kernel — complete baseline

New NDSharp/pysharp/02-contours-kernel notebook run in JupyterNet/PySharp (.NET11), DLL metadata verified and two PNG320×240 outputs embedded for lines/bands. No review image/screenshot. The binding used an old preview: the opt-in source ScienceUseSourcePlots and the isolated kernel-source-plots now prove the new dependency, package default preserved.
Core100/100 targeted tests and complete gate --no-images D:\Dev\BuildArtifacts\ecosystem-validation\20261028-203820, 31/31 exit0 after all the changes. Pre-existing build warnings not suppressed. D03 partial, not native JupyterSharp/CPython pixel oracle/total migration/release.

### Previous first D03 adoption — linear contours actually delegated

NDSharp.Plot DrawLines now uses RectilinearContours/ContourKernel with physical X/Y and the original ND conventions. The index-based Navier core shares traversal/interpolation but keeps its own edge direction/default/mask/bounds; no incorrect global conversion. Endpoint-level filtering, Ticks/AutoLevels, contourf, colours and Skia native remain ND.
New ProjectReference, exact geometry tested on non-uniform data/degeneracies/NaN/overflow and native pixel baselines. Build+204/204 shared and e18/18 ND passed, zero images. D03 partial, not full gate/notebook/family migration/release; old previews not updated.

### Previous D03 preparation — NDSharp.Plot inventory

Active NDSharp source (not import) examined: Skia3.119.0/native/font dependencies, net11 and Figure/Axes. Candidate contour-line geometry; non-uniform physical grid, invalid-cell/range-level policies and edge directions differ from the index mapping and must be preserved. Ticks/AutoLevels/contourf/styles/PNG renderer not migrated.
Four baseline tests on in-memory PNG added, build and 18/18 tests passed. Source hashes/dependencies report in MathemaSharp/ops/inventory/ndsharp-plot-contract.json. D03 in preparation, no PlotSharp reference or production renderer changed, no images/release/full gate.

### Previous complete baseline — consolidated FieldPlot

Gate --no-images D:\Dev\BuildArtifacts\ecosystem-validation\20261028-185732, 29/29 exit 0 after all the extractions. Suite198 cases and source consumer fixtures reconfirmed, CLI/Web and complete numerical selftest, UI/browser and in-memory raster verified without new PNG/SVG/screenshot review. Only JSON in the no-images folders, recent evidence/policy archived. It prevails over the notes further down about the full gate not rerun.
D02 remains partial for consumer perimeter/non-UI-tested/licences/distribution, not only for the now shared renderer. Other families and full accessibility not concluded. No release.

### Previous targeted verification FieldPlot — shared node/raster renderer

Header/styles/elements assembly consolidated in NodeFieldPlot, Navier options/API facade. Four complete goldens verify markup/RGB pixels, Heatmap default unchanged. Same-shape overlay/contour0–32/obstacles validated; previously unvalidated inputs not promised compatible. Dead-code private helpers removed, public API utilities preserved.
Build/suite198/198, Web/quadro/smoke without images green. Not a new full baseline or release; numerics/UI in the consumers and D02 still to be fully certified.

### Previous targeted verification of node layout/projection

Navier viewport geometry centralised in NodeFieldLayout, previous margins/rounding/Y and fractional coordinates preserved; not cell-based Heatmap. Shape/dimensions/collapses validated, finite external indices extrapolated without clamping. Build/suite192/192, Web/quadro/smoke without images green. Assembly/styles/header local and D02 to be consolidated; not a new full gate or release.

### Previous targeted verification of the colorbar — legacy gradient

Gradient/tick/label fragment shared, keeping samples11, tick3 and anchor/escaping including odd height. Three production markup fixtures coincide after normalisation of GUIDs only. Default strip colorbar unchanged. Build/suite178/178, Web/quadro/smoke without images passed; frame and contour projection stay local. Not a new full gate or release, D02 partial.

### Previous targeted verification of arrows — compatible node variant

NodeVectorOverlay path centralised, four FieldPlot goldens preserved and directions/normalisation/stride/threshold tested. Default physical/cell-centred quiver unchanged. Shapes/values/projection/stride validated, style/frame local and no new clipping. Build and suite 168/168, Web/quadro/pressure smoke green without images. Not a new full gate/release; colorbar/contour projection and other families not concluded.

### Previous targeted verification of colour limits — legacy semantics

Shared statistical limit selection (sort/floor percentiles, NaN omitted, symmetry and min+1 fallback), ColorScale default unchanged. Eight frozen FieldPlot label fixtures and additional limits tested; new Resolve with finite/max100k/clip [0,0.5] contract, direct NormalizeLegacy without new guards. Out-of-contract inputs not compatible by promise.
Build/suite 154/154 passed, Web Release/quadro/smoke without images. Field rendering/projection/colorbar/arrows still local. No new full gate or release, D02 partial.

### Previous targeted verification of the RGB sampler — bilinear nodes

Shared scanline preparation, using node coordinates/pixel centres and legacy bilinear order, not cell-based Heatmap mapping. Navier keeps normalisation and mask colour; the callback handles non-finite values. Fields min 2×2, masks in sample indices and explicit colour.
Raw PNG fixtures still coincide; new cases on orientation/centre/mask/input bring the suite to 138/138, build and Web/quadro smoke green. Zero images. Complete field/colorbar/arrows and other families not concluded; not a new full gate or release.

### Previous targeted verification of the PNG encoder — RGB8/filter-0 only

Minimal shared BCL encoder, used by Navier's FieldToPng signature without changing sampling/masking; duplicated encoder/CRC removed. Dimensions/lengths/filters verified, immutable static CRC and concurrency tested. Three pre-migration raw/hash fixtures, no image file: pixels/dimensions/CRC always compared, compressed hashes on the same runtime/OS.
Build and suite 127/127 passed, Web Release/quadro/adoption smoke --no-images rechecked. Sampling, FieldPlot/projection/colorbar/arrows still local. Not RGB-alpha/Skia/Figure-Axes, not a new full gate or release; D02 partial.

### Previous targeted verification of the palette — compatible RGB and bytes

Shared immutable 4/8/9-stop presets: PlotSharp compact default and Navier palette distinct; historical spacing and truncation preserved. Navier ColorMap facade, PNG/normalisation/colorbar rendering/projection still local. Fixture of 323 inputs frozen before the change with source hashes, including stops/ULP and colorbar samples, not regenerated by the tests. Byte/hex match of the three presets verified.
Build and suite 117/117 passed, Web Release/quadro smoke rechecked. Zero images, not a new full gate or release. The previous previews do not automatically include ColorPalette; D02 remains partial.

### Previous targeted verification of contour — NavierStokes compatibility

Shared MarchingSquares geometry: new compatibility overload preserves the legacy null segments at the vertices, while the previous overload/default remains unchanged. Navier keeps the tuple signature and the node→SVG projection; FieldPlot/PNG/colorbar/arrows local. Contract max100000 samples and finite values/levels ±1e12, buffer per level.
Build and PlotSharp suite 109/109 passed, including 16 cell patterns, saddle pairing, vertices, coordinates, limits and SVG field path; Web Release and quadro smoke passed. Zero images, not a new full gate or benchmark. D02 partial, new overload source-build only, no release. The following complete baseline is historical with respect to these changes.

### Previous complete baseline — without new review images

Gate --no-images: D:\Dev\BuildArtifacts\ecosystem-validation\20261028-150530, 29/29 steps exit 0. All checks run: no raster/browser skip, in-memory PNG encoding and separate JSON reports, no saving of review PNG/SVG/screenshots. Previous images preserved, not declared regenerated; notebook outputs updated.
Policy and recent reports protected by tests (operational suite 66/66): process timestamp and SaveImages must match; seven JSONs archived with the logs. This baseline prevails over the previous targeted notes, but is not a new visual review nor a completion of D01–D04/distribution. No release.

### Previous incremental Web contrast verification — without images

Only the HTML palette of the NavierStokes consumer: accents and muted texts darkened. Edge on home/pipe/wind: 17 solid samples with min about 4.97:1, exceeding the 4.5 criterion; keyboard/layout/calculations still green. PlotSharp series and algorithms unchanged. Seven sRGB cases added to the operational suite (50/50 passed), zero images and no new full gate (29 expected). Not a full audit of accessibility, SVG texts or UI states; unsupported gradients/group opacity flagged.

### Previous incremental Web controls verification — without images

In the NavierStokes consumer, mobile input/select raised from min 16 to min 44 px and visible focus. --mobile passed with native keyboard on wind and updated calculation, no overflow, SVG texts min 11 px. Desktop rechecked. No change to PlotSharp/calculations and zero images. Eight new cases of the height criterion: operational suite 43/43 passed; future gate 29, no new full gate run. Real touch and full accessibility not certified.

### Previous incremental Web text/scrolling verification — without images

In the NavierStokes consumer, min SVG text measured 5.85 px at a 360×800 viewport: fixed with native SVG and a scrollable box below 600 px, without touching PlotSharp or the data. Web build and mobile test passed: min 11 px, no page overflow, keyboard scrolling and end edge reached. Desktop rechecked. Zero images; eight new cases of the font criterion in the operational suite, 35/35 green. No full gate rerun (29 steps expected). Does not certify full accessibility, touch or mobile simulations.

### Previous incremental narrow-viewport verification — without images

NavierStokes home/pipe/wind verified in Kestrel+Edge at 360×800: no document/controls overflow, calculators updated. No change to CSS or renderer; zero images. Eight layout-validator tests in the operational library, suite 27/27 green. Does not cover readable SVG text/touch/real phones/mobile simulations. Future 29-step gate compiled, no new full gate run.

### Previous incremental whistle verification — without images

Two UI cycles of LBM start/progress/stop in Kestrel+Edge, 140×50 grid, controls restored. Quadro mode rechecked. Two core tests on an already-cancelled token/resumption with a new token: PlotSharp suite 98/98 passed with a successful build. No images and no change to rendering/algorithms; does not certify convergence or Strouhal. Future gate 28 steps, no new full gate run.

### Previous incremental quadro verification — without images

Start/progress/stop of the quadro page checked in Kestrel+Edge with grid 31, two cycles and controls restored. Two tests on the solver protect an already-cancelled token and resumption with a new token: PlotSharp suite 96/96 passed. No change to the renderer or the algorithms, no image generated. Not convergence/accuracy nor verification of whistle/mobile. Future gate 27 steps, no new full gate run.

### Previous incremental Web verification — limited scope

NavierStokes.Web built in Release; live Kestrel+Edge tool verified home/pipe/wind, range changes with result updates and presence of shared SVGs. No image generated; UI parsers covered by 13 new cases in the operational suite, 19/19 green. Quadro/whistle simulations, mobile and full UI not covered. Tool added to the gate, but no new full gate run: the complete baseline remains the 25-step one below.

### Current state — NavierStokes adoption, 2026-10-28

The active source is articles/2026.10 NavierStokes, not the imported snapshots. Core → PlotSharp for line/scatter, keeping Series/Options/LineChart signature; CLI/Web consume the library transitively. Seven new integration cases, PlotSharp suite 94/94 passed. CLI/Web builds successful and the complete original numerical selftest passed. Three linear reports on smoke fixtures verified in Edge and screenshots observed; original field/PNG available and XML verified, not migrated.
D02 is partial: fields/PNG/contour/ColorMap remain local, Web UI and the article notebook not E2E verified. Clip/layout and validation of invalid inputs deliberately differ; Ghia report with a wider viewBox. It is not regime accuracy of the smoke fixtures: the separate numerical test is the complete selftest.
MathemaSharp apps: scan of the Lab (2 files) and Docs (11) sources without a local renderer, no artificial graphics dependency added; assets/Markdown separate. Complete gate: D:\Dev\BuildArtifacts\ecosystem-validation\20261028-104324, 25/25 steps exit 0. D01/D02 partial, 0/4 families complete; rights/distribution still blocked, no preview published.

### Previous state — application scenarios, 2026-10-28

A1 warehouse and D1 photovoltaic migrated: respectively 3/2 SVGs; warehouse gap verified, one KPI group and two answers per scenario preserved. Eight notebooks, 43 SVGs. Edge: 43 individual charts and two assembled HTML dashboards without horizontal overflow at the verified desktop viewport, five tiles and two non-empty answers each. Two full-page screenshots observed. MathemaSharp Core: 98/98 tests passed separately. Updated complete gate: D:\Dev\BuildArtifacts\ecosystem-validation\20261028-101452, 21/21 steps exit 0.
D01 remains partial: apps and other paths to verify. An assembled dashboard is not equivalent to notebook UI, mobile or VS Code; native popups and overlapping targets remain limits. No transfer of host helpers into the graphics library and no preview published.

### Previous state — STL and tour, 2026-10-28

It prevails over the following historical counts. Migrated TimeSeries/09 (6 SVGs, tables and NDSharp bridge preserved) and 10 (5 SVGs, calculations/numerical agreement unaltered). Now six source-build application notebooks, 38 SVGs run and XML-validated. Edge: 38/38 with DOM checks and hit targets for the declared perimeter; 11 new browser screenshots observed. The gate generates the migrated steps from the manifest and verifies the required tables. MathemaSharp Core: 96/96 tests passed separately. Complete gate: D:\Dev\BuildArtifacts\ecosystem-validation\20261028-075021, 19/19 exit 0. No preview published; D01 still partial, other families open.

### Previous state — browser and legends, 2026-10-28

It prevails over the following historical notes. All 19 rasters of notebooks 06/08 observed; initial Edge verification on 27 SVGs detected three title/legend collisions. Opt-in SeparateLegendRow adds a row, IndexedCharts enables it; four new tests, legacy default unchanged. Native tooltips added to the stems. Second Edge verification: 27/27 with text within limits, no title/text collision and at least one reachable title hit target each. Three browser screenshots of the corrected cases observed. PlotSharp 87/87 tests passed. Updated complete gate: D:\Dev\BuildArtifacts\ecosystem-validation\20261028-065717, 17/17 exit 0; MathemaSharp Core 94/94.
It is not a verification of the OS tooltip popup, of the individual reachability of all overlapping samples, of VS Code or of all browsers. The reports keep the total/reachable target counts. Source-build only, distribution and other families still open.

### Previous state — markers and adoption 06/08, 2026-10-28

This section prevails over the following historical notes. Opt-in PreserveBoundaryMarkers keeps sample markers/tooltips at the limits: unclipped overlay for finite centres in the closed domain, external centres omitted. Scales and clip of lines/stems/bars unchanged; legacy core default preserved, IndexedCharts opt-in. Highlights not modified. Four tests added; PlotSharp 83/83 passed. Last complete gate: D:\Dev\BuildArtifacts\ecosystem-validation\20261028-061520, 16/16 steps exit 0; MathemaSharp Core 93/93 verified separately. Two spectra regenerated and observed: complete circles at zero and at the maxima, no change of scale.
TimeSeries/06 and 08 also migrated, real JupyterNet, respectively 6 and 13 XML-validated SVGs, tables preserved. Four migrated application notebooks, 27 SVGs; manifest ops/migrated-notebooks.json and single runner. Visual review of the new 06/08 not yet run; browser still open. D01 partial, 0/4 families complete; new contracts source-build only, no preview overwritten.

### Second adoption and gaps — 2026-10-28 (historical)

TimeSeries/07-smoothing-kalman migrated: six SVGs and HTML tables run in the real JupyterNet; 30-observation gap preserved and verified. Core LineChart keeps the strict default; AllowLineGaps allows only NaN Y of lines with finite data, not Infinity/X/stems/bar or entirely missing series. IndexedCharts Lines/Panels enable gaps and native tooltips. Original Stems overload preserved; new overload with a k label used by the spectra. Table/Kpi/Tile extracted into Presentation.csx, also loaded by the legacy renderer.
Eight raster review images observed: readable layout and visible gap; marker clipping at the axis limits to be resolved. The Svg.Skia raster in ops is only a review tool, not the product's PNG backend. New APIs in the local sources, previous NuGet previews not updated. D01 remains partial, 0/4 families complete.

### First application adoption — 2026-10-28

MathemaSharp/Core/03-complex-signals.ipynb uses the PlotSharp bootstrap instead of the legacy renderer: two spectrum SVGs run in the real JupyterNet host and XML-validated. The baseline builds PlotSharp Release before the notebooks and the gate includes a dedicated runner. Tests on the bootstrap/output added. D01 is now partial, not closed; all four families remain without complete migration. The JupyterSharp pilot remains a distinct proof. Visual verification still open.

## 2. Current dashboard

| Dimension | State | Evidence / limit |
|---|---|---|
| Shared SVG core | **Partial, working** | Line/scatter, indexed/panels, stems/bar, heatmap/colorbar/mask/extent, basic contour/quiver |
| Consolidation of the whole Charts heritage | **Not completed** | Missing: exhaustive API matrix, PNG backend, Figure/Axes, layout and further artists |
| Automated validation of the active solution | **Verified for the implemented perimeter** | Successful build, 227 passing xUnit tests (verified 2026-10-29, including NavierStokes and NDSharp consumer cases); gallery of 8 examples historical baseline, application notebooks validated separately |
| Visual verification | **Preliminary, partial** | Eight previous rasters and 19 rasters 06/08 observed; 43 SVGs verified in Edge; three corrected screenshots, 11 STL/tour and two assembled dashboards observed. OS popups, overlapping targets and VS Code remain open |
| End-to-end notebooks | **Partial, run** | Three JupyterSharp pilots and eight JupyterNet application notebooks; XML verified, partial visual review |
| Adoption in the repositories of origin | **0/4 complete; D01–D03 partial, D04 pilots only** | MathemaSharp Core/03, TimeSeries/06–10 and scenarios A1/D1 migrated and verified; other paths and families open |
| Active dependencies | **2 internal, 2 external** | PlotSharp.Tests and PlotSharp.Gallery in the solution; NDSharp.Plot and NavierStokes.Core reference the library from their own repositories |
| Distribution | **Blocked** | NavierStokes/MathemaSharp rights to confirm; no package published |
| Documentation | **Present, to be maintained** | README, USAGE/Reference, ARCHITECTURE, this register |

**Global completion: not reached and not expressed as a percentage.** The denominator of the original APIs is not yet complete. The 227 tests are not a measure of coverage of the whole imported heritage. The 0/4 ratio measures only the adoption of the four known families, not the completion of the product.

### States used

- **To start**: planned activity without a verified deliverable.
- **In progress / partial**: a result exists, at least one acceptance criterion is missing.
- **Verified**: executable evidence and indicated criteria satisfied within the declared perimeter.
- **Blocked**: prerequisite not satisfied; the code may exist but not be distributable.
- **Completed**: all the milestone's criteria satisfied, including documentation and required checks.
- **Excluded**: explicit decision with reasons and a path for the consumers; not a synonym of missing.

### P01 progress — snapshot inventory run

- [x] Regenerable and tested tool; `ops/inventory/` report with files/hashes, declarations, notebook uses and references.
- [x] Baseline: 148 files; 478 public lines (not a number of semantic APIs); 68 notebook occurrences; 86 project/package references.
- [x] `.ralfai/undo` backups distinguished from current material: kept in the file catalogue, excluded from the consumer analysis.
- [x] 0 ProjectReference to missing csproj in the current perimeter; complete builds/sources of the consumers **not verified**.
- [x] Per-file matrix of NDSharp.Plot and proposed destinations in ARCHITECTURE; precise baseline in the CSV.
- [ ] Semantic review/approval of the complete API matrix, aliases and dynamic calls.
- [ ] Catalogue of the real repositories and missing dependencies/sources, rights and approved exclusion criteria.

P01 remains **in progress**. D01 and D02 are partial; D03/D04 not completed, 0/4 migrations complete. The tool does not replace end-to-end integration.

## 3. Product roadmap

| ID | Milestone | State | Depends on | Completion criterion |
|---|---|---|---|---|
| P01 | Catalogue and provenance | In progress | — | Every imported API/renderer/asset/test classified; missing dependencies identified; source → destination → kept/adapted/excluded matrix approved |
| P02 | Rights and attributions | Blocked | P01 | Licences verified for all the extracted code; third-party notices and authorisations present; documented decision on the package licence |
| P03 | Public contract of the core | In progress | P01 | Series, axes, layout, styles, non-finite data, precision and serialisation/export defined; API/versioning and compatibility documented; contract tests |
| P04 | SVG renderer consolidation | In progress | P03 | Selected SVG features equivalent or differences approved; numerical/geometric and layout cases verified; duplicates destined to facades rather than new engines |
| P05 | NDSharp.Plot and raster consolidation | In progress (D03 partial: contour/sampler/bands) | P01, P03 | Figure/Axes/Artists, PNG/Skia, fonts, tick/layout and NDArray adaptation separated coherently; relevant original tests run or replaced with equivalents; native platforms verified |
| P06 | Visual quality and notebook gallery | In progress | P04 | Representative gallery visually checked; notebooks run in the supported kernels; artifact/regression report; all tests of the perimeter passing |
| P07 | Adapters and consumer compatibility | To start | P03, P04, P05 depending on the APIs used | NDArray/display/Blazor adapters or facades only where necessary; unidirectional dependencies; integration tests; no dependency of the core on apps or kernels |
| P08 | Adoption and retirement of duplicates | To start | P07, P06, P02 for transfer/distribution | D01–D04 register closed: build/test/examples in the real repositories, local charts replaced or leftovers justified, documentation updated |
| P09 | Release and maintenance | Blocked | P02, P03, P06; P05/P08 for a complete consolidation release | Versioned package, CI build/test, notices, verified installation, platform support and breaking-changes policy; no implicit publication |

An experimental release of the subset may precede P05/P08, but it must declare the exclusions and not be presented as a complete consolidation.

### Next order of work

1. **P01:** complete the API matrix and verify accessibility/completeness of the consumers' sources. Do not add more charts before understanding what is really missing.
2. **P02 + P03:** close rights/attributions and establish the contract with the real requirements of the consumers.
3. **D01/P06:** first integration demonstrated in a MathemaSharp notebook, with checked output and known differences; do not declare the whole family migrated after a single example.
4. **D02:** migrate NavierStokes by feature, resolving sample/centre differences, masks and raster before removing the local FieldPlot.
5. **P05/D03:** consolidate NDSharp.Plot preserving capabilities and compatibility; then NDSharp.Notebooks.
6. **D04:** display integration and JupyterSharp runtime distribution, without diverging copies of the assemblies.
7. **P08/P09:** verified retirement of duplicates and release.

## 4. State of features and consolidation

| Area | State in the active library | Consolidation / remaining work |
|---|---|---|
| Lines, scatter, basic ticks/axes | Implemented and tested | To compare with all the original APIs; double precision and layout still limited |
| Indexed series, Start, dashed, panels | Implemented and tested | Facade inspired by MathemaSharp; default NaN reject, gap opt-in (AllowLineGaps); used by the eight migrated MathemaSharp notebooks |
| Stems/correlograms | Implemented and tested | Zero/band baseline; scale not forced symmetric; original visual compatibility to verify |
| Vertical bars | Implemented and tested | Grouping, stacking, categories and NDSharp variants not consolidated |
| Heatmap, colorbar, mask, extent | Implemented and tested | Cell-based SVG, no raster/interpolation; physical aspect ratio and diverging palette missing |
| Contour | Basic implemented and tested | Marching squares shared by NavierStokes and NDSharp.Plot's linear contours (compatibility overload), fixed saddle pairing; no decider, stitching, fill or label. For contourf only the physical sampler and band selection are shared; palette/alpha/Skia remain in NDSharp |
| Quiver | Basic implemented and tested | Geometry adapted from NDSharp, explicit scale; no autoscale/streamlines; pixel units instead of points/DPI |
| Figure/Axes/Artists | Not consolidated | NDSharp snapshot; public model and facade to decide |
| PNG/Skia, fonts, tight_layout and advanced layout | Not consolidated | NDSharp snapshot and NavierStokes encoder to compare; native dependencies/licences to verify |
| Histograms, 3D and other artists/colormaps | Not consolidated | Complete catalogue and keep/exclude decision still open |
| NDArray adaptation | Not implemented | Must stay out of the core; explicit precision conversion/normalisation |
| Notebook display / Blazor | No verified adapter | The library returns SVG/HTML; it does not integrate a host directly |
| MathemaSharp Tables, Tiles, Quotes | Outside the proposed Charts core | Host presentation, not a graphics engine; destination to approve in P01 |

The material in `src/imports/` remains an unchanged archive excluded from the solution. The presence of an active feature does not mean that the original file has been deleted or that all its options are supported.

## 5. Register of dependent projects

### Effective dependencies in the active solution

| ID | Project | Relationship | State / evidence |
|---|---|---|---|
| I01 | PlotSharp.Tests | ProjectReference → PlotSharp and PlotSharp.Inventory | Active, 227 passing tests (verified 2026-10-29) |
| I02 | PlotSharp.Gallery | ProjectReference → PlotSharp | Active, 8 examples generated and XML-validated; visual not verified |

Tool added to the solution: `src/tools/PlotSharp.Inventory/PlotSharp.Inventory.csproj`, **does not depend on PlotSharp** and is not a new Charts consumer.

Paths: `src/PlotSharp.Tests/PlotSharp.Tests/PlotSharp.Tests.csproj`, `src/PlotSharp.Gallery/PlotSharp.Gallery/PlotSharp.Gallery.csproj`.

### Expected external consumers — not yet dependent on PlotSharp

The table describes what is observable in the snapshots, **not an updated scan of the original repositories**. No imported consumer is included in the active solution. Owners, migration branches/commits and access to the real repositories are not yet recorded: to be defined before execution.

| ID | Family / components to follow | Chart dependency observed | Adoption state | Next deliverable / blockers |
|---|---|---|---|---|
| D01 | MathemaSharp: notebook/shared/Charts.csx, TimeSeries/Core notebooks; Lab/Docs apps to evaluate | SVG helpers with Display.Html; MathemaSharp.NDSharp references NDSharp (not proof of NDSharp.Plot adoption) | **Partial** | Eight notebooks/43 SVGs and two dashboards migrated; shared presentation helpers, apps without a local renderer in the inspected sources. Complete inventory/API/assets, precision and licences |
| D02 | NavierStokes.Core, NavierStokes.Cli, NavierStokes.Web, notebook | Charts/Reports in Core; Cli and Web → Core; Web/Shared/Chart.razor shows SVG | **Partial** | Line/scatter and FieldPlot (palette, RGB8 PNG, sampler, colour limits, arrows, colorbar, layout, contour) delegated; CLI/Web build, UI smoke (home/pipe/wind, quadro, whistle, mobile) and numerical selftest passed in the gate. Remaining: article notebook, uncovered UI paths, full accessibility and rights |
| D03 | NDSharp.Plot, NDSharp.Plot.Tests, NDSharp.Notebooks, NDSharp.Notebooks.Tests; notebooks 11/12 | Plot → NDSharp + Skia; Notebooks → Plot; the two Tests projects → their projects | **Partial: linear contours, sampler and bands delegated** | ProjectReference and common physical/core adapter; ND 19/19 and shared 227/227 (from roadmap, shared reverified), real contour notebook in the source-build kernel, green gate. Remaining: palette/alpha/Skia, Figure/Axes/raster/NDArray, other categories, NDSharp.Notebooks and distribution |
| D04 | JupyterSharp.Engine/Host/kernel, samples, vscode-extension runtime | Host → Engine; Engine → kernel; NDSharp.Plot and PySharpLib.Matplotlib DLLs collected in the pysharp runtime | **Pilots run; native display integration not started** | Identify all display/loading/distribution points, integrate PlotSharp assembly/MIME, run kernel smoke tests; PySharpLib.Matplotlib sources not collected |

PySharpLib.Matplotlib is a **dependency/integration to investigate**, not a fifth migrated family or a source project available here. Replacing the collected DLL is not sufficient.

### Checklist for each D01–D04

- [ ] Owner, real repository, baseline branch/commit and component perimeter recorded.
- [ ] Complete list of chart uses and original APIs within the perimeter.
- [ ] Compatibility matrix; missing features and accepted differences explicit.
- [ ] Reference to PlotSharp or facade/adapter inserted in the consumer repository.
- [ ] Consumer build and tests successful, with commands/commits recorded.
- [ ] End-to-end examples run and output visually verified.
- [ ] Local implementation removed/delegated; leftovers justified and tracked.
- [ ] Documentation/installation updated, PlotSharp version pinned.

**A family is migrated only when the checklist is closed for the whole agreed perimeter.** A ProjectReference, a change to the snapshots or a build of the library alone are not enough.

## 6. Library dependencies and blockers

Currently the core uses only .NET/BCL; no dependency on NDSharp, SkiaSharp, Blazor or kernels. Future renderers/adapters must depend on the core, never the other way round.

- **B01 — rights:** NavierStokes and MathemaSharp licences/attributions not verified in the collected selection. NDSharp includes MIT and fonts with a separate licence; keep complete notices when distributed.
- **B02 — completeness of sources:** heuristic import; csproj present do not guarantee complete dependencies/classes. Targeted recovery needed for real integrations.
- **B03 — compatibility contract:** choose public model and facade without losing Figure/Axes capabilities or multiplying renderers.
- **B04 — precision:** double, ±1e12 limits, rounded pixels; Generic Math/Decimal normalisation unresolved.
- **B05 — host/visual validation:** partial: JupyterSharp/JupyterNet CLI notebooks, 43 SVGs and two Edge dashboards, three NavierStokes smoke reports verified; native popups, VS Code renderer and other browsers not verified.
- **B06 — release:** package/version, CI, native platforms and breaking-changes policy to be defined.

PNG, 3D or advanced artists are not automatically out of scope just because they are missing: the decision must be taken in P01. Publication/installation are not implicit in the development activities.

## 7. Evidence and updating of the register

Historical baseline: `src/PlotSharp.slnx`, 70 tests and a gallery of eight XML examples. Current total verified on 2026-10-29: 227 tests, including the integration cases with NavierStokes.Core and NDSharp.Plot active (not snapshots). Current evidence in the gate above; the snapshots remain excluded from the solution, consumer/visual reports have distinct perimeters and do not demonstrate coverage of the whole imported heritage.

```sh
dotnet build src/PlotSharp.slnx
dotnet test src/PlotSharp.slnx
dotnet run --project src/PlotSharp.Gallery/PlotSharp.Gallery/PlotSharp.Gallery.csproj -- artifacts/gallery
```

At each objective concluded update: Pxx/Dxx state, criteria satisfied, residual blockers, commands/results and the consumer's commit when available. Update counts only after verification; keep core tests, compatibility, visual output and migrations separate. This document is the operational state; ARCHITECTURE keeps the decisions/provenance, USAGE the actual reference.
