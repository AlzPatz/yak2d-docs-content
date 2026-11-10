---
uid: uid_cheatsheet
---

# Cheat sheet

The whole of everyday yak2D on one page. Each section links to the guide that explains it, and every name links to its [API reference](../api/index.md) entry.

## A complete application

Every yak2D program is one class implementing [IApplication](xref:Yak2D.IApplication), passed to [Launcher.Run()](xref:Yak2D.Launcher.Run*). This is a complete program, a good starting point to copy:

```csharp
using System.Numerics;
using Yak2D;

Launcher.Run(new Hello());
```

[!code-csharp[](../code/Snippets/Hello.cs#hello)]

## When each method runs

| Method | Runs | Put here | Gets |
|---|---|---|---|
| `Configure()` | once, first | window size, title, timing ([StartupConfig](configuration.md)) | |
| `OnStartup()` | once | your own (non-yak2D) objects | |
| `CreateResources()` | once, then *by you* after a device reset | textures, stages, cameras, fonts, render targets | `yak` |
| `ProcessMessage()` | per [message](lifecycle.md#framework-messages), before `Update()` | device reset, focus, resize, gamepads | `yak` |
| `Update()` | every step (120 per second by default) | input, movement, game rules. `return false` quits | `yak`, step length |
| `PreDrawing()` | every frame | camera moves, effect settings | `yak` |
| `Drawing()` | every frame | `draw.…` calls: what to draw | `draw`, `fps`, `input`, `transforms` |
| `Rendering()` | every frame | `q.…` calls: how to render it | `q`, the window |
| `Shutdown()` | once, last | save settings, release your own things | |

Full details: [Application lifecycle](lifecycle.md).

## Services (`yak.…`)

| Property | For | Common calls |
|---|---|---|
| [Surfaces](xref:Yak2D.ISurfaces) | textures, render targets | `LoadTexture(name, AssetSourceEnum.Embedded)`, `CreateRenderTarget(w, h)`, `CreateRgbaFromData(…)` |
| [Stages](xref:Yak2D.IStages) | render stages, viewports | `CreateDrawStage()`, `CreateBloomStage(w, h)`, `CreateViewport(x, y, w, h)`, `Set…Config(stage, config, transitionSeconds)` |
| [Cameras](xref:Yak2D.ICameras) | 2D and 3D cameras | `CreateCamera2D(960, 540)`, `SetCamera2DFocusAndZoom(cam, focus, zoom)`, `SetCamera2DFocusZoomAndRotation(…)` |
| [Fonts](xref:Yak2D.IFonts) | bitmap fonts | `LoadFont(name, AssetSourceEnum.Embedded)`, `MeasureStringLength(…)` |
| [Input](xref:Yak2D.IInput) | keyboard, mouse, gamepads | see [Input](#input) below |
| [Display](xref:Yak2D.IDisplay) | the window | `SetDisplayState(…)`, `SetWindowResolution(w, h)`, `SetVsync(on)`, `SetCursorVisible(on)` |
| [Backend](xref:Yak2D.IBackend) | graphics API | `GraphicsApi`, `IsGraphicsApiSupported(api)`, `SetGraphicsApi(api)` |
| [FPS](xref:Yak2D.IFps) | performance | `DrawFPS`, `UpdateFPS` |
| [Helpers](xref:Yak2D.IHelpers) | utilities | `CoordinateTransforms`, `CommonMeshBuilder`, `DistortionHelper` |

## Drawing (in `Drawing()`)

All drawing goes to a [draw stage](drawing.md) and takes a [coordinate space](#coordinate-spaces), a depth and (usually) a layer.

| Draw | Call |
|---|---|
| Rectangle | `draw.Helpers.DrawColouredQuad(stage, space, colour, centre, width, height, depth)` |
| Sprite | `draw.Helpers.DrawTexturedQuad(stage, space, texture, tint, centre, width, height, depth)` |
| Part of a texture (sprite sheet) | `…DrawTexturedQuad(…, texcoord_min_x: …, texcoord_max_x: …)` ([recipe](cookbook.md#sprite-sheet-animation)) |
| Polygon or circle | `draw.Helpers.DrawColouredPoly(stage, space, colour, centre, sides, radius, depth)` |
| Line, arrow | `draw.Helpers.DrawLine(stage, space, from, to, width, colour, depth)`, `DrawArrow(…)` |
| Text | `draw.DrawString(stage, space, text, colour, size, topPosition, TextJustify.Centre, depth, layer)` ([Text](text.md)) |
| Outlines and more shapes | `draw.Helpers.Construct().Coloured(c).Poly(…).Outline(w).SubmitDraw(stage, space, depth, layer)` ([fluent builder](drawing.md#the-fluent-shape-builder)) |
| Your own triangles | `draw.Draw(stage, new DrawRequest { … })` ([draw requests](drawing.md#draw-requests)) |

**Ordering:** higher **layer** is always on top. Within a layer, lower **depth** (0 to 1) is nearer. Positions are the **centre** of a shape, except text, where y is the **top**. Angles are **radians**, clockwise.

## Rendering (in `Rendering()`)

The render queue runs in the order you build it. A render target can be the output of one step and the input of the next.

```csharp
q.ClearColour(target, Colour.Black);       // start each target you render to
q.ClearDepth(target);
q.Draw(drawStage, camera, target);         // draw stage → target, through a 2D camera
q.Bloom(bloom, source, target);            // effects: read a texture, write a target
q.Blur(blur, source, target);              //   also Blur1D, ColourEffects, StyleEffects
q.Distortion(distortion, camera, source, target);
q.Mix(mix, mask, tex0, tex1, tex2, tex3, target);
q.MeshRender(mesh, camera3D, source, target);
q.CustomShader(shader, tex0, tex1, tex2, tex3, target);
q.Copy(source, target);                    // stretch any surface onto a target
q.SetViewport(viewport);                   // render into part of a target...
q.RemoveViewport();                        // ...then the whole target again
```

Details: [The render queue and render stages](renderstages.md), [Post-processing effects](effects.md).

## Input

Check input in **`Update()`**. "This frame" means this update step.

| Question | Keyboard | Mouse | Gamepad |
|---|---|---|---|
| Held now? | `IsKeyCurrentlyPressed(KeyCode.Left)` | `IsMouseCurrentlyPressed(MouseButton.Left)` | `IsGamepadButtonCurrentlyPressed(id, button)` |
| Just pressed? | `WasKeyPressedThisFrame(KeyCode.Space)` | `WasMousePressedThisFrame(…)` | `WasGamepadButtonPressedThisFrame(id, button)` |
| Just released? | `WasKeyReleasedThisFrame(…)` | `WasMouseReleasedThisFrame(…)` | `WasGamepadButtonReleasedThisFrame(id, button)` |
| Held for how long? | `HowLongHasKeyBeenHeldDown(…)` | `HowLongHasMouseBeenHeldDown(…)` | `HowLongHasGamepadButtonBeenHeldDown(id, button)` |
| Where / how far? | | `MousePosition` (window pixels) | `GamepadAxisValue(id, axis)` |

Gamepads: `ConnectedGamepadIds()`. Details: [Input](input.md).

## Coordinate spaces

| Space | Origin | +Y | Units | Used for |
|---|---|---|---|---|
| **World** | world (0,0), wherever the camera looks | up | world units | the game world: moves, zooms and rotates with the camera |
| **Screen** | centre of the camera's view | up | virtual resolution (e.g. 960 x 540) | HUDs and menus: fixed to the view |
| **Window** | top-left of the window | **down** | real pixels | mouse position, viewports |

Convert with `transforms` (in `Drawing()`) or `yak.Helpers.CoordinateTransforms`: `WorldFromWindow(input.MousePosition, camera)` and friends. Details: [Coordinate systems](coordinatesystems.md).

## Blend states

Set per draw stage: `yak.Stages.CreateDrawStage(true, BlendState.AdditiveAlpha)`.

| BlendState | Use for |
|---|---|
| `Alpha` (default) | almost everything |
| `AdditiveAlpha` | light, fire, sparks, glows |
| `AdditiveComponentWise` | accumulating values |
| `Override` | replacing pixels outright |
| `Premultiplied` | drawing a transparent render target over something else (1.2.0+) |

Details: [Drawing: Blend states](drawing.md#blend-states).

## Assets

| Asset | Put it in | Load it with |
|---|---|---|
| Texture | `Textures/hero.png` | `yak.Surfaces.LoadTexture("hero", AssetSourceEnum.Embedded)` |
| Font | `Fonts/pixel_16.fnt` + pages | `yak.Fonts.LoadFont("pixel", AssetSourceEnum.Embedded)` |
| Shader | `Shaders/wavy.glsl` | `yak.Stages.CreateCustomShaderStage("wavy", AssetSourceEnum.Embedded, …)` |

To embed, add `<EmbeddedResource Include="Textures\**" />` (and the same for `Fonts`, `Shaders`) to your `.csproj`. No extension in the name. Details: [Assets](assets.md).

## Remember

- Create **every** yak2D resource in `CreateResources()`, and call it again on `FrameworkMessage.GraphicsDeviceRecreated`. ([Device resets](lifecycle.md#device-resets))
- Draw stages are **emptied every frame** by default: submit everything you want to see, every frame. ([Drawing things once](drawing.md#drawing-things-once))
- Keep a camera's virtual resolution the **same shape** as what it renders to, or the picture stretches. ([Cameras](cameras.md#virtual-resolution))
- Nothing on screen? See [Troubleshooting](troubleshooting.md#nothing-appears).
