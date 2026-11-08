# CLAUDE.md

Guidance for Claude Code (and smith runs) working in this repo.

The DocFX source for the yak2D documentation site (https://alzpatz.github.io/yak2d-docs/).
`README.md` explains the layout, the code example projects, building locally and publishing; read
it first.

## Commits

Commit as the user only: never add `Co-Authored-By: Claude` trailers, "Generated with Claude Code"
lines or any other AI attribution to commit messages or PR descriptions.

## Key rules

- Every code example lives in a real project under `code/` and is pulled into pages with
  `[!code-csharp[](path#region)]`. Change the project, not a pasted copy, and keep the `#region`
  names that pages refer to.
- The examples use the published `Yak2D` package (`1.*`), not yak2d's source, because the docs
  describe the released API. The publishing workflow builds every project in `code/` first, so an
  example that no longer compiles fails the docs build.
- Build the examples from `code/` before committing (each folder holds a normal .NET project).
- A push to `master` publishes the site (`.github/workflows/build-documentation.yml`), and so does
  every yak2d release.
- `docfx.json` publishes every root `*.md` file as a page except those it excludes (`README.md`,
  `CLAUDE.md`). A new root Markdown file that isn't a page must be added to that exclude list.
- `code/DocsTools/capture.sh` regenerates the screenshots and needs a desktop session, so it can't
  run in a smith run or CI.
- Docs and setup instructions must suit Windows, Linux and macOS equally.
