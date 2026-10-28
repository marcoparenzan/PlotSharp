namespace NavierStokes.Core;

/// <summary>I grafici "standard" degli scenari, condivisi da CLI, app web e notebook.</summary>
public static class Reports
{
    public static string CavityVelocity(CavitySolver s, string? subtitle = null) =>
        Charts.FieldPlot(s.Speed(), new Charts.FieldPlotOptions
        {
            Title = "Velocità dell'aria e linee di flusso",
            Subtitle = subtitle ?? $"Re = {s.Reynolds:0}, griglia {s.N}×{s.N}, t = {s.Time:0.0} s",
            ColorbarLabel = "velocità / velocità del coperchio",
            Map = Charts.ColorMap.Sequential,
            Min = 0, Max = 1,
            Contours = s.StreamFunction(),
        });

    public static string CavityPressure(CavitySolver s) =>
        Charts.FieldPlot(s.P, new Charts.FieldPlotOptions
        {
            Title = "Pressione e direzione del flusso",
            Subtitle = "Rosso: sovrapressione · blu: depressione (rispetto al centro del lato superiore)",
            ColorbarLabel = "pressione / (ρ·U²)",
            Map = Charts.ColorMap.Diverging,
            Symmetric = true,
            ClipPercentile = 0.03,
            Arrows = (s.U, s.V),
            ArrowStride = Math.Max(3, s.N / 13),
        });

    public static string CavityGhia(CavitySolver s)
    {
        var (y, u) = s.VerticalCenterline();
        double rmse = GhiaReference.Rmse(y, u);
        return Charts.LineChart(new[]
        {
            new Charts.Series("Simulazione C#", u, y, Charts.Series1),
            new Charts.Series("Ghia et al. 1982", GhiaReference.URe100, GhiaReference.Y, Charts.Series2, Markers: true, Line: false),
        }, new Charts.LineChartOptions
        {
            Title = "Verifica: profilo di velocità sulla verticale centrale (Re = 100)",
            Subtitle = $"Errore quadratico medio rispetto al benchmark: {rmse:0.0000}",
            XLabel = "u / velocità del coperchio",
            YLabel = "altezza y / L",
            Width = 560, Height = 380,
        });
    }

    /// <param name="timeScale">Fattore per passare dal tempo del modello al tempo reale (L / U).</param>
    public static string CavityEnergy(CavitySolver s, double timeScale = 1) =>
        Charts.LineChart(new[]
        {
            new Charts.Series("Energia cinetica", s.EnergyHistory.Select(e => e.Time * timeScale).ToArray(),
                              s.EnergyHistory.Select(e => e.Energy).ToArray(), Charts.Series1),
        }, new Charts.LineChartOptions
        {
            Title = "Energia cinetica nel tempo: dal fermo al regime",
            Subtitle = "Una time series: il fluido si avvia e si stabilizza",
            XLabel = "tempo [s]",
            YLabel = "energia (unità del modello)",
            YMin = 0,
        });

    public static string CylinderVorticity(CylinderLbm c, string? subtitle = null) =>
        Charts.FieldPlot(c.Vorticity(), new Charts.FieldPlotOptions
        {
            Title = "Scia di von Kármán: i vortici che si staccano dal palo",
            Subtitle = subtitle ?? $"Re = {c.Reynolds:0}, passo {c.Step:N0}",
            ColorbarLabel = "vorticità ω·D/U",
            Map = Charts.ColorMap.Diverging,
            Min = -3, Max = 3,
            Obstacle = (c.CenterX, c.CenterY, c.Diameter / 2),
            PlotWidth = 620,
        });

    public static string CylinderProbe(CylinderLbm c)
    {
        var t = Enumerable.Range(0, c.ProbeV.Count).Select(k => (double)(k * c.ProbeEvery) * c.InletSpeed / c.Diameter).ToArray();
        var st = c.Strouhal();
        return Charts.LineChart(new[] { new Charts.Series("Velocità trasversale", t, c.ProbeV.ToArray(), Charts.Series1) },
            new Charts.LineChartOptions
            {
                Title = "Sonda a valle: la scia oscilla come un pendolo",
                Subtitle = st is { } v ? $"Numero di Strouhal misurato St = {v:0.000}" : "Scia non ancora periodica",
                XLabel = "tempo adimensionale t·U/D",
                YLabel = "v / U",
            });
    }
}
