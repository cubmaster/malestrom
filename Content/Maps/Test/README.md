# EmptySector test map

The playable map asset lives at `/Game/Maps/Test/EmptySector` (Git LFS).

Generate it once after the UE5 project compiles:

```powershell
$env:UE_ROOT = "C:\Program Files\Epic Games\UE_5.5"
.\Scripts\Initialize-Content.ps1
```

Or open the project in the editor and save a new level to this path.
