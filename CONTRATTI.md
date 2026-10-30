# Contratti pubblici dei kernel condivisi (R05 / N3)

Stato al 2026-10-29. Descrive il comportamento **osservato e coperto da test**, non un comportamento ideale. Dove differisce da matplotlib è indicato; dove la scelta spetta all'autore è marcato **DECIDERE**. Fonti: `RectilinearContours`, `RectilinearFieldSampler`, `ContourBands`, `Charts.MarchingSquares` in questo repository; `ContourArtist` in `NDSharp/src/NDSharp.Plot/Contour.cs`.

## 1. Convenzioni di coordinate

| Kernel | Convenzione | Note |
|---|---|---|
| `RectilinearContours.Trace(xs, ys, z, level)` | **Nodi** in coordinate **fisiche**. `z` row-major, `ys.Length` righe × `xs.Length` colonne. Il segmento interpola linearmente sul lato fisico della cella. | Una colonna/riga singola non ha celle: risultato vuoto. |
| `RectilinearFieldSampler.Sample(x, y)` | Interpolazione **bilineare** su cella fisica; ascissa in `xs`, ordinata in `ys`. | Fuori dal dominio la cella è bloccata alla più vicina e il peso **estrapola linearmente** (non satura). |
| `Charts.MarchingSquares(field, …)` (Navier) | **Indici di campione**: x = colonna, y = riga. | Nessuna coordinata fisica; il consumatore proietta. |
| `ContourBands.Find` | Valore → indice di banda, estremi inclusivi. | Vedi §4. |

I tre kernel ND hanno anche la forma generica `INumber<T>` (`RectilinearContours<T>`, `RectilinearFieldSampler<T>`, `ContourBands<T>`) con gli **stessi contratti** di questo documento; per `decimal` (senza NaN/Inf) l'overflow della somma dei vertici lancia `OverflowException` invece di saltare la cella. Dettagli in `PIATTAFORME-PRECISIONE.md` §8.

## 2. Valori non finiti e maschere

| Caso | `RectilinearContours` (ND) | `Charts.MarchingSquares` (Navier) | `ContourArtist` righe | `ContourArtist` riempito |
|---|---|---|---|---|
| NaN/Inf in `z` | La cella è **saltata** se la somma dei quattro vertici non è finita (anche per overflow di valori finiti); i vicini finiti si disegnano | Rifiutato (`ArgumentException`) se non mascherato | Come ND | Il campione NaN non dipinge; `+Inf` tollerato |
| Z tutta non finita | Vuoto | Rifiutato | **Non disegna nulla** (prima: `InvalidOperationException` accidentale, corretto il 2026-10-29) | Non disegna nulla |
| Maschera | Non supportata | `bool[,]`: cella saltata se uno dei 4 vertici è mascherato; dimensioni uguali al campo | Non supportata | Non supportata |
| Coordinate non finite | `ArgumentException` | n/a | `ArgumentException` | `ArgumentException` |
| Livello NaN | `ArgumentOutOfRangeException` | `ArgumentOutOfRangeException` | `ArgumentOutOfRangeException` | `ArgumentException` (da `ContourBands`) |
| Livello `+Inf` | `ArgumentOutOfRangeException` | idem | **Saltato in silenzio** (fuori dall'intervallo dei dati) | `ArgumentException` |

## 3. Limiti dimensionali e numerici

| Percorso | Limite |
|---|---|
| ND (`RectilinearContours`, `RectilinearFieldSampler`) | Nessun tetto su dimensioni o ampiezza dei valori. Il riempito alloca `larghezza × altezza × 4` byte pari alla casella degli assi: il limite pratico è la dimensione della figura. |
| Navier (`MarchingSquares`) | 1 … 100000 campioni; valori non mascherati finiti e entro ±1e12; livello entro ±1e12. |
| Overlay heatmap | Al massimo 32 livelli, finiti, entro ±1e12; colore `#rrggbb`; vettori: scala 0 < s ≤ 1e6, stride 1…100000, proiezione entro ±1e7 pixel. |

Le restrizioni Navier **non** sono state estese al percorso ND: i due consumatori mantengono i propri limiti storici.

## 4. Ordine di assi e livelli

| Elemento | Righe (tracer) | Riempito (sampler + bande) | matplotlib |
|---|---|---|---|
| Assi `xs`, `ys` | Qualsiasi ordine, **anche decrescenti o con ripetizioni** (finiti) | **Strettamente crescenti**, altrimenti `ArgumentException` | Accetta assi decrescenti e ripetuti, sia `contour` sia `contourf` (verificato: nessun errore) |
| Almeno 2×2 | No (1 colonna → vuoto) | Sì | Sì |
| Livelli | Qualsiasi ordine, ripetuti ammessi, anche vuoti | **Non decrescenti**, finiti, almeno uno; ripetuti ammessi | **Strettamente crescenti** (`ValueError`) |
| Un solo livello | Ammesso | Ammesso: nessuna banda, non dipinge | n/a |

Regole delle bande (`ContourBands.Find`): estremi inclusivi, quindi un valore esattamente su un livello interno appartiene alla banda **superiore**; il valore uguale all'ultimo livello appartiene all'ultima banda; con livelli ripetuti vince **l'ultimo confine corrispondente**. Nel riempito una banda di larghezza zero (livello ripetuto) **consuma un indice colore**: con colori `[Blu, Rosso, Verde]` e livelli `[0,1,1,2]` la banda superiore è verde.

Colore di banda: punto medio dei limiti, normalizzato sull'intero intervallo dei livelli (come matplotlib); `hi == lo` → 0. Colore di linea: livello normalizzato sull'intervallo dei livelli.

## 5. Arrotondamenti e aritmetica

Conservata alla lettera rispetto ai codici d'origine, con test di non regressione esatta:

- Intersezione: `t = (level - v1) / (v2 - v1)`, punto `p1 + t·(p2 - p1)`; lato attraversato se `(v1 < level) != (v2 < level)`.
- Sella: i punti d'intersezione si accoppiano in ordine fisso (0–1, 2–3). Ordine dei lati: ND avanti (`d→c`, `a→d`), Navier inverso (`c→d`, `d→a`).
- Vertici esattamente a livello: i segmenti degeneri (lunghezza zero) sono **preservati** da `RectilinearContours` e dal modo di compatibilità Navier; `ContourArtist` esclude i livelli agli estremi osservati dei dati (`lv <= zmin` o `lv >= zmax`).
- Sampler: quattro termini `z00(1-tx)(1-ty) + z01·tx(1-ty) + z10(1-tx)·ty + z11·tx·ty`, **senza riassociazione**; l'overflow si propaga come in origine.
- Punto medio di banda: `(a + b) / 2`, poi `(mid - lo) / (hi - lo)`; overflow/NaN passano al chiamante.
- Livelli automatici (`ContourArtist.AutoLevels(zmin, zmax, n)`): identici a `MaxNLocator(n + 1, min_n_ticks=1)` di matplotlib 3.11.2 su 7 casi verificati.

## 6. Default di `ContourArtist`

Colormap `viridis`; `LineWidth` 1.5; stile `-`, **tratteggiato `--` per i livelli negativi solo con un colore unico** (`Colors` di lunghezza 1), come matplotlib; `Colors` più corto dei livelli: ciclico; `Alpha` scala la trasparenza del riempimento (verificato su pixel composito su fondo bianco).

## 7. Differenze accettate rispetto a matplotlib

Verificate con matplotlib 3.11.2 (esecuzione reale, solo presenza/assenza di eccezione; non ho confrontato cosa viene disegnato):

1. **Livelli non crescenti:** matplotlib li rifiuta (`ValueError`) in righe e riempito; NDSharp li accetta nelle righe e rifiuta solo i decrescenti nel riempito.
2. **Assi decrescenti o ripetuti:** matplotlib li accetta ovunque; NDSharp li accetta nelle righe ma il **riempito li rifiuta**. Qui NDSharp è *più restrittivo*.
3. **Livelli NaN e `+Inf`:** matplotlib non solleva eccezioni; NDSharp solleva per NaN (righe) e per ogni livello non finito (riempito), e ignora `+Inf` nelle righe.
4. **Z tutta non finita:** matplotlib non solleva; NDSharp non disegna (corretto un crash accidentale nelle righe).

**DECISO 2026-10-29: opzione (c)** — lasciare tutto com'è e dichiarare le differenze. Le opzioni valutate erano: (nessuna modifica finché non confermi): (a) rifiutare i livelli non crescenti anche nelle righe e nel riempito, come matplotlib; (b) accettare assi decrescenti nel riempito (richiede di ordinare/riflettere il campione, modifica del kernel con fixture congelata); (c) lasciare tutto com'è e dichiarare le differenze. Allineare a matplotlib sui livelli è una **restrizione nuova** per chi oggi li passa alle righe.

## 8. Copertura

| Contratto | Test |
|---|---|
| Tracer, sella, vertici, overflow, assi decrescenti, singoletti, input malformati | `PlotSharp.Tests/RectilinearContourTests.cs` (6) |
| Sampler, fixture congelata 81 query, estrapolazione, snapshot, rifiuti | `RectilinearFieldSamplerTests.cs` |
| Bande, estremi, ripetuti, punto medio, overflow, snapshot, rifiuti | `ContourBandsTests.cs` |
| Livelli automatici contro matplotlib, alpha, livelli ripetuti, non finiti nel riempito | `NDSharp.Plot.Tests/ContourBaselineTests.cs` |
| Differenze righe/riempito di §2 e §4, «non disegna nulla» confrontato byte a byte col grafico vuoto | `NDSharp.Plot.Tests/ContourContractTests.cs` |

## 9. Non coperto

- Limiti numerici del percorso ND non sono testati oltre l'overflow della somma dei vertici.
- Non ho misurato il tempo o la memoria del riempito per figure molto grandi.
- Il confronto con matplotlib è limitato ai livelli automatici; geometria dei segmenti e colori delle bande non sono confrontati con l'oracolo in questa tranche.
- Quiver e 3D non hanno contratti scritti (R14).
