# Framework, platforms and precision (R06)

State as of 2026-10-29. Data taken from csproj, nuspec, `project.assets.json` and from an isolated consumer that was run; the **supported platform is a decision of the author** (§5), here it is only a proposal. No retargeting was performed.

## 1. Effective target frameworks

| Scope | TFM | Notes |
|---|---|---|
| All projects of PlotSharp, NDSharp, MatSharp, MathemaSharp, PySharp, JupyterSharp, NavierStokes, DecimalInDotNet11 | `net11.0` | SDK `11.0.100-rc.3.26522.148` (MathemaSharp's `global.json`, `latestFeature`, prerelease allowed). They are **RC3**: no guarantee of long-term support. |
| `Ecosystem.Validation`, `Ecosystem.Validation.Tests` | `net10.0` | Exception already declared (R01); runs on runtime 10.0.14 or with roll-forward. It is not a distributed package. |
| `src/imports/*` snapshots | n/a | Historical copies, not compiled as a product. |

Runtimes installed on the verification machine: 5.0, 6.0, 9.0, 10.0.14, 11.0.0-rc.3 (SDK 6, 10 RC1, 11 RC3).

## 2. Packages: what they declare and what they contain

| Package (local feed `D:\Dev\NuGetLocalFeed`) | TFM in the package | Dependencies found | Notes |
|---|---|---|---|
| `NDSharp.Plot 2.6.3-net11-preview.2` (27/10) | `net11.0` | NDSharp 2.8.1-net11-preview.1, SkiaSharp 3.119.0, Skia natives Linux and macOS | **Does not depend on PlotSharp**: it contains the contour code predating the extractions. It is the version that `MatSharpLib.Plot` and `PySharpLib.Matplotlib` reference. |
| `PlotSharp 0.1.1-net11-preview.1` (27/10) | `net11.0`, no dependencies | `authors` = "PlotSharp", **no licence** | Blocker L1 of R04 |
| Current sources, packed into a temporary feed as `9.9.9-r06` | `net11.0` | NDSharp.Plot → NDSharp + **PlotSharp** + SkiaSharp 3.119.0 + Skia natives Linux/macOS | Test for N4; the local feed was **not touched** |

Consequence: until NDSharp.Plot is republished with the dependency on PlotSharp, **the downstream consumers do not use the shared kernel** (R12/N4 open).

## 3. Verification with an isolated consumer

Empty project in a temporary folder, temporary feed + nuget.org, separate package cache, no pre-existing local artifact.

| Test | Result |
|---|---|
| `net11.0` consumer, `dotnet run` on Windows x64, runtime 11.0.0, reference to `NDSharp.Plot` only | **OK**: clean restore, `PlotSharp 9.9.9` resolved transitively, non-uniform filled contour rendered, expected pixel verified (red in the upper band) |
| `net10.0` consumer | **Rejected at restore**: `NU1202`, the package supports only `net11.0` |
| Restore for `linux-x64`, `linux-arm64`, `linux-musl-x64`, `osx-x64`, `osx-arm64`, `win-x86`, `win-x64`, `win-arm64` | The package graph contains `libSkiaSharp` for win-x86/x64/arm64, osx, linux-{x64, arm, arm64, x86, loongarch64, riscv64} and linux-musl-{x64, arm, arm64, loongarch64, riscv64}. **File resolution only: not run.** |
| `browser-wasm`, `android-arm64` | No Skia native in NDSharp.Plot's graph |

Really run: **win-x64 only**. Linux and macOS were not tried (no host available).

## 4. Platforms per component

| Component | Native dependencies | Platforms (from data) |
|---|---|---|
| PlotSharp | None (BCL only, SVG/HTML, RGB8 PNG encoder) | Any .NET 11 host |
| NDSharp.Plot | SkiaSharp 3.119.0 + embedded DejaVu fonts | Windows, macOS, Linux glibc/musl as per §3; not browser/mobile |
| NDSharp.Image | OpenCvSharp4.runtime.win | **Windows only** for the included native runtime |
| PySharp.Torch | TorchSharp-cpu | Outside the graphics perimeter |

## 5. Numeric conversion policy (precision)

The chart works in `double`; the stored data is not modified. Tests in `NDSharp.Plot.Tests/PrecisionPolicyTests.cs`.

| Step | Policy | Verified |
|---|---|---|
| NDArray → `double` for display (`ToArray<double>`) | Exact for `Float32` and for integers up to ±(2^53 − 1); **rounded** beyond (e.g. 2^53 + 1 → 2^53, tie to even) | Yes, 3 tests |
| `Float32` → `double` | Exact extension; it does not recover the decimal (0.1f ≠ 0.1) | Yes |
| Stored values | Unchanged by the conversion | Yes (the original `long` stays 2^53 + 1) |
| Complex | `PySharp.Matplotlib` uses the real part of scalars; `MatSharp.Plot` uses `Mat.Re`. The imaginary part is not plotted | From the code, not tested |
| Decimal | No decimal `DType` in NDSharp: MathemaSharp's `decimal` values do not enter the plot without an explicit conversion by the caller | From the code |
| Raster pixels | Skia coordinates in `float` (84 casts in NDSharp.Plot): sub-pixel resolution is not double | From the code |
| SVG | Coordinates with `0.##` (2 decimals of a pixel); labels with `Charts.Num` (e.g. 1234567.891 → `1.2e6`), scientific outside [1e-3, 1e5) | Yes, 2 cases |

Rule: **the precision of the calculation is not that of the drawing.** The drawing is not a source from which to recover the data; whoever must keep integers > 2^53 or exact decimals has to keep them out of the plot path or convert them explicitly.

## 6. Author's decisions (2026-10-29)

- **D-F1 — decided:** only **`net11.0`** is supported. No second target (`net10.0`); the `Ecosystem.Validation(+Tests)` exception remains an exception.
- **D-F2 — decided:** try **all** platforms, **in the future** (R15). Today only win-x64 is declared as tried; Linux/macOS/arm64/musl remain unverified until it is run on those hosts.
- **D-F3 — postponed:** whether to republish NDSharp.Plot with PlotSharp as a dependency (N4) will be decided later. Publication and global installation remain on explicit request.
- **Additional indication:** use **Generic Math where possible** (see §8).

## 8. Generic Math (author's indication: "where possible")

Done on 2026-10-29, only with `System.Numerics` (BCL: PlotSharp stays without dependencies). Constraint `T : INumber<T>`.

| Kernel | Generic form | Existing `double` API |
|---|---|---|
| Contour tracer | `RectilinearContours<T>.Trace(T[], T[], T[], T)` → `Charts.Segment<T>` | Unchanged, delegates to the `double` instance |
| Bilinear sampler | `RectilinearFieldSampler<T>` | Unchanged, delegates |
| Level bands | `ContourBands<T>` | Unchanged, delegates |
| Internal cell/interpolation | `ContourKernel.Trace<T>` (also shared by Navier's `Charts.MarchingSquares`) | `double` |

**Compatibility guarantee:** for `double` the sequence of operations is identical. The existing frozen tests (81-query fixture, pre-migration arithmetic) that remained unchanged and passing show it: PlotSharp 234/234 (227 + 7 new), NDSharp.Plot.Tests 46/46, NavierStokes `selftest` passed (Strouhal 0.188).

**What the caller gains** (`GenericMathKernelTests`): with `decimal` the intersection of 0.1 and 0.3 at level 0.2 falls exactly on 0.15, while with `double` it is 0.15 plus one ulp; `float` and `Half` trace the same geometry within their own precision.

**Behaviour differences for non-IEEE types:** `decimal` has no infinities or NaN. The "non-finite vertex sum" test that skips the cell for `double` raises `OverflowException` for `decimal`. It is documented and tested.

**Where it does NOT apply (reason):**
- *Drawing and colour* (`Heatmap`, `ColorScale`, `ColorPalette`, SVG, Skia): the result is pixels or text anyway; they stay `double`/`float`.
- *`ContourArtist` and NDSharp consumers*: they use `double[]` by contract with PySharp.Matplotlib and MatSharp.Plot; they were not generalised. A caller who wants `decimal` precision can today use the generic kernels directly, but not the artist.
- *Navier overlay* (`double[,]`, limits ±1e12 and 100000 samples): historical contract, unchanged.

## 7. Limits

- Only one host run (Windows x64, runtime 11.0.0 RC3).
- Resolution by RID shows the files present in the package, not that loading works.
- I did not measure the effect of the `float` conversion on the raster.
- The `-net11-preview` tag of the packages was not evaluated against the versioning rules (R16).
