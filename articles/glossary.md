---
uid: uid_glossary
---

# **yak2D** Glossary

Definitions used (hopefully) consistently across **yak2D**'s source code and documentation.

## Application
Refers to user code run with **yak2D**, consisting of the implemenation of the [IApplication](xref:Yak2D.IApplication) interface and supporting code.

## Camera

## Drawing

## Resource
An object created by the **yak2D** framework. Internally, **yak2D** holds an `int64` key for each resouce. A user application holds resource references either in the form of lightweight objects derived from [IKeyed](Yak2D.IKeyed) or simply the underlying `int64` values. All API methods accept the appropriate reference objects, whereas most (but not all) accept just the `int64` keys. Resource examples include [Textures](xref:Yak2D.ITexture), [Cameras](xref:Yak2D.ICamera2D) and [RenderStages](xref:Yak2D.IRenderStage).

## RenderQueue

## RenderStage

## RenderStep
A term used in the **yak2D** documentation that refers to a single iteration of the framework's rending loop. From a user standpoint, a RenderStep will encompass the calling of all [drawing](xref:uid_glossary#drawing) and [rendering](xref:uid_glossary#rendering) methods within the user's implementation of the [IApplication](xref:Yak2D.IApplication) interface, namely: [PreDrawing()](xref:Yak2D.IApplication.PreDrawing), [Drawing()](xref:Yak2D.IApplication.Drawing) and [Rendering](xref:Yak2D.IApplication.Rendering); called in that order.

## RenderArea
A term used in the **yak2D** documentation that refers to either an entire [RenderTarget](xref:uid_rendertargets) or a rectangular area of one as defined by a [viewport](xref:uid_viewports). There are no objects named 'RenderArea' used directly in the **yak2D** API.

## Rendering


## Service


## Surface


## Camera

## Virtual Resolution

## UpdateStep
A term used in the **yak2D** documentation that refers to single iteration of the framework's update (non-rendering) loop. In a single UpdateStep, the [update](xref:Yak2D.IApplication.Update) method of the user's [IApplication](xref:Yak2D.IApplication) implementation is called.





