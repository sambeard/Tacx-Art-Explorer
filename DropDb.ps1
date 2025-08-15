# v1-safe, order-agnostic args: [-DryRun] [Debug|Release] [-Config <val>] [-c <val>]
$Config = "Debug"
$DryRun = $false

for ($i = 0; $i -lt $args.Length; $i++) {
    $a = $args[$i]
    $al = $a.ToLower()

    if ($al -eq "-dryrun") { $DryRun = $true; continue }
    if ($al -eq "-config" -or $al -eq "-c") {
        if ($i + 1 -lt $args.Length) { $Config = $args[$i + 1]; $i++; continue }
    }
    if ($a -notmatch '^-') { $Config = $a; continue }
}

$scriptDir  = Split-Path -Parent $MyInvocation.MyCommand.Path
$projectDir = Join-Path $scriptDir "TacxArtExplorer"
$binDir     = Join-Path $projectDir ("bin\" + $Config)

Write-Host "DropDb starting (Config=$Config, DryRun=$DryRun)"
Write-Host "Looking under: $binDir"

$dataDirs = Get-ChildItem -Path $binDir -Directory -Recurse -Force -ErrorAction SilentlyContinue |
            Where-Object { $_.Name -eq "Data" }

if (-not $dataDirs) { Write-Host "No Data directories found."; exit 0 }

foreach ($dir in $dataDirs) {
    Write-Host "Deleting $($dir.FullName)"
    $files = Get-ChildItem -Path $dir.FullName -Recurse -Force
    foreach ($f in $files) {
        Write-Host " - $($f.FullName)"
        if (-not $DryRun) { Remove-Item -LiteralPath $f.FullName -Force }
    }
    if (-not $DryRun) { Remove-Item -LiteralPath $dir.FullName -Force }
}
Write-Host "Done."
