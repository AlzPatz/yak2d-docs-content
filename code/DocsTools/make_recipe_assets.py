#!/usr/bin/env python3
"""Generates the sprite sheet used by the Cookbook's animation recipe (needs Pillow).

Writes code/Snippets/Textures/coin.png: eight 64 x 64 frames of a spinning coin, side by side.
Run from anywhere: python3 code/DocsTools/make_recipe_assets.py
"""
import math
from pathlib import Path

from PIL import Image, ImageDraw

FRAME = 64
FRAMES = 8
SCALE = 4  # draw large, then shrink, for smooth edges

out = Path(__file__).resolve().parent.parent / "Snippets" / "Textures" / "coin.png"
sheet = Image.new("RGBA", (FRAME * FRAMES * SCALE, FRAME * SCALE), (0, 0, 0, 0))
draw = ImageDraw.Draw(sheet)

for n in range(FRAMES):
    # Half a turn over the eight frames: a full-width face, narrowing to an edge and back
    half_width = max(0.08, abs(math.cos(math.pi * n / FRAMES))) * 26 * SCALE
    cx = (n * FRAME + FRAME / 2) * SCALE
    cy = FRAME / 2 * SCALE
    r = 26 * SCALE
    draw.ellipse([cx - half_width, cy - r, cx + half_width, cy + r], fill=(176, 120, 20, 255))
    inner = max(0.0, half_width - 4 * SCALE)
    draw.ellipse([cx - inner, cy - r + 4 * SCALE, cx + inner, cy + r - 4 * SCALE], fill=(255, 196, 40, 255))
    if inner > 10 * SCALE:
        # A highlight, and a raised star-ish mark that squashes with the face
        draw.ellipse([cx - inner * 0.55, cy - r * 0.6, cx - inner * 0.15, cy - r * 0.2], fill=(255, 236, 150, 255))
        mark = inner * 0.35
        draw.rectangle([cx - mark * 0.25, cy - r * 0.45, cx + mark * 0.25, cy + r * 0.45], fill=(214, 150, 24, 255))

sheet = sheet.resize((FRAME * FRAMES, FRAME), Image.LANCZOS)
sheet.save(out)
print(f"Saved {out}")
