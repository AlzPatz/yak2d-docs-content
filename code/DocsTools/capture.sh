#!/usr/bin/env bash
# Regenerates the documentation screenshots by running the documentation code through the capture harness.
# Run from anywhere: code/DocsTools/capture.sh [filter]
# Needs a desktop session (a window opens briefly for each capture).
set -euo pipefail
cd "$(dirname "$0")/Harness"
DOCS="$(cd ../../.. && pwd)"
OUT="$DOCS/images"
FILTER="${1:-}"

# target | app | seconds | output (relative to images/) | extra harness arguments
CAPTURES=(
  "GettingStarted/MyFirstYakApp|MyApplication|1|tutorials/gettingstarted.png|"
  "CustomShader|WavyYak|1.5|tutorials/customshader.png|"
  "YakRun/Part1|Game|1|yakrun/part1.png|"
  "YakRun/Part2|Game|1.45|yakrun/part2.png|--script runjump"
  "YakRun/Part3|Game|7.0|yakrun/part3.png|--script yakrun"
  "YakRun/Part4|Game|2.95|yakrun/part4.png|--script yakrun"
  "YakRun/Part4|Game|5.6|yakrun/part4-fall.png|--script fall"
  "YakRun/Part4|Game|16|yakrun/part4-finish.png|--script yakrun"
  "Snippets|DrawingBasics|1|guide/drawing-basics.png|"
  "Snippets|LayersAndDepth|1|guide/layers-depth.png|"
  "Snippets|DrawRequests|1|guide/draw-requests.png|"
  "Snippets|FluentShapes|1|guide/fluent-shapes.png|"
  "Snippets|TextureModes|1|guide/texture-modes.png|"
  "Snippets|TextExample|1|guide/text.png|"
  "Snippets|CameraExample|3|guide/camera.png|"
  "Snippets|SplitScreen|1|guide/split-screen.png|"
  "Snippets|MousePicking|1|guide/mouse-picking.png|--mouse 620,210"
  "Snippets|ColourEffectsExample|1|guide/effects-colour.png|"
  "Snippets|BloomExample|1|guide/effects-bloom.png|"
  "Snippets|BlurExample|1|guide/effects-blur.png|"
  "Snippets|StyleEffectsExample|1.5|guide/effects-style.png|"
  "Snippets|MixExample|1|guide/effects-mix.png|"
  "Snippets|DistortionExample|2.5|guide/effects-distortion.png|"
  "Snippets|PixelArt|1|guide/pixel-art.png|"
  "Snippets|TextureFromData|1|guide/texture-from-data.png|"
  "Snippets|SurfaceReadback|1|guide/readback.png|--mouse 300,270"
  "Snippets|MeshExample|1.2|guide/mesh.png|"
  "Snippets|AssetLoading|1|guide/assets.png|"
)

build_for() {
  dotnet build -c Release -p:DocsTarget="../../$1" -v q -nologo > /dev/null
}

current=""
for entry in "${CAPTURES[@]}"; do
  IFS='|' read -r target app seconds output extra <<< "$entry"
  if [[ -n "$FILTER" && "$output" != *"$FILTER"* ]]; then continue; fi
  if [[ "$target" != "$current" ]]; then
    echo "Building harness for $target"
    build_for "$target"
    current="$target"
  fi
  mkdir -p "$(dirname "$OUT/$output")"
  # shellcheck disable=SC2086
  timeout 90 dotnet bin/Release/net10.0/Harness.dll --app "$app" --seconds "$seconds" --out "$OUT/$output" --scale-to 800 $extra \
    | grep -E "Saved|Timed out|Exception" || echo "FAILED: $output"
done

# Shrink the PNGs a little
python3 - "$OUT" <<'PY'
import sys, pathlib
from PIL import Image
for p in pathlib.Path(sys.argv[1]).glob("**/*.png"):
    if p.parent.name in ("guide", "yakrun", "tutorials"):
        im = Image.open(p)
        im.save(p, optimize=True)
PY
echo "Done"
