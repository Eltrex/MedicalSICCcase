# Build and package the server mod into dist/ for easy deployment
$ErrorActionPreference = 'Stop'

# Paths
$root = Split-Path -Parent $MyInvocation.MyCommand.Path
$proj = Join-Path $root 'src/MedicalSICCcase.csproj'
$outDir = Join-Path $root 'src/bin/Release/MedicalSICCcase'
$dist = Join-Path $root 'dist/MedicalSICCcase'

Write-Host "[pack] Building Release..."
dotnet build $proj -c Release | Out-Host

if (-not (Test-Path $outDir)) {
  throw "Output directory not found: $outDir"
}

Write-Host "[pack] Preparing dist folder..."
if (Test-Path $dist) { Remove-Item -Recurse -Force $dist }
New-Item -ItemType Directory -Path $dist | Out-Null

Write-Host "[pack] Copying DLL and PDB..."
Copy-Item (Join-Path $outDir 'MedicalSICCcase.dll') -Destination $dist -Force
#if (Test-Path (Join-Path $outDir 'MedicalSICCcase.pdb')) { Copy-Item (Join-Path $outDir 'MedicalSICCcase.pdb') -Destination $dist -Force }

Write-Host "[pack] Copying config..."
$builtConfig = Join-Path $outDir 'config'
$repoConfig = Join-Path $root 'config'
if (Test-Path $builtConfig) {
  Copy-Item $builtConfig -Destination (Join-Path $dist 'config') -Recurse -Force
} elseif (Test-Path $repoConfig) {
  Copy-Item $repoConfig -Destination (Join-Path $dist 'config') -Recurse -Force
}

Write-Host "[pack] Copying bundles..."
$builtConfig = Join-Path $outDir 'bundles'
$repoConfig = Join-Path $root 'bundles'
if (Test-Path $builtConfig) {
  Copy-Item $builtConfig -Destination (Join-Path $dist 'bundles') -Recurse -Force
} elseif (Test-Path $repoConfig) {
  Copy-Item $repoConfig -Destination (Join-Path $dist 'bundles') -Recurse -Force
}

Write-Host "[pack] Copying bundles.json..."
$bundlesJson = Join-Path $root 'bundles.json'
if (Test-Path $bundlesJson) {
  Copy-Item $bundlesJson -Destination (Join-Path $dist 'bundles.json') -Force
}

Write-Host "[pack] Done. Package at: $dist"