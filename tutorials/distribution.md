---
uid: uid_tut_distribution
---

# Distributing your game

Once your game runs with `dotnet run`, you will want to give it to people who don't have the .NET SDK. This tutorial packages a game (here, [Yak Run](yakrun-1.md)) for Windows, Linux and macOS.

## Publish

`dotnet publish` builds a ready-to-run folder. You can build for any platform from any platform, by choosing a **runtime identifier (RID)**:

| Platform | RID |
|---|---|
| Windows (64 bit) | `win-x64` (or `win-arm64`) |
| Linux (64 bit) | `linux-x64` (or `linux-arm64`) |
| macOS on Apple Silicon | `osx-arm64` |
| macOS on Intel | `osx-x64` |

```bash
dotnet publish -c Release -r win-x64 --self-contained -o publish/windows
dotnet publish -c Release -r linux-x64 --self-contained -o publish/linux
dotnet publish -c Release -r osx-arm64 --self-contained -o publish/macos
```

`--self-contained` includes the .NET runtime, so players need nothing installed. The yak2D NuGet package brings the right native libraries for each platform (SDL2, the shader compiler, and MoltenVK on macOS), and `publish` copies them into the folder automatically. Each folder is around 140 MB; zipped, much less.

Zip the folder and share it. The player unzips it and runs `YakRun.exe` (Windows) or `YakRun` (Linux and macOS).

### Smaller downloads

- **Framework-dependent**: leave out `--self-contained`. The download is only a few MB, but players must install the [.NET 10 runtime](https://dotnet.microsoft.com/download) first.
- **Fewer files**: add `-p:PublishSingleFile=true`. The .NET parts are combined into the executable, leaving it plus a handful of native libraries (`libSDL2-2.0.so`, `libshaderc_shared.so`, `libspirv-cross.so` on Linux, and their equivalents elsewhere). Keep those next to the executable.

> [!WARNING]
> Do **not** add `-p:IncludeNativeLibrariesForSelfExtract=true` to squeeze everything into one file: the game then fails at start up because SDL2 cannot be found. Keep the native libraries as separate files.

Trimming (`-p:PublishTrimmed=true`) is not supported: yak2D uses reflection internally, which trimming can break.

## Assets

If your assets are **embedded** (as in the tutorials), they are inside `YakRun.dll`, and there is nothing else to ship. If you load assets as **files**, make sure they are copied to the output (see [Assets](../articles/assets.md#files-on-disk)), and set the working directory at start up, because players will not always start your game from its own folder:

```csharp
Directory.SetCurrentDirectory(AppContext.BaseDirectory);
Launcher.Run(new Game());
```

## Windows

yak2D applications are console applications, so a console window opens alongside the game. To hide it, change the output type in your `.csproj`:

```xml
<OutputType>WinExe</OutputType>
```

Framework messages are then no longer visible; if you want to keep them, pass your own [IFrameworkMessenger](../articles/lifecycle.md#framework-console-messages) that writes them to a log file.

To set the executable's icon, add `<ApplicationIcon>icon.ico</ApplicationIcon>`.

## Linux

The published executable needs to be marked as executable after unzipping (`chmod +x YakRun`); archiving with `tar` preserves this, while some zip tools do not:

```bash
tar -czf YakRun-linux-x64.tar.gz -C publish/linux .
```

For desktop integration (a menu entry and icon), add a `.desktop` file. For wider distribution, tools such as AppImage or Flatpak can wrap the published folder. Players need working OpenGL or Vulkan drivers, which desktop Linux distributions normally have.

## macOS

A published folder runs from the terminal. For a double-clickable application, put it inside an **app bundle**:

```text
YakRun.app/
└── Contents/
    ├── Info.plist
    ├── MacOS/
    │   └── (everything from publish/macos)
    └── Resources/
        └── YakRun.icns
```

`Info.plist` needs at least `CFBundleExecutable` (`YakRun`), `CFBundleIdentifier` (e.g. `com.example.yakrun`), `CFBundleName` and `CFBundlePackageType` (`APPL`).

Apps downloaded from the internet must be **signed and notarised** with an Apple Developer ID before macOS will open them normally; otherwise players have to allow the app explicitly in System Settings, under Privacy & Security. See Apple's documentation on notarising macOS software.

> [!NOTE]
> macOS support in yak2D is less thoroughly tested than Windows and Linux, and the steps above have not been tested on a Mac as part of writing this tutorial. Please [report problems](https://github.com/AlzPatz/yak2d/issues).

## Checklist

- Build with `-c Release`.
- Test the published build on each platform you ship for, ideally on a machine without the .NET SDK.
- Run it from a different folder, to check your assets are found.
- Try it on OpenGL and Vulkan (set `PreferredGraphicsApi`) if you are unsure which your players will get.
