---
uid: uid_tutorials
---

# Tutorials

The tutorials build real, working programs step by step. Each one's finished code is in the [documentation repository](https://github.com/AlzPatz/yak2d-docs-content/tree/master/code), compiled every time these docs are published.

## Getting Started

**[Getting Started](gettingstarted.md)**: install the .NET SDK on Windows, Linux or macOS, create a project, add yak2D and open your first window. *About 10 minutes.*

## Yak Run: a platform game

[![Yak Run](../images/yakrun/part4.png)](yakrun-1.md)

A four part tutorial that builds a small, complete platform game: a yak runs and jumps across floating platforms, collecting glowing stars on the way to the finishing flag. It covers most of what you need to know to make your own 2D game with yak2D, without any clever framework code of its own.

| Part | You build | You learn |
|---|---|---|
| [1. A yak on screen](yakrun-1.md) | The project and a yak standing on the ground | Embedded textures, draw stages, cameras, textured and coloured quads, depth |
| [2. Running and jumping](yakrun-2.md) | A yak you can control, with gravity and collisions | Keyboard and gamepad input, fixed update steps, simple physics, sprite animation |
| [3. Building a level](yakrun-3.md) | A scrolling level with scenery, stars and a finish flag | Tiled textures, custom polygons, world and screen space, a following camera, parallax, text |
| [4. Effects and polish](yakrun-4.md) | Glowing stars, ripples, and a fade when you fall | Render targets, post-processing pipelines, bloom, distortion, colour effect transitions |

## Topic tutorials

- **[Writing a custom shader](customshader.md)**: write a GLSL fragment shader that runs on every graphics API.
- **[Distributing your game](distribution.md)**: package your game for Windows, Linux and macOS.

## After the tutorials

The **[Manual](../articles/intro.md)** explains every feature in depth, and the **[samples](../articles/samples.md)** show each one working on its own.
