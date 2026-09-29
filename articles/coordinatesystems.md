---
uid: uid_coordinatesystems
---

# Coordinate systems

## RenderStage Output

A RenderStage's output is render area size agnostic. This means that a [RenderStage](xref:uid_renderstages)'s output will fill the entire destination [render area](xref:uid_glossary#renderarea), which is either an entire [RenderTarget](xref:uid_surfaces) or a rectangular area of one defined by a [viewport](xref:uid_viewports). 

The pixel size of the [render area](xref:uid_glossary#renderarea) does not matter. 

However, if the aspect ratio of the destination [render area](xref:uid_glossary#renderarea) does not match that of the [RenderStage](xref:uid_renderstages)'s output, the output will be distorted (stretched or squashed) to fit.

## Drawing

During [Drawing](xref:uid_glossary#drawing), [DrawRequests](xref:Yak2D.DrawRequest) are submitted to, and rendered by, a [DrawStage](xref:Yak2D.IDrawStage). The final position of vertices upon the [render area](xref:uid_glossary#renderarea) is transformed by the assigned [Camera2D](xref:Yak2D.ICamera2D).

Each [Camera2D](xref:Yak2D.ICamera2D) has a [virtual resolution](xref:uid_glossary#virtual-resolution) that defines the boundaries of [screen space](xref:uid_coordinatesystems#screen-space) as well as [world space](xref:uid_coordinatesystems#world-space) once a camera's zoom, world focus point and rotation are also accounted for.

## What this means in practice

A user can choose a [virtual resolution](xref:uid_glossary#virtual-resolution) and draw everything in relation to these effective screen dimensions, but it can be rendered at a final real resolution chosen simply by the an appropriately sized [render area](xref:uid_glossary#renderarea).  

## Coordinate Systems (***spaces***)

**yak2D** uses 3 coordinate systems (or ***spaces***) for 2D rendering:

1. [**Screen**](xref:uid_coordinatesystems#screen-space) space
2. [**World**](xref:uid_coordinatesystems#world-space) space
3. [**Window**](xref:uid_coordinatesystems#window-space) space

### **Screen Space**
The coordinate system used when drawing with [CoordinateSpace.Screen](xref:Yak2D.CoordinateSpace). Positions are defined in relation to a fixed visible area defined by the camera's [virtual resolution](xref:uid_glossary#virtual-resolution).

The origin (0,0) is located in the centre. Positive X-axis runs from left to right, with positive Y-axis running 'upwards' towards the top of the visible area.

Visible positions in screen space are defined by:

`-w/2 <= x < w/2`

and...

`-h/2 <= y < h/2`

where:
- w = [virtual resolution](xref:uid_glossary#virtual-resolution) width
- h = [virtual resolution](xref:uid_glossary#virtual-resolution) height

#### Screen Space Diagram
![](../images/screenspace.png)

### **World Space**
The coordinate system used when drawing with [CoordinateSpace.World](xref:Yak2D.CoordinateSpace). Positions are defined in relation to an origin. The visible area is defined by the camera's world focus point (position) in relation to the origin, the camera's zoom scalar, rotation and [virtual resolution](xref:uid_glossary#virtual-resolution).

**World space axis are similar to screen space**, in that when unrotated, positive X-axis runs from left to right, with positive Y-axis running 'upwards' towards the top of the visible area.

Therefore, if the camera's world focus point is (0,0), it's zoom is 1.0 and there is no camera rotation, then a position in world space will match a position in screen space.

### **Window Space**
Window space is used when defining [viewports](xref:uid_viewports) and is the coordinate system used for [mouse position](xref:Yak2D.IInput.MousePosition).

The origin (0,0) of window space is positioned at the top-left corner of the window (or [RenderTarget](xref:uid_surfaces)), with positive X-axis running from left to right, and positive Y-axis running 'downwards' towards the bottom of the window (or [RenderTarget](xref:uid_surfaces)).

Window space units are absolute pixels.

Valid / visible pixel positions in window space are defined by:

`0 <= x < w`

and...

`0 <= y < h`

where:
- w = Window or [RenderTarget](xref:uid_surfaces) width
- h = Window or [RenderTarget](xref:uid_surfaces) height


#### Window Space Diagram
![](../images/windowspace.png)

## Viewports
[Viewports](xref:uid_viewports) are used to define rectangular [render areas](xref:uid_glossary#renderarea) of a [RenderTarget](xref:uid_surfaces). 

Viewport position and size are defined in pixel units and use [window space](xref:uid_coordinatesystems#window-space) coordinates.

## Texture Coordinates

**yak2D** texture coordinates are origin top-left and do not change across any graphics API backends. This is the same as used in Direct3D, Vulkan and Metal APIs, and is the same as [window space](xref:uid_coordinatesystems#window-space).

**Note:** When writing shaders for a [CustomVeldrid](xref:Yak2D.ICustomVeldridStage) [RenderStage](xref:uid_renderstages), the user is responsible for accounting for the backend differences between graphics APIs.

When writing shaders for a [CustomShader](xref:Yak2D.ICustomShaderStage) [RenderStage](xref:uid_renderstages), the vertex shader provided ***already accounts for the differences*** in OpenGL texture coordinates. Therefore all shaders can be written assuming origin top-left.

## 3D Rendering

The [MeshRender](xref:Yak2D.IMeshRenderStage) [RenderStage](xref:uid_renderstages) uses a [Camera3D](xref:Yak2D.ICamera3D) to manage the required view and projection matrices used during rendering. 

**yak2D** uses a Right-Handed coordinate system, which does not change across any graphics API backends. This coordinate system is positive-x towards the right, positive-y upwards and positive-z out of the screen, towards the camera.

**Note:** When writing shaders for a [CustomVeldrid](xref:Yak2D.ICustomVeldridStage) [RenderStage](xref:uid_renderstages), the user is responsible for accounting for the backend differences between graphics APIs.

## Mouse Input
**yak2D** provides [mouse position](xref:Yak2D.IInput.MousePosition) as a position on the application window, in [window space](xref:uid_coordinatesystems#window-space). Convert it to world or screen space with the helpers below.

## Converting between spaces

[ICoordinateTransforms](xref:Yak2D.ICoordinateTransforms) converts positions between the three spaces for a given camera. It is available as the `transforms` parameter of `Drawing()`, and as `yak.Helpers.CoordinateTransforms` everywhere else.

| Method | Converts |
|---|---|
| [WorldFromScreen](xref:Yak2D.ICoordinateTransforms.WorldFromScreen*), [ScreenFromWorld](xref:Yak2D.ICoordinateTransforms.ScreenFromWorld*) | between world and screen space |
| [WorldFromWindow](xref:Yak2D.ICoordinateTransforms.WorldFromWindow*), [ScreenFromWindow](xref:Yak2D.ICoordinateTransforms.ScreenFromWindow*) | from a window position (such as the mouse) into world or screen space |
| [WindowFromWorld](xref:Yak2D.ICoordinateTransforms.WindowFromWorld*), [WindowFromScreen](xref:Yak2D.ICoordinateTransforms.WindowFromScreen*) | from world or screen space to a window position |

Methods involving window space take an optional [viewport](xref:uid_viewports), for when the camera is rendered into part of the window, and return a [TransformResult](xref:Yak2D.TransformResult): the converted `Position`, plus `Contained`, which says whether the point is actually inside the camera's view (or, converting to window space, inside the window).

A common use is finding what the mouse is pointing at in the game world:

![Mouse picking through a zoomed, rotated camera](../images/guide/mouse-picking.png)

[!code-csharp[](../code/Snippets/TextAndCameras.cs#picking)]

The conversions take the camera's focus, zoom, rotation and virtual resolution into account, and the window's current size, so they keep working when the window is resized.
