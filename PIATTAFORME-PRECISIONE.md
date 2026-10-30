# Framework, piattaforme e precisione (R06)

Stato al 2026-10-29. Dati rilevati da csproj, nuspec, `project.assets.json` e da un consumer isolato eseguito; la **piattaforma supportata è una delibera dell'autore** (§5), qui è solo proposta. Nessun retargeting è stato eseguito.

## 1. Target framework effettivi

| Ambito | TFM | Note |
|---|---|---|
| Tutti i progetti di PlotSharp, NDSharp, MatSharp, MathemaSharp, PySharp, JupyterSharp, NavierStokes, DecimalInDotNet11 | `net11.0` | SDK `11.0.100-rc.3.26522.148` (`global.json` di MathemaSharp, `latestFeature`, prerelease consentita). Sono **RC3**: nessuna garanzia di supporto a lungo termine. |
| `Ecosystem.Validation`, `Ecosystem.Validation.Tests` | `net10.0` | Eccezione già dichiarata (R01); gira su runtime 10.0.14 o in roll-forward. Non è un pacchetto distribuito. |
| Snapshot `src/imports/*` | n/d | Copie storiche, non compilate come prodotto. |

Runtime installati sulla macchina di verifica: 5.0, 6.0, 9.0, 10.0.14, 11.0.0-rc.3 (SDK 6, 10 RC1, 11 RC3).

## 2. Pacchetti: cosa dichiarano e cosa contengono

| Pacchetto (feed locale `D:\Dev\NuGetLocalFeed`) | TFM nel pacchetto | Dipendenze rilevate | Note |
|---|---|---|---|
| `NDSharp.Plot 2.6.3-net11-preview.2` (27/10) | `net11.0` | NDSharp 2.8.1-net11-preview.1, SkiaSharp 3.119.0, Skia nativi Linux e macOS | **Non dipende da PlotSharp**: contiene il codice contour precedente alle estrazioni. È la versione che `MatSharpLib.Plot` e `PySharpLib.Matplotlib` referenziano. |
| `PlotSharp 0.1.1-net11-preview.1` (27/10) | `net11.0`, nessuna dipendenza | `authors` = "PlotSharp", **nessuna licenza** | Blocco L1 di R04 |
| Sorgenti correnti, impacchettati in un feed temporaneo come `9.9.9-r06` | `net11.0` | NDSharp.Plot → NDSharp + **PlotSharp** + SkiaSharp 3.119.0 + Skia nativi Linux/macOS | Prova per N4; il feed locale **non è stato toccato** |

Conseguenza: finché non si ripubblica NDSharp.Plot con la dipendenza da PlotSharp, **i consumatori a valle non usano il kernel condiviso** (R12/N4 aperti).

## 3. Verifica con consumer isolato

Progetto vuoto in cartella temporanea, feed temporaneo + nuget.org, cache pacchetti separata, nessun artefatto locale preesistente.

| Prova | Esito |
|---|---|
| Consumer `net11.0`, `dotnet run` su Windows x64, runtime 11.0.0, riferimento al solo `NDSharp.Plot` | **OK**: restore pulito, `PlotSharp 9.9.9` risolto transitivamente, contour riempito non uniforme renderizzato, pixel atteso verificato (rosso nella banda alta) |
| Consumer `net10.0` | **Rifiutato in restore**: `NU1202`, il pacchetto supporta solo `net11.0` |
| Restore per `linux-x64`, `linux-arm64`, `linux-musl-x64`, `osx-x64`, `osx-arm64`, `win-x86`, `win-x64`, `win-arm64` | Il grafo dei pacchetti contiene `libSkiaSharp` per win-x86/x64/arm64, osx, linux-{x64, arm, arm64, x86, loongarch64, riscv64} e linux-musl-{x64, arm, arm64, loongarch64, riscv64}. **Solo risoluzione dei file: non eseguito.** |
| `browser-wasm`, `android-arm64` | Nessun nativo Skia nel grafo di NDSharp.Plot |

Eseguito davvero: **solo win-x64**. Linux e macOS non sono stati provati (nessun host disponibile).

## 4. Piattaforme per componente

| Componente | Dipendenze native | Piattaforme (da dati) |
|---|---|---|
| PlotSharp | Nessuna (solo BCL, SVG/HTML, encoder PNG RGB8) | Qualunque host .NET 11 |
| NDSharp.Plot | SkiaSharp 3.119.0 + font DejaVu incorporati | Windows, macOS, Linux glibc/musl come da §3; non per browser/mobile |
| NDSharp.Image | OpenCvSharp4.runtime.win | **Solo Windows** per il runtime nativo incluso |
| PySharp.Torch | TorchSharp-cpu | Fuori dal perimetro grafico |

## 5. Politica di conversione numerica (precisione)

Il grafico lavora in `double`; i dati memorizzati non vengono modificati. Test in `NDSharp.Plot.Tests/PrecisionPolicyTests.cs`.

| Passaggio | Politica | Verificato |
|---|---|---|
| NDArray → `double` per la visualizzazione (`ToArray<double>`) | Esatta per `Float32` e per interi fino a ±(2^53 − 1); **arrotondata** oltre (es. 2^53 + 1 → 2^53, pareggio al pari) | Sì, 3 test |
| `Float32` → `double` | Estensione esatta; non recupera il decimale (0,1f ≠ 0,1) | Sì |
| Valori memorizzati | Immutati dalla conversione | Sì (il `long` originale resta 2^53 + 1) |
| Complessi | `PySharp.Matplotlib` usa la parte reale degli scalari; `MatSharp.Plot` usa `Mat.Re`. La parte immaginaria non è tracciata | Dal codice, non testato |
| Decimal | Nessun `DType` decimale in NDSharp: i `decimal` di MathemaSharp non entrano nel plot senza una conversione esplicita del chiamante | Dal codice |
| Pixel raster | Coordinate Skia in `float` (84 cast in NDSharp.Plot): la risoluzione sub-pixel non è double | Dal codice |
| SVG | Coordinate con `0.##` (2 decimali di pixel); etichette con `Charts.Num` (es. 1234567,891 → `1.2e6`), scientifica fuori da [1e-3, 1e5) | Sì, 2 casi |

Regola: **la precisione di calcolo non è quella del disegno.** Il disegno non è una fonte per recuperare i dati; chi deve conservare interi > 2^53 o decimali esatti deve tenerli fuori dal percorso di plot o convertirli esplicitamente.

## 6. Decisioni dell'autore (2026-10-29)

- **D-F1 — deciso:** si supporta **solo `net11.0`**. Nessun secondo target (`net10.0`); l'eccezione `Ecosystem.Validation(+Tests)` resta tale.
- **D-F2 — deciso:** provare **tutte** le piattaforme, **in futuro** (R15). Oggi è dichiarata come provata solo win-x64; Linux/macOS/arm64/musl restano non verificate finché non si esegue su quegli host.
- **D-F3 — rinviato:** se ripubblicare NDSharp.Plot con PlotSharp come dipendenza (N4) si decide dopo. Pubblicazione e installazione globale restano su richiesta esplicita.
- **Indicazione aggiuntiva:** usare **Generic Math dove possibile** (vedi §8).

## 8. Generic Math (indicazione dell'autore: «dove possibile»)

Fatto il 2026-10-29, solo con `System.Numerics` (BCL: PlotSharp resta senza dipendenze). Vincolo `T : INumber<T>`.

| Kernel | Forma generica | API `double` esistente |
|---|---|---|
| Tracciatore di contour | `RectilinearContours<T>.Trace(T[], T[], T[], T)` → `Charts.Segment<T>` | Invariata, delega all'istanza `double` |
| Campionatore bilineare | `RectilinearFieldSampler<T>` | Invariata, delega |
| Bande di livelli | `ContourBands<T>` | Invariata, delega |
| Cella/interpolazione interna | `ContourKernel.Trace<T>` (condiviso anche da `Charts.MarchingSquares` Navier) | `double` |

**Garanzia di compatibilità:** per `double` la sequenza di operazioni è identica. Lo dimostrano i test congelati esistenti (fixture a 81 query, aritmetica pre-migrazione) rimasti invariati e passanti: PlotSharp 234/234 (227 + 7 nuovi), NDSharp.Plot.Tests 46/46, NavierStokes `selftest` superato (Strouhal 0,188).

**Cosa guadagna il chiamante** (`GenericMathKernelTests`): con `decimal` l'intersezione di 0,1 e 0,3 a livello 0,2 cade esattamente su 0,15, mentre con `double` è 0,15 più un ulp; `float` e `Half` tracciano la stessa geometria entro la propria precisione.

**Differenze di comportamento per i tipi non IEEE:** `decimal` non ha infiniti né NaN. Il test «somma dei vertici non finita» che per `double` salta la cella, per `decimal` fa emergere `OverflowException`. È documentato e testato.

**Dove NON si applica (motivo):**
- *Disegno e colore* (`Heatmap`, `ColorScale`, `ColorPalette`, SVG, Skia): il risultato è comunque pixel o testo; restano `double`/`float`.
- *`ContourArtist` e consumatori NDSharp*: usano `double[]` per contratto con PySharp.Matplotlib e MatSharp.Plot; non sono stati generalizzati. Un chiamante che vuole la precisione di `decimal` può oggi usare i kernel generici direttamente, ma non l'artista.
- *Overlay Navier* (`double[,]`, limiti ±1e12 e 100000 campioni): contratto storico, invariato.

## 7. Limiti

- Un solo host eseguito (Windows x64, runtime 11.0.0 RC3).
- La risoluzione per RID mostra i file presenti nel pacchetto, non che il caricamento funzioni.
- Non ho misurato l'effetto della conversione `float` sul raster.
- Il tag `-net11-preview` dei pacchetti non è stato valutato rispetto alle regole di versionamento (R16).
