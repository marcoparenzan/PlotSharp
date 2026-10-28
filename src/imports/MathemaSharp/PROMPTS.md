# Registro degli obiettivi

## 1
> Con .NET 11 vengono introdotti i nuovi formati Decimal. E' ora di fare un recap della matematica in .NET con GenericMath, Complex numbers, Decimal ecc..
> Test performance esempi notebooks
> tutta una serie di esempi
> Target .NET non .NET 10

Intento: realizzare un laboratorio riproducibile sulla matematica in .NET, con Generic Math, Complex, decimal, test, benchmark e notebook, verificando lo stato delle novità .NET 11 e gli SDK disponibili. Il vincolo di sviluppo di questa sessione impone net10.0 per i progetti compilati: documentare esplicitamente questa discrepanza, senza spacciare API future per disponibili.

## 2
> in refs\timeseries ho aggiunto una immagine sulla timeserie forecasting. Che dici se approfondisci i temi e fai qualche implementazione con la matematica?
> Creerei anche dei notebook con JupiterNet. Che dici?
> Puoi riorganizzare il repo in modo da ottimizzare e fare una libreria di utilizzo. Poi in PySharp c'è una implementazione di NumPy....forse la libreria matematica potrebbe essere generalizzata...che dici?

Intento: implementare la cronologia dell'infografica (benchmark, AR, smoothing, Kalman, ARIMA, VAR, GARCH, damped trend, STL) con test sulle identità fra modelli; riorganizzare il repository in librerie pacchettizzabili; collegare NDSharp con un ponte separato invece di fondere le librerie; migrare tutti i notebook a JupyterNet ed eseguirli davvero, compreso il 05 su runtime .NET 11.

## 3
> (dalla stessa sessione) Tutta la libreria deve essere in .NET 11 e i formati di IGenericMath.
> L'organizzazione dei contenuti deve diventare sostanzialmente un libro.
> Crea un file di indice degli scenari possibili. Prima una spiegazione delle formule in teoria e realizzazione della libreria. Poi scenari: prima A poi D. Tieni tutti i samples. Vedremo in seguito.
> Ho caricato tutte le immagini con l'elenco delle teorie: splitta ogni immagine in una sottocartella, una cartella per ogni riga con i dettagli della teoria, un file in testa con l'elenco completo delle teorie e l'implementazione (testo, notebook C# e PySharp, riferimenti alla libreria). Poi la libreria: indicazione, uso funzione per funzione, overlap con NumPy in PySharp, esempi in C# e PySharp.
> Contestualizza ogni esempio in uno scenario di IoT industriale (persone che acquisiscono dati da automazioni, KPI, dashboard, risposte ai clienti). Le bottiglie no.

Intento: portare tutte le librerie a net11.0 e renderle generiche su `IFloatingPointIeee754<T>` (float, double, Half, Decimal32/64/128, `Complex<T>`); scrivere il libro (Parte I formule e realizzazione, Parte II libreria con overlap NumPy, atlante delle teorie); realizzare gli scenari A e D con dati sintetici, notebook C# e PySharp con gli stessi numeri. **Non ancora fatta**: la divisione delle 18 infografiche in sottocartelle con una scheda per ciascuna teoria (resta l'atlante come indice).

## 1. "puoi proseguire con l'analisi delle immagini quindi andare avanti con l'implementazione delle infogr…"

puoi proseguire con l'analisi delle immagini quindi andare avanti con l'implementazione delle infografiche

_2026-10-24 20:08_

## 2. "Puoi creare un sito web Blazor Server, tailwindcss javascript con classi per navigare nella document…"

Puoi creare un sito web Blazor Server, tailwindcss javascript con classi per navigare nella documentazione?

_2026-10-24 21:50_

## 3. "fai partire il sito"

_2026-10-24 22:02_

## 4. "fai partire il sito"

_2026-10-24 22:05_

## 5. "si può fare qualcosa sulla rappresentazione html? Qualcosa con i css?"

_2026-10-24 22:06_

## 6. "fai partire il sito"

_2026-10-24 22:08_

## 7. "Puoi formattare Tabelle con righe alternate, header con grassetti e padding"

_2026-10-24 22:18_

## 8. "fai partire il sito"

_2026-10-24 22:19_

## 9. "Che ne dici di implementare nel progetto 1793027138639.jpg"

_2026-10-26 23:19_

## 10. "mmmhh...sbaglio o sotto timeseries c'è Linear Algebra che starebbe meglio in LinearAlgebra?"

_2026-10-26 23:38_

## 11. "Se Numbers è una libreria generica di matematica, se guardi dentro D:\Dev\2026\repos\NDArray trovi m…"

Se Numbers è una libreria generica di matematica, se guardi dentro D:\Dev\2026\repos\NDArray trovi materiale che vale più come Numbers? Intanto importi?

_2026-10-26 23:44_

## 12. "scusa D:\Dev\2026\repos\NDSharp"

_2026-10-26 23:46_

## 13. "Puoi rendere tutta la libreria Generic Math?"

_2026-10-26 23:50_

## 14. "come siamo con la conversione?"

_2026-10-26 23:55_

## 15. "Allora. Ho trovato un nome. MàthemaNet."

Allora. Ho trovato un nome. MàthemaNet.
La radice della parola Matematica.
Una libreria in Generic Math, aggiornato a .NET 11 quindi anche con i nuovi Decimal32/64 dove dentro c'è tutto.
Tutti i sample sono notebook in radice (notebooks/*) divisi per come sono divise le librerie.
Quindi serve:
- riorganizzare
- rinominare MathemaNet.<lib>
- notebooks
- <lib>: <usage>, <reference>
Il logo da generare deve invece riportare l'accento altrimenti succede che finisce sulla seconda.

_2026-10-27 00:06_

## 16. "Per favore rinomina MathemaNet in MathemaSharp"

_2026-10-27 00:12_

## 17. "domanda (senza agire): qui ci sono Charts, in NDSharp ci sono charts, in D:\dev\2026\repos\JupyterSh…"

domanda (senza agire): qui ci sono Charts, in NDSharp ci sono charts, in D:\dev\2026\repos\JupyterSharp ci sono charts. 
IN D:\Dev\2026\articoli\2026.10 NavierStokes ci sono charts. Che dici ha senso fare una libreria di Charts?
Se si, hanno senso come notebook? Li lasciamo qui in MathemaSharp?

_2026-10-27 00:19_

## 18. "Che nome possiamo darle?"

_2026-10-27 00:19_

## 19. "Ok, crea allora la libreria D:\dev\2027\repos\PlotSharp"

Ok, crea allora la libreria D:\dev\2027\repos\PlotSharp
Intanto raccogli tutto e butta dentro. Poi aggiorniamo i vari progetti.

_2026-10-27 00:20_

## 20. "quindi?"

_2026-10-27 00:21_

## 21. "Non puoi farlo tu?"

_2026-10-27 00:21_
