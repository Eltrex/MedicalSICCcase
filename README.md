# Medical SICC Case (C# Server Mod)

A server-side SPT 4.0 mod that adds a configurable Medical SICC case and sells it at Therapist for roubles.

Converted from the original TypeScript mod (SPT <= 3.11) to a C# server mod for SPT 4.0.

## What It Does
- Adds a new Medical SICC case cloned from the base SICC case
- Sells the case at Therapist for roubles
- Uses a configurable loyalty level requirement
- Lets you set the case grid size (columns × rows)
- Dynamically applies grid width/height from config on server load

## Configure
- File: `config/config.json` (next to the compiled DLL; keys are case-insensitive)
- Keys:
	- `price`: Rouble price at Therapist; also used for handbook/flea price.
	- `loyalty_lvl`: Therapist loyalty level required (1–4).
	- `cellH`: Grid width (columns) of the case.
	- `cellV`: Grid height (rows) of the case.
	- `ItemWidth`: Grid width in the inventory of the case.
	- `ItemHeight`: Grid height in the inventory of the case.
	- `AllowMedBarter`: If Med Barter Items should be allowed in the case. (true/false)
	- `containers`: A list of containers that allow the MICC to be placed into.

Example:
```json
{
	"price": 350000,
	"loyalty_lvl": 2,
	"cellH": 5,
	"cellV": 5,
	"ItemWidth": 2,
	"ItemHeight": 1,
	"AllowMedBarter": true,
	"containers": [
		"566abbc34bdc2d92178b4576",
		"5857a8b324597729ab0a0e7d"
	]
}
```

## Build From Source
1. Install .NET 9 SDK.
2. Open folder `src` in IDE or a terminal.
3. Restore + build Release.

### CLI
```powershell
cd src
dotnet restore
dotnet build -c Release
```

Build output: `src\\bin\\Release\\MedicalSICCcase\\MedicalSICCcase`.

## Package & Deploy
### Package
Use the packaging script from the workspace root to assemble a deployable folder:

```powershell
powershell -ExecutionPolicy Bypass -File .\pack.ps1
```

This creates `dist/MedicalSICCcase` containing the compiled DLL, PDB, and `config/`.

### Deploy
Copy the folder `dist/MedicalSICCcase` into your SPT server's server-mods directory. Ensure the folder contains the compiled DLL + `config/`.

Notes on server paths (relative to your SPT server root):
- SPT server mods: `SPT_Data/Server/mods`

Place `MedicalSICCcase` as a child folder under the appropriate `mods` directory.

## Config
The mod reads `config/config.json` at runtime (next to the DLL). See Configure section for details.

## Notes
- Requires SPTarkov packages 4.0.X.

## Troubleshooting
- If item not visible: check server logs for Medical SICC case load messages.

## License
This project is licensed under the University of Illinois/NCSA Open Source License. See `../LICENSE`.
