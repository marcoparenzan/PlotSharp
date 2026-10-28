$ErrorActionPreference = 'Stop'
$target = 'D:\Dev\2027\repos\PlotSharp'
New-Item -ItemType Directory -Force $target | Out-Null
$sources = [ordered]@{ MathemaSharp='D:\Dev\2027\repos\MathemaSharp'; NDSharp='D:\Dev\2026\repos\NDSharp'; JupyterSharp='D:\Dev\2026\repos\JupyterSharp'; NavierStokes='D:\Dev\2026\articoli\2026.10 NavierStokes' }
$manifest = @()
foreach ($entry in $sources.GetEnumerator()) {
 if (!(Test-Path $entry.Value)) { Write-Output "MISSING: $($entry.Value)"; continue }
 $files = Get-ChildItem $entry.Value -Recurse -File | Where-Object { $_.FullName -notmatch '[\\/](bin|obj|\.git|node_modules|artifacts|imports|publish)[\\/]' }
 $selected = $files | Where-Object {
  $_.FullName.Substring($entry.Value.Length) -match '(?i)(chart|plot|visualiz)' -or $_.Name -match '(?i)^(license|copying)' -or $_.Extension -in '.ipynb','.csproj' -or ($_.Extension -in '.cs','.razor','.js','.html','.csx','.py','.md' -and (Select-String -LiteralPath $_.FullName -Pattern 'chart|plotly|matplotlib|ScottPlot|OxyPlot|quiver|streamplot' -Quiet))
 }
 foreach ($file in $selected) {
  $relative = $file.FullName.Substring($entry.Value.Length).TrimStart('\')
  $dest = Join-Path "$target\src\imports\$($entry.Key)" $relative
  if (Test-Path $dest) { throw "Refusing to overwrite $dest" }
  New-Item -ItemType Directory -Force (Split-Path $dest -Parent) | Out-Null
  Copy-Item -LiteralPath $file.FullName -Destination $dest
  $manifest += [pscustomobject]@{Source=$entry.Key; Original=$file.FullName; Imported=$dest.Substring($target.Length+1); SHA256=(Get-FileHash $dest -Algorithm SHA256).Hash}
 }
 Write-Output "$($entry.Key): $(@($selected).Count) files copied"
}
$manifest | Export-Csv "$target\import-manifest.csv" -NoTypeInformation -Encoding utf8
foreach ($dir in 'notebooks','ops','src/PlotSharp') { New-Item -ItemType Directory -Force (Join-Path $target $dir) | Out-Null }
@'
# PlotSharp

Scientific visualization library: initial collection of chart implementations from MathemaSharp, NDSharp, JupyterSharp and NavierStokes.

Imported source snapshots are in `src/imports/<origin>`, preserving original relative paths. `import-manifest.csv` records provenance and SHA256 hashes. Originals were not modified. Selection includes chart/plot paths, matching source files, notebooks, project files and licenses; dependencies may still need collecting.

Snapshots are reference material, not active projects: no compilation or API compatibility is claimed. Notebook snapshots remain with their source; curated runnable examples will live in root `notebooks/`.

Next: review licenses and dependencies, compare rendering approaches, extract shared kernels, add tests and a notebook gallery. Consumers are not updated yet.
'@ | Set-Content "$target\README.md" -Encoding utf8
Write-Output "Manifest entries: $($manifest.Count). Target: $target"
