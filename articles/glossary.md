---
uid: uid_glossary
---

# **yak2D** Glossary

Definitions used (mostly) consistently across **yak2D**'s source code and documentation.

## Application
Refers to user code run with **yak2D**, consisting of the implemenation of the [IApplication](xref:Yak2D.IApplication) interface and supporting code.

## Camera

## Drawing

## Resource
An object created by the **yak2D** framework. Internally, **yak2D** holds an `int64` key for each resouce. A user application holds resource references either in the form of lightweight objects derived from [IKeyed](Yak2D.IKeyed) or simply the underlying `int64` values. All API methods accept the appropriate reference objects, whereas most (but not all) accept just the `int64` keys. Resource examples include [Textures](xref:Yak2D.ITexture), [Cameras](xref:Yak2D.ICamera2D) and [RenderStages](xref:Yak2D.IRenderStage).

## RenderStage

## Render Area

## Service


## Surface


## Camera



