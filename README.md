<p align="center">
  <picture>
    <source media="(prefers-color-scheme: dark)" srcset="branding/logo-dark.svg">
    <img alt="PlotSharp" src="branding/logo.svg" height="72">
  </picture>
</p>

# PlotSharp

> **Part of [ScienceSharp](../ScienceSharp)** — the Science suite. Targets **.NET 11**. Status: preview, not published. License: MIT.
> **Form:** NuGet library `PlotSharp` plus global tool `plotsharp` (render a CSV to an SVG line or scatter chart).

| [README](README.md) | [USAGE](USAGE.md) (install first) | [ARCHITECTURE](ARCHITECTURE.md) | [ROADMAP](ROADMAP.md) | [OPENPOINTS](OPENPOINTS.md) |
| --- | --- | --- | --- | --- |

Experimental Charts library for .NET 11: line/scatter, dashes, panels, stems, vertical bars and cell-based SVG heatmaps with colorbar, masks, physical ticks and optional contour/quiver overlays. The base renderer is extracted from NavierStokes; the indexed facade is inspired by MathemaSharp, without creating parallel engines for each host. No dependency on kernels, NDArray, Blazor or native libraries.

## Build

```sh
dotnet build src/PlotSharp.slnx
dotnet test src/PlotSharp.slnx
```

The solution includes the library, tests, a console gallery and the import inventory tool (`src/tools/PlotSharp.Inventory`). The regenerable catalogue is in `ops/inventory/`; the compatibility review remains open. To generate the examples:

```sh
dotnet run --project src/PlotSharp.Gallery/PlotSharp.Gallery/PlotSharp.Gallery.csproj -- artifacts/gallery
```

Open `artifacts/gallery/index.html` in the browser. The gallery was run and the individual SVGs/panels validated as XML; no visual check was made in the browser. `src/imports/` contains reference snapshots, **not active projects** and not a complete copy of the original repositories. The snapshots and `import-manifest.csv` have not been modified; the snapshots are archived as they were and are not translated (some of their documents are in Italian).

To adopt locally: add a ProjectReference to `src/PlotSharp/PlotSharp/PlotSharp.csproj`. No NuGet version has been published and no consumer updated.

- [Usage and Reference](USAGE.md)
- [Architecture, inventory and decisions](ARCHITECTURE.md)
- [Product roadmap, completion status and consumer register](ROADMAP.md)

**Consolidation status:** working SVG core, Charts heritage still partially consolidated; **0/4 external families migrated**. The roadmap distinguishes product milestones, implemented features, active dependencies and planned adoptions, with verifiable acceptance criteria.

The library is usable for internal evaluation. Since 2026-10-29 the code is under the MIT licence (see the License section at the bottom); whether to publish remains to be decided. The universal naming indicates the goal of reuse across hosts, not the already complete coverage of chart types.

## License

MIT, Copyright (c) 2026 Marco Parenzan; see [LICENSE](LICENSE). This code was developed with the assistance of generative AI tools. Third-party material, where present, keeps its own license; see THIRD-PARTY-NOTICES.md in `NDSharp/src/NDSharp.Plot` for the matplotlib-derived parts.

## A note on licensing

This is a development exercise, not a commercial effort and not a way to take anything from anyone. It tries to show that .NET and C# can do the same kind of
scientific work as Python, in the hope that someone will try it. Its licensing, including the third-party material it adapts or credits, was reviewed with the help
of generative AI, with the aim of the greatest respect for the rights of the owners. That is not legal advice, and some points are still open: they are listed in
[LICENSING.md](../ScienceSharp/LICENSING.md). If you own material that is used here and have a concern, please open an issue: it will be discussed and, if needed, fixed or removed.

## Geometry phase 4 — contour migration
PlotSharp delegates its shared contour traversal to MathemaSharp.Geometry.IsoContours; Charts.MarchingSquares overloads and Segment/Segment<T> return types are unchanged. RectilinearContours<T> retains INumber<T> (including decimal); the engine exposes a legacy-compatible Trace plus the IFloatingPointIeee754 MarchingSquares API returning Segment2<T>. Fixed edge order, summed-corner policy, exact-level segments and arithmetic order preserved. Geometry source preview.7; PlotSharp preview.3; neither packed. Direction PlotSharp -> Geometry -> BCL, no cycle.
Fingerprint: 2922 cases, two pre-change runs and two post-change runs identical: 69B10DD0254A1AE95D1B0881D341E5A34EF6E2397ED6E994B6EDF22C0714FEB6. Harness in BuildArtifacts/tmp/geometry-contour-fingerprint.cs, logs in BuildArtifacts/logs/geometry-contours.log. Old algorithm removed only after fingerprint match and green tests.
Initial PlotSharp baseline could not compile NavierContourTests because the article had already removed Charts.MarchingSquares. Tests now call PlotSharp compatibility overload through a tuple adapter; frozen expected coordinates and assertions unchanged. This is a repaired test target, not an independent article oracle. Post-removal solution build and tests pass: PlotSharp 242 + CLI 10; MathemaSharp 891 (Geometry 34). Both builds zero warnings/errors. Public signatures preserved by source inspection; prebuilt NuGet consumer runtime loading, full NDSharp tests, article selftest, rendering/UI and ecosystem gate not executed. PlotSharp solution references currently build article/engine dependencies in Debug even under Release; article output still follows its preexisting local bin configuration. Phase 5 camera/projection pending; phase 6 excluded.
