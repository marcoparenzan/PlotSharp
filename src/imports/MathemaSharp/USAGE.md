# Uso quotidiano

## Console

Dalla root:

```sh
dotnet run --project src/apps/MathemaSharp.Lab -- --topic generic
dotnet run --project src/apps/MathemaSharp.Lab -- --topic precision
dotnet run --project src/apps/MathemaSharp.Lab -- --topic decimal
dotnet run --project src/apps/MathemaSharp.Lab -- --topic complex
dotnet run --project src/apps/MathemaSharp.Lab -- --topic integers
dotnet run --project src/apps/MathemaSharp.Lab -- --topic simd
dotnet run --project src/apps/MathemaSharp.Lab -- --topic quantum
dotnet run --project src/apps/MathemaSharp.Lab -- --topic timeseries
```

Il topic `quantum` mostra prodotto interno, valori attesi di Pauli, commutatore, autovalori, evoluzione temporale e matrice densità; vedi [QUANTUM.md](QUANTUM.md). Il topic `timeseries` percorre benchmark, Yule-Walker, Hannan-Rissanen, Kalman/SES, GARCH e STL su dati simulati con parametri noti; vedi [TIMESERIES.md](TIMESERIES.md).

Senza opzioni viene selezionato `all`. Topic sconosciuto: exit code 2 e log di errore. Configurazione via Microsoft.Extensions.Hosting: anche variabile ambiente `topic`; la riga di comando prevale. Output numerico con cultura invariante.

Risultati significativi: `0.1+0.2` double = `0.30000000000000004`; decimal = `0.3`. `[1e16,1,1,-1e16]`: somma naive 0, Kahan 2. Tre righe da 0.005: ToEven per riga 0.00, sul totale 0.02. Per z=3+4i: dot hermitiano 25, bilineare -7+24i.

## Benchmark

```sh
dotnet run -c Release --project src/benchmarks/MathemaSharp.Benchmarks -- --list flat
dotnet run -c Release --project src/benchmarks/MathemaSharp.Benchmarks -- --filter '*DotBenchmarks*' --job Dry --artifacts ops/artifacts/benchmarks
dotnet run -c Release --project src/benchmarks/MathemaSharp.Benchmarks -- --filter '*SumBenchmarks*' --job Short --artifacts ops/artifacts/benchmarks
dotnet run -c Release --project src/benchmarks/MathemaSharp.Benchmarks -- --filter '*IntegerBenchmarks*' --artifacts ops/artifacts/integers
dotnet run -c Release --project src/benchmarks/MathemaSharp.Benchmarks -- --filter '*ComplexBenchmarks*' --artifacts ops/artifacts/complex
```

`Dry`: solo verifica; `Short`: esplorazione; job predefinito: misure più robuste. Non fare confronti con debugger o da Stopwatch in un notebook. Chiudere carichi concorrenti e registrare runtime effettivo, CPU, GC e JIT. CSV/HTML/Markdown prodotti automaticamente. Le righe confrontabili hanno stessa N; diversi tipi hanno contratti di precisione diversi. Array preparati fuori dalla misura; DFT invece alloca deliberatamente l'output. SIMD e LINQ possono cambiare l'ordine della somma.

Il run locale Dry ha eseguito tutte le quattro combinazioni Dot (N=31,4099). Le medie cold-start nell'ordine dei millisecondi includono warm-up/JIT non rappresentativi: non significano che SIMD sia più lento. Non sono disponibili misure sui nuovi decimal IEEE .NET 11.

## Notebook (JupyterNet)

Kernel JupyterNet (C#, Roslyn scripting, repository `D:\Dev\2026\repos\JupyterNet`), tool `jupyternet` dal feed locale `D:\Dev\NuGetLocalFeed` (manifest in `.config/dotnet-tools.json`, `dotnet tool restore`). I notebook sono salvati **eseguiti**, con output e grafici SVG.

```powershell
./scripts/run-notebooks.ps1                    # build Release + esegue tutti i notebook (C# e PySharp, anche quelli degli scenari)
./scripts/run-notebooks.ps1 -Filter '0[6-9]*'  # solo serie temporali
```

Interattivo: VS Code con l'estensione JupyterNet, tipo notebook `jupyternet-notebook`.

Come sono collegati alle librerie (richiede JupyterNet ≥ 0.3.0, versione fissata nel manifest dei tool):
- ogni notebook inizia con `#load "shared/MathemaSharp.csx"`, relativo alla cartella del notebook; quel file referenzia `artifacts/bin/*/release/*.dll` con `#r` relativi a sé stesso. Nessun percorso assoluto: il repository si può spostare;
- le direttive `using` di un `#load` non si propagano: ogni notebook dichiara le proprie;
- `Console.WriteLine` finisce nel notebook (il kernel C# la cattura dalla 0.3.0), raggruppata per cella e nell'ordine rispetto ai grafici;
- `shared/Kernel.csx` imposta la cultura invariante anche come default dei thread, perché ogni cella può girare su un thread diverso;
- `shared/Charts.csx`: grafici SVG statici (`Chart.Lines`, `Chart.Panels`, `Chart.Stems`) e `Table.Show`, via `Display.Html`;
- il notebook 05 gira sul runtime .NET 11 RC2 via `DOTNET_ROLL_FORWARD=LatestMajor` + `DOTNET_ROLL_FORWARD_TO_PRERELEASE=1`: Roslyn compila contro gli assembly del runtime in esecuzione, quindi Decimal32/64/128 e Complex<T> sono disponibili.

## Pacchetti

```powershell
./scripts/pack.ps1            # artifacts/package/release/*.nupkg
./scripts/pack.ps1 -Publish   # copia anche nel feed locale (aumentare prima <Version>)
```

## Librerie: Usage

I sample didattici sono in `notebooks/<lib>/`; gli helper condivisi sono in `notebooks/shared/`. Gli output sono stati azzerati durante la migrazione: rieseguire i notebook prima di considerarli verificati. La console Lab resta un harness di verifica, non il catalogo dei sample.

### MathemaSharp.Core — Usage

Namespace: `MathemaSharp.Core`. Notebook: `notebooks/Core/`.

- [01-generic-math](notebooks/Core/01-generic-math.ipynb)
- [02-precision-decimal](notebooks/Core/02-precision-decimal.ipynb)
- [03-complex-signals](notebooks/Core/03-complex-signals.ipynb)
- [04-performance](notebooks/Core/04-performance.ipynb)
- [05-dotnet11-decimals](notebooks/Core/05-dotnet11-decimals.ipynb)

### MathemaSharp.LinearAlgebra — Usage

Namespace: `MathemaSharp.LinearAlgebra`. Notebook: `notebooks/LinearAlgebra/`.

- [01-lu](notebooks/LinearAlgebra/01-lu.ipynb)

Usare `LuFactorization<T>.Create(matrix)` e `Solve(rhs)` per riutilizzare la stessa fattorizzazione; i pivot quasi singolari sono rifiutati.

### MathemaSharp.Quantum — Usage

Namespace: `MathemaSharp.Quantum`. Notebook: `notebooks/Quantum/`.


### MathemaSharp.TimeSeries — Usage

Namespace: `MathemaSharp.TimeSeries`. Notebook: `notebooks/TimeSeries/`.

- [06-ts-reference-autoregression](notebooks/TimeSeries/06-ts-reference-autoregression.ipynb)
- [07-ts-smoothing-kalman](notebooks/TimeSeries/07-ts-smoothing-kalman.ipynb)
- [08-ts-arima-var-garch](notebooks/TimeSeries/08-ts-arima-var-garch.ipynb)
- [09-ts-stl-ndsharp](notebooks/TimeSeries/09-ts-stl-ndsharp.ipynb)
- [10-ts-libreria-tour](notebooks/TimeSeries/10-ts-libreria-tour.ipynb)
- [10-ts-libreria-tour](notebooks/TimeSeries/pysharp/10-ts-libreria-tour.ipynb)
- [A1-magazzino-frigorifero-csharp](notebooks/TimeSeries/Scenarios/A-magazzino-frigorifero/A1-magazzino-frigorifero-csharp.ipynb)
- [A1-magazzino-frigorifero-pysharp](notebooks/TimeSeries/Scenarios/A-magazzino-frigorifero/A1-magazzino-frigorifero-pysharp.ipynb)
- [D1-fotovoltaico-csharp](notebooks/TimeSeries/Scenarios/D-fotovoltaico/D1-fotovoltaico-csharp.ipynb)
- [D1-fotovoltaico-pysharp](notebooks/TimeSeries/Scenarios/D-fotovoltaico/D1-fotovoltaico-pysharp.ipynb)

### MathemaSharp.NDSharp — Usage

Namespace: `MathemaSharp.NDSharp`. Notebook: `notebooks/NDSharp/`.


Il ponte usa dtype/storage runtime e contiene ancora conversioni via double: non è un kernel interamente Generic Math. Il notebook STL in TimeSeries mostra l'integrazione.
