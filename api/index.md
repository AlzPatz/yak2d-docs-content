# API Reference

This reference covers every public type in yak2D. It is generated from the comments in the [source code](https://github.com/AlzPatz/yak2d) every time yak2D is released, so it always matches the current NuGet package.

## Where to start

Almost everything is reached through a few entry points:

| Type | What it is |
|---|---|
| [Launcher](xref:Yak2D.Launcher) | Starts yak2D: `Launcher.Run(new MyGame())`. |
| [IApplication](xref:Yak2D.IApplication) | The interface your game implements. |
| [StartupConfig](xref:Yak2D.StartupConfig) | Window, graphics and timing settings, returned from `Configure()`. |
| [IServices](xref:Yak2D.IServices) | Every yak2D service, passed to most `IApplication` methods as `yak`. |
| [IDrawing](xref:Yak2D.IDrawing) | Submitting draw requests and text, in `Drawing()`. |
| [IDrawingHelpers](xref:Yak2D.IDrawingHelpers) | Quads, polygons, lines, arrows and the fluent shape builder (`draw.Helpers`). |
| [IRenderQueue](xref:Yak2D.IRenderQueue) | Building the frame's rendering, in `Rendering()`. |

## The services

| Service | Access | For |
|---|---|---|
| [ISurfaces](xref:Yak2D.ISurfaces) | `yak.Surfaces` | textures and render targets |
| [IStages](xref:Yak2D.IStages) | `yak.Stages` | render stages, their configuration, and viewports |
| [ICameras](xref:Yak2D.ICameras) | `yak.Cameras` | 2D and 3D cameras |
| [IFonts](xref:Yak2D.IFonts) | `yak.Fonts` | loading fonts, measuring text |
| [IInput](xref:Yak2D.IInput) | `yak.Input` | keyboard, mouse and gamepads |
| [IDisplay](xref:Yak2D.IDisplay) | `yak.Display` | the window |
| [IBackend](xref:Yak2D.IBackend) | `yak.Backend` | the graphics API |
| [IFps](xref:Yak2D.IFps) | `yak.FPS` | update and draw rates |
| [IHelpers](xref:Yak2D.IHelpers) | `yak.Helpers` | coordinate transforms, mesh builders, distortion helpers |

Most methods that take a reference (such as an [ITexture](xref:Yak2D.ITexture)) also have an overload taking the raw `ulong` id instead. See [Resources and references](../articles/resources.md).

For explanations and examples, see the [Manual](../articles/intro.md).
