---
uid: uid_resources
---

# Resources and references

## What is a resource?

A **resource** is anything yak2D creates and manages for you, usually because it lives on the GPU:

| Resource | Reference type | Created with |
|---|---|---|
| Texture | [ITexture](xref:Yak2D.ITexture) | `yak.Surfaces.LoadTexture()`, `CreateRgbaFromData()`, `CreateFloat32FromData()` |
| Render target | [IRenderTarget](xref:Yak2D.IRenderTarget) (also an [ITexture](xref:Yak2D.ITexture)) | `yak.Surfaces.CreateRenderTarget()` |
| Render stage | [IDrawStage](xref:Yak2D.IDrawStage), [IBloomStage](xref:Yak2D.IBloomStage), ... (all [IRenderStage](xref:Yak2D.IRenderStage)) | `yak.Stages.Create...Stage()` |
| Viewport | [IViewport](xref:Yak2D.IViewport) | `yak.Stages.CreateViewport()` |
| Camera | [ICamera2D](xref:Yak2D.ICamera2D), [ICamera3D](xref:Yak2D.ICamera3D) | `yak.Cameras.CreateCamera2D()`, `CreateCamera3D()` |
| Font | [IFont](xref:Yak2D.IFont) | `yak.Fonts.LoadFont()` |
| Persistent queue | [IPersistentDrawQueue](xref:Yak2D.IPersistentDrawQueue), [IPersistentDistortionQueue](xref:Yak2D.IPersistentDistortionQueue) | `draw.CreatePersistentDrawQueue()` |

## References and ids

The objects you get back are lightweight **references**. Every one of them implements [IKeyed](xref:Yak2D.IKeyed), which has a single property: a `ulong` [Id](xref:Yak2D.IKeyed.Id). yak2D holds the real object and looks it up by that id.

Each resource type has its own reference interface, so the compiler stops you passing, say, a camera where a texture is expected. Almost every yak2D method also has an overload that takes the raw `ulong` id instead, which can be convenient if you store ids in your own data structures:

```csharp
ulong textureId = texture.Id;
draw.Helpers.DrawTexturedQuad(drawStage.Id, CoordinateSpace.World, textureId, Colour.White, position, 64, 64, 0.5f);
```

If you have an id and need an [ITexture](xref:Yak2D.ITexture) reference back (for example for a [DrawRequest](xref:Yak2D.DrawRequest)), use [IDrawing.WrapTextureId()](xref:Yak2D.IDrawing.WrapTextureId*).

> [!WARNING]
> Ids are just numbers. If you pass the id of one kind of resource to a method expecting another, yak2D cannot tell at compile time; it will report an error in the console when it fails to find the resource. Prefer the typed references unless you have a reason not to.

## When to create resources

Create resources in [CreateResources()](xref:Yak2D.IApplication.CreateResources*). You *can* create them later (for example when a level loads), but everything must be re-creatable if the graphics device is reset, and the simplest way to guarantee that is to keep all creation in one method that can be called again:

[!code-csharp[](../code/Snippets/Application.cs#recreation)]

After a device reset, every old reference is invalid. The new resources have new ids.

## Destroying resources

Resources you no longer need can be destroyed, which frees their GPU memory:

| To destroy... | Call |
|---|---|
| A texture or render target | `yak.Surfaces.DestroySurface(surface)` |
| All of your textures and render targets | `yak.Surfaces.DestoryAllUserSurfaces()` *(sic)*, `DestroyAllUserTextures()`, `DestroyAllUserRenderTargets()` |
| A render stage | `yak.Stages.DestroyStage(stage)`, or `DestroyAllStages()` |
| A viewport | `yak.Stages.DestroyViewport(viewport)`, or `DestroyAllViewports()` |
| A camera | `yak.Cameras.DestroyCamera(camera)`, `DestroyAllCameras()`, `DestroyAllCameras2D()`, `DestroyAllCameras3D()` |
| A font | `yak.Fonts.DestroyFont(font)`, or `DestroyAllUserFonts()` |
| A persistent draw queue | `draw.RemovePersistentDrawQueue(stage, queue)` |

Destruction is **deferred** until the current frame has finished rendering, so it is safe to destroy something that is still used in this frame's render queue. It is good practice to destroy things in `PreDrawing()` or `Drawing()`, close to the rendering they affect.

You do not need to destroy anything at shutdown; yak2D releases everything itself.

## Counting resources

For debugging, the services report how many resources exist: [ISurfaces.TotalUserSurfaceCount](xref:Yak2D.ISurfaces.TotalUserSurfaceCount), [UserTextureCount](xref:Yak2D.ISurfaces.UserTextureCount), [UserRenderTargetCount](xref:Yak2D.ISurfaces.UserRenderTargetCount), [IStages.CountRenderStages](xref:Yak2D.IStages.CountRenderStages), [CountViewports](xref:Yak2D.IStages.CountViewports), [ICameras.Camera2DCount](xref:Yak2D.ICameras.Camera2DCount), [Camera3DCount](xref:Yak2D.ICameras.Camera3DCount) and [IFonts.UserFontCount](xref:Yak2D.IFonts.UserFontCount). A number that keeps growing usually means something is being created every frame by mistake.

The [FrameworkItems_CreationAndDestruction](https://github.com/AlzPatz/yak2d-samples/tree/master/src/FrameworkItems_CreationAndDestruction) sample creates and destroys every kind of resource interactively.
