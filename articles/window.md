---
uid: uid_window
---

# Window, display and graphics API

## The window

The window is set up from your [StartupConfig](configuration.md), and can be changed at any time through [IDisplay](xref:Yak2D.IDisplay) (`yak.Display`):

[!code-csharp[](../code/Snippets/Application.cs#window-update)]

| Member | Does |
|---|---|
| [SetDisplayState(state)](xref:Yak2D.IDisplay.SetDisplayState*), [DisplayState](xref:Yak2D.IDisplay.DisplayState) | Switch between `Normal`, `FullScreen`, `BorderlessFullScreen`, `Maximised` and `Minimised`. (`Hidden` hides the window completely; you probably don't want it.) |
| [SetWindowResolution(width, height)](xref:Yak2D.IDisplay.SetWindowResolution*), [WindowResolutionWidth](xref:Yak2D.IDisplay.WindowResolutionWidth), [WindowResolutionHeight](xref:Yak2D.IDisplay.WindowResolutionHeight) | The window's size in pixels. |
| [SetWindowTitle(title)](xref:Yak2D.IDisplay.SetWindowTitle*) | The title bar text. |
| [SetVsync(on)](xref:Yak2D.IDisplay.SetVsync*) | Vertical sync. |
| [SetCursorVisible(visible)](xref:Yak2D.IDisplay.SetCursorVisible*) | Show or hide the mouse pointer over the window. |
| [WindowResizable](xref:Yak2D.IDisplay.WindowResizable), [WindowBorderVisible](xref:Yak2D.IDisplay.WindowBorderVisible), [WindowOpacity](xref:Yak2D.IDisplay.WindowOpacity) | Resizable edges, the border, and see-through windows. |
| [WindowPositionX](xref:Yak2D.IDisplay.WindowPositionX), [WindowPositionY](xref:Yak2D.IDisplay.WindowPositionY) | The window's position on the desktop. |
| [WindowIsInFocus](xref:Yak2D.IDisplay.WindowIsInFocus) | Whether the window has keyboard focus. |

`BorderlessFullScreen` (a borderless window covering the screen) switches in and out faster than exclusive `FullScreen` and plays better with other windows, so it is usually the better choice for a fullscreen toggle.

Some window managers (notably on Linux) treat window position and borders slightly differently; see the notes in the [IDisplay](xref:Yak2D.IDisplay) API reference.

## When the window size changes

Resizing (by the user, by [SetWindowResolution()](xref:Yak2D.IDisplay.SetWindowResolution*), or by going fullscreen) recreates the window's render target. You receive `WindowWasResized` and `SwapChainFramebufferReCreated` [messages](lifecycle.md#framework-messages).

- **Cameras** need nothing: their virtual resolution is independent of the window, so the picture simply scales (see [Cameras](cameras.md#virtual-resolution)). If the window's shape (aspect ratio) changes, the picture stretches; adjust the virtual resolution if you want to avoid that.
- **Render targets** you created keep their size. A fixed-size render target pipeline copied to the window at the end scales automatically.
- **Viewports** are in pixels, so recreate any that depend on the window size (see [Viewports](viewports.md#viewports-and-window-size)).
- The `windowRenderTarget` passed to `Rendering()` is always the current one.

## Switching graphics API

[IBackend](xref:Yak2D.IBackend) (`yak.Backend`) tells you which graphics API is in use and can switch to another while the application runs:

```csharp
if (yak.Backend.IsGraphicsApiSupported(GraphicsApi.Vulkan))
{
    yak.Backend.SetGraphicsApi(GraphicsApi.Vulkan);
}
```

Switching recreates the graphics device and the window, so:

- **every yak2D resource is lost**: handle `GraphicsDeviceRecreated` by recreating them (see [Device resets](lifecycle.md#device-resets));
- you also receive `ApplicationWindowClosing` twice during the switch. Don't treat that message as "quit" if you allow API switching.

Which APIs are available depends on the platform:

| Platform | Available |
|---|---|
| Windows | Direct3D 11, Vulkan, OpenGL |
| Linux | Vulkan, OpenGL |
| macOS | OpenGL, Vulkan (via MoltenVK). The Metal backend is currently disabled. |

Most applications never need to switch at runtime. It is mainly useful for a "graphics API" option in a settings menu, and for testing.

## See also

- Sample: [Window_ChangingWindowProperties](https://github.com/AlzPatz/yak2d-samples/tree/master/src/Window_ChangingWindowProperties)
- [Configuration](configuration.md)
