---
uid: uid_customshaders
---

# Custom shaders

A custom shader stage ([ICustomShaderStage](xref:Yak2D.ICustomShaderStage)) runs a **fragment shader you write** over a whole render target (or viewport). It can read up to four textures and any amount of your own data, so anything the built-in effects cannot do is a shader away.

For a step-by-step walk-through, see the **[Custom Shader tutorial](../tutorials/customshader.md)**. This page is the reference.

## The two ways to supply a shader

| | SPIR-V compile (recommended) | Precompiled per graphics API |
|---|---|---|
| `useSpirvCompile` | `true` | `false` |
| You write | one Vulkan-style GLSL 450 file | one compiled file per API |
| File(s) | `Shaders/<Name>.glsl` | `Shaders/Vulkan/<Name>.spv`, `Shaders/OpenGL/<Name>.glsl`, `Shaders/Direct3D/<Name>.hlsl.bytes`, `Shaders/Metal/<Name>.metallib` |
| Runs on | every API, compiled at runtime | only the APIs you provide files for |

With SPIR-V compile, yak2D compiles your GLSL when the stage is created and cross-compiles it for whichever graphics API is in use. It is by far the easiest option, and the one used in these docs. Precompiled shaders avoid the (small) runtime compile cost, but you have to produce each version yourself (with tools such as `glslangValidator`, SPIRV-Cross and `fxc`). The `Shaders` folder follows the same [embedded or file rules](assets.md) as other assets.

## Creating the stage

[!code-csharp[](../code/CustomShader/WavyYak.cs#create)]

[CreateCustomShaderStage()](xref:Yak2D.IStages.CreateCustomShaderStage*) takes:

- the shader's name (no extension, no folder);
- where to load it from ([AssetSourceEnum](xref:Yak2D.AssetSourceEnum));
- a list of **uniform descriptions**, one for each input the shader has (see below);
- a [BlendState](xref:Yak2D.BlendState) for how the output combines with the target. `Override` replaces the target's pixels; `Alpha` blends by your output's alpha;
- `useSpirvCompile`.

## Uniforms: the shader's inputs

Each [ShaderUniformDescription](xref:Yak2D.ShaderUniformDescription) becomes one **resource set** in the shader, numbered in the order they are listed: the first description is `set = 0`, the second `set = 1`, and so on.

| UniformType | In the shader | In C# |
|---|---|---|
| `Texture` | a `texture2D` at binding 0 and a `sampler` at binding 1 of its set | `SizeInBytes = 0`. Filled from the textures passed to `q.CustomShader()`, in order. |
| `Data` | a `uniform` block at binding 0 of its set | a struct, set with [SetCustomShaderUniformValues()](xref:Yak2D.IStages.SetCustomShaderUniformValues*). |

The shader for the example above:

```glsl
#version 450

// Description 0, "Texture" (a texture): set 0
layout(set = 0, binding = 0) uniform texture2D Texture_Texture;
layout(set = 0, binding = 1) uniform sampler Sampler_Texture;

// Description 1, "Settings" (data): set 1. The block name must match the description's Name
layout(set = 1, binding = 0) uniform Settings
{
    float Time;
    float Amount;
    vec2 Pad;
};

layout(location = 0) in vec2 FTex;         // texture coordinates, from yak2D's vertex shader
layout(location = 0) out vec4 fragColor;   // the output colour

void main()
{
    vec2 offset = Amount * vec2(sin(FTex.y * 20.0 + Time * 3.0), cos(FTex.x * 16.0 + Time * 2.0)) * 0.02;
    fragColor = texture(sampler2D(Texture_Texture, Sampler_Texture), FTex + offset);
}
```

Rules that matter:

- **The uniform block name must be exactly the description's `Name`.** On OpenGL, blocks are matched by name; a mismatch is silently ignored and the shader sees zeros. (Vulkan matches by set and binding, so it may appear to work there anyway.)
- **Texture first, sampler second** within a texture's set (bindings 0 and 1). The `Texture_<Name>` / `Sampler_<Name>` naming shown is a sensible convention.
- **Data blocks must be a multiple of 16 bytes**, and the C# struct's layout must match the GLSL layout exactly. GLSL's `std140` rules align a `vec3` or `vec4` to 16 bytes, so pad with spare floats or `Vector4`s, or set explicit `[FieldOffset]`s. `SizeInBytes` in the description must be the struct's size.
- **Texture coordinates** in `FTex` run from (0,0) at the top-left to (1,1) at the bottom-right on every graphics API; yak2D's vertex shader takes care of the differences.

The matching C# struct:

[!code-csharp[](../code/CustomShader/WavyYak.cs#uniforms)]

## Setting data each frame

[SetCustomShaderUniformValues()](xref:Yak2D.IStages.SetCustomShaderUniformValues*) sends a struct (or an array of structs) to the named uniform. Call it whenever the values change, typically in `PreDrawing()`:

[!code-csharp[](../code/CustomShader/WavyYak.cs#update)]

## Rendering

[q.CustomShader(stage, tex0, tex1, tex2, tex3, target)](xref:Yak2D.IRenderQueue.CustomShader*) runs the shader over the target. The textures fill the stage's `Texture` descriptions in order; pass `null` for any you don't use:

[!code-csharp[](../code/CustomShader/WavyYak.cs#render)]

## Debugging shaders

- If the stage cannot find the shader file, a message is written to the console when the stage is created. Check the [embedded asset rules](assets.md#embedding-assets): the file must be in a `Shaders` folder that is embedded (or copied to the output).
- A GLSL error makes `CreateCustomShaderStage()` throw a `NeoVeldrid.SPIRV.SpirvCompilationException` whose message gives the line number and the compiler's error, e.g. `GLSL compilation failed: <veldrid-spirv-input>:27: error: syntax error, unexpected SEMICOLON`.
- A black result often means a data block was not bound (check the block name) or `Opacity`-style values are zero. A result that ignores your uniforms on one API but not another is almost always a name mismatch.
- Test on more than one graphics API: switch with [StartupConfig.PreferredGraphicsApi](xref:Yak2D.StartupConfig.PreferredGraphicsApi).

## See also

- [Custom Shader tutorial](../tutorials/customshader.md)
- Sample: [CustomShader_Example](https://github.com/AlzPatz/yak2d-samples/tree/master/src/CustomShader_Example), which includes precompiled versions for every API
- For shaders that need more than a single full-screen pass (compute shaders, custom vertex formats), see [Custom Veldrid stages](customveldrid.md)
