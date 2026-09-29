# yak2D documentation source

The source for the [yak2D documentation site](https://alzpatz.github.io/yak2d-docs/), built with [DocFX](https://dotnet.github.io/docfx/).

## Layout

| Folder | Contents |
|---|---|
| `index.md`, `toc.yml` | Home page and top navigation |
| `articles/` | The Manual: core concepts, guides, help |
| `tutorials/` | Getting Started, Yak Run (4 parts), custom shader, distribution |
| `api/` | API reference landing page. The rest is generated from the yak2D source's XML comments |
| `images/` | Diagrams and screenshots (`guide/`, `yakrun/`, `tutorials/` are generated, see below) |
| `code/` | Every code example in the docs, as real projects (see below) |

## Code examples

Pages pull code from real projects with DocFX includes, e.g. `[!code-csharp[](../code/Snippets/Drawing.cs#layers)]`, where `#layers` refers to a `#region layers` in the file. The publishing workflow builds every project in `code/` first, so an example that no longer compiles against the latest Yak2D package fails the docs build instead of going out of date.

| Project | Used by |
|---|---|
| `code/GettingStarted/MyFirstYakApp` | Getting Started tutorial |
| `code/YakRun/Part1` ... `Part4` | Yak Run tutorial: a complete snapshot per part. `Assets/` holds the source textures |
| `code/Snippets` | The Manual's guide examples. `dotnet run -- <ExampleName>` runs one; no argument lists them |
| `code/CustomShader` | Custom shader tutorial |
| `code/DocsTools/Harness` | Not published. Wraps any example app, scripts keyboard input and saves a PNG of a frame |
| `code/DocsTools/capture.sh` | Regenerates all screenshots (needs a desktop session) |
| `code/DocsTools/make_yakrun_assets.py` | Regenerates the Yak Run textures (needs Python + Pillow) |

## Building locally

```bash
dotnet tool install -g docfx

# API metadata (needs the yak2d repo checked out alongside this one, built in Release):
#   cd ../yak2d/src && dotnet build Yak2D/Yak2D.csproj -c Release -p:GenerateDocumentationFile=true -p:CopyLocalLockFileAssemblies=true
# then create api-metadata.json as in .github/workflows/build-documentation.yml and run:
docfx metadata api-metadata.json

docfx build docfx.json --serve   # http://localhost:8080
```

## Publishing

`.github/workflows/build-documentation.yml` runs on every push to `master` here, and whenever yak2d's CI publishes a release (via `repository_dispatch`). It builds the code examples, generates the API metadata from yak2d's `master`, builds the site, and pushes it to [AlzPatz/yak2d-docs](https://github.com/AlzPatz/yak2d-docs), which GitHub Pages serves.
