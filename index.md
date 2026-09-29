# **yak2D** Documentation

The **yak2D** framework enables the creation of interactive **cross platform** desktop 2D graphics applications.

It offers simple 2D polygon (coloured, textured and dual textured) drawing functions, in addition to flexible render path creation making use of shader effects (both inbuilt and user defined).

Quickly create 2D games and prototypes that run on all major desktop operating systems. Graphics, Input, Windowing and Application Lifecycle are all provided for and managed by the framework (*just add sound*). Avoid the bloat of large game engines (or make life harder for yourself - whatever your opinion). No GUI, **do it all in code..** :)

yak2D is a .NET 10 library, built upon [NeoVeldrid](https://www.nuget.org/packages/NeoVeldrid) (a maintained fork of the [Veldrid](https://github.com/mellinoe/veldrid) cross-platform, graphics API agnostic rendering library). Application windowing and input are handled by SDL2, which is bundled with the NuGet package: there is nothing else to install.

**Supported Desktop Platforms: Windows, Linux and MacOS**

**Supported Graphics APIs: Direct3D 11, Vulkan, OpenGL** *(Metal support is currently disabled; macOS uses OpenGL or Vulkan via MoltenVK)*

[![NuGet](https://img.shields.io/nuget/v/yak2d.svg)](https://www.nuget.org/packages/Yak2D/)

Please see the [Demo Samples](https://github.com/AlzPatz/yak2d-samples) for usage examples, the [Getting Started](xref:uid_tut_gettingstarted) tutorial to create your first application, or the [Usage Guide](xref:uid_intro) for additional guidance on **yak2D** concepts and operation.