# Architettura e inventario

## Layout

- `src/PlotSharp.slnx`: soluzione attiva .NET 11.
- `src/PlotSharp/PlotSharp/`: libreria, API Charts e renderer SVG.
- `src/PlotSharp.Tests/PlotSharp.Tests/`: test xUnit.
- `src/imports/<origine>/`: archivio immutato, escluso dalla soluzione.
- `import-manifest.csv`: provenienza e hash degli import originali; non descrive gli hash dei file estratti/modificati.
- `notebooks/`: spazio riservato alla galleria futura.
- `ops/`: spazio operativo; nessuna pipeline attivata.

## Inventario rigenerabile (P01)

Tool attivo `src/tools/PlotSharp.Inventory`, senza dipendenza da PlotSharp. Tests lo riferisce per testare CSV, dichiarazioni, decodifica notebook e mapping. Output in `ops/inventory/`: files.csv (provenienza/classificazione/hash), public-declarations.csv (dichiarazioni e destinazione proposta), notebook-calls.csv (cella/riga/call), references.csv (ProjectReference/PackageReference e presenza csproj).

Ultima esecuzione: 148 file, 478 righe con dichiarazioni public, 68 occorrenze chart/display nei notebook, 86 riferimenti di progetto/package, 0 ProjectReference mancanti nel perimetro corrente. **Zero csproj mancanti non prova sorgenti complete, restore o build dei consumatori.** Trovati import `.ralfai/undo`: restano nel catalogo file/hash come provenienza, ma sono esclusi da API/usi/riferimenti correnti. Nessuno snapshot modificato.

Limiti: scanner lessicale, non Roslyn/semantic API; non raccoglie dichiarazioni non-public, non ricostruisce firme multilinea né scope completo e può includere righe pubbliche in commenti multilinea. Notebook analizzati via JSON (escape Unicode decodificati), solo source delle code cells, non output/markdown; regex Chart/Charts/IndexedCharts/plt non copre alias o chiamate dinamiche e può includere stringhe/commenti. CSV generati sono baseline di revisione, non decisioni di compatibilità approvate. Non modificarli manualmente: modificare le regole del tool e rigenerare; decisioni curate in questa matrice/roadmap.

### Matrice di destinazione per materiale NDSharp.Plot

Tutte le righe sono **proposte da verificare**, non estrazioni completate. Le dichiarazioni puntuali e numeri di riga sono nel CSV.

| Fonte | Superficie raccolta | Destinazione proposta / stato |
|---|---|---|
| Artists.cs | Artist/DrawContext, Line2D, Scatter, Patch/Rect/Ellipse/Polygon, TextArtist, ImageArtist, ArrowProps/CoordSpace | P03 modello + P05 backend; nessuna equivalenza completa con Series attuale |
| Axes.cs | GridCell, Legend, Axes, limiti/inversioni, tick lineari/log, conversione coordinate, Colorbar | P05 facade Figure/Axes; tick log/inversioni/legend avanzate mancanti |
| Figure.cs | Figure/SubplotParams, Renderer, Layout/tight_layout, RenderPng | P05 modello figure + renderer raster; non consolidato |
| FigureExtensions.cs | Estensioni Figure | P05 compatibilità, da portare con il modello |
| Colormap.cs / Colormaps.g.cs | Colormap/Colors e palette generate | P03 scala/colormap + P05 compatibilità; palette completa non estratta |
| Contour.cs | ContourArtist/IMappable e algoritmo | P05 compatibilità contour/fill; overlay attuale non sostituisce questa API |
| Quiver.cs | Artist Quiver, bounds e geometria shaft/testa | Geometria adattata in FieldOverlays, API Artist/bounds/legend non consolidata |
| Plot3D.cs | Artist3D, Line3D/Scatter3D/Poly3D/Text3D, proiezione | P05 modello/proiezione/backend; non escluso né implementato |
| Text.cs | Fonts, TextMetrics, MathText | P05 tipografia/backend; font e licenze da preservare |
| Ticks.cs | Locator/formatter, offset, log | P03/P05 assi; NiceTicks attuale non equivalente |
| Fonts/*.ttf e LICENSE_DEJAVU | Asset terzi | P02/P05 notice e risorse backend; non incorporati |
| HighLevelTests.cs | Test originali API plotting | P05 test compatibilità; archiviati, non eseguiti |
| tools/oracle/gen_colormaps.py | Generazione palette | Tool di origine da conservare come provenance/generazione; non runtime core |

NavierStokes: LineChart/Series/options/Num/NiceTicks adattati; MarchingSquares adattato con contratto differente; FieldPlot/FieldToPng e ColorMap completa non sostituiti. MathemaSharp: Lines/Panels/Stems e Line hanno facade parziale; display, NaN-gap, tooltip e scale differiscono; Table/Tiles/Quote restano presentazione da classificare. JupyterSharp: host/assembly/notebook sono consumatori, non nuovo renderer; PySharpLib.Matplotlib disponibile solo come binario, nessuna API sorgente ricostruita.

## Inventario funzionale e decisioni

L'import iniziale dichiara 148 file (MathemaSharp 54, NDSharp 50, JupyterSharp 29, NavierStokes 15); il manifest resta la fonte puntuale della selezione euristica. Non è un inventario completo dei repository originali.

| Fonte | Materiale / dipendenze | Decisione |
|---|---|---|
| NavierStokes/Core/Charts.cs | SVG linee, scatter tramite flag, legenda/tick; field PNG scritto in C# con ZLib, color map, marching squares e frecce | Estrarre line/scatter e helper nella prima tranche; campi restano archivio fino a validazione dedicata. |
| NavierStokes/Web/Shared/Chart.razor | Visualizza stringa SVG tramite MarkupString | Mantenere come esempio di adattatore host, non dipendenza del core. |
| NavierStokes/Reports.cs, pagine e notebook | Consumatori di dati di simulazione | Non trasferire logica applicativa nella libreria. |
| MathemaSharp/notebooks/shared/Charts.csx | Lines, Panels, Stems; Display.Html; tabelle, tile e quote | Riutilizzare in seguito composizione/presentazione; non importare il kernel. Duplicato concettuale delle linee NavierStokes, non secondo renderer attivo. |
| NDSharp.Plot | Figure/Axes/Artists, contour, quiver, 3D, colormap, tick, layout e PNG | Patrimonio più ricco da conservare; separare modello matplotlib-shaped, backend Skia e adattamento dati prima di estrarlo. |
| NDSharp.Plot.Tests/HighLevelTests.cs | Test ad alto livello dell'API precedente | Riferimento per futura compatibilità; non eseguiti dalla soluzione attiva. |
| NDSharp.Plot/Fonts | DejaVu Sans, tre TTF e LICENSE_DEJAVU | Asset terzi, non incorporati nella libreria attuale. |
| JupyterSharp | Notebook, kernel/host csproj e DLL NDSharp.Plot / PySharpLib.Matplotlib | Integrazione di display/runtime; le DLL non sono sorgente autorevole e non vanno ricompilate o redistribuite alla cieca. |

NDSharp.Plot richiede NDSharp, SkiaSharp 3.119.0 e native assets Linux/macOS; gli snapshot delle dipendenze non sono necessariamente completi. Le csproj applicative raccolte non implicano la presenza di tutte le classi richieste. Nessuno snapshot è stato compilato in questa tranche.

## Provenienza dell'estrazione

`src/PlotSharp/PlotSharp/Charts.cs` deriva da `src/imports/NavierStokes/src/NavierStokes.Core/Charts.cs`: conservati Series, LineChartOptions, LineChart, Header, NiceTicks, Num, Esc e colori; esclusi campi/PNG/contour. Namespace cambiato; aggiunta validazione di coordinate, bounds, dimensioni e colori; rafforzata la guardia sui tick. Non è stata implementata da zero un'altra libreria grafica.

Licenze: NDSharp include LICENSE e intestazioni MIT; i font hanno licenza distinta. JupyterSharp include LICENSE. La selezione NavierStokes e MathemaSharp non include un LICENSE: l'assenza non implica libertà di riuso. **Bloccare pubblicazione e redistribuzione dell'estrazione finché l'autore/proprietario non conferma i diritti e le attribuzioni.** Nessuna licenza globale è stata inventata per questo repository.

## Confini

Per ora un unico assembly piccolo: separare backend/adattatori solo quando esiste un secondo implementato. Il core non dipende da Microsoft.Extensions perché non ha servizi, configurazione o logging; tali componenti spettano alle future app/integrazioni. Non modificare gli originali né inserire indiscriminatamente gli import nella soluzione.

Test attivi: XML/escaping, line/scatter, input vuoti e incoerenti, colori/iniezione, dati non finiti, serie costanti, limiti, dimensioni, tick e cultura italiana. Non sono test di regressione visiva, di PNG o di compatibilità completa. Nessuna verifica visiva o notebook eseguito è dichiarata.

## Quinta tranche

`FieldOverlays.cs`: MarchingSquares deriva dall'algoritmo in NavierStokes.Core/Charts.cs (ordine dei bordi e pairing conservati). Aggiunti validazione eager, maschere, risultati Segment e rimozione dei segmenti degeneri. I casi a sella rimangono a pairing fisso: un asymptotic decider resta una futura opzione da confrontare, non una proprietà dichiarata dell'algoritmo attuale.

La geometria SVG quiver adatta la costruzione shaft/triangolo di `src/imports/NDSharp/src/NDSharp.Plot/Quiver.cs`, origine NDSharp, copyright Marco Parenzan, repository con licenza MIT in `src/imports/NDSharp/LICENSE`. La distribuzione dovrà conservare il notice MIT completo; il repository continua a non essere pronto alla distribuzione per i diritti NavierStokes ancora da confermare. Nessuna dipendenza SkiaSharp introdotta: testa in pixel (8 massimi) anziché points/DPI, Scale esplicita.

Heatmap interpreta valori e vettori ai centri delle celle; FieldExtent continua a descrivere bordi esterni. Overlays condividono plotW/plotH e orientamento del campo e usano clip GUID univoci. La maschera esclude un quadrilatero contour se uno dei campioni è escluso; quiver elimina solo i campioni mascherati, non le parti della freccia sopra altre celle escluse. Il mapping vettoriale può deformare angoli se l'aspect ratio fisico non è conservato.

66 test passanti, incluso campo lineare, saddle, equality, singleton, maschere, direzione vettoriale con entrambe le origini, shape/stride/color invalidi. Galleria eseguita con otto esempi XML validati. Nessuna verifica visiva o notebook eseguito dichiarata.

## Quarta tranche

`ColorScale.cs` separa normalizzazione dal renderer. Min/Max immutabili, valori saturati prima della sottrazione, colori ottenuti dalla palette esistente. Heatmap e colorbar condividono il contratto; le 128 strisce della colorbar campionano la stessa palette, con tick agli estremi e al centro. La legenda è opzionale e il layout predefinito delle heatmap precedenti è conservato.

`FieldExtent` descrive bordi esterni fisici; la griglia SVG è ancora uniforme. Non conserva l'aspect ratio fisico e non rappresenta coordinate non uniformi. Tick Y coerenti con OriginUpper, centri dei tooltip ancora espressi come indici. Mask true omette la cella prima di analizzarne il valore: non finiti mascherati sono validi, non mascherati rifiutati. Campo interamente mascherato ammesso solo con range esplicito. Questo non modifica la policy reject delle linee.

50 test attivi passanti; galleria con sette esempi eseguita e XML validato. Nessuna verifica visiva browser/notebook o migrazione dei repository originali è stata eseguita.

## Terza tranche

Stems e BarWidth estendono Series nello stesso renderer, mantenendo assi, validazioni e clipping condivisi. `IndexedCharts.Stems` traduce il correlogramma MathemaSharp in serie di aste più due serie tratteggiate per la banda: nessun secondo motore SVG.

`Heatmap.cs` estende Charts tramite partial e usa Header/F/Num comuni. Nuovo percorso SVG a celle: orientamento predefinito [j,i] con j=0 in basso come NavierStokes, a differenza degli imshow che possono usare origine superiore. Scelta deliberata per una prima API senza dipendenze native: non è un port del PNG bilineare NavierStokes né una sostituzione compatibile di NDSharp.Axes.imshow. Quattro stop della palette NavierStokes sono riusati; interpolazione cromatica lineare, normalizzazione e saturazione separate dal mapping spaziale. Limite 100000 celle per contenere l'output; backend raster e colorbar ancora da estrarre.

La galleria eseguita include ora sei esempi: linee, scatter, pannelli, stems, barre e heatmap. 31 test passanti, inclusi baseline positive/negative, band, orientamento, colori estremi/costanti, limiti e matrici vuote/non finite/sovradimensionate. Gli snapshot restano invariati; vincoli di licenza e assenza di controllo visivo restano validi.

## Seconda tranche

`IndexedCharts.cs` è una facade host-independent ispirata a Line/Lines/Panels del materiale MathemaSharp: delega sempre a Charts.LineChart, non incorpora un secondo renderer. Non replica la policy originale dei gap NaN: il contratto rimane reject. Dashed si applica a path e legenda, gli ID di clipping usano GUID completi e il clip coincide con il rettangolo degli assi.

`src/PlotSharp.Gallery/PlotSharp.Gallery/` è un eseguibile senza servizi/configurazione; genera `artifacts/gallery/index.html`, lines.svg, scatter.svg e panels.html. Eseguito con successo, con parsing XML degli esempi prima della scrittura. Gli output non sono snapshot di riferimento e si possono rigenerare. Test attivi totali: 19, inclusi mapping di estremi espliciti, scale indipendenti, copia dati, tratteggio e null text. Nessuna verifica browser o notebook è stata effettuata.
