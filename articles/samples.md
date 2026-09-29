---
uid: uid_samples
---

# The samples

The [yak2d-samples](https://github.com/AlzPatz/yak2d-samples) repository has around 35 small projects, each demonstrating one feature. They are the best place to see a feature working in isolation.

## Running them

You need the [.NET 10 SDK](https://dotnet.microsoft.com/download) on Windows, Linux or macOS. Nothing else needs installing.

```bash
git clone https://github.com/AlzPatz/yak2d-samples.git
cd yak2d-samples/src
dotnet run --project Bloom_Example
```

Or run `AllDemoLauncher` to browse every demo from one application:

```bash
dotnet run --project AllDemoLauncher
```

In every sample, **Escape** quits and the number keys **1**-**4** switch graphics API (OpenGL, Direct3D 11, Metal, Vulkan) where the platform supports it.

The samples use the [Yak2D NuGet package](https://www.nuget.org/packages/Yak2D/). If the [yak2d](https://github.com/AlzPatz/yak2d) source repository is cloned next to the samples repository, they build against that source instead, which is handy when working on yak2D itself.

## The list

| Sample | Shows | Guide |
|---|---|---|
| **Drawing** | | |
| [Draw_BasicPolygonHelperFunctions](https://github.com/AlzPatz/yak2d-samples/tree/master/src/Draw_BasicPolygonHelperFunctions) | Quads, polygons, lines and arrows with the helper functions | [Drawing](drawing.md) |
| [Draw_PolygonsFromVertices](https://github.com/AlzPatz/yak2d-samples/tree/master/src/Draw_PolygonsFromVertices) | Draw requests built from vertices: coloured, textured and dual-textured | [Drawing](drawing.md#draw-requests) |
| [Draw_FluentInterfaceDrawingHelper](https://github.com/AlzPatz/yak2d-samples/tree/master/src/Draw_FluentInterfaceDrawingHelper) | The fluent shape builder, texture brushes and transforms | [Drawing](drawing.md#the-fluent-shape-builder) |
| [Draw_PersistentDrawQueue](https://github.com/AlzPatz/yak2d-samples/tree/master/src/Draw_PersistentDrawQueue) | Persistent draw queues mixed with per-frame drawing | [Drawing](drawing.md#drawing-things-once) |
| [Draw_ImageFileFormats](https://github.com/AlzPatz/yak2d-samples/tree/master/src/Draw_ImageFileFormats) | Loading PNG, BMP, GIF, JPG and TGA images | [Assets](assets.md) |
| [Draw_FontExample](https://github.com/AlzPatz/yak2d-samples/tree/master/src/Draw_FontExample) | The built-in font and a loaded bitmap font | [Text](text.md) |
| **Cameras and viewports** | | |
| [Draw_Camera2DWorldAndScreen](https://github.com/AlzPatz/yak2d-samples/tree/master/src/Draw_Camera2DWorldAndScreen) | Drive a car round a map: a following, zooming, rotating camera, with a screen space HUD | [Cameras](cameras.md) |
| [Draw_SplitScreenExample](https://github.com/AlzPatz/yak2d-samples/tree/master/src/Draw_SplitScreenExample) | Two players, two cameras, one draw stage, two viewports | [Viewports](viewports.md) |
| [Helper_CoordinateTranforms](https://github.com/AlzPatz/yak2d-samples/tree/master/src/Helper_CoordinateTranforms) | Converting between window, screen and world positions inside a viewport | [Coordinate systems](coordinatesystems.md#converting-between-spaces) |
| **Effects** | | |
| [ColourEffects_Example](https://github.com/AlzPatz/yak2d-samples/tree/master/src/ColourEffects_Example) | Single colour, colourise, grayscale, negative and opacity, and random transitions | [Effects](effects.md#colour-effects) |
| [Bloom_Example](https://github.com/AlzPatz/yak2d-samples/tree/master/src/Bloom_Example) | Bloom | [Effects](effects.md#bloom) |
| [Blur_Example](https://github.com/AlzPatz/yak2d-samples/tree/master/src/Blur_Example) | Blur | [Effects](effects.md#blur) |
| [StyleEffects_Pixellate](https://github.com/AlzPatz/yak2d-samples/tree/master/src/StyleEffects_Pixellate) | Pixellate | [Effects](effects.md#style-effects) |
| [StyleEffects_EdgeDetection](https://github.com/AlzPatz/yak2d-samples/tree/master/src/StyleEffects_EdgeDetection) | Edge detection | [Effects](effects.md#style-effects) |
| [StyleEffects_Static](https://github.com/AlzPatz/yak2d-samples/tree/master/src/StyleEffects_Static) | Static noise | [Effects](effects.md#style-effects) |
| [StyleEffects_OldMovie](https://github.com/AlzPatz/yak2d-samples/tree/master/src/StyleEffects_OldMovie) | Old movie reel | [Effects](effects.md#style-effects) |
| [StyleEffects_CRT](https://github.com/AlzPatz/yak2d-samples/tree/master/src/StyleEffects_CRT) | CRT monitor, on a curved mesh | [Effects](effects.md#style-effects) |
| [StyleEffects_UsingConfigurationHelpers](https://github.com/AlzPatz/yak2d-samples/tree/master/src/StyleEffects_UsingConfigurationHelpers) | The `PreSet()` helpers for every style effect | [Effects](effects.md#style-effects) |
| [Mix_SimpleWholeTextureMixingWithFactors](https://github.com/AlzPatz/yak2d-samples/tree/master/src/Mix_SimpleWholeTextureMixingWithFactors) | Mixing four textures by overall amounts | [Effects](effects.md#mix) |
| [Mix_UsingATextureForPerPixelMixing](https://github.com/AlzPatz/yak2d-samples/tree/master/src/Mix_UsingATextureForPerPixelMixing) | Mixing with a per-pixel mask texture | [Effects](effects.md#mix) |
| [Distortion_ExampleUsingHelperFunctions](https://github.com/AlzPatz/yak2d-samples/tree/master/src/Distortion_ExampleUsingHelperFunctions) | Ripples in a pool, with the distortion helpers (click!) | [Distortion](distortion.md) |
| [Distortion_ManualTextureCreation](https://github.com/AlzPatz/yak2d-samples/tree/master/src/Distortion_ManualTextureCreation) | Height map textures made by hand, and distortion draw requests | [Distortion](distortion.md) |
| [DrawingAndEffects_PrerenderingTexturesForUseLater](https://github.com/AlzPatz/yak2d-samples/tree/master/src/DrawingAndEffects_PrerenderingTexturesForUseLater) | Chaining stages to pre-render an animated explosion into textures | [Surfaces](surfaces.md) |
| [Copy_Example](https://github.com/AlzPatz/yak2d-samples/tree/master/src/Copy_Example) | Copying a surface to the window | [Surfaces](surfaces.md) |
| **3D** | | |
| [Mesh_HelperExamples](https://github.com/AlzPatz/yak2d-samples/tree/master/src/Mesh_HelperExamples) | Quad, CRT, sphere and cube meshes with lights and a fly-through camera | [3D meshes](mesh.md) |
| [Mesh_ManualMeshSimple](https://github.com/AlzPatz/yak2d-samples/tree/master/src/Mesh_ManualMeshSimple) | A mesh built by hand | [3D meshes](mesh.md) |
| [Mesh_ManualChangingMesh](https://github.com/AlzPatz/yak2d-samples/tree/master/src/Mesh_ManualChangingMesh) | A mesh that changes every frame | [3D meshes](mesh.md) |
| **Surfaces and data** | | |
| [Surfaces_CreateTextureFromDataRGBA](https://github.com/AlzPatz/yak2d-samples/tree/master/src/Surfaces_CreateTextureFromDataRGBA) | Textures generated from pixel data | [Surfaces](surfaces.md#textures-from-data) |
| [GpuToCpu_SurfaceCopyRGBA](https://github.com/AlzPatz/yak2d-samples/tree/master/src/GpuToCpu_SurfaceCopyRGBA) | Reading colours back from the GPU | [Readback](readback.md) |
| [GpuToCpu_SurfaceCopyFloat32](https://github.com/AlzPatz/yak2d-samples/tree/master/src/GpuToCpu_SurfaceCopyFloat32) | Reading float data back from the GPU | [Readback](readback.md) |
| **Custom rendering** | | |
| [CustomShader_Example](https://github.com/AlzPatz/yak2d-samples/tree/master/src/CustomShader_Example) | A fragment shader, with runtime SPIR-V compile or precompiled for every API | [Custom shaders](customshaders.md) |
| [CustomVeldrid_ComputeShaderExample](https://github.com/AlzPatz/yak2d-samples/tree/master/src/CustomVeldrid_ComputeShaderExample) | Conway's Game of Life as a compute shader | [Custom Veldrid stages](customveldrid.md) |
| **Input and window** | | |
| [Input_MouseAndKeyboardUsage](https://github.com/AlzPatz/yak2d-samples/tree/master/src/Input_MouseAndKeyboardUsage) | Type to make letters, click to shoot them | [Input](input.md) |
| [Input_GamepadUsage](https://github.com/AlzPatz/yak2d-samples/tree/master/src/Input_GamepadUsage) | Every gamepad button and axis | [Input](input.md#gamepads) |
| [Window_ChangingWindowProperties](https://github.com/AlzPatz/yak2d-samples/tree/master/src/Window_ChangingWindowProperties) | Fullscreen, size, title, opacity, borders, cursor | [Window](window.md) |
| **Framework** | | |
| [FrameworkItems_CreationAndDestruction](https://github.com/AlzPatz/yak2d-samples/tree/master/src/FrameworkItems_CreationAndDestruction) | Creating and destroying every kind of resource | [Resources](resources.md) |

## The documentation examples

Every example in these guides, and the tutorial projects, are in the [code folder of the documentation repository](https://github.com/AlzPatz/yak2d-docs-content/tree/master/code). The guide examples are all in one project; run any of them by name:

```bash
git clone https://github.com/AlzPatz/yak2d-docs-content.git
cd yak2d-docs-content/code/Snippets
dotnet run -- BloomExample
dotnet run            # lists them all
```
