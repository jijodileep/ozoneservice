# Run at the BEGINNING of each chunk (when you say "start chunk NN").
# Commits and pushes a chunk-start marker — not at the end of the chunk.
# Usage: .\scripts\start-chunk.ps1 -Chunk "04" -Title "Multi-tenancy"

param(
    [Parameter(Mandatory = $true)]
    [string]$Chunk,

    [Parameter(Mandatory = $true)]
    [string]$Title
)

$ErrorActionPreference = "Stop"
$root = Split-Path -Parent $PSScriptRoot
Set-Location $root

$markerDir = Join-Path $root "plans"
$markerFile = Join-Path $markerDir ".current-chunk"
$chunkId = $Chunk.PadLeft(2, '0')

$previous = $null
if (Test-Path $markerFile) {
    $previous = Get-Content $markerFile -Raw
}

Set-Content -Path $markerFile -Value "chunk-$chunkId" -NoNewline

git add -A
$status = git status --porcelain

if ($status) {
    if ($previous) {
        git commit -m "Complete $previous (committed at start of chunk-$chunkId)"
    }
    else {
        git commit -m "Begin chunk-${chunkId}: $Title"
    }
    git push origin HEAD
    Write-Host "Committed and pushed at chunk-$chunkId start."
}
else {
    Write-Host "No file changes to commit. Marker updated to chunk-$chunkId."
}

Write-Host "Now implement chunk-${chunkId}: $Title"
