---
uid: uid_assets
---

# Assets: textures, fonts and shaders

yak2D loads three kinds of asset file: **images** (textures), **bitmap fonts**, and **shaders** (for [custom shader stages](customshaders.md)). Each can be loaded in one of two ways, chosen with [AssetSourceEnum](xref:Yak2D.AssetSourceEnum):

| Source | The files are... | Good for |
|---|---|---|
| `AssetSourceEnum.Embedded` | compiled into your program's .dll | Most games. Nothing can go missing; the program is self-contained. |
| `AssetSourceEnum.File` | separate files on disk, relative to the working directory | Assets you want to swap without recompiling (modding, level editors), or very large asset sets. |

Textures can also be created from a `Stream` or from pixel data in code, see [Surfaces](surfaces.md).

[!code-csharp[](../code/Snippets/Application.cs#asset-loading)]

## Where yak2D looks

Asset names never include the file extension, and use `/` for sub-folders. The folders are set in the [StartupConfig](xref:Yak2D.StartupConfig):

| Asset | Folder setting (default) | `LoadTexture("characters/yak", ...)` looks for |
|---|---|---|
| Textures | [TextureFolderRootName](xref:Yak2D.StartupConfig.TextureFolderRootName) (`Textures`) | `Textures/characters/yak.png` |
| Fonts | [FontFolder](xref:Yak2D.StartupConfig.FontFolder) (`Fonts`) | `Fonts/characters/yak_*.fnt` (see [Text](text.md)) |
| Shaders | always `Shaders` | see [Custom shaders](customshaders.md) |

The image format defaults to PNG. For others pass an [ImageFormat](xref:Yak2D.ImageFormat): `PNG`, `JPG`, `BMP`, `GIF` or `TGA`. PNG is recommended because it supports transparency and is lossless.

## Embedding assets

Add an `EmbeddedResource` item to your `.csproj` for each asset folder:

```xml
<ItemGroup>
  <EmbeddedResource Include="Textures\**" />
  <EmbeddedResource Include="Fonts\**" />
  <EmbeddedResource Include="Shaders\**" />
</ItemGroup>
```

`**` includes every file in the folder and its sub-folders. Put the folders next to your `.csproj`:

```text
MyGame/
├── MyGame.csproj
├── Program.cs
├── Textures/
│   ├── yak.png
│   └── characters/
│       └── hero.png          <- LoadTexture("characters/hero", AssetSourceEnum.Embedded)
└── Fonts/
    ├── pixel_16.fnt
    └── pixel_16_0.png        <- LoadFont("pixel", AssetSourceEnum.Embedded)
```

> [!WARNING]
> **Embedded assets are looked up by your assembly name.** yak2D expects a texture to be embedded as `<AssemblyName>.Textures.yak.png`. MSBuild names embedded files using the project's `RootNamespace`, which is normally the same as the assembly name, so it just works. But if the two differ, every embedded asset fails to load (the console says `The provided texture name was not found in assembly`).
>
> This happens if you set `<RootNamespace>` yourself, and also if your project name contains a hyphen: `dotnet new console -n my-game` writes `<RootNamespace>my_game</RootNamespace>` into the project. To fix it, add this to the `<PropertyGroup>` in your `.csproj`:
>
> ```xml
> <RootNamespace>$(AssemblyName)</RootNamespace>
> ```
>
> Your code can still use whatever `namespace` you like.

> [!TIP]
> Avoid spaces and other unusual characters in asset folder and file names. Embedded resource names are built from the folder path, and MSBuild changes some characters (spaces become underscores, for example).

## Files on disk

To load assets as files, copy them to the build output with a `Content` item:

```xml
<ItemGroup>
  <Content Include="Textures\**" CopyToOutputDirectory="PreserveNewest" />
  <Content Include="Fonts\**" CopyToOutputDirectory="PreserveNewest" />
</ItemGroup>
```

File paths are relative to the **current working directory** of the process, not the location of the executable. `dotnet run` sets the working directory to the project folder, and double-clicking an executable usually sets it to the executable's folder, but a shortcut, a script or macOS's Finder may start your program somewhere else. To make file loading reliable, set the working directory at the very start of your program:

```csharp
Directory.SetCurrentDirectory(AppContext.BaseDirectory);
Launcher.Run(new MyGame());
```

## When loading fails

- [LoadTexture()](xref:Yak2D.ISurfaces.LoadTexture*) returns **`null`** and writes a message to the console if the texture cannot be found. Drawing with a `null` texture throws an exception, so check the console first if something goes wrong.
- [LoadFont()](xref:Yak2D.IFonts.LoadFont*) **throws a [Yak2DException](xref:Yak2D.Yak2DException)** if no matching `.fnt` files can be found.
- [CreateCustomShaderStage()](xref:Yak2D.IStages.CreateCustomShaderStage*) writes a message to the console if the shader file cannot be found.

A quick way to see what is really embedded in your program is to list the resource names:

```csharp
foreach (var name in typeof(MyGame).Assembly.GetManifestResourceNames())
{
    Console.WriteLine(name);   // e.g. MyGame.Textures.yak.png
}
```

## Texture options

[LoadTexture()](xref:Yak2D.ISurfaces.LoadTexture*) has two more optional parameters:

- **samplerType** ([SamplerType](xref:Yak2D.SamplerType)): how the texture is filtered when drawn larger or smaller than its real size. `Anisotropic` (the default) and `Linear` are smooth; `Point` keeps hard pixel edges, which is what you want for pixel art. `PointMagLinearMin` uses point sampling when enlarging and linear when shrinking.
- **generateMipMaps** (default `true`): creates smaller copies of the texture so it still looks smooth when drawn much smaller than its real size. Turn it off for pixel art, or for textures always drawn at full size.

See [Drawing](drawing.md#texture-coordinates-wrapping-and-filtering) for an example of each.

## Preparing images

- Leave a transparent border of a pixel or two round sprites. With the default `Wrap` texture mode, the GPU's filtering can pull a few pixels from the opposite edge of the image into the edge of your sprite, which shows up as faint specks or lines.
- Textures do not need to be a power of two in size, but very large textures (over 4096 pixels across) may not be supported by every GPU.
- For animation, either load one texture per frame, or put all frames in one image (a sprite sheet) and choose the frame with texture coordinates (see [Drawing](drawing.md)).
