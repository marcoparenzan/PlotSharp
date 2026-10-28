# PlotSharp

Libreria sperimentale di Charts per .NET 10: line/scatter, tratteggi, pannelli, stems, barre verticali e heatmap SVG a celle con colorbar, maschere, tick fisici e overlay contour/quiver opzionali. Il renderer di base è estratto da NavierStokes; la facade indicizzata è ispirata a MathemaSharp, senza creare motori paralleli per ogni host. Nessuna dipendenza da kernel, NDArray, Blazor o librerie native.

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

La libreria è utilizzabile per valutazioni interne, non pronta alla distribuzione: manca una verifica dei diritti sul codice NavierStokes estratto. La denominazione universale indica l'obiettivo di riuso tra host, non la copertura già completa dei tipi di grafico.
