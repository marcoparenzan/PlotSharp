# PlotSharp — roadmap e stato di completamento

Questo documento è il registro del prodotto e dei consumatori. Sostituisce la precedente sequenza di tranche: una funzionalità presente non implica che il materiale originale sia consolidato o che un consumatore sia migrato.

## 1. Obiettivo del progetto

Una libreria di visualizzazione condivisa, indipendente da host e librerie matematiche, che diventi la fonte autorevole dei Charts oggi distribuiti fra MathemaSharp, NDSharp, NavierStokes e JupyterSharp. I consumatori devono usare PlotSharp direttamente o tramite adattatori/facade, senza mantenere motori duplicati. Le API precedenti devono avere un percorso di migrazione esplicito; eventuali esclusioni vanno deliberate, non confuse con lavoro completato.

Target attuale dei progetti attivi: .NET 11 RC3 (snapshot importati esclusi). Output attivo: SVG/HTML statico.

Aggiornamento 2026-10-28: gate ecosistema eseguito con successo, inclusi i due piloti NuGet. Aggiunto in MathemaSharp il pilota opt-in `03-indexed-charts.ipynb` e la facade `notebooks/shared/PlotSharpCharts.csx`: Lines, Panels e Stems delegano a IndexedCharts, quattro SVG eseguiti e validati XML. Non equivale a migrazione completa di MathemaSharp: renderer legacy conservato, dati non finiti e layout non equivalenti, verifica visiva ancora aperta. I conteggi locali sotto sono la baseline storica di PlotSharp; il gate centrale è documentato in MathemaSharp/ROADMAP.md. PNG/Skia e compatibilità Figure/Axes sono da consolidare, non già disponibili.

### Baseline completa aggiornata — D03 sampler/bande

Gate --no-images D:\Dev\BuildArtifacts\ecosystem-validation\20261028-215324, 31/31 exit0 dopo sampler fisico e lookup bande condivisi. Build/test/audit e reale integrazione kernel/notebook/UI/browser/selftest completo riconfermati. Nessun nuovo review PNG/SVG/screenshot, output notebook incorporati e raster in memoria. Warning/sospesi non nascosti; evidenze JSON recenti archiviate.
Prevale sulle note precedenti di non full gate. D03 non completamente migrata: palette/alpha/native raster/altre categorie/consumer/licenze/versioni restano separate. Nessuna pubblicazione.

### Verifica mirata precedente D03 — lookup bande e midpoint

Selezione inclusiva/duplicati/final bound e midpoint normalization condivisi in ContourBands, preserving legacy operation order. Dati esterni/non-finiti senza banda, livelli finiti/nondecrescenti snapshot. Color/alpha/Skia renderer restano ND. Shared227/227 eND19/19 con build verde, kernel/notebook vero rechecked, zero review images. Non nuovo full gate o release.

### Verifica precedente mirata D03 — sampler fisico bilineare dei filled contour

Sample NDSharp delegata a RectilinearFieldSampler con snapshot e aritmetica legacy; 81 query congelate coincidono. Assi crescenti/finite e shape coerente, query finite/extrapolazione conservata, valori non finiti lasciati al chiamante. Bande/colori/alpha/Skia restano ND; line geometry e Navier sampler non alterati.
Shared211/211 eND19/19, kernel source ricostruito e notebook vero linee/bande320×240 verificati, zero immagini di revisione. Warning noti non soppressi. Non nuovo full gate/release; D03 parziale.

### Integrazione D03 precedente nel kernel reale — baseline completa

Nuovo notebook NDSharp/pysharp/02-contours-kernel eseguito in JupyterNet/PySharp (.NET11), metadata DLL verificati e due output PNG320×240 incorporati per linee/bande. Nessuna immagine di revisione/screenshot. Il binding usava preview old: source opt-in ScienceUseSourcePlots e kernel-source-plots isolato ora provano la nuova dipendenza, default pacchetti preservato.
Core100/100 test mirati e gate completo --no-images D:\Dev\BuildArtifacts\ecosystem-validation\20261028-203820, 31/31 exit0 dopo tutte le modifiche. Warning build preesistenti non soppressi. D03 parziale, non native JupyterSharp/oracle pixel CPython/migrazione totale/release.

### Prima adozione D03 precedente — contour lineari effettivamente delegati

NDSharp.Plot DrawLines ora usa RectilinearContours/ContourKernel con X/Y fisici e convenzioni ND originali. Core Navier a indici condivide traversal/interpolation ma conserva propria edge direction/default/mask/bounds; niente conversione globale scorretta. Endpoint-level filtering, Ticks/AutoLevels, contourf, colori e Skia native restano ND.
Nuovo ProjectReference, geometria esatta testata su dati nonuniformi/degeneracies/NaN/overflow e baseline pixel native. Build+204/204 shared e18/18 ND passati, zero immagini. D03 parziale, non full gate/notebook/migrazione famiglia/release; vecchie preview non aggiornate.

### Preparazione D03 precedente — inventario NDSharp.Plot

Sorgente attiva NDSharp (non import) esaminata: dipendenze Skia3.119.0/native/font, net11 e Figure/Axes. Candidata contour-line geometry; nonuniform physical grid, invalid-cell/range-level policies e direzioni degli edge differiscono dal mapping a indici e vanno preservati. Ticks/AutoLevels/contourf/stili/renderer PNG non migrati.
Quattro test baseline su PNG in-memory aggiunti, build e 18/18 test passati. Report source hashes/dipendenze in MathemaSharp/ops/inventory/ndsharp-plot-contract.json. D03 in preparazione, nessun riferimento PlotSharp o renderer di produzione cambiato, niente immagini/release/full gate.

### Baseline completa precedente — FieldPlot consolidato

Gate --no-images D:\Dev\BuildArtifacts\ecosystem-validation\20261028-185732, 29/29 exit 0 dopo tutte le estrazioni. Suite198 casi e fixture source consumer riconfermate, CLI/Web e selftest numerico completo, UI/browser e raster in memoria verificati senza nuova review PNG/SVG/screenshot. Solo JSON nelle cartelle no-images, evidenze recenti/politica archiviate. Prevale sulle note più avanti di full gate non rieseguito.
D02 resta parziale per perimetro consumer/non-UI-testati/licenze/distribuzione, non per il solo renderer ora condiviso. Altre famiglie e accessibilità completa non concluse. Nessuna release.

### Verifica mirata precedente FieldPlot — renderer node/raster condiviso

Assemblaggio header/stili/elementi consolidato in NodeFieldPlot, facade opzioni/API Navier. Quattro golden complete verificano markup/pixel RGB, default Heatmap non modificato. Overlay same-shape/contour0–32/ostacoli validati; input prima non validati non promessi compatibili. Helper privati dead-code rimossi, utility API pubbliche preservate.
Build/suite198/198, Web/quadro/smoke senza immagini verdi. Non nuova baseline full o release; numerica/UI nei consumatori e D02 ancora da certificare completamente.

### Verifica precedente mirata layout/proiezione nodi

Geometria viewport Navier centralizzata in NodeFieldLayout, margini/rounding/Y e coordinate frazionarie precedenti preservati; non Heatmap a celle. Shape/dimensioni/collassi validati, indici finiti esterni extrapolati senza clamp. Build/suite192/192, Web/quadro/smoke senza immagini verdi. Assemblaggio/styles/header locali e D02 da consolidare; non nuovo full gate o release.

### Verifica precedente mirata colorbar — gradiente legacy

Frammento gradiente/tick/etichetta condiviso, mantenendo campioni11, tick3 e anchor/escaping incluso altezza dispari. Tre fixture markup di produzione coincidono dopo normalizzazione dei soli GUID. Colorbar strip default invariata. Build/suite178/178, Web/quadro/smoke senza immagini passati; frame e proiezione contour restano locali. Non nuovo full gate o release, D02 parziale.

### Verifica precedente mirata frecce — variante nodi compatibile

Path NodeVectorOverlay centralizzato, quattro golden FieldPlot preservati e direzioni/normalizzazione/stride/soglia testati. Quiver fisico/cell-centered default non modificato. Forme/valori/proiezione/stride validati, stile/frame locali e niente nuovo clipping. Build e suite 168/168, Web/quadro/pressione smoke verdi senza immagini. Non nuovo full gate/release; colorbar/proiezione contour e altre famiglie non concluse.

### Verifica precedente mirata limiti colore — semantica legacy

Selezione statistica limiti condivisa (sort/floor percentili, NaN omessi, simmetria e fallback min+1), default ColorScale non cambiato. Otto fixture di label FieldPlot congelate e limiti aggiuntivi testati; nuovo Resolve con contratto finito/max100k/clip [0,0.5], direct NormalizeLegacy senza nuove guardie. Input fuori contratto non compatibili per promessa.
Build/suite 154/154 passate, Web Release/quadro/smoke senza immagini. Rendering campo/proiezione/colorbar/frecce ancora locali. Nessun nuovo full gate o release, D02 parziale.

### Verifica precedente mirata sampler RGB — nodi bilineari

Preparazione scanline condivisa, usando coordinate di nodi/centri pixel e ordine bilineare legacy, non mapping Heatmap a celle. Navier conserva normalizzazione e colore maschera; il callback gestisce i non finiti. Campi min 2×2, maschere in indici campione e colore esplicito.
Fixture raw PNG restano coincidenti; nuovi casi su orientamento/centro/maschera/input portano la suite a 138/138, build e Web/quadro smoke verdi. Zero immagini. Campo completo/colorbar/frecce e altre famiglie non concluse; non nuovo full gate o release.

### Verifica precedente mirata encoder PNG — solo RGB8/filter-0

Encoder minimo BCL condiviso, usato dalla firma FieldToPng Navier senza cambiare sampling/masking; rimossi encoder/CRC duplicati. Dimensioni/lunghezze/filtri verificati, CRC statico immutabile e concorrenza testata. Tre fixture pre-migrazione raw/hash, nessun file immagine: pixel/dimensioni/CRC sempre confrontati, hash compressi sullo stesso runtime/OS.
Build e suite 127/127 passate, Web Release/quadro/smoke adozione --no-images ricontrollati. Campionamento, FieldPlot/proiezione/colorbar/frecce ancora locali. Non RGB-alpha/Skia/Figure-Axes, non nuovo full gate o release; D02 parziale.

### Verifica precedente mirata palette — RGB e byte compatibili

Preset immutabili condivisi 4/8/9 stop: default compatto PlotSharp e palette Navier distinti; spacing e troncamento storici conservati. ColorMap Navier facade, PNG/normalizzazione/colorbar rendering/proiezione ancora locali. Fixture di 323 input congelata prima della modifica con hash sorgenti, compresi stop/ULP e campioni colorbar, non rigenerata dai test. Corrispondenza byte/hex dei tre preset verificata.
Build e suite 117/117 passate, Web Release/quadro smoke ricontrollati. Zero immagini, non nuovo full gate o release. Le preview precedenti non includono automaticamente ColorPalette; D02 resta parziale.

### Verifica precedente mirata contour — compatibilità NavierStokes

Geometria MarchingSquares condivisa: nuovo overload di compatibilità preserva i segmenti nulli del legacy ai vertici, mentre l'overload/default precedente rimane invariato. Navier conserva firma tuple e proiezione nodo→SVG; FieldPlot/PNG/colorbar/frecce locali. Contratto max100000 campioni e valori/livelli finiti ±1e12, buffer per livello.
Build e suite PlotSharp 109/109 passate, compresi 16 pattern di cella, pairing a sella, vertici, coordinate, limiti e path field SVG; Web Release e quadro smoke passati. Zero immagini, non nuovo full gate o benchmark. D02 parziale, nuovo overload source-build solo, nessuna release. La baseline completa successiva è storica rispetto a queste modifiche.

### Baseline completa precedente — senza nuove immagini di revisione

Gate --no-images: D:\Dev\BuildArtifacts\ecosystem-validation\20261028-150530, 29/29 passi exit 0. Tutte le verifiche eseguite: nessuno skip del raster/browser, encoding PNG in memoria e report JSON separati, nessun salvataggio review PNG/SVG/screenshot. Immagini precedenti preservate, non dichiarate rigenerate; output notebook aggiornati.
Politica e report recenti protetti da test (suite operational 66/66): timestamp del processo e SaveImages devono corrispondere; sette JSON archiviati con i log. Questa baseline prevale sulle note mirate precedenti, ma non costituisce una nuova revisione visiva o un completamento di D01–D04/distribuzione. Nessuna release.

### Verifica incrementale contrasto Web precedente — senza immagini

Solo palette HTML del consumatore NavierStokes: accenti e testi attenuati scuriti. Edge su home/tubo/vento: 17 campioni solidi con min circa 4.97:1, superano il criterio 4.5; tastiera/layout/calcoli ancora verdi. Serie PlotSharp e algoritmi invariati. Sette casi sRGB aggiunti alla suite operational (50/50 passati), zero immagini e nessun nuovo full gate (29 previsti). Non audit completo di accessibilità, testi SVG o stati UI; gradienti/opacità di gruppo fuori supporto segnalati.

### Verifica incrementale controlli Web precedente — senza immagini

Nel consumatore NavierStokes, input/select mobile portati da min 16 a min 44 px e focus visibile. --mobile è passato con tastiera nativa sul vento e calcolo aggiornato, no overflow, testi SVG min 11 px. Desktop ricontrollato. Nessuna modifica a PlotSharp/calcoli e zero immagini. Otto nuovi casi del criterio di altezza: suite operational 43/43 passata; gate futuro 29, non nuovo full gate eseguito. Touch reale e accessibilità completa non certificati.

### Verifica incrementale testi/scorrimento Web precedente — senza immagini

Nel consumatore NavierStokes, min testo SVG misurato 5.85 px a viewport 360×800: corretto con SVG nativo e riquadro scrollabile sotto 600 px, senza toccare PlotSharp o i dati. Build Web e test mobile passati: min 11 px, no overflow pagina, scorrimento tastiera e bordo finale raggiunti. Desktop ricontrollato. Zero immagini; otto nuovi casi del criterio di font nella suite operational, 35/35 verdi. Nessun full gate rieseguito (29 passi previsti). Non certifica accessibilità completa, touch o simulazioni mobile.

### Verifica incrementale viewport stretto precedente — senza immagini

NavierStokes home/tubo/vento verificati in Kestrel+Edge a 360×800: no overflow documento/controlli, calcolatori aggiornati. Nessuna modifica al CSS o al renderer; zero immagini. Otto test del validatore layout nella libreria operational, suite 27/27 verde. Non copre testo SVG leggibile/touch/telefoni reali/simulazioni mobile. Gate futuro 29 passi compilato, non nuovo full gate eseguito.

### Verifica incrementale fischio precedente — senza immagini

Due cicli UI di avvio/progresso/stop LBM in Kestrel+Edge, griglia 140×50, controlli ripristinati. Modalità quadro ricontrollata. Due test core su token già cancellato/ripresa con token nuovo: suite PlotSharp 98/98 passata con build riuscita. Nessuna immagine e nessuna modifica di rendering/algoritmi; non certifica convergenza o Strouhal. Gate futuro 28 passi, non nuovo full gate eseguito.

### Verifica incrementale quadro precedente — senza immagini

Avvio/progresso/stop della pagina quadro controllato in Kestrel+Edge con griglia 31, due cicli e comandi ripristinati. Due test sul solutore proteggono token già cancellato e ripresa con token nuovo: suite PlotSharp 96/96 passata. Nessuna modifica al renderer o agli algoritmi, nessuna immagine generata. Non è convergenza/accuratezza né verifica di fischio/mobile. Gate futuro 27 passi, non nuovo full gate eseguito.

### Verifica incrementale Web precedente — scope limitato

NavierStokes.Web costruita Release; tool live Kestrel+Edge ha verificato home/tubo/vento, cambi dei range con aggiornamento dei risultati e presenza SVG condivisi. Nessuna immagine generata; parser UI coperti da 13 nuovi casi nella suite operational, 19/19 verdi. Simulazioni quadro/fischio, mobile e UI completa non coperti. Tool aggiunto al gate, ma non nuovo full gate eseguito: baseline completa rimane quella a 25 passi sotto.

### Stato corrente — adozione NavierStokes, 2026-10-28

La sorgente attiva è articoli/2026.10 NavierStokes, non gli snapshot importati. Core → PlotSharp per line/scatter, mantenendo Series/Options/firma LineChart; CLI/Web transitivamente consumano la libreria. Sette nuovi casi d'integrazione, suite PlotSharp 94/94 passata. CLI/Web build riuscite e selftest numerico originale completo passato. Tre report lineari su fixture smoke verificati in Edge e screenshot osservati; campo/PNG originale disponibile e XML verificato, non migrato.
D02 è parziale: campi/PNG/contour/ColorMap restano locali, UI Web e notebook dell'articolo non E2E verificati. Clip/layout e validazione degli input invalidi differiscono deliberatamente; report Ghia con viewBox più ampio. Non è accuratezza di regime delle fixture smoke: la prova numerica separata è il selftest completo.
App MathemaSharp: scan dei sorgenti Lab (2 file) e Docs (11) senza renderer locale, nessuna dipendenza grafica artificiale aggiunta; asset/Markdown separati. Gate completo: D:\Dev\BuildArtifacts\ecosystem-validation\20261028-104324, 25/25 passi exit 0. D01/D02 parziali, 0/4 famiglie complete; diritti/distribuzione ancora bloccati, nessuna preview pubblicata.

### Stato precedente — scenari applicativi, 2026-10-28

A1 magazzino e D1 fotovoltaico migrati: rispettivamente 3/2 SVG; gap del magazzino verificato, un gruppo KPI e due risposte per scenario preservati. Otto notebook, 43 SVG. Edge: 43 grafici individuali e due dashboard HTML assemblate senza overflow orizzontale al viewport desktop verificato, cinque tile e due risposte non vuote ciascuna. Due screenshot full-page osservati. Core MathemaSharp: 98/98 test passati separatamente. Gate completo aggiornato: D:\Dev\BuildArtifacts\ecosystem-validation\20261028-101452, 21/21 passi exit 0.
D01 rimane parziale: app e altri percorsi da verificare. Dashboard assemblata non equivale a notebook UI, mobile o VS Code; popup nativi e target sovrapposti restano limiti. Nessun trasferimento di helper host nella libreria grafica e nessuna preview pubblicata.

### Stato precedente — STL e tour, 2026-10-28

Prevale sui conteggi storici successivi. Migrati TimeSeries/09 (6 SVG, tabelle e ponte NDSharp preservati) e 10 (5 SVG, calcoli/concordanza numerica non alterati). Ora sei notebook applicativi source-build, 38 SVG eseguiti e XML validati. Edge: 38/38 con verifiche DOM e hit target per il perimetro dichiarato; 11 nuovi screenshot browser osservati. Il gate genera i passi migrati dal manifest e verifica le tabelle richieste. Core MathemaSharp: 96/96 test passati separatamente. Gate completo: D:\Dev\BuildArtifacts\ecosystem-validation\20261028-075021, 19/19 exit 0. Nessuna preview pubblicata; D01 ancora parziale, altre famiglie aperte.

### Stato precedente — browser e legende, 2026-10-28

Prevale sulle note storiche seguenti. Tutti i 19 raster dei notebook 06/08 osservati; iniziale verifica Edge su 27 SVG ha rilevato tre collisioni titolo/legenda. SeparateLegendRow opt-in aggiunge una riga, IndexedCharts lo abilita; quattro nuovi test, default legacy invariato. Tooltip nativi aggiunti alle aste. Seconda verifica Edge: 27/27 con testo nei limiti, nessuna collisione titolo/testi e almeno un title hit target raggiungibile ciascuno. Tre screenshot browser dei casi corretti osservati. PlotSharp 87/87 test passati. Gate completo aggiornato: D:\Dev\BuildArtifacts\ecosystem-validation\20261028-065717, 17/17 exit 0; Core MathemaSharp 94/94.
Non è una verifica del popup tooltip OS, della raggiungibilità individuale di tutti i campioni sovrapposti, di VS Code o di tutti i browser. I report conservano conteggi target totali/raggiungibili. Source-build soltanto, distribuzione e altre famiglie ancora aperte.

### Stato precedente — marker e adozione 06/08, 2026-10-28

Questa sezione prevale sulle note storiche successive. PreserveBoundaryMarkers opt-in conserva marker/tooltip campione ai limiti: overlay senza clip per centri finiti nel dominio chiuso, centri esterni omessi. Scale e clip di linee/aste/barre invariati; default core legacy preservato, IndexedCharts opt-in. Highlight non modificati. Quattro test aggiunti; PlotSharp 83/83 passati. Ultimo gate completo: D:\Dev\BuildArtifacts\ecosystem-validation\20261028-061520, 16/16 passi exit 0; Core MathemaSharp 93/93 verificati separatamente. Due spettri rigenerati e osservati: cerchi completi a zero e ai massimi, nessuna modifica della scala.
Migrati anche TimeSeries/06 e 08, veri JupyterNet, rispettivamente 6 e 13 SVG XML validati, tabelle preservate. Quattro notebook applicativi migrati, 27 SVG; manifest ops/migrated-notebooks.json e runner unico. Review visiva dei nuovi 06/08 non ancora eseguita; browser ancora aperto. D01 parziale, 0/4 famiglie complete; nuovi contratti solo source-build, nessuna preview sovrascritta.

### Seconda adozione e gap — 2026-10-28 (storico)

TimeSeries/07-smoothing-kalman migrato: sei SVG e tabelle HTML eseguiti nel vero JupyterNet; gap di 30 osservazioni preservato e verificato. Core LineChart mantiene il default rigoroso; AllowLineGaps consente solo NaN Y di linee con dati finiti, non Infinity/X/stems/bar o serie interamente mancanti. IndexedCharts Lines/Panels abilitano gap e tooltip nativi. Originale overload Stems preservato; nuovo overload con etichetta k usato dagli spettri. Table/Kpi/Tile estratti in Presentation.csx, caricata anche dal renderer legacy.
Otto immagini raster di review osservate: layout leggibile e gap visibile; clipping dei marker sui limiti degli assi da risolvere. Raster Svg.Skia in ops è solo strumento di review, non backend PNG del prodotto. API nuove nei sorgenti locali, preview NuGet precedenti non aggiornate. D01 rimane parziale, 0/4 famiglie complete.

### Prima adozione applicativa — 2026-10-28

MathemaSharp/Core/03-complex-signals.ipynb usa il bootstrap PlotSharp invece del renderer legacy: due SVG di spettro eseguiti nel vero host JupyterNet e validati XML. La baseline costruisce PlotSharp Release prima dei notebook e il gate include un runner dedicato. Test sul bootstrap/output aggiunto. D01 è ora parziale, non chiuso; tutte le quattro famiglie rimangono senza migrazione completa. Il pilota JupyterSharp resta una prova distinta. Verifica visiva ancora aperta.

## 2. Dashboard attuale

| Dimensione | Stato | Evidenza / limite |
|---|---|---|
| Nucleo SVG condiviso | **Parziale, funzionante** | Line/scatter, indexed/panels, stems/bar, heatmap/colorbar/mask/extent, contour/quiver di base |
| Consolidamento di tutto il patrimonio Charts | **Non completato** | Mancano matrice API esaustiva, backend PNG, Figure/Axes, layout e ulteriori artist |
| Validazione automatizzata della soluzione attiva | **Verificata per il perimetro implementato** | Build riuscita, 94 test xUnit passanti (inclusi sette casi del consumatore NavierStokes); galleria di 8 esempi baseline storica, notebook applicativi validati separatamente |
| Verifica visiva | **Preliminare, parziale** | Otto raster precedenti e 19 raster 06/08 osservati; 43 SVG verificati in Edge; tre screenshot corretti, 11 STL/tour e due dashboard assemblate osservati. Popup OS, target sovrapposti e VS Code restano aperti |
| Notebook end-to-end | **Parziale, eseguito** | Tre piloti JupyterSharp e otto notebook applicativi JupyterNet; XML verificato, revisione visiva parziale |
| Adozione nei repository di origine | **0/4 complete, D01 parziale** | Core/03, TimeSeries/06–10 e scenari A1/D1 MathemaSharp migrati e verificati; altri percorsi e famiglie aperti |
| Dipendenze attive interne | **2 consumatori** | PlotSharp.Tests e PlotSharp.Gallery riferiscono la libreria |
| Distribuzione | **Bloccata** | Diritti NavierStokes/MathemaSharp da confermare; nessun package pubblicato |
| Documentazione | **Presente, da mantenere** | README, USAGE/Reference, ARCHITECTURE, questo registro |

**Completamento globale: non raggiunto e non espresso in percentuale.** Il denominatore delle API originali non è ancora completo. I 70 test non costituiscono una misura di copertura di tutto il patrimonio importato. Il rapporto 0/4 misura solo l'adozione delle quattro famiglie note, non il completamento del prodotto.

### Stati usati

- **Da iniziare**: attività prevista senza deliverable verificato.
- **In corso / parziale**: esiste un risultato, manca almeno un criterio di accettazione.
- **Verificato**: evidenza eseguibile e criteri indicati soddisfatti nel perimetro dichiarato.
- **Bloccato**: prerequisito non soddisfatto; il codice può esistere ma non essere distribuibile.
- **Completato**: tutti i criteri della milestone soddisfatti, inclusi documentazione e verifiche richieste.
- **Escluso**: decisione esplicita con motivazione e percorso per i consumatori; non sinonimo di mancante.

### Avanzamento P01 — inventario degli snapshot eseguito

- [x] Tool rigenerabile e testato; report `ops/inventory/` con file/hash, dichiarazioni, usi notebook e riferimenti.
- [x] Baseline: 148 file; 478 righe public (non numero di API semantiche); 68 occorrenze notebook; 86 riferimenti project/package.
- [x] Distinti backup `.ralfai/undo` da materiale corrente: conservati nel catalogo file, esclusi dall'analisi dei consumatori.
- [x] 0 ProjectReference a csproj mancanti nel perimetro corrente; build/sorgenti complete dei consumatori **non verificate**.
- [x] Matrice per file NDSharp.Plot e destinazioni proposte in ARCHITECTURE; baseline puntuale nel CSV.
- [ ] Revisione semantica/approvazione della matrice API completa, alias e chiamate dinamiche.
- [ ] Catalogo dei repository reali e dipendenze/sorgenti mancanti, diritti e criteri di esclusione approvati.

P01 rimane **in corso**. D01 e D02 sono parziali; D03/D04 non completate, 0/4 migrazioni complete. Il tool non sostituisce l'integrazione end-to-end.

## 3. Roadmap del prodotto

| ID | Milestone | Stato | Dipende da | Criterio di completamento |
|---|---|---|---|---|
| P01 | Catalogo e provenienza | In corso | — | Ogni API/renderer/asset/test importato classificato; dipendenze mancanti identificate; matrice fonte → destinazione → mantenuto/adattato/escluso approvata |
| P02 | Diritti e attribuzioni | Bloccato | P01 | Licenze verificate per tutto il codice estratto; notice di terzi e autorizzazioni presenti; decisione documentata sulla licenza del package |
| P03 | Contratto pubblico del nucleo | In corso | P01 | Serie, assi, layout, stili, dati non finiti, precisione e serializzazione/export definiti; API/versionamento e compatibilità documentati; test di contratto |
| P04 | Consolidamento renderer SVG | In corso | P03 | Funzionalità SVG selezionate equivalenti o differenze approvate; casi numerici/geometrici e layout verificati; duplicati destinati a facade anziché nuovi motori |
| P05 | Consolidamento NDSharp.Plot e raster | Da iniziare | P01, P03 | Figure/Axes/Artists, PNG/Skia, font, tick/layout e adattamento NDArray separati coerentemente; test originali pertinenti eseguiti o sostituiti con equivalenti; piattaforme native verificate |
| P06 | Qualità visiva e gallery notebook | In corso | P04 | Gallery rappresentativa controllata visivamente; notebook eseguiti nei kernel supportati; report artefatti/regressioni; tutti i test del perimetro passanti |
| P07 | Adattatori e compatibilità consumatori | Da iniziare | P03, P04, P05 secondo API usate | Adattatori NDArray/display/Blazor o facade solo dove necessari; dipendenze unidirezionali; test integrazione; nessuna dipendenza del core da app o kernel |
| P08 | Adozione e dismissione duplicati | Da iniziare | P07, P06, P02 per trasferimento/distribuzione | Registro D01–D04 chiuso: build/test/esempi nei repository reali, chart locali sostituiti o residui motivati, documentazione aggiornata |
| P09 | Release e manutenzione | Bloccato | P02, P03, P06; P05/P08 per release di consolidamento completo | Package versionato, CI build/test, notice, installazione verificata, supporto piattaforme e policy breaking changes; nessuna pubblicazione implicita |

Una release sperimentale del sottoinsieme può precedere P05/P08, ma deve dichiarare le esclusioni e non essere presentata come consolidamento completo.

### Prossimo ordine di lavoro

1. **P01:** completare la matrice API e verificare accessibilità/completamento delle sorgenti dei consumatori. Non aggiungere altri chart prima di capire cosa manca davvero.
2. **P02 + P03:** chiudere diritti/attribuzioni e stabilire il contratto con i requisiti reali dei consumatori.
3. **D01/P06:** prima integrazione dimostrata in un notebook MathemaSharp, con output controllato e differenze note; non dichiarare migrata tutta la famiglia dopo un solo esempio.
4. **D02:** migrare NavierStokes per feature, risolvendo differenze campioni/centri, maschere e raster prima di eliminare FieldPlot locale.
5. **P05/D03:** consolidare NDSharp.Plot preservando capacità e compatibilità; poi NDSharp.Notebooks.
6. **D04:** integrazione display e distribuzione runtime JupyterSharp, senza copie divergenti degli assembly.
7. **P08/P09:** dismissione duplicati verificata e release.

## 4. Stato delle funzionalità e del consolidamento

| Area | Stato nella libreria attiva | Consolidamento / lavoro residuo |
|---|---|---|
| Linee, scatter, tick/assi base | Implementato e testato | Da confrontare con tutte le API originali; precisione double e layout ancora limitati |
| Serie indicizzate, Start, dashed, pannelli | Implementato e testato | Facade ispirata a MathemaSharp; differenza NaN reject vs gap; nessun notebook migrato |
| Stems/correlogrammi | Implementato e testato | Baseline zero/banda; scala non forzata simmetrica; compatibilità visiva originale da verificare |
| Barre verticali | Implementato e testato | Raggruppamento, stacking, categorie e varianti NDSharp non consolidati |
| Heatmap, colorbar, mask, extent | Implementato e testato | SVG a celle, no raster/interpolazione; aspect ratio fisico e palette divergente mancanti |
| Contour | Base implementata e testata | Marching squares NavierStokes con pairing fisso saddle; niente decider, stitching, fill o label; non equivalenza NDSharp contour completa |
| Quiver | Base implementata e testata | Geometria adattata NDSharp, scala esplicita; no autoscale/streamlines; unità pixel anziché points/DPI |
| Figure/Axes/Artists | Non consolidato | Snapshot NDSharp; modello pubblico e facade da decidere |
| PNG/Skia, font, tight_layout e layout avanzato | Non consolidato | Snapshot NDSharp e encoder NavierStokes da confrontare; dipendenze native/licenze da verificare |
| Istogrammi, 3D e altri artist/colormap | Non consolidato | Catalogo completo e decisione di conservazione/esclusione ancora aperti |
| Adattamento NDArray | Non implementato | Deve rimanere fuori dal core; conversione/normalizzazione di precisione esplicita |
| Display notebook / Blazor | Nessun adattatore verificato | La libreria restituisce SVG/HTML; non integra direttamente un host |
| Tabelle, Tiles, Quote MathemaSharp | Fuori dal nucleo Charts proposto | Presentazione host, non motore grafico; destinazione da approvare in P01 |

Il materiale in `src/imports/` resta archivio immutato ed escluso dalla soluzione. La presenza di una funzionalità attiva non significa che il file originale sia stato eliminato o che tutte le sue opzioni siano supportate.

## 5. Registro dei progetti dipendenti

### Dipendenze effettive nella soluzione attiva

| ID | Progetto | Relazione | Stato / evidenza |
|---|---|---|---|
| I01 | PlotSharp.Tests | ProjectReference → PlotSharp e PlotSharp.Inventory | Attivo, 70 test passanti nell'ultima verifica |
| I02 | PlotSharp.Gallery | ProjectReference → PlotSharp | Attivo, 8 esempi generati e validati XML; visuale non verificata |

Tool aggiunto alla soluzione: `src/tools/PlotSharp.Inventory/PlotSharp.Inventory.csproj`, **non dipende da PlotSharp** e non è un nuovo consumatore Charts.

Percorsi: `src/PlotSharp.Tests/PlotSharp.Tests/PlotSharp.Tests.csproj`, `src/PlotSharp.Gallery/PlotSharp.Gallery/PlotSharp.Gallery.csproj`.

### Consumatori esterni previsti — non ancora dipendenti da PlotSharp

La tabella descrive ciò che è osservabile negli snapshot, **non uno scan aggiornato dei repository originali**. Nessun consumatore importato è incluso nella soluzione attiva. Responsabili, branch/commit di migrazione e accesso ai repository reali non sono ancora registrati: da definire prima dell'esecuzione.

| ID | Famiglia / componenti da seguire | Dipendenza chart osservata | Stato adozione | Prossimo deliverable / blocchi |
|---|---|---|---|---|
| D01 | MathemaSharp: notebook/shared/Charts.csx, notebook TimeSeries/Core; app Lab/Docs da valutare | Helper SVG con Display.Html; MathemaSharp.NDSharp riferisce NDSharp (non prova adozione NDSharp.Plot) | **Parziale** | Otto notebook/43 SVG e due dashboard migrati; helper presentazione condivisi, app senza renderer locale nei sorgenti ispezionati. Completare inventario/API/asset, precisione e licenze |
| D02 | NavierStokes.Core, NavierStokes.Cli, NavierStokes.Web, notebook | Charts/Reports in Core; Cli e Web → Core; Web/Shared/Chart.razor mostra SVG | **Parziale** | Line/scatter Core delegati, CLI/Web build e selftest passati, tre report Edge smoke verificati; migrare campi/PNG/contour; home/tubo/vento Web smoke passato, completare quadro/fischio/UI/notebook; diritti ancora aperti |
| D03 | NDSharp.Plot, NDSharp.Plot.Tests, NDSharp.Notebooks, NDSharp.Notebooks.Tests; notebook 11/12 | Plot → NDSharp + Skia; Notebooks → Plot; i due progetti Tests → relativi progetti | **Parziale: line-contour geometry delegata** | ProjectReference e adapter fisico/core comune, baseline18/18 ND e204/204 shared verdi. Servono full gate/notebook, Figure/Axes/raster/NDArray e restante compatibilità/distribuzione |
| D04 | JupyterSharp.Engine/Host/kernel, samples, vscode-extension runtime | Host → Engine; Engine → kernel; DLL NDSharp.Plot e PySharpLib.Matplotlib raccolte nel runtime pysharp | **Non iniziata** | Identificare tutti i punti di display/caricamento/distribuzione, integrare assembly/MIME PlotSharp, eseguire smoke test kernel; sorgenti PySharpLib.Matplotlib non raccolte |

PySharpLib.Matplotlib è una **dipendenza/integrazione da investigare**, non una quinta famiglia migrata o un progetto sorgente disponibile qui. Non è sufficiente sostituire la DLL raccolta.

### Checklist per ogni D01–D04

- [ ] Proprietario, repository reale, branch/commit baseline e perimetro componenti registrati.
- [ ] Elenco usi chart e API originali completo nel perimetro.
- [ ] Matrice compatibilità; funzionalità mancanti e differenze accettate esplicite.
- [ ] Riferimento a PlotSharp o facade/adattatore inserito nel repository consumatore.
- [ ] Build e test del consumatore riusciti, con comandi/commit registrati.
- [ ] Esempi end-to-end eseguiti e output verificato visivamente.
- [ ] Implementazione locale rimossa/delegata; residui motivati e tracciati.
- [ ] Documentazione/installazione aggiornata, versione PlotSharp fissata.

**Una famiglia è migrata solo quando la checklist è chiusa per tutto il perimetro concordato.** Un ProjectReference, una modifica agli snapshot o una build della sola libreria non bastano.

## 6. Dipendenze della libreria e blocchi

Attualmente il core usa solo .NET/BCL; nessuna dipendenza da NDSharp, SkiaSharp, Blazor o kernel. Renderer/adattatori futuri devono dipendere dal core, mai il contrario.

- **B01 — diritti:** licenze/attribuzioni NavierStokes e MathemaSharp non verificate nella selezione raccolta. NDSharp include MIT e font con licenza distinta; conservare notice completi quando distribuiti.
- **B02 — completezza sorgenti:** import euristico; csproj presenti non garantiscono dipendenze/classi complete. Recupero mirato necessario per integrazioni reali.
- **B03 — contratto compatibilità:** scegliere modello pubblico e facade senza perdere capacità Figure/Axes o moltiplicare renderer.
- **B04 — precisione:** double, limiti ±1e12, pixel arrotondati; normalizzazione Generic Math/Decimal non risolta.
- **B05 — validazione host/visuale:** parziale: notebook CLI JupyterSharp/JupyterNet, 43 SVG e due dashboard Edge, tre report NavierStokes smoke verificati; popup nativi, renderer VS Code e altri browser non verificati.
- **B06 — release:** package/versione, CI, piattaforme native e policy breaking changes da definire.

Non sono automaticamente fuori scope PNG, 3D o artist avanzati solo perché mancanti: la decisione va presa in P01. Pubblicazione/installazione non sono implicite nelle attività di sviluppo.

## 7. Evidenze e aggiornamento del registro

Baseline storica: `src/PlotSharp.slnx`, 70 test e gallery di otto esempi XML. Non è il totale corrente: 94 test, inclusi sette casi di integrazione con NavierStokes.Core attivo (non snapshot). Evidenze correnti nel gate sopra; gli snapshot restano esclusi dalla soluzione, report consumer/visuali hanno perimetri distinti e non dimostrano copertura dell'intero patrimonio importato.

```sh
dotnet build src/PlotSharp.slnx
dotnet test src/PlotSharp.slnx
dotnet run --project src/PlotSharp.Gallery/PlotSharp.Gallery/PlotSharp.Gallery.csproj -- artifacts/gallery
```

A ogni obiettivo concluso aggiornare: stato Pxx/Dxx, criteri soddisfatti, blocchi residui, comandi/risultati e commit del consumatore quando disponibile. Aggiornare conteggi solo dopo verifica; mantenere separati test del core, compatibilità, output visivo e migrazioni. Questo documento è lo stato operativo; ARCHITECTURE conserva le decisioni/provenienza, USAGE la reference effettiva.
