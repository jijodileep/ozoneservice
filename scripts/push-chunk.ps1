# Push completed chunk work to GitHub.
# Usage: .\scripts\push-chunk.ps1 -Chunk "03" -Message "Chunk 03: Auth API"

param(
    [Parameter(Mandatory = $true)]
    [string]$Chunk,

    [Parameter(Mandatory = $true)]
    [string]$Message
)

$ErrorActionPreference = "Stop"
$root = Split-Path -Parent $PSScriptRoot
Set-Location $root

git add -A
$status = git status --porcelain
if (-not $status) {
    Write-Host "Nothing to commit for chunk $Chunk."
    exit 0
}

git commit -m $Message
git push origin HEAD
Write-Host "Chunk $Chunk pushed to origin."
