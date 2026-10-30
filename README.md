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
