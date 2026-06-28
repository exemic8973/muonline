# MuOnline Clone — Reasonix quick-reference

## Stack

- **C# / .NET 10** — target framework `net10.0`
- **MonoGame 3.8+** — DesktopGL and WindowsDX variants; content pipeline via `dotnet mgcb`
- **MUnique.OpenMU.Network** — Season 6 protocol packet definitions
- **Pipelines.Sockets.Unofficial** — async socket networking
- **Microsoft.Extensions.Logging / Configuration / DI** — structured logging, JSON config, service injection

## Layout

| Dir | What |
|---|---|
| `Client.Main/` | Core engine library: scenes, controls (UI/terrain), networking (packet router/handlers/services), object model, rendering, content loading |
| `Client.Data/` | MU proprietary format readers: BMD (models), ATT (terrain attrs), MAP, OZB/OZG, CWS, OBJS, textures, LANG, CAP, ModulusCryptor |
| `Client.Editor/` | Windows Forms asset editor (not needed for runtime) |
| `MuWinDX/` `MuWinGL/` `MuLinux/` `MuMac/` `MuAndroid/` `MuIos/` | Platform heads — each is a runnable project referencing `Client.Main` |

## Commands

```sh
# Build + run (OpenGL)
dotnet run --project MuWinGL/MuWinGL.csproj

# Build + run (DirectX — Windows only)
dotnet run --project MuWinDX/MuWinDX.csproj -p:MonoGameFramework=MonoGame.Framework.WindowsDX

# Tool restore (needed once before any build that uses content pipeline)
dotnet tool restore

# Publish release (example: Windows DX)
dotnet publish MuWinDX/MuWinDX.csproj -c Release -r win-x64 -o publish -p:MonoGameFramework=MonoGame.Framework.WindowsDX
```

No test project, no linter/formatter config found in the repo.

## Conventions

- **Packet handlers** — registered via `[PacketHandler(mainCode, subCode)]` attribute on methods in `Networking/PacketHandling/Handlers/`
- **World setup** — each world is a class in `Objects/Worlds/` named `<Name>World.cs`, decorated with `[WorldInfo(index, name)]`
- **Constants-driven tuning** — rendering/UI/behavior defaults live in `Client.Main/Constants.cs` (not inline literals)
- **Commit messages** — concise present-tense, e.g. `"bonfire improvements - sparks and smoke"` (from AGENTS.md)
- **UI from main thread only** — network/background work must marshal via `MuGame.ScheduleOnMainThread`
- **Data assets** — loaded via `Constants.DataPath` concat with relative path; all asset readers live in `Client.Data/`

## Watch out for

- **`-p:MonoGameFramework=` is required on Windows.** Without it the wrong MonoGame package resolves and shaders / DX vs GL references break.
- **`DataPath` must point to local MU Season 20 data.** Debug heads hardcode `C:\Games\MU_Red_1_20_61_Full\Data` in `Program.cs`; Release uses relative `Data/`. Missing assets = blank screen.
- **Two backends (DX + GL).** Test rendering changes on both. The `MonoGameFramework` MSBuild property flips which package is restored.
- **`AllowUnsafeBlocks true** throughout — native memory access for BMD mesh data and texture decoding.
- **Network → UI thread crossing crashes.** Packet handlers run on thread-pool threads; never touch controls or scene objects directly from a handler.
