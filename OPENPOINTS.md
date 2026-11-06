# PlotSharp — Open points

Suite-wide points are in [../ScienceSharp/OPENPOINTS.md](../ScienceSharp/OPENPOINTS.md). The long status log is [ROADMAP.md](ROADMAP.md).

| # | Point | Status |
| --- | --- | --- |
| PL-1 | **Chart families:** 0 of 4 families are complete. A feature being present does not mean the original material is consolidated or that a consumer is migrated. | open |
| PL-2 | **Scope split with NDSharp.Plot** (R03/R14): which categories stay in NDSharp and which live here. Blocks new MatSharp plot work. | open |
| PL-3 | Palette, alpha, native raster and the Skia renderer are still local to NDSharp.Plot. Not moved. | known |
| PL-4 | Consumers: only `NDSharp.Plot` and `NavierStokes.Core` reference PlotSharp from source. MathemaSharp uses an opt-in facade; JupyterSharp's copy in `src/imports` is a snapshot. | known |
| PL-5 | **Rights:** the old "blocked for rights" statements (P02/B01) in ARCHITECTURE.md and ROADMAP.md are outdated since the MIT decision, but not rewritten. `src/imports/*` snapshots have no audited origin. | open |
| PL-6 | CLI `plotsharp` exists (line and scatter from a CSV to SVG). No contour, heatmap or field charts from the command line yet. | done, extend later (suite G9) |
| PL-7 | Output is static SVG/HTML only. Hosts: VS Code renderer plus other browsers, tooltips, overlapping samples, SVG contrast, focus and keyboard navigation are not complete (R13). | open |
| PL-8 | Public contracts for node/cell/physical coordinates, non-finite values, masks, limits and defaults are written in CONTRATTI.md but not frozen (R05). | open |
| PL-9 | Not published; package `0.1.1-net11-preview.1` is in the local feed only. | deferred (D-F3) |
| PL-10 | Documents are in Italian. | open (suite G7) |
| PL-11 | `PIATTAFORME-PRECISIONE.md` platform and precision matrix is only checked on Windows x64 (R06). | open |

## Geometry phase 4 — contour migration
PlotSharp delegates its shared contour traversal to MathemaSharp.Geometry.IsoContours; Charts.MarchingSquares overloads and Segment/Segment<T> return types are unchanged. RectilinearContours<T> retains INumber<T> (including decimal); the engine exposes a legacy-compatible Trace plus the IFloatingPointIeee754 MarchingSquares API returning Segment2<T>. Fixed edge order, summed-corner policy, exact-level segments and arithmetic order preserved. Geometry source preview.7; PlotSharp preview.3; neither packed. Direction PlotSharp -> Geometry -> BCL, no cycle.
Fingerprint: 2922 cases, two pre-change runs and two post-change runs identical: 69B10DD0254A1AE95D1B0881D341E5A34EF6E2397ED6E994B6EDF22C0714FEB6. Harness in BuildArtifacts/tmp/geometry-contour-fingerprint.cs, logs in BuildArtifacts/logs/geometry-contours.log. Old algorithm removed only after fingerprint match and green tests.
Initial PlotSharp baseline could not compile NavierContourTests because the article had already removed Charts.MarchingSquares. Tests now call PlotSharp compatibility overload through a tuple adapter; frozen expected coordinates and assertions unchanged. This is a repaired test target, not an independent article oracle. Post-removal solution build and tests pass: PlotSharp 242 + CLI 10; MathemaSharp 891 (Geometry 34). Both builds zero warnings/errors. Public signatures preserved by source inspection; prebuilt NuGet consumer runtime loading, full NDSharp tests, article selftest, rendering/UI and ecosystem gate not executed. PlotSharp solution references currently build article/engine dependencies in Debug even under Release; article output still follows its preexisting local bin configuration. Phase 5 camera/projection pending; phase 6 excluded.
