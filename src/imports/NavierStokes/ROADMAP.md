# Punti aperti

## Contenuti

- [ ] Rileggere e pubblicare l'articolo LinkedIn (`docs/articolo-linkedin.md`) e decidere se usare anche il post sarcastico (`docs/post-sarcastico.md`).
- [ ] Contestualizzare il progetto dentro **Numbers** (time series): l'energia cinetica della cavità e la sonda dietro il cilindro
      sono serie temporali; si possono esportare/ingerire come segnali di un impianto e analizzare con gli stessi strumenti.
- [ ] Prima di pubblicare, ricontrollare lo stato della verifica della dimostrazione di OpenAI (notizia dell'8 settembre 2026, ancora in discussione al 25 ottobre).
- [ ] Pubblicare il repository (GitHub) e aggiungere il link all'articolo.

## Codice

- [ ] Test: oggi c'è `ns selftest` (nessun pacchetto richiesto). Valutare un progetto xUnit/MSTest con gli stessi controlli.
- [ ] Cavità: griglia sfalsata (MAC) o 129×129 per scendere sotto RMSE 0,01 rispetto a Ghia; aggiungere i dati di Ghia per Re 400 e 1000.
- [ ] Cilindro: canale più largo o pareti a scorrimento libero per avvicinare lo Strouhal al valore di letteratura (≈ 0,165 a Re 100);
      calcolo di portanza e resistenza sul cilindro (serie temporali di Cl e Cd).
- [ ] App: animazione più fluida (meno dati per frame, es. PNG più piccoli o canvas), tema scuro, esportazione CSV dei risultati.
- [ ] Nuovi scenari possibili: raffreddamento di un componente (convezione), camino e tiraggio, ventilazione di una stanza,
      onda di piena in un canale, miscelazione in un serbatoio.
- [ ] Valutare un'esecuzione in Jupiternet/PySharp del notebook o un kernel che esponga gli stessi scenari.
- [x] Aggiornare il target a .NET 11 (net11.0).

## Note sull'ambiente di sviluppo originale

La soluzione è stata scritta in un ambiente senza accesso a NuGet: per questo non usa pacchetti. In locale si possono
aggiungere liberamente (es. ScottPlot o Plotly.NET per i grafici nel notebook).
