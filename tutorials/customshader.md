---
uid: uid_tut_customshader
---

# Writing a custom shader

yak2D's built-in effects cover a lot, but sooner or later you will want an effect of your own. A **custom shader stage** runs a *fragment shader* that you write: a small program that runs on the GPU once for every pixel of the output, and decides its colour.

In this tutorial you will write a shader that makes an image ripple like a flag and slowly cycles its colours, with a setting you can change from C#. It runs on every graphics API yak2D supports.

![The wavy yak](../images/tutorials/customshader.png)

**You will need:** a yak2D project (see [Getting Started](gettingstarted.md)). No shader experience is assumed, though some familiarity with C-like syntax helps.

The finished code: [code/CustomShader](https://github.com/AlzPatz/yak2d-docs-content/tree/master/code/CustomShader).

## 1. The project

Create a project and add two folders, `Textures` (with an image in it: we use [this yak photo](https://github.com/AlzPatz/yak2d-docs-content/raw/master/code/CustomShader/Textures/yakphoto.png), saved as `yakphoto.png`) and `Shaders`. Embed both:

[!code-xml[](../code/CustomShader/CustomShader.csproj)]

## 2. The shader

Create `Shaders/Wavy.glsl`:

[!code-glsl[](../code/CustomShader/Shaders/Wavy.glsl)]

Shaders are written in **GLSL**, a C-like language. Going through it:

- `#version 450` says which version of GLSL this is. yak2D compiles it with the Vulkan flavour of GLSL 4.50, and converts it to whatever the current graphics API needs.
- **The texture**, `Texture_Texture` and `Sampler_Texture`: the image we will read. In modern GLSL a texture and its *sampler* (which says how to filter it) are separate, at bindings 0 and 1 of the same set.
- **The settings**: a `uniform` block called `Settings`, with our own values in it. Every pixel sees the same values; we set them from C# each frame.
- **`FTex`**: the input from yak2D's vertex shader, the position of this pixel as a texture coordinate, from (0,0) at the top-left of the output to (1,1) at the bottom-right.
- **`fragColor`**: the output, the pixel's colour.
- **`main()`** runs once per pixel. It nudges the coordinate it reads the texture at by a sine wave that changes over time (the ripple), reads the texture there, then mixes in a version with its red and blue channels rotated (the colour cycling). `Amount` controls both.

The `set` numbers must match the order of the uniform descriptions we give yak2D in C#, and the uniform block's name must match its description's `Name`. Those are the only links between the two sides.

## 3. The C# side

Create `WavyYak.cs`.

### The settings struct

[!code-csharp[](../code/CustomShader/WavyYak.cs#uniforms)]

This struct must have the same layout as the `Settings` block in the shader: two floats, then two more to pad it to 16 bytes, because GPU uniform data comes in 16 byte chunks. `[StructLayout(LayoutKind.Sequential)]` makes sure C# keeps the fields in order.

### Creating the stage

[!code-csharp[](../code/CustomShader/WavyYak.cs#create)]

The uniform descriptions tell yak2D what the shader expects, in `set` order:

1. `"Texture"`, a texture: `set = 0` in the shader.
2. `"Settings"`, 16 bytes of data: `set = 1`.

`useSpirvCompile: true` tells yak2D to compile the `.glsl` file itself, for whichever graphics API is in use. `BlendState.Override` means the shader's output replaces whatever was on the target.

### Updating the settings

[!code-csharp[](../code/CustomShader/WavyYak.cs#update)]

`SetCustomShaderUniformValues` sends a struct to a named uniform. We send the time and the current strength every frame, and the arrow keys change the strength.

### Rendering

[!code-csharp[](../code/CustomShader/WavyYak.cs#render)]

A custom shader stage can read up to four textures, filling the stage's texture uniforms in order. This shader has one, so the other three are `null`. The output fills the target: here, the whole window.

### Program.cs

[!code-csharp[](../code/CustomShader/Program.cs)]

## 4. Run it

```bash
dotnet run
```

The yak ripples and changes colour. Hold the up and down arrows to change the strength.

To check it works on another graphics API, set `PreferredGraphicsApi` in `Configure()`:

```csharp
public StartupConfig Configure()
{
    var config = StartupConfig.Default(960, 540, "Custom Shader", false);
    config.PreferredGraphicsApi = GraphicsApi.Vulkan;   // or OpenGL, or Direct3D11 on Windows
    return config;
}
```

## When things go wrong

- **An exception when the stage is created, mentioning `GLSL compilation failed`**: a mistake in the shader. The message gives the line number.
- **"Unable to load shader file" in the console**: the file is not in the embedded `Shaders` folder, or the name passed to `CreateCustomShaderStage()` is wrong (it has no extension or folder).
- **Your settings have no effect on one graphics API**: the uniform block's name does not match the description's `Name`. OpenGL matches them by name.
- **Garbage values**: the C# struct's layout does not match the shader's. Check the padding.

## Using a shader as a post-process

The input doesn't have to be a loaded texture. Render your game into a [render target](../articles/surfaces.md) and pass that in, and the shader becomes a full-screen effect for your game, just like the built-in effects in [Yak Run part 4](yakrun-4.md).

## Going further

- The [Custom shaders](../articles/customshaders.md) guide has the full rules, including multiple textures, arrays of data and precompiled shaders.
- [The Book of Shaders](https://thebookofshaders.com/) is a gentle introduction to writing fragment shaders.
- For work that needs more than one full-screen pass, see [Custom Veldrid stages](../articles/customveldrid.md).
