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
