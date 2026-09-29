---
uid: uid_glossary
---

# Glossary

Terms used (hopefully consistently) across yak2D's API and documentation.

## Application
Your code: the class implementing [IApplication](xref:Yak2D.IApplication) that you pass to [Launcher.Run()](xref:Yak2D.Launcher.Run*), and everything it uses. See [How yak2D works](overview.md).

## Batch
A group of consecutive draw requests that share textures and settings, sent to the GPU as one draw call. Draw stages batch automatically after sorting. See [Drawing](drawing.md#layers-and-depth).

## Blend state
How a pixel being drawn combines with the pixel already on a surface: normal transparency, additive, or replace. See [Drawing: Blend states](drawing.md#blend-states).

## Camera
Decides what part of the world appears on a surface. A **2D camera** ([ICamera2D](xref:Yak2D.ICamera2D)) has a virtual resolution, focus, zoom and rotation and is used by draw and distortion stages. A **3D camera** ([ICamera3D](xref:Yak2D.ICamera3D)) is used by mesh render stages. See [Cameras](cameras.md).

## Coordinate space
Whether a draw request's positions are in [world space](coordinatesystems.md#world-space) (moved by the camera) or [screen space](coordinatesystems.md#screen-space) (fixed to the view). See [Coordinate systems](coordinatesystems.md).

## Depth
A number from 0.0 (front) to 1.0 (back) that orders drawing within a layer. See [Drawing: Layers and depth](drawing.md#layers-and-depth).

## Draw request
One thing to be drawn: a set of triangles (vertices and indices), how to fill them, and where they sit in the drawing order. See [DrawRequest](xref:Yak2D.DrawRequest) and [Drawing](drawing.md#draw-requests).

## Draw stage
A render stage that collects draw requests, sorts and batches them, and renders them through a 2D camera. See [IDrawStage](xref:Yak2D.IDrawStage) and [Drawing](drawing.md).

## Drawing
The step of each frame in which your application describes what should be drawn, by submitting draw requests to draw stages in [Drawing()](xref:Yak2D.IApplication.Drawing*). Nothing reaches the screen until the draw stages are *rendered*. See [How yak2D works](overview.md#drawing-and-rendering-are-two-different-steps).

## Fill type
Whether a draw request's triangles are filled with colour only, one texture, or two blended textures. See [FillType](xref:Yak2D.FillType).

## Frame
One pass of drawing and rendering, producing one image in the window. Also called a **RenderStep** in these docs. See [Application lifecycle](lifecycle.md).

## Framework message
A notification from yak2D to your application, such as a resized window or a lost graphics device, delivered to [ProcessMessage()](xref:Yak2D.IApplication.ProcessMessage*). See [FrameworkMessage](xref:Yak2D.FrameworkMessage) and [Application lifecycle](lifecycle.md#framework-messages).

## Layer
An integer (0 or more) that orders drawing: higher layers are always drawn on top of lower ones. See [Drawing: Layers and depth](drawing.md#layers-and-depth).

## Reference
The lightweight object (an [ITexture](xref:Yak2D.ITexture), [IDrawStage](xref:Yak2D.IDrawStage), ...) you are given when a resource is created. It holds the resource's `ulong` id. See [Resources and references](resources.md).

## Render queue
The ordered list of GPU operations for a frame, built in [Rendering()](xref:Yak2D.IApplication.Rendering*) through [IRenderQueue](xref:Yak2D.IRenderQueue). See [The render queue and render stages](renderstages.md).

## Render stage
A reusable step of GPU work, such as drawing, bloom or blur, created once and used in the render queue. See [IRenderStage](xref:Yak2D.IRenderStage) and [The render queue and render stages](renderstages.md#render-stages).

## Render target
A surface that can be rendered onto, as well as read from. The window is a render target. See [IRenderTarget](xref:Yak2D.IRenderTarget) and [Surfaces](surfaces.md).

## RenderArea
Either a whole render target, or the part of one set by a viewport. Render stages always fill their render area. There is no object called "RenderArea" in the API.

## Rendering
The step of each frame in which your application builds the render queue in [Rendering()](xref:Yak2D.IApplication.Rendering*), and the GPU work that follows.

## RenderStep
A single frame: the calls to [PreDrawing()](xref:Yak2D.IApplication.PreDrawing*), [Drawing()](xref:Yak2D.IApplication.Drawing*) and [Rendering()](xref:Yak2D.IApplication.Rendering*), in that order, and the GPU work they produce.

## Resource
Anything yak2D creates and owns on your behalf, usually on the GPU: textures, render targets, render stages, cameras, viewports, fonts and persistent queues. You hold references to them; all are lost if the graphics device is reset. See [Resources and references](resources.md).

## Screen space
The coordinate space fixed to a camera's view: (0,0) in the middle, +Y up, edges at plus and minus half the virtual resolution. See [Coordinate systems](coordinatesystems.md#screen-space).

## Service
One of the interfaces on [IServices](xref:Yak2D.IServices) (`yak.Surfaces`, `yak.Stages`, `yak.Input`, ...), through which your application uses yak2D. See [How yak2D works](overview.md#services-everything-you-can-ask-yak2d-to-do).

## Surface
An image on the GPU: a texture or a render target. See [Surfaces](surfaces.md).

## Texture
A surface that can be read (drawn as a sprite, or used as an effect's input) but not rendered onto. See [ITexture](xref:Yak2D.ITexture) and [Surfaces](surfaces.md).

## Texture coordinates
Positions within a texture, from (0,0) at the top-left to (1,1) at the bottom-right, on every graphics API. See [Drawing: Texture coordinates](drawing.md#texture-coordinates-wrapping-and-filtering).

## UpdateStep
A single pass of the update (non-drawing) part of the loop: framework messages are processed and [Update()](xref:Yak2D.IApplication.Update*) is called. See [Application lifecycle](lifecycle.md).

## Vertex
A corner of a triangle, with a position, colour and texture coordinates. See [Vertex2D](xref:Yak2D.Vertex2D) and [Vertex3D](xref:Yak2D.Vertex3D).

## Viewport
A rectangle of a render target that rendering is limited to. See [Viewports](viewports.md).

## Virtual Resolution
The width and height, in units, that a 2D camera shows at zoom 1, independent of the real size of the surface it renders onto. See [Cameras](cameras.md#virtual-resolution).

## Window space
Positions in window (or render target) pixels, from (0,0) at the top-left with +Y down. Used for the mouse position and viewports. See [Coordinate systems](coordinatesystems.md#window-space).

## World space
The coordinate space of the game world, viewed through a camera's focus, zoom and rotation, with +Y up. See [Coordinate systems](coordinatesystems.md#world-space).
