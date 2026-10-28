# Architettura

```
src/NavierStokes.sln
├── src/NavierStokes.Core     libreria, zero dipendenze: fisica, numerica, grafici
├── src/NavierStokes.Cli      eseguibile `ns`: simulazioni, export SVG/CSV, selftest
├── src/NavierStokes.Web      Blazor Server: le quattro domande, simulazioni live
├── notebooks/                notebook C# (Polyglot) che referenzia la DLL di Core
├── chat/                     trascrizioni originali delle conversazioni
└── docs/                     materiale editoriale: articolo, post, appunti, immagini
```

Core non conosce né la console né il web: CLI, app e notebook chiamano le stesse classi e gli stessi `Reports`.

## NavierStokes.Core

| File | Contenuto |
|---|---|
| `CavitySolver.cs` | Cavità con coperchio mobile. Griglia collocata N×N, differenze finite, metodo di proiezione di Chorin: Poisson per la pressione con SOR (ω = 1,7, 20 iterazioni per passo con warm start), poi aggiornamento della velocità. Convezione ibrida: centrata se il Péclet di cella è < 2, upwind altrimenti. Unità adimensionali (L = 1, U = 1, ρ = 1, ν = 1/Re). Contiene anche `GhiaReference` (dati Re = 100). |
| `CylinderLbm.cs` | Flusso attorno a un cilindro con Lattice Boltzmann D2Q9, collisione BGK, bounce-back sul solido, ingresso a equilibrio, uscita a gradiente nullo, periodico in alto/basso. Una "folata" trasversale nei primi 600 passi rompe la simmetria. Sonda a valle per la serie temporale e il numero di Strouhal (attraversamenti dello zero). `Parallel.For` sulle righe. |
| `Scenarios.cs` | Modelli analitici e semi-empirici: `Fluid` (aria, acqua, olio, glicole), `Flow` (Reynolds, regime), `Pipe` (Hagen-Poiseuille / Darcy-Weisbach con Haaland), `Wind` (resistenza, scala Beaufort), `VortexShedding` (frequenza, nota musicale). |
| `Charts.cs` | Grafici in SVG scritti a mano: mappe di colore sequenziale e divergente, heatmap come PNG incorporato (encoder PNG con `ZLibStream` e CRC32 propri), linee di flusso con marching squares, frecce, grafici a linee con legenda ed evidenziazione. |
| `Reports.cs` | I grafici standard di ogni scenario, condivisi da CLI, web e notebook. |

### Scelte numeriche

- **Proiezione su griglia collocata**: semplice da leggere, accurata a Re = 100 (RMSE ≈ 0,02 contro Ghia). Una griglia sfalsata
  (MAC) sarebbe più accurata ma meno didattica.
- **Lattice Boltzmann per il cilindro**: gestisce l'ostacolo curvo con il bounce-back senza griglie adattate, ed è naturalmente parallelo.
- **Reynolds limitati nell'app**: cavità ≤ 400, cilindro 60–150. Il Reynolds reale viene sempre mostrato e dichiarato.

## NavierStokes.Web

Blazor Server, render mode `InteractiveServer` per pagina. Le simulazioni girano in `Task.Run` con `CancellationToken`
(annullate su `Dispose` o con il pulsante "Ferma"); la callback di avanzamento rigenera l'SVG e chiama
`InvokeAsync(StateHasChanged)`. Gli SVG si inseriscono con `MarkupString` tramite il componente `Chart`.

| Pagina | Scenario |
|---|---|
| `/quadro` | `CavitySolver` 61×61 live, quota di aria "quasi ferma" (< 10% della velocità ventola) |
| `/tubo` | `Pipe`, ricalcolo istantaneo a ogni slider |
| `/fischio` | `CylinderLbm` live, frequenza e nota |
| `/vento` | `Wind`, curva forza-velocità |

## Notebook

Kernel C# di Polyglot Notebooks / .NET Interactive. La prima cella fa `#r` della DLL Release di Core;
gli output (testo e grafici) sono già salvati nel file, generati eseguendo le stesse celle in un'app console.
