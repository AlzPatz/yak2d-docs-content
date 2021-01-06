---
uid: uid_overview
---
# **yak2D** Overview and Getting Started

## Overview

A framework that simplifies the creation of interactive cross platform desktop 2D graphics applications.

It provides 2D polygon sorting and drawing functions, in addition to shader effects. Bitmap Font Rendering is also provided.

**yak2D** offers basic input functionality (mouse & keyboard, as well as gamepad).

Audio functionality is ***not*** provided by the framework.

## Structure
For those that care about such definitions, **yak2D** is described (and structured) as a *framework* rather than a *library*. This structure is suitable given the control that **yak2D** must have over the [application lifecycle](xref:uid_lifecycle) - in particular, the timing and sequencing of **update** and **render** steps. 

The framework accepts an instance of the user's implementation of the [IApplication](xref:Yak2D.IApplication) interface and calls its methods during the [application lifecycle](xref:uid_lifecycle).

## Rendering

**yak2D** offers simple and flexible configuration of the render path, enabling complex 2D effects to be quickly implemented.

For each render frame, a render queue is built out of [RenderStages](xref:uid_renderstages). A [RenderStage](xref:uid_renderstages) provides a specific render function, (usually) taking [Surfaces](xref:uid_surfaces) as input and a [RenderTarget](xref:uid_render_targets) as output.

[Drawing](xref:uid_glossary#Drawing) in **yak2D** refers to the collection, sorting, transforming and ultimately rendering of coloured and textured polygons (here is where you draw your 'sprites'...) upon a [RenderTarget](xref:uid_render_targets). [Drawing](xref:uid_glossary#Drawing) functionality is encompassed within a special type of [RenderStage](xref:uid_renderstages). Prior to rendering, [DrawRequests](xref:Yak2D.DrawRequest), must be submitted to a [DrawStage](xref:Yak2D.IDrawStage). When rendering, a [Camera](xref:uid_glossary#Cameras) defines the transformation of the positions of [DrawRequests](xref:Yak2D.DrawRequest). [IApplication](xref:Yak2D.IApplication) exposes specific [Drawing](xref:uid_glossary#Drawing) methods that run before [Rendering](xref:Yak2D.IApplication.Render) for the submission of [DrawRequests](xref:Yak2D.DrawRequest), as well as the configuration of [Cameras](xref:uid_glossary#Cameras) and the general configuration of [RenderStages](xref:uid_renderstages).

## Core Loop

A user must define three main parts of a **yak2D** application's core loop:

1. Update 
    - Designed for non-graphical application code, such as simulation updates. Fixed and Variable timesteps may be chosen for updates.
2. Draw (and Pre-Draw)
    - Used for submitting [Draw Requests](xref:Yak2D.DrawRequest) to  [DrawStages](xref:Yak2D.IDrawStage) and configuring [RenderStages](xref:uid_renderstages)
3. Render
    - For building the render queue by ordering [RenderStages](xref:uid_renderstages) and defining the input and output [Surfaces](xref:uid_surfaces) for each step

## Resources

**yak2D** manages the creation and destruction of all objects related to framework usage ([Textures](xref:Yak2D.ITexture), [Cameras](xref:Yak2D.ICamera2D), [Render Stages](xref:Yak2D.IRenderStage)... being examples). These objects are called [Resources](xref:uid_glossary#Resources) and are not passed to the user directly. Rather, a user application holds lightweight objects that reference the underlying resource. These lightweight objects are simple wrappers around `int64` keys and in some instances a user may wish to hold only these `int64` keys. Many of **yak2D**'s functions accept either object (derived from [IKeyed](xref:Yak2D.IKeyed)) or the raw int64 keys. The [IKeyed](xref:Yak2D.IKeyed) object hierarchy is used so that C#'s type system ensures a user avoids passing the wrong objects to functions, as well as get instant feedback on object type from intellisense.

[IApplication](xref:Yak2D.IApplication) exposes the method [CreateResources](xref:Yak2D.IApplication.CreateResources) which is where all [Resource](xref:uid_glossary#Resources) creation should be performed. This method is called once at application start up, and then whenever [Resources](xref:uid_glossary#Resources) should be re-created (such as when a graphics device is lost during a Graphics API change).

## Basic Usage

* Create a new .NET core console application
* Add **yak2D** to the project via the Nuget package manager
* Create a class that implements [IApplication](xref:Yak2D.IApplication)
* In your application's `Program.cs` , or somewhere appropriate, pass an instance of your [IApplication](xref:Yak2D.IApplication) implementation to [Launcher.Run()](xref:Yak2D.Launcher.Run)

Please see the [tutorial](xref:uid_tutorial) for a more detailed step by step guide to creating an application.

## Further Reading

Please refer to:

[Glossary of Terms](xref:uid_glossary) for a summary of terms used (hopefully consistently) across **yak2D**'s API and Documentation.

[Framework Features](xref:uid_features) for a detailed overview of **yak2D**'s features.

[Application Lifecycle](xref:uid_lifecycle) for a description of the general flow of an application running on the **yak2D** framework, including a description of when a user's [IApplication](xref:Yak2D.IApplication) method implementations are called.
