# PlotSharp

> **Part of [ScienceSharp](../ScienceSharp)** — the Science suite. Targets **.NET 11**. Status: preview, not published. License: MIT.
> **Form:** NuGet library `PlotSharp` plus global tool `plotsharp` (render a CSV to an SVG line or scatter chart).

| [README](README.md) | [USAGE](USAGE.md) (install first) | [ARCHITECTURE](ARCHITECTURE.md) | [ROADMAP](ROADMAP.md) | [OPENPOINTS](OPENPOINTS.md) |
| --- | --- | --- | --- | --- |

Libreria sperimentale di Charts per .NET 11: line/scatter, tratteggi, pannelli, stems, barre verticali e heatmap SVG a celle con colorbar, maschere, tick fisici e overlay contour/quiver opzionali. Il renderer di base è estratto da NavierStokes; la facade indicizzata è ispirata a MathemaSharp, senza creare motori paralleli per ogni host. Nessuna dipendenza da kernel, NDArray, Blazor o librerie native.

## Build

```sh
dotnet build src/PlotSharp.slnx
dotnet test src/PlotSharp.slnx
```

La soluzione include libreria, test, galleria console e tool di inventario degli import (`src/tools/PlotSharp.Inventory`). Il catalogo rigenerabile è in `ops/inventory/`; la revisione di compatibilità rimane aperta. Per generare gli esempi:

```sh
dotnet run --project src/PlotSharp.Gallery/PlotSharp.Gallery/PlotSharp.Gallery.csproj -- artifacts/gallery
```

Aprire `artifacts/gallery/index.html` nel browser. La galleria è stata eseguita e i singoli SVG/pannelli validati come XML; non è stato effettuato un controllo visivo nel browser. `src/imports/` contiene snapshot di riferimento, **non progetti attivi** e non una copia completa dei repository originali. Gli snapshot e `import-manifest.csv` non sono stati modificati.

Per adottare localmente: aggiungere un ProjectReference a `src/PlotSharp/PlotSharp/PlotSharp.csproj`. Non è stata pubblicata una versione NuGet né aggiornato alcun consumatore.

- [Usage e Reference](USAGE.md)
- [Architettura, inventario e decisioni](ARCHITECTURE.md)
- [Roadmap del prodotto, stato di completamento e registro dei consumatori](ROADMAP.md)

**Stato del consolidamento:** nucleo SVG funzionante, patrimonio Charts ancora parzialmente consolidato; **0/4 famiglie esterne migrate**. La roadmap distingue milestone del prodotto, funzionalità implementate, dipendenze attive e adozioni previste, con criteri di accettazione verificabili.

La libreria è utilizzabile per valutazioni interne. Dal 2026-10-29 il codice è sotto licenza MIT (vedi la sezione License in fondo); resta da decidere se pubblicare. La denominazione universale indica l'obiettivo di riuso tra host, non la copertura già completa dei tipi di grafico.

## License

MIT, Copyright (c) 2026 Marco Parenzan; see [LICENSE](LICENSE). This code was developed with the assistance of generative AI tools. Third-party material, where present, keeps its own license; see THIRD-PARTY-NOTICES.md in `NDSharp/src/NDSharp.Plot` for the matplotlib-derived parts.
