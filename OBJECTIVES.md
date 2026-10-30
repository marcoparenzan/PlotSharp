# PlotSharp — objectives for the new session

## Target
`D:\Dev\2027\repos\PlotSharp`

## Initial state
148 reference copies were collected in `src/imports/<origin>/`: MathemaSharp (54), NDSharp (50), JupyterSharp (29), NavierStokes (15). The relative paths are preserved. `import-manifest.csv` holds provenance and SHA256; `README.md` describes the state. The originals were not modified. The selection is heuristic and does not guarantee all the dependencies. There is not yet a verified active library.

## Objective
Create PlotSharp, a standalone scientific-visualisation library shared by MathemaSharp, NDSharp, JupyterSharp and the NavierStokes examples. First collect and compare, then extract; do not create a fifth duplicated system.

## Phases
1. Inventory the snapshots: API, renderers, dependencies, tests, notebooks, assets and licences. Distinguish original and third-party code. Verify the licences before integrating or distributing.
2. Compare implementations and duplicates. Document which to keep, adapt or discard, with reasons. Identify missing dependencies and collect them only if necessary.
3. Propose a small core: series, axes, legends, colours, styles and layout. Choose the initial renderer after the analysis; SVG/HTML is a suitable candidate for notebooks.
4. Extract a first compilable tranche: lines and scatter, then bars and heatmap. Evaluate contour, quiver and streamlines for NavierStokes, without promising them all in the first version.
5. Separate adapters for notebook, NDArray and Blazor from the core; create separate projects only when needed. PlotSharp must not depend on MathemaSharp's kernels or on the consumer applications.
6. Accept Generic Math data where useful. Make the conversion to graphic coordinates explicit: no premature conversions to double that erase differences in Decimal32/64/128. Normalisation, non-finite values and limits must be documented and tested. Use .NET 11 if the new session allows it and it is available; otherwise declare the actual limit.
7. Create runnable notebooks in the root `notebooks/`, organised by modules/chart types. They are a gallery and documentation, not implementation. The mathematical/application notebooks stay in the projects of origin.
8. Add xUnit tests for logic, coordinate mapping, empty/non-finite data, SVG/HTML escaping, serialisation and export. Run build and tests; verify representative examples visually without declaring checks that were not made.
9. Document installation, usage, reference, architecture and roadmap in the root Markdown files. Record clearly which snapshots are excluded from the solution and which examples were run.

## Desired layout
- `src/`: solution, library, any renderers/adapters, tests and reference snapshots.
- `notebooks/`: curated, runnable examples.
- `ops/`: configuration, build and operational tools.
- Documentation Markdown in the root, not in `docs/`.

## Constraints
- Do not modify the originals during inventory and extraction.
- Do not include all the snapshots indiscriminately in the solution.
- Do not transfer bulky simulation data or build output.
- Do not update the consumers yet: the migration of MathemaSharp/NDSharp/JupyterSharp/NavierStokes will be a later phase, to be authorised.
- Do not declare a library ready without a successful build and tests.

## First expected result
Inventory and documented decisions, a first small compilable library, passing tests, at least one notebook with verified line/scatter and a plan for the following tranches.
