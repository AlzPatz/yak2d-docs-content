---
uid: uid_troubleshooting
---

# Troubleshooting and FAQ

**Start with the console.** yak2D writes what it is doing, and what went wrong, to the console: which graphics API was chosen, assets it could not find, invalid requests, and so on. Most problems are explained there.

## Nothing appears

- **Did you render the draw stage?** Submitting draw requests is not enough; `Rendering()` must call `q.Draw(stage, camera, target)`. See [How yak2D works](overview.md#drawing-and-rendering-are-two-different-steps).
- **Is it inside the camera's view?** In screen space the visible area is plus and minus half the camera's virtual resolution. In world space it also depends on the camera's focus and zoom.
- **Is it hidden behind something?** Check layers and depths: higher layers are on top; within a layer, lower depth is in front. When two draw stages render onto the same surface, clear the depth between them. See [Layers and depth](drawing.md#layers-and-depth).
- **Is it transparent?** A colour with alpha 0 draws nothing. That includes `default(Colour)`, and any [Vertex2D](xref:Yak2D.Vertex2D) whose `Colour` was not set, even in a textured request: vertex colours multiply the texture, and the default is transparent black. Use `Colour.White` for "no change".
- **Is a colour effects stage turning everything black?** `Opacity` in [ColourEffectConfiguration](xref:Yak2D.ColourEffectConfiguration) must be 1 for a normal picture; a new struct has it at 0.

## A texture or font will not load

- The console says `The provided texture name was not found in assembly: MyGame.Textures.yak.png`:
  - Is the file in the `Textures` folder, with the right name? Names are given without extension or folder: `LoadTexture("yak", ...)`.
  - Is the folder embedded? The `.csproj` needs `<EmbeddedResource Include="Textures\**" />`.
  - **Is your RootNamespace the same as your assembly name?** If your project name contains a hyphen, or you set `<RootNamespace>` yourself, embedded files are named differently from what yak2D looks for. Add `<RootNamespace>$(AssemblyName)</RootNamespace>` to your `.csproj`. See [Assets](assets.md#embedding-assets).
- `Unable to find Texture file from path`: file assets are relative to the working directory. See [Assets: Files on disk](assets.md#files-on-disk).
- A failed [LoadTexture()](xref:Yak2D.ISurfaces.LoadTexture*) returns `null`, and drawing with `null` then throws an exception, so the real cause is the earlier console message.

## Input

- **Key presses are missed or happen twice.** "Pressed/released this frame" means this *update step*: check them in `Update()`, not in `Drawing()`. See [Input](input.md#held-pressed-and-released).
- **My gamepad's up and down are reversed.** Stick Y values are negative when pushed up. See [Input: Gamepads](input.md#gamepads).

## The picture is stretched, or the wrong size

- Keep each camera's virtual resolution in the same proportion as the surface (or viewport) it renders to. See [Cameras](cameras.md#virtual-resolution).
- Viewports are in real pixels and do not follow window size changes; recreate them when the window changes. See [Viewports](viewports.md#viewports-and-window-size).

## Sprites have faint lines or specks at their edges

Texture filtering with the `Wrap` mode blends each edge of an image with the opposite edge. Leave a transparent border of a pixel or two round sprite images, or use `TextureCoordinateMode.Mirror`. See [Drawing](drawing.md#texture-coordinates-wrapping-and-filtering).

## A texture made from pixel data is upside down on OpenGL only (after updating to 1.3.0)

From yak2D 1.3.0, [CreateRgbaFromData()](xref:Yak2D.ISurfaces.CreateRgbaFromData*) and [CreateFloat32FromData()](xref:Yak2D.ISurfaces.CreateFloat32FromData*) put row 0 at the top on every graphics API, OpenGL included. If your code flipped the pixel rows itself for OpenGL, remove that flip. See [Textures from data](surfaces.md#textures-from-data).

## Translucent shapes look too dark, or have dark fringes

- **After an effect (colour effects, distortion):** this was a bug in yak2D before 1.2.0. Update the `Yak2D` package (`dotnet add package Yak2D`).
- **When drawing a transparent render target over something else:** the render target holds premultiplied colour, so composite it with a draw stage created with `BlendState.Premultiplied`, not `Alpha`. See [Transparent render targets](drawing.md#transparent-render-targets-and-premultiplied).

## My application quits when I switch graphics API

Switching API sends `ApplicationWindowClosing` (twice) as the window is recreated. If you quit on that message, you will quit on a switch. See [Window: Switching graphics API](window.md#switching-graphics-api).

## Everything disappeared after switching graphics API (or the device was lost)

All resources are destroyed when the graphics device is recreated. Recreate them when you receive `GraphicsDeviceRecreated`. See [Device resets](lifecycle.md#device-resets).

## Linux

- **The window does not open / "failed to create graphics device".** Check your GPU drivers: `glxinfo -B` (OpenGL) or `vulkaninfo --summary` (Vulkan) should show your GPU. Install your distribution's Mesa packages, or your GPU vendor's drivers.
- **Which API is being used?** The console says at start up. yak2D prefers OpenGL over Vulkan on Linux by default (`AvoidVulkanWherePossible`); set `PreferredGraphicsApi = GraphicsApi.Vulkan` to try Vulkan.
- Both X11 and Wayland sessions work.

## macOS

- yak2D runs on OpenGL, or on Vulkan through the bundled MoltenVK. The Metal backend is currently disabled.
- macOS support is less thoroughly tested than Windows and Linux; please [report problems](https://github.com/AlzPatz/yak2d/issues).

## The framework debug overlay

Debug builds of yak2D itself (not the NuGet package) include an overlay of timing information, toggled with **Ctrl+Shift+D**. It is useful when working on yak2D; in your own application, draw [IFps](xref:Yak2D.IFps) values for a simple frame rate display.

## Still stuck?

[Raise an issue on GitHub](https://github.com/AlzPatz/yak2d/issues) with the console output and, ideally, a small program that shows the problem.
