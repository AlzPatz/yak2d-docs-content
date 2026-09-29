#!/usr/bin/env python3
"""Generates the Yak Run tutorial textures (run from the docs repo root).
The yak itself is the yak2D logo artwork. Grass and soil are procedurally drawn and tile seamlessly."""
import random
from PIL import Image, ImageDraw, ImageFilter

OUT = "code/YakRun/Assets/Textures"
random.seed(2026)

# Yak sprite: the logo, scaled down (keeps its transparency)
# A transparent border is left round the edge: with Wrap texture sampling, pixels on one edge are
# filtered together with the opposite edge, which would otherwise show as dark specks
yak = Image.open("images/logo_big.png").convert("RGBA")
canvas = Image.new("RGBA", (256, 228), (0, 0, 0, 0))
yak = yak.resize((244, round(244 * yak.height / yak.width)), Image.LANCZOS)
canvas.paste(yak, ((256 - yak.width) // 2, (228 - yak.height) // 2))
canvas.save(f"{OUT}/yak.png")

# Soil: 128x128, tiles in both directions
S = 128
soil = Image.new("RGBA", (S, S), (120, 78, 45, 255))
px = soil.load()
for y in range(S):
    for x in range(S):
        n = random.randint(-10, 10)
        r, g, b, a = px[x, y]
        px[x, y] = (r + n, g + n, b + n // 2, 255)
d = ImageDraw.Draw(soil)
for _ in range(26):
    cx, cy = random.randint(0, S), random.randint(0, S)
    rx, ry = random.randint(3, 8), random.randint(2, 6)
    shade = random.randint(70, 110)
    colour = (shade + 40, shade + 25, shade, 255)
    for ox in (-S, 0, S):
        for oy in (-S, 0, S):
            d.ellipse([cx - rx + ox, cy - ry + oy, cx + rx + ox, cy + ry + oy], fill=colour, outline=(50, 32, 20, 255))
soil.save(f"{OUT}/soil.png")

# Grass: 128x40 strip, tiles horizontally, transparent above the blade tips
G = 40
grass = Image.new("RGBA", (S, G), (0, 0, 0, 0))
d = ImageDraw.Draw(grass)
d.rectangle([0, 18, S, G], fill=(76, 153, 58, 255))
for i in range(60):
    x = random.randint(0, S)
    h = random.randint(8, 18)
    w = random.randint(2, 4)
    shade = random.randint(-25, 25)
    colour = (70 + shade, 150 + shade, 55 + shade // 2, 255)
    for ox in (-S, 0, S):
        d.polygon([(x - w + ox, 20), (x + w + ox, 20), (x + random.randint(-3, 3) + ox, 20 - h)], fill=colour)
d.rectangle([0, 34, S, G], fill=(58, 118, 44, 255))
grass.save(f"{OUT}/grass.png")
print("done")
