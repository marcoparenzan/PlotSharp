# Public contracts of the shared kernels (R05 / N3)

State as of 2026-10-29. Describes the behaviour **observed and covered by tests**, not an ideal behaviour. Where it differs from matplotlib this is indicated; where the choice belongs to the author it is marked **DECIDE**. Sources: `RectilinearContours`, `RectilinearFieldSampler`, `ContourBands`, `Charts.MarchingSquares` in this repository; `ContourArtist` in `NDSharp/src/NDSharp.Plot/Contour.cs`.

## 1. Coordinate conventions

| Kernel | Convention | Notes |
|---|---|---|
| `RectilinearContours.Trace(xs, ys, z, level)` | **Nodes** in **physical** coordinates. `z` row-major, `ys.Length` rows × `xs.Length` columns. The segment interpolates linearly on the physical side of the cell. | A single column/row has no cells: empty result. |
| `RectilinearFieldSampler.Sample(x, y)` | **Bilinear** interpolation on a physical cell; abscissa in `xs`, ordinate in `ys`. | Outside the domain the cell is clamped to the nearest one and the weight **extrapolates linearly** (it does not saturate). |
| `Charts.MarchingSquares(field, …)` (Navier) | **Sample indices**: x = column, y = row. | No physical coordinates; the consumer projects. |
| `ContourBands.Find` | Value → band index, inclusive extremes. | See §4. |

The three ND kernels also have the generic `INumber<T>` form (`RectilinearContours<T>`, `RectilinearFieldSampler<T>`, `ContourBands<T>`) with the **same contracts** as this document; for `decimal` (without NaN/Inf) overflow of the vertex sum throws `OverflowException` instead of skipping the cell. Details in `PIATTAFORME-PRECISIONE.md` §8.

## 2. Non-finite values and masks

| Case | `RectilinearContours` (ND) | `Charts.MarchingSquares` (Navier) | `ContourArtist` lines | `ContourArtist` filled |
|---|---|---|---|---|
| NaN/Inf in `z` | The cell is **skipped** if the sum of the four vertices is not finite (also for overflow of finite values); finite neighbours are drawn | Rejected (`ArgumentException`) if not masked | As ND | The NaN sample does not paint; `+Inf` tolerated |
| Z entirely non-finite | Empty | Rejected | **Draws nothing** (before: accidental `InvalidOperationException`, fixed on 2026-10-29) | Draws nothing |
| Mask | Not supported | `bool[,]`: cell skipped if one of the 4 vertices is masked; dimensions equal to the field | Not supported | Not supported |
| Non-finite coordinates | `ArgumentException` | n/a | `ArgumentException` | `ArgumentException` |
| NaN level | `ArgumentOutOfRangeException` | `ArgumentOutOfRangeException` | `ArgumentOutOfRangeException` | `ArgumentException` (from `ContourBands`) |
| `+Inf` level | `ArgumentOutOfRangeException` | same | **Silently skipped** (outside the data range) | `ArgumentException` |

## 3. Dimensional and numerical limits

| Path | Limit |
|---|---|
| ND (`RectilinearContours`, `RectilinearFieldSampler`) | No cap on dimensions or magnitude of values. The filled path allocates `width × height × 4` bytes equal to the axes box: the practical limit is the size of the figure. |
| Navier (`MarchingSquares`) | 1 … 100000 samples; unmasked values finite and within ±1e12; level within ±1e12. |
| Heatmap overlay | At most 32 levels, finite, within ±1e12; colour `#rrggbb`; vectors: scale 0 < s ≤ 1e6, stride 1…100000, projection within ±1e7 pixels. |

The Navier restrictions have **not** been extended to the ND path: the two consumers keep their own historical limits.

## 4. Order of axes and levels

| Element | Lines (tracer) | Filled (sampler + bands) | matplotlib |
|---|---|---|---|
| Axes `xs`, `ys` | Any order, **even decreasing or with repetitions** (finite) | **Strictly increasing**, otherwise `ArgumentException` | Accepts decreasing and repeated axes, both `contour` and `contourf` (verified: no error) |
| At least 2×2 | No (1 column → empty) | Yes | Yes |
| Levels | Any order, repeats allowed, even empty | **Non-decreasing**, finite, at least one; repeats allowed | **Strictly increasing** (`ValueError`) |
| A single level | Allowed | Allowed: no band, paints nothing | n/a |

Band rules (`ContourBands.Find`): inclusive extremes, so a value exactly on an inner level belongs to the **upper** band; the value equal to the last level belongs to the last band; with repeated levels **the last matching boundary** wins. In the filled path a zero-width band (repeated level) **consumes a colour index**: with colours `[Blue, Red, Green]` and levels `[0,1,1,2]` the upper band is green.

Band colour: midpoint of the limits, normalised over the whole range of levels (like matplotlib); `hi == lo` → 0. Line colour: level normalised over the range of levels.

## 5. Rounding and arithmetic

Preserved to the letter with respect to the original code, with exact non-regression tests:

- Intersection: `t = (level - v1) / (v2 - v1)`, point `p1 + t·(p2 - p1)`; side crossed if `(v1 < level) != (v2 < level)`.
- Saddle: the intersection points are paired in fixed order (0–1, 2–3). Order of the sides: ND forward (`d→c`, `a→d`), Navier reverse (`c→d`, `d→a`).
- Vertices exactly at the level: degenerate segments (zero length) are **preserved** by `RectilinearContours` and by the Navier compatibility mode; `ContourArtist` excludes the levels at the observed extremes of the data (`lv <= zmin` or `lv >= zmax`).
- Sampler: four terms `z00(1-tx)(1-ty) + z01·tx(1-ty) + z10(1-tx)·ty + z11·tx·ty`, **without reassociation**; overflow propagates as in the original.
- Band midpoint: `(a + b) / 2`, then `(mid - lo) / (hi - lo)`; overflow/NaN pass to the caller.
- Automatic levels (`ContourArtist.AutoLevels(zmin, zmax, n)`): identical to matplotlib 3.11.2's `MaxNLocator(n + 1, min_n_ticks=1)` on 7 verified cases.

## 6. `ContourArtist` defaults

Colormap `viridis`; `LineWidth` 1.5; style `-`, **dashed `--` for negative levels only with a single colour** (`Colors` of length 1), like matplotlib; `Colors` shorter than the levels: cyclic; `Alpha` scales the transparency of the fill (verified on the composite pixel over a white background).

## 7. Accepted differences from matplotlib

Verified with matplotlib 3.11.2 (real execution, only presence/absence of an exception; I did not compare what is drawn):

1. **Non-increasing levels:** matplotlib rejects them (`ValueError`) in lines and filled; NDSharp accepts them in lines and rejects only decreasing ones in filled.
2. **Decreasing or repeated axes:** matplotlib accepts them everywhere; NDSharp accepts them in lines but **filled rejects them**. Here NDSharp is *more restrictive*.
3. **NaN and `+Inf` levels:** matplotlib raises no exception; NDSharp raises for NaN (lines) and for every non-finite level (filled), and ignores `+Inf` in lines.
4. **Z entirely non-finite:** matplotlib does not raise; NDSharp draws nothing (an accidental crash in lines was fixed).

**DECIDED 2026-10-29: option (c)** — leave everything as it is and declare the differences. The options evaluated were (no change until you confirm): (a) reject non-increasing levels also in lines and filled, like matplotlib; (b) accept decreasing axes in filled (requires sorting/reflecting the sample, a kernel change with a frozen fixture); (c) leave everything as it is and declare the differences. Aligning with matplotlib on levels is a **new restriction** for those who pass them to lines today.

## 8. Coverage

| Contract | Test |
|---|---|
| Tracer, saddle, vertices, overflow, decreasing axes, singletons, malformed inputs | `PlotSharp.Tests/RectilinearContourTests.cs` (6) |
| Sampler, frozen fixture of 81 queries, extrapolation, snapshot, rejections | `RectilinearFieldSamplerTests.cs` |
| Bands, extremes, repeats, midpoint, overflow, snapshot, rejections | `ContourBandsTests.cs` |
| Automatic levels against matplotlib, alpha, repeated levels, non-finite in filled | `NDSharp.Plot.Tests/ContourBaselineTests.cs` |
| Lines/filled differences of §2 and §4, "draws nothing" compared byte by byte with the empty chart | `NDSharp.Plot.Tests/ContourContractTests.cs` |

## 9. Not covered

- Numerical limits of the ND path are not tested beyond the overflow of the vertex sum.
- I did not measure the time or memory of the filled path for very large figures.
- The comparison with matplotlib is limited to the automatic levels; segment geometry and band colours are not compared with the oracle in this tranche.
- Quiver and 3D have no written contracts (R14).
