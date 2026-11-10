---
uid: uid_whatsnew
---

# What's new

Changes in each release of the [Yak2D package](https://www.nuget.org/packages/Yak2D/) that affect applications. yak2D follows [semantic versioning](https://semver.org/) from 1.1.0: `1.x.y` releases never break your code, so you can always update within 1.x with:

```bash
dotnet add package Yak2D
```

The full history is in the [yak2d repository](https://github.com/AlzPatz/yak2d/commits/master).

## 1.2.3 (November 2026)

- **Custom shaders on OpenGL:** with SPIR-V compiled shaders, textures are now bound by name, so a shader that reads more than one texture gets the right one on OpenGL. Name each texture `Texture_<Name>` and its sampler `Sampler_<Name>`, after the uniform description's `Name`. See [Custom shaders](customshaders.md#uniforms-the-shaders-inputs).

## 1.2.2 and 1.2.1 (November 2026)

- **Window resizing:** fixed crashes and errors that could happen while the window was being resized, when the window's render target was recreated at the same moment a frame was being presented.

## 1.2.0 (October 2026)

- **Correct transparency on render targets.** `BlendState.Alpha` now keeps a surface's alpha channel correct. Before, translucent shapes drawn onto a render target lowered its alpha, so they came out too dark after effects such as [colour effects](effects.md#colour-effects) and [distortion](distortion.md). Drawing onto opaque surfaces looks exactly as before.
- **New `BlendState.Premultiplied`** for drawing a transparent render target over something else without dark edges. See [Transparent render targets](drawing.md#transparent-render-targets-and-premultiplied), and the [Cookbook recipe](cookbook.md#a-transparent-layer-over-a-scene).
- **Lighter image loading:** images are now decoded with StbImageSharp instead of ImageSharp. The same formats are supported (PNG, JPG, BMP, GIF and TGA) and nothing changes in your code.
- **Fixed** a crash when a stage was destroyed in the same frame as shutdown or a device reset.

## 1.1.0 (September 2026)

The first stable 1.x release.

- **Faster, stable draw sorting:** draw requests are sorted about twice as fast, and the sort is now stable: requests that would otherwise sort equally (same layer, depth and textures) always draw in the order they were submitted, instead of possibly swapping between frames.
- **Licence:** the package is now MIT licensed throughout (the last Apache-licensed code was removed).
- Built on [NeoVeldrid](https://www.nuget.org/packages/NeoVeldrid) 1.2.1 and .NET 10, with SDL2 and all native libraries included in the package.

> [!NOTE]
> Version 1.0.0 is deprecated and unlisted on NuGet. Use 1.1.0 or later.
