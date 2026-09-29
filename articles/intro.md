---
uid: uid_intro
---

# The yak2D Manual

**yak2D** is a small C# framework for making 2D games and interactive graphics applications that run on Windows, Linux and macOS. You write a class, hand it to the framework, and yak2D runs the window, the game loop, input, and a flexible GPU rendering pipeline for you. There is no editor: everything is done in code.

![Yak Run, the tutorial game](../images/yakrun/part4.png)

*[Yak Run](../tutorials/yakrun-1.md), the game built step by step in the tutorials.*

## How these docs are organised

| Section | Use it to... |
|---|---|
| **[Tutorials](../tutorials/index.md)** | Learn by building. Start here if you are new. |
| **Manual: Core concepts** | Understand how a yak2D application is put together. Short, and worth reading once. |
| **Manual: Guides** | Learn a feature area in depth (drawing, cameras, effects, input...), with working examples. |
| **[API Reference](../api/index.md)** | Look up every type, method and parameter. Generated from the source code. |
| **[Samples](samples.md)** | Browse ~35 small, runnable demo projects, one per feature. |

## A suggested learning path

1. **[Getting Started](../tutorials/gettingstarted.md)**: install .NET, create a project and open a window (10 minutes).
2. **[How yak2D works](overview.md)**: the handful of ideas the whole framework is built on.
3. **[Yak Run tutorial](../tutorials/yakrun-1.md)**: build a small platform game in four parts, using most of yak2D's features along the way.
4. Dip into the **Guides** as you need them, and keep the **API Reference** open while you code.

## Conventions used in these docs

- Code examples are taken from real projects that are compiled every time the documentation is published, so they always match the current version of yak2D. Each guide links to the complete source.
- Most guide examples derive from a tiny [`Example` base class](https://github.com/AlzPatz/yak2d-docs-content/blob/master/code/Snippets/Example.cs) that fills in empty [IApplication](xref:Yak2D.IApplication) methods, so the examples only show the code that matters. Your own applications implement [IApplication](xref:Yak2D.IApplication) directly.
- **Units**: 2D positions and sizes are in *world units* or *screen units* (see [Coordinate systems](coordinatesystems.md)), not necessarily pixels. Angles are in **radians** unless a parameter name says otherwise.
- **Colours** are [Colour](xref:Yak2D.Colour) values with red, green, blue and alpha components from 0 to 1. There are also around 140 named colours (`Colour.CornflowerBlue`, `Colour.Gold`, ...).
- British spelling is used throughout the API (`Colour`, `Normalise`). An American-spelled [Color](xref:Yak2D.Color) helper class exists as well.

## Getting help

If something in yak2D or these docs doesn't work the way you expect, check [Troubleshooting and FAQ](troubleshooting.md), then [raise an issue on GitHub](https://github.com/AlzPatz/yak2d/issues).
