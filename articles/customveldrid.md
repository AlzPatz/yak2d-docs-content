---
uid: uid_customveldrid
---

# Custom Veldrid stages

yak2D is built on [NeoVeldrid](https://www.nuget.org/packages/NeoVeldrid), a maintained fork of the [Veldrid](https://github.com/mellinoe/veldrid) graphics library. A **custom Veldrid stage** ([ICustomVeldridStage](xref:Yak2D.ICustomVeldridStage)) hands you NeoVeldrid's graphics device and command list in the middle of yak2D's render queue, so you can do anything NeoVeldrid can: compute shaders, custom vertex formats, instancing, your own pipelines, integrating another rendering library.

Reach for this only when a [custom shader stage](customshaders.md) is not enough, since you take on the work (and the cross-platform details) that yak2D normally does for you.

## Writing a stage

Derive from [CustomVeldridBase](xref:Yak2D.CustomVeldridBase) and implement its four methods:

[!code-csharp[](../code/Snippets/CustomVeldrid.cs#custom-veldrid-stage)]

| Method | Called | Use it to |
|---|---|---|
| `Initialise(device, window, factory)` | once, when the stage is created | create pipelines, buffers, shaders, textures. Resources made with the `factory` are disposed of automatically. |
| `Update(seconds, inputSnapshot)` | every update step | advance your own simulation; `inputSnapshot` is NeoVeldrid's raw input for the step. |
| `Render(cl, device, texture0..3, framebufferTarget)` | when the stage appears in the render queue | record commands into `cl`. The textures passed to `q.CustomVeldrid()` arrive as `ResourceSet`s; the target is a `Framebuffer`. |
| `DisposeOfResources()` | when the stage is destroyed | dispose of anything *not* created through the factory. |

Don't call `cl.Begin()` or `cl.End()` in `Render()`: the command list belongs to yak2D and is already recording. Do set the framebuffer, and restore anything you change that later yak2D stages might rely on.

## Using a stage

[!code-csharp[](../code/Snippets/CustomVeldrid.cs#custom-veldrid-use)]

[CreateCustomVeldridStage()](xref:Yak2D.IStages.CreateCustomVeldridStage*) takes an instance of your class. [q.CustomVeldrid(stage, tex0, tex1, tex2, tex3, target)](xref:Yak2D.IRenderQueue.CustomVeldrid*) runs it, with up to four input surfaces (or `null`) and a render target to draw into, which later yak2D stages can then use like any other surface.

## Things to watch out for

- **Name clashes.** NeoVeldrid and yak2D both define some types with the same names (for example `MouseButton`), so a file with `using NeoVeldrid;` and `using Yak2D;` may need aliases such as `using Colour = Yak2D.Colour;`.
- **Graphics API differences** are yours to handle: texture coordinate origins, clip space depth, and shader languages all vary between Direct3D, Vulkan, OpenGL and Metal. NeoVeldrid's `NeoVeldrid.SPIRV` package (already a yak2D dependency) can cross-compile GLSL for you, as the sample below does.
- **Device resets** destroy your stage along with everything else; create it again in `CreateResources()`.
- The pipeline's output description must match the target's format. yak2D render targets are RGBA with a depth buffer.

## See also

- Sample: [CustomVeldrid_ComputeShaderExample](https://github.com/AlzPatz/yak2d-samples/tree/master/src/CustomVeldrid_ComputeShaderExample): Conway's Game of Life running as a compute shader, drawn into a render target and then displayed with a normal draw stage.
- [NeoVeldrid](https://www.nuget.org/packages/NeoVeldrid) and the original [Veldrid documentation](https://veldrid.dev/)
