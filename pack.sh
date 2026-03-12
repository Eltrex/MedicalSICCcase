#!/bin/bash
set -e  # Exit immediately if a command exits with a non-zero status

# Paths
root="$(dirname "$(realpath "$0")")"
proj="$root/src/MedicalSICCcase.csproj"
outDir="$root/src/bin/Release/MedicalSICCcase"
dist="$root/dist/MedicalSICCcase"

echo "[pack] Building Release..."
dotnet build "$proj" -c Release

if [ ! -d "$outDir" ]; then
  echo "Output directory not found: $outDir"
  exit 1
fi

echo "[pack] Preparing dist folder..."
if [ -d "$dist" ]; then
  rm -rf "$dist"
fi
mkdir -p "$dist"

echo "[pack] Copying DLL..."
cp "$outDir/MedicalSICCcase.dll" "$dist/"

# Uncomment the following lines if you want to copy the PDB file
# if [ -f "$outDir/MedicalSICCcase.pdb" ]; then
#   cp "$outDir/MedicalSICCcase.pdb" "$dist/"
# fi

echo "[pack] Copying config..."
builtConfig="$outDir/config"
repoConfig="$root/config"
if [ -d "$builtConfig" ]; then
  cp -r "$builtConfig" "$dist/"
elif [ -d "$repoConfig" ]; then
  cp -r "$repoConfig" "$dist/"
fi

echo "[pack] Copying bundles..."
builtBundles="$outDir/bundles"
repoBundles="$root/bundles"
if [ -d "$builtBundles" ]; then
  cp -r "$builtBundles" "$dist/"
elif [ -d "$repoBundles" ]; then
  cp -r "$repoBundles" "$dist/"
fi

echo "[pack] Copying bundles.json..."
bundlesJson="$root/bundles.json"
if [ -f "$bundlesJson" ]; then
  cp "$bundlesJson" "$dist/"
fi

echo "[pack] Done. Package at: $dist"
