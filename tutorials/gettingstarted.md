---
uid: uid_tut_gettingstarted
---
# Getting Started

## Aim
By the end of this article you will have created a new project that uses **yak2D**: an application that opens a window, clears it to a colour every frame, and exits when the escape key is pressed.

It is the starting point for the [Yak Run tutorial](yakrun-1.md), and for your own applications.

## Prerequisites
* A desktop OS: **Windows**, **Linux** or **macOS** (x64 or arm64)
* A GPU and driver supporting at least one of Direct3D 11, Vulkan, OpenGL or Metal
* Basic C# knowledge

## Environment Setup

**yak2D** applications are .NET applications. **yak2D** targets **.NET 10**, so you need the **.NET 10 SDK** (or later).

The steps in this tutorial use the `dotnet` command line tool, which works the same way on every platform. Any editor can be used; popular choices are [Visual Studio](https://visualstudio.microsoft.com/) (Windows), [JetBrains Rider](https://www.jetbrains.com/rider/) (all platforms) and [Visual Studio Code](https://code.visualstudio.com/) with the C# Dev Kit extension (all platforms).

> [!NOTE]
> There is no need to install SDL2 or any other native library. The **yak2D** NuGet package brings in everything it needs (including SDL2 for windowing and input) for Windows, Linux and macOS.

### Install the .NET SDK

#### Windows
Either:
* Install [Visual Studio](https://visualstudio.microsoft.com/) with the **.NET desktop development** workload (this includes the SDK), or
* Download the .NET 10 SDK installer from [dot.net](https://dotnet.microsoft.com/download), or
* From a terminal: `winget install Microsoft.DotNet.SDK.10`

#### Linux
Most distributions package the .NET SDK. For example:

```bash
# Fedora
sudo dnf install dotnet-sdk-10.0

# Ubuntu (older releases may first need the .NET backports repository - see the guide below)
sudo apt-get update && sudo apt-get install -y dotnet-sdk-10.0
```

For other distributions, or if your distribution does not yet package .NET 10, see Microsoft's [Linux install guide](https://learn.microsoft.com/dotnet/core/install/linux).

Make sure your graphics drivers are installed. On most desktop distributions the default Mesa drivers (Intel / AMD) or the vendor's proprietary drivers (NVIDIA) provide OpenGL and Vulkan support. Both X11 and Wayland sessions are supported.

#### macOS
Download the .NET 10 SDK installer (Arm64 for Apple Silicon, x64 for Intel Macs) from [dot.net](https://dotnet.microsoft.com/download), or use Homebrew:

```bash
brew install --cask dotnet-sdk
```

> [!NOTE]
> macOS support is less thoroughly tested than Windows and Linux. The Metal backend is currently disabled, so on macOS **yak2D** renders using OpenGL (or Vulkan, via the bundled MoltenVK). Please [raise an issue](https://github.com/AlzPatz/yak2d/issues) if you run into problems.

### Check the installation
Open a terminal (Command Prompt / PowerShell on Windows) and run:

```bash
dotnet --version
```

The version reported should be `10.0.100` or higher.

## Create the Project

From a terminal, navigate to the folder in which you want your project folder to be created, then run:

```bash
dotnet new console -n MyFirstYakApp
cd MyFirstYakApp
dotnet add package Yak2D
```

This creates a console application project containing `MyFirstYakApp.csproj` and `Program.cs`, and adds a reference to the latest [**yak2D** NuGet package](https://www.nuget.org/packages/Yak2D/).

> [!TIP]
> In Visual Studio or Rider you can instead create a new **Console App** project and add the `Yak2D` package using the NuGet package manager. The rest of this tutorial is the same.

Open the `MyFirstYakApp` folder in your editor.

## Writing Code

To launch a **yak2D** application you create a class that implements the [IApplication](xref:Yak2D.IApplication) interface, and pass an instance of it to [Launcher.Run()](xref:Yak2D.Launcher.Run*). The framework then takes control, calling your class's methods at the right points in the [application lifecycle](xref:uid_lifecycle). (Why it works this way is explained in [How yak2D works](../articles/overview.md).)

### Create the application class

Create a new file in the project folder called `MyApplication.cs`, containing:

```csharp
using Yak2D;

namespace MyFirstYakApp;

public class MyApplication : IApplication
{

}
```

Your editor will now complain that `MyApplication` does not implement the members of [IApplication](xref:Yak2D.IApplication). We add each one in turn below. *(Most editors offer an "Implement interface" quick-fix that generates empty methods for you. The order the methods appear in the file does not matter, but the order below follows the order in which the framework calls them.)*

All of the following methods go between the curly brackets of the `MyApplication` class.

**[Configure()](xref:Yak2D.IApplication.Configure)** is called first, once, and returns a [StartupConfig](xref:Yak2D.StartupConfig) describing the window, graphics API and timing settings. The [StartupConfig.Default()](xref:Yak2D.StartupConfig.Default*) helper fills in sensible defaults; we pass a window size of 960 x 540 pixels, a window title, and `false` to request a window rather than fullscreen:

```csharp
public StartupConfig Configure()
{
    return StartupConfig.Default(960, 540, "My First yak2D App", false);
}
```

**[OnStartup()](xref:Yak2D.IApplication.OnStartup)** runs next, once the window has been created: a place to set up your own objects. There is nothing we need to do here yet:

```csharp
public void OnStartup() { }
```

**[CreateResources()](xref:Yak2D.IApplication.CreateResources*)** is where all [resources](xref:uid_glossary#resource) (textures, fonts, render stages, cameras, etc.) are created. It runs once before the main loop starts, and again if the application ever loses its resources (for example, if the graphics API is changed at runtime). A blank application uses no resources. Returning `true` tells the framework creation succeeded:

```csharp
public bool CreateResources(IServices yak)
{
    return true;
}
```

**[ProcessMessage()](xref:Yak2D.IApplication.ProcessMessage*)** is called once for every [FrameworkMessage](xref:Yak2D.FrameworkMessage) generated by the framework (such as a gamepad being connected or the window being resized), ahead of each [UpdateStep](xref:uid_glossary#updatestep). We can ignore them for now:

```csharp
public void ProcessMessage(FrameworkMessage msg, IServices yak) { }
```

**[Update()](xref:Yak2D.IApplication.Update*)** is called during each [UpdateStep](xref:uid_glossary#updatestep), and is where non-drawing application logic (game simulation, input handling, etc.) lives. Returning `false` shuts the application down, so here we return `false` when the escape key is pressed:

```csharp
public bool Update(IServices yak, float secondsSinceLastUpdate)
{
    return !yak.Input.IsKeyCurrentlyPressed(KeyCode.Escape);
}
```

**[PreDrawing()](xref:Yak2D.IApplication.PreDrawing*)** is the first of three methods called during each [RenderStep](xref:uid_glossary#renderstep). It is a place to run non-drawing code ahead of drawing, such as moving cameras or updating render stage settings. Nothing is needed here yet:

```csharp
public void PreDrawing(IServices yak, float secondsSinceLastDraw, float secondsSinceLastUpdate) { }
```

**[Drawing()](xref:Yak2D.IApplication.Drawing*)** is the second. It is where [DrawRequests](xref:Yak2D.DrawRequest) are created and submitted to [DrawStages](xref:Yak2D.IDrawStage) (i.e. where you draw your sprites and shapes). The blank application draws nothing:

```csharp
public void Drawing(IDrawing draw, IFps fps, IInput input, ICoordinateTransforms transforms, float secondsSinceLastDraw, float secondsSinceLastUpdate) { }
```

**[Rendering()](xref:Yak2D.IApplication.Rendering*)** is the third. Here you build the [RenderQueue](xref:uid_glossary#render-queue): the ordered list of rendering operations the GPU performs this frame. The blank application has one operation: clearing the window's [RenderTarget](xref:Yak2D.IRenderTarget) to a colour:

```csharp
public void Rendering(IRenderQueue q, IRenderTarget windowRenderTarget)
{
    q.ClearColour(windowRenderTarget, Colour.CornflowerBlue);
}
```

> [!NOTE]
> [StartupConfig](xref:Yak2D.StartupConfig) also has `AutoClearMainWindowColourEachFrame` and `AutoClearMainWindowDepthEachFrame` settings which, when `true`, clear the window to transparent black before your render queue runs. `StartupConfig.Default()` sets both to `false`, which is why we clear explicitly here.

**[Shutdown()](xref:Yak2D.IApplication.Shutdown)** is called when the application exits. Use it to release anything your application holds that is *not* managed by **yak2D** (framework resources are cleaned up automatically):

```csharp
public void Shutdown() { }
```

### Launch the application

Replace the contents of `Program.cs` with:

```csharp
using Yak2D;
using MyFirstYakApp;

Launcher.Run(new MyApplication());
```

## Running the Application

From the project folder, run:

```bash
dotnet run
```

*(In Visual Studio or Rider, press F5 or the Run button.)*

## Result

A 960 x 540 window titled "My First yak2D App" opens, filled with a cornflower blue background. Press escape to exit.

![The blank application](../images/tutorials/gettingstarted.png)

The console also shows some start up information, including which graphics API was chosen, for example:

```text
Graphics API Requested: SystemDefault
Graphics API Chosen: OpenGL
...
Yak2D Framework entering core loops
```

## Troubleshooting

* **Choosing a graphics API**: by default, **yak2D** uses the system's default graphics API if it supports it (Direct3D 11 on Windows; Vulkan on Linux). Otherwise it tries Direct3D 11, then OpenGL, then Vulkan, using the first one available. `StartupConfig.Default()` sets [AvoidVulkanWherePossible](xref:Yak2D.StartupConfig.AvoidVulkanWherePossible) to `true`, so Vulkan is only used if nothing else is available (which is why the Linux example output above shows OpenGL). To request a specific API, set [PreferredGraphicsApi](xref:Yak2D.StartupConfig.PreferredGraphicsApi) on the returned config (if the system does not support it, the default selection above is used instead).
* **Linux: the application fails to create a window or graphics device**: check that your GPU drivers are installed and working (e.g. `glxinfo -B` or `vulkaninfo --summary`).

## Complete Code Listing

This project is also in the documentation repository: [code/GettingStarted](https://github.com/AlzPatz/yak2d-docs-content/tree/master/code/GettingStarted/MyFirstYakApp).

Program.cs

[!code-csharp[](../code/GettingStarted/MyFirstYakApp/Program.cs)]

MyApplication.cs

[!code-csharp[](../code/GettingStarted/MyFirstYakApp/MyApplication.cs)]

## Next steps

- Build a small game in the **[Yak Run tutorial](yakrun-1.md)**.
- Read **[How yak2D works](../articles/overview.md)** for the ideas behind the framework.
