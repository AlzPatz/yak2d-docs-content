---
uid: uid_coordinatesystems
---

# **yak2D** Coordinate Systems, Resolutions and Rendering

## A RenderStage's output is render area size agnostic

A [RenderStage](xref:uid_renderstages)'s output will fill the entire destination [render area](xref:uidglossary#RenderArea), which is either an entire [RenderTarget](xref:uid_rendertargets) or a rectangular area of one defined by a [viewport](xref:uid_viewports). 

This means that rendering operations are 'resolution agnostic'. The final size of the [render area](xref:uidglossary#RenderArea) does not matter. 

If the aspect ratio of the destination [render area](xref:uidglossary#RenderArea) does not match that of the [RenderStage](xref:uid_renderstages)'s output, the output will be distorted (stretched or squashed) to fit.

## Drawing

During [Drawing](xref:uid_glossary#Drawing), [DrawRequests](xref:Yak2D.DrawRequest) are submitted to, and rendered by, a [DrawStage](xref:Yak2D.IDrawStage). The final position of vertices upon the [render area](xref:uidglossary#RenderArea) is transformed by the assigned [Camera2D](xref:Yak2D.ICamera2D).

Each [Camera2D](xref:Yak2D.ICamera2D) has a [virtual resolution](xref:uid_glossary#VirtualResolution) that defines the boundaries of [screen space](xref:uid_coordinatesystems#screenspace) as well as [world space](xref:uid_coordinatesystems#worldspace) once a camera's zoom, world focus point and rotation are also accounted for.

## What this means in practice

A user can choose a [virtual resolution](xref:uid_glossary#VirtualResolution) and draw everything in relation to these effective screen dimensions, but it can be rendered at a final real resolution chosen simply by the an appropriately sized [render area](xref:uidglossary#RenderArea).  


## Coordinate Systems (***Spaces***)

**yak2D** uses 3 coordinate systems (or ***spaces***) for 2D rendering:

1. [**Screen**](xref:uid_coordinatesystems#screen-space) space
2. [**World**](xref:uid_coordinatesystems#world-space) space
3. [**Window**](xref:uid_coordinatesystems#window-space) space

### **Screen Space**
The coordinate system used when drawing with [CoordinateSpace.Screen](xref:Yak2D.CoordinateSpace). Positions are defined in relation to a fixed visible area defined by the camera's [virtual resolution](xref:uid_glossary#VirtualResolution).

The origin (0,0) is located in the centre. Positive X-axis runs from left to right, with positive Y-axis running 'upwards' towards the top of the visible area.

Visible positions in screen space are defined by:

<img src="https://render.githubusercontent.com/render/math?math=-w/2 \geq x < w/2">

and...

<img src="https://render.githubusercontent.com/render/math?math=-h/2 \geq y < h/2">

where:
- w = [virtual resolution](xref:uid_glossary#VirtualResolution) width
- h = [virtual resolution](xref:uid_glossary#VirtualResolution) height

#### Screen Space Diagram
![](../images/screenspace.png)

### **World Space**
The coordinate system used when drawing with [CoordinateSpace.World](xref:Yak2D.CoordinateSpace). Positions are defined in relation to an origin. The visible area is defined by the camera's world focus point (position) in relation to the origin, the camera's zoom scalar, rotation and [virtual resolution](xref:uid_glossary#VirtualResolution).

World space's axis are similar to screen space, in that when unrotated, positive X-axis runs from left to right, with positive Y-axis running 'upwards' towards the top of the visible area.

Therefore, if the camera's world focus point is (0,0), it's zoom is 1.0 and there is no camera rotation, then a position in world space will match a position in screen space.

### **Window Space**

Next typing

## Viewports

## 3D Rendering

## Mouse Input

