# PlotSharp — obiettivi per la nuova sessione

## Target
`D:\Dev\2027\repos\PlotSharp`

## Stato iniziale
Sono state raccolte 148 copie di riferimento in `src/imports/<provenienza>/`: MathemaSharp (54), NDSharp (50), JupyterSharp (29), NavierStokes (15). I percorsi relativi sono conservati. `import-manifest.csv` contiene provenienza e SHA256; `README.md` descrive lo stato. Gli originali non sono stati modificati. La selezione è euristica, non garantisce tutte le dipendenze. Non esiste ancora una libreria attiva verificata.

## Obiettivo
Creare PlotSharp, libreria autonoma di visualizzazione scientifica condivisa da MathemaSharp, NDSharp, JupyterSharp e dagli esempi NavierStokes. Prima raccogliere e confrontare, poi estrarre; non creare un quinto sistema duplicato.

## Fasi
1. Inventariare gli snapshot: API, renderer, dipendenze, test, notebook, asset e licenze. Distinguere codice originale e di terze parti. Verificare le licenze prima di integrare o distribuire.
2. Confrontare implementazioni e duplicati. Documentare quali conservare, adattare o scartare, con motivazioni. Individuare dipendenze mancanti e raccoglierle solo se necessarie.
3. Proporre un nucleo piccolo: serie, assi, legende, colori, stili e layout. Scegliere il renderer iniziale dopo l'analisi; SVG/HTML è un candidato adatto ai notebook.
4. Estrarre una prima tranche compilabile: linee e scatter, poi barre e heatmap. Valutare contour, quiver e streamlines per NavierStokes, senza prometterli tutti nella prima versione.
5. Separare adattatori per notebook, NDArray e Blazor dal nucleo; creare progetti separati solo quando servono. PlotSharp non deve dipendere dai kernel di MathemaSharp o dalle applicazioni consumatrici.
6. Accettare dati Generic Math dove utile. Rendere esplicita la conversione alle coordinate grafiche: niente conversioni premature a double che cancellino differenze nei Decimal32/64/128. Normalizzazione, valori non finiti e limiti vanno documentati e testati. Usare .NET 11 se consentito dalla nuova sessione e disponibile; altrimenti dichiarare il limite effettivo.
7. Creare notebook eseguibili nella root `notebooks/`, organizzati per moduli/tipi di grafico. Sono galleria e documentazione, non implementazione. I notebook matematici/applicativi restano nei progetti di origine.
8. Aggiungere test xUnit per logica, mapping coordinate, dati vuoti/non finiti, escaping SVG/HTML, serializzazione ed esportazione. Eseguire build e test; verificare visivamente esempi rappresentativi senza dichiarare controlli non effettuati.
9. Documentare installazione, usage, reference, architettura e roadmap nei Markdown della root. Registrare chiaramente quali snapshot sono esclusi dalla soluzione e quali esempi sono stati eseguiti.

## Layout desiderato
- `src/`: soluzione, libreria, eventuali renderer/adattatori, test e snapshot di riferimento.
- `notebooks/`: esempi curati ed eseguibili.
- `ops/`: configurazione, build e strumenti operativi.
- Markdown di documentazione nella root, non in `docs/`.

## Vincoli
- Non modificare gli originali durante inventario ed estrazione.
- Non includere indiscriminatamente tutti gli snapshot nella soluzione.
- Non trasferire dati di simulazione voluminosi o build output.
- Non aggiornare ancora i consumatori: la migrazione di MathemaSharp/NDSharp/JupyterSharp/NavierStokes sarà una fase successiva, da autorizzare.
- Non dichiarare una libreria pronta senza build e test riusciti.

## Primo risultato atteso
Inventario e decisioni documentate, prima libreria piccola compilabile, test passanti, almeno un notebook con line/scatter verificato e un piano delle tranche successive.
