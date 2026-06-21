# Local Multiplayer Test — REQ-035

## Prerequisites

- Unity 6000.x installed and the `Client/` project opens without errors
- Netcode for GameObjects and Unity Transport packages resolved

## Steps

### Quick start (Zed / terminal)

```powershell
.\Scripts\Launch-LocalMultiplayerDev.ps1
```

In Zed: **task: spawn** → **Iron Exiles: Local Multiplayer Dev (Server + Unity)**.

This opens the server in a new terminal and launches Unity on EmptySector. Press **Play**, then **Network Session Manager → Connect To Server**.

## Manual workflow

#### 1. Build the Dedicated Server

```powershell
./Scripts/Build-DedicatedServer.ps1
```

This produces `Builds/Server/Windows/IronExilesServer.exe`.

#### 2. Launch the Server

```powershell
./Scripts/Run-UnityDedicatedServer.ps1
```

Server listens on `0.0.0.0:7878` by default.

#### 3. Launch Client A

Open the `Client/` project in Unity Editor and press Play.

By default the editor **does not** auto-connect (so you can test the scene offline without socket errors).

To join the server from step 2, either:
- Enable **Auto Connect In Editor** on `EmptySectorMultiplayerBootstrap` in the scene, then press Play; or
- Press Play, then on the runtime `NetworkSession` object use **Network Session Manager > Connect To Server** from the component context menu.

#### 4. Launch Client B

Either:
- Use **Multiplayer Play Mode** (Window > Multiplayer > Play Mode) to add a second virtual player
- Or build a standalone client and run a second instance:
  ```powershell
  ./Scripts/Run-UnityGame.ps1
  ```

#### 5. Verify

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

- **"Dedicated Server support for Win is not installed"**: The build script auto-installs the `windows-server` module via Unity Hub on first run. You can also install it manually in Unity Hub → Installs → 6000.4.11f1 → Add modules → **Windows Dedicated Server Build Support**. The project also falls back to a headless player build if the module is unavailable.
- **"All socket receive requests were marked as failed"**: The editor client tried to connect with no server on `127.0.0.1:7878`. Start `Run-UnityDedicatedServer.ps1` first, or leave **Auto Connect In Editor** off for offline play.
- **"Connection refused"**: Server not running or wrong port. Check console output.
- **Only one ship visible**: Second client may not have connected. Check server logs for `OnClientConnected`.
- **Both ships at same position**: SpawnPointManager missing from scene or has no spawn points configured.
