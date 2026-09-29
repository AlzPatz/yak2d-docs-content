---
uid: uid_configuration
---

# Configuration

Your application's [Configure()](xref:Yak2D.IApplication.Configure) method returns a [StartupConfig](xref:Yak2D.StartupConfig), which yak2D reads once, before creating the window.

## The quick way

[StartupConfig.Default()](xref:Yak2D.StartupConfig.Default*) fills in sensible values for everything except the window size and title:

```csharp
public StartupConfig Configure()
{
    return StartupConfig.Default(960, 540, "My Game", false); // false = windowed, true = fullscreen
}
```

You can then change any property before returning it:

```csharp
public StartupConfig Configure()
{
    var config = StartupConfig.Default(1280, 720, "My Game", false);
    config.WindowIsResizable = true;
    config.PreferredGraphicsApi = GraphicsApi.Vulkan;
    return config;
}
```

## Every setting

[!code-csharp[](../code/Snippets/Application.cs#full-config)]

| Property | Default() value | What it does |
|---|---|---|
| **Window** | | |
| [WindowTitle](xref:Yak2D.StartupConfig.WindowTitle) | *(parameter)* | Title bar text. Change at runtime with [IDisplay.SetWindowTitle()](xref:Yak2D.IDisplay.SetWindowTitle*). |
| [WindowWidth](xref:Yak2D.StartupConfig.WindowWidth), [WindowHeight](xref:Yak2D.StartupConfig.WindowHeight) | *(parameters)* | Window size in pixels. This is also the size of the window's render target. |
| [WindowPositionX](xref:Yak2D.StartupConfig.WindowPositionX), [WindowPositionY](xref:Yak2D.StartupConfig.WindowPositionY) | 100, 100 | Position of the window's top-left corner on the desktop. |
| [WindowState](xref:Yak2D.StartupConfig.WindowState) | `Normal` or `FullScreen` | [DisplayState](xref:Yak2D.DisplayState): `Normal`, `FullScreen`, `BorderlessFullScreen`, `Maximised`, `Minimised` or `Hidden`. |
| [WindowIsResizable](xref:Yak2D.StartupConfig.WindowIsResizable) | `false` | Whether the user can resize the window by dragging its edges. |
| **Graphics** | | |
| [PreferredGraphicsApi](xref:Yak2D.StartupConfig.PreferredGraphicsApi) | `SystemDefault` | [GraphicsApi](xref:Yak2D.GraphicsApi) to use: `Direct3D11`, `Vulkan`, `OpenGL`, or `SystemDefault`. If the system does not support it, the default choice is made instead. |
| [AvoidVulkanWherePossible](xref:Yak2D.StartupConfig.AvoidVulkanWherePossible) | `true` | With `SystemDefault`, use Vulkan only if nothing else is available. |
| [SyncToVerticalBlank](xref:Yak2D.StartupConfig.SyncToVerticalBlank) | `true` | Vsync: limit frames to the display refresh rate, avoiding tearing. |
| [AutoClearMainWindowColourEachFrame](xref:Yak2D.StartupConfig.AutoClearMainWindowColourEachFrame) | `false` | Clear the window to transparent black at the start of every frame. |
| [AutoClearMainWindowDepthEachFrame](xref:Yak2D.StartupConfig.AutoClearMainWindowDepthEachFrame) | `false` | Clear the window's depth buffer at the start of every frame. |
| **Timing** (see [Application lifecycle](lifecycle.md#update-timing)) | | |
| [UpdatePeriodType](xref:Yak2D.StartupConfig.UpdatePeriodType) | `Fixed` | `Fixed`, `Fixed_Adaptive` or `Variable` update steps. |
| [FixedOrSmallestUpdateTimeStepInSeconds](xref:Yak2D.StartupConfig.FixedOrSmallestUpdateTimeStepInSeconds) | 1/120 | The fixed step length, or the minimum step for `Variable`. |
| [ProcessFractionalUpdatesBeforeDraw](xref:Yak2D.StartupConfig.ProcessFractionalUpdatesBeforeDraw) | `true` | Run a short "catch up" update just before each frame. |
| [RequireAtleastOneUpdatePerDraw](xref:Yak2D.StartupConfig.RequireAtleastOneUpdatePerDraw) | `true` | Skip drawing a frame if nothing has updated since the last one. |
| [FpsCalculationUpdatePeriod](xref:Yak2D.StartupConfig.FpsCalculationUpdatePeriod) | 1 | Seconds over which [IFps](xref:Yak2D.IFps) averages its rates. |
| **Assets** (see [Assets](assets.md)) | | |
| [TextureFolderRootName](xref:Yak2D.StartupConfig.TextureFolderRootName) | `"Textures"` | Folder that texture names are relative to. |
| [FontFolder](xref:Yak2D.StartupConfig.FontFolder) | `"Fonts"` | Folder that font names are relative to. |

## How the graphics API is chosen

1. If `PreferredGraphicsApi` is not `SystemDefault` and the system supports it, it is used.
2. Otherwise the system's default is used (Direct3D 11 on Windows, Vulkan on Linux), unless that is Vulkan and `AvoidVulkanWherePossible` is `true`.
3. Otherwise the first supported API from Direct3D 11, OpenGL, Vulkan is used (skipping Vulkan if avoiding it, unless it is the only option).

The console output at start up says which API was chosen. The choice can be changed while running, see [Window, display and graphics API](window.md#switching-graphics-api).

> [!NOTE]
> yak2D's Metal backend is currently disabled, so on macOS yak2D runs on OpenGL, or on Vulkan via MoltenVK (which is included in the NuGet package).

## Changing things after start up

Most window properties can be changed while the application runs, through [IDisplay](xref:Yak2D.IDisplay) (`yak.Display`): size, fullscreen state, title, vsync, border, opacity, cursor. The window colour and depth auto-clear flags can be changed with [ISurfaces.SetMainWindowRenderTargetAutoClearColour()](xref:Yak2D.ISurfaces.SetMainWindowRenderTargetAutoClearColour*) and [SetMainWindowRenderTargetAutoClearDepth()](xref:Yak2D.ISurfaces.SetMainWindowRenderTargetAutoClearDepth*). The update timing settings and asset folders are fixed at start up.
