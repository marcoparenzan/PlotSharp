// Bootstrap comune dei notebook JupyterNet (≥ 0.3.0): i notebook lo caricano con #load "shared/MathemaSharp.csx" e i #r qui sotto
// sono relativi a questo file. Prerequisito: dotnet build src/MathemaSharp.slnx -c Release.
#r "../../artifacts/bin/MathemaSharp.Core/release/MathemaSharp.Core.dll"
#r "../../artifacts/bin/MathemaSharp.Quantum/release/MathemaSharp.Quantum.dll"
#r "../../artifacts/bin/MathemaSharp.LinearAlgebra/release/MathemaSharp.LinearAlgebra.dll"
#r "../../artifacts/bin/MathemaSharp.TimeSeries/release/MathemaSharp.TimeSeries.dll"
#load "Kernel.csx"
#load "Charts.csx"

// Le direttive using di un file caricato con #load NON si propagano al notebook (i tipi sì): ogni notebook dichiara le proprie.

