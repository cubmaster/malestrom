# Local Multiplayer Test — REQ-035

## Prerequisites

- Unity 6000.x installed and the `Client/` project opens without errors
- Netcode for GameObjects and Unity Transport packages resolved

## Steps

### 1. Build the Dedicated Server

```powershell
./Scripts/Build-DedicatedServer.ps1
```

This produces `Builds/Server/Windows/IronExilesServer.exe`.

### 2. Launch the Server

```powershell
./Scripts/Run-UnityDedicatedServer.ps1
```

Server listens on `0.0.0.0:7878` by default.

### 3. Launch Client A

Open the `Client/` project in Unity Editor and press Play.
The client auto-connects to `127.0.0.1:7878`.

### 4. Launch Client B

Either:
- Use **Multiplayer Play Mode** (Window > Multiplayer > Play Mode) to add a second virtual player
- Or build a standalone client and run a second instance:
  ```powershell
  ./Scripts/Run-UnityGame.ps1
  ```

### 5. Verify

- Both clients should see **two ships** in the EmptySector scene
- Each client controls only its own ship (cockpit camera + HUD on local ship)
- Remote ships are visible as static cubes at their spawn positions

## CLI Overrides

| Argument | Default | Description |
|----------|---------|-------------|
| `-serverPort` | 7878 | Server listen port |
| `-connectAddress` | 127.0.0.1 | Client connection target |
| `-connectPort` | 7878 | Client connection port |

## Troubleshooting

- **"Connection refused"**: Server not running or wrong port. Check console output.
- **Only one ship visible**: Second client may not have connected. Check server logs for `OnClientConnected`.
- **Both ships at same position**: SpawnPointManager missing from scene or has no spawn points configured.
