"""Generates M3UPlaylistHelper/Resources/AppIcon.ico (and a preview PNG).

Run with: python3 tools/make_icon.py [preview.png]
Each size is drawn separately (supersampled, then downscaled) so small sizes stay crisp,
and the smallest sizes use a simplified mark with fewer, thicker lines.
"""

import os
import sys

from PIL import Image, ImageDraw

SIZES = [16, 20, 24, 32, 40, 48, 64, 96, 128, 256]
SUPERSAMPLE = 8

TOP_LEFT = (37, 99, 235)  # blue
BOTTOM_RIGHT = (124, 58, 237)  # violet
WHITE = (255, 255, 255, 255)


def gradient(size):
    """Diagonal gradient from the top-left color to the bottom-right color."""
    small = Image.new("RGBA", (64, 64))
    pixels = small.load()
    for y in range(64):
        for x in range(64):
            t = (x + y) / 126
            pixels[x, y] = tuple(round(a + (b - a) * t) for a, b in zip(TOP_LEFT, BOTTOM_RIGHT)) + (255,)
    return small.resize((size, size), Image.BILINEAR)


# Hand-placed layouts for the smallest sizes, in pixels, so every line sits on whole pixel rows.
# bars: (left, top, right, bottom); triangle: (left, top, bottom, tip_x)
SMALL_LAYOUTS = {
    16: {"bars": [(3, 3, 13, 5), (3, 6, 13, 8), (3, 10, 8, 12)], "triangle": (9, 8.6, 13.4, 13.2)},
    20: {"bars": [(4, 4, 16, 6), (4, 8, 16, 10), (4, 12, 10, 14), (4, 15, 10, 17)], "triangle": (11.5, 11, 18, 16.6)},
    24: {"bars": [(5, 5, 19, 7), (5, 9, 19, 11), (5, 13, 11, 15), (5, 17, 11, 19)], "triangle": (12.8, 12.5, 19.5, 19)},
}


def layout(size):
    """Two long lines, two short lines with a play triangle beside them; snapped to whole pixels."""
    if size in SMALL_LAYOUTS:
        return SMALL_LAYOUTS[size]

    thickness = max(2, round(size * 0.08))
    left, right, short_end = round(size * 0.23), round(size * 0.77), round(size * 0.47)
    tops = [round(size * c - thickness / 2) for c in (0.27, 0.42, 0.57, 0.72)]
    bars = [(left, top, right if i < 2 else short_end, top + thickness) for i, top in enumerate(tops)]

    cy = (tops[2] + tops[3] + thickness) / 2
    height = size * 0.26
    triangle = (right - height * 0.87, cy - height / 2, cy + height / 2, right)
    return {"bars": bars, "triangle": triangle}


def render(size):
    s = size * SUPERSAMPLE
    image = Image.new("RGBA", (s, s), (0, 0, 0, 0))

    # Rounded tile; a little inset at large sizes, full bleed at small ones where every pixel counts
    inset = 0 if size <= 32 else round(s * 0.03)
    radius = round(s * 0.22)
    mask = Image.new("L", (s, s), 0)
    ImageDraw.Draw(mask).rounded_rectangle((inset, inset, s - 1 - inset, s - 1 - inset), radius=radius, fill=255)
    image.paste(gradient(s), (0, 0), mask)

    draw = ImageDraw.Draw(image)
    spec = layout(size)

    for left, top, right, bottom in spec["bars"]:
        box = [v * SUPERSAMPLE for v in (left, top, right, bottom)]
        box[2] -= 1
        box[3] -= 1
        draw.rounded_rectangle(box, radius=(bottom - top) * SUPERSAMPLE / 2, fill=WHITE)

    t_left, t_top, t_bottom, t_tip = (v * SUPERSAMPLE for v in spec["triangle"])
    draw.polygon([(t_left, t_top), (t_left, t_bottom), (t_tip, (t_top + t_bottom) / 2)], fill=WHITE)

    return image.resize((size, size), Image.LANCZOS)


def main():
    root = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
    out_dir = os.path.join(root, "M3UPlaylistHelper", "Resources")
    os.makedirs(out_dir, exist_ok=True)

    images = [render(size) for size in SIZES]
    largest = images[-1]
    largest.save(
        os.path.join(out_dir, "AppIcon.ico"),
        format="ICO",
        sizes=[(i.width, i.height) for i in images],
        append_images=images[:-1],
    )
    largest.save(os.path.join(out_dir, "AppIcon.png"))

    if len(sys.argv) > 1:
        # Preview: every size, on a light and a dark strip
        width = sum(SIZES) + 20 * (len(SIZES) + 1)
        preview = Image.new("RGBA", (width, 2 * (256 + 40)), (255, 255, 255, 255))
        ImageDraw.Draw(preview).rectangle((0, 296, width, 2 * 296), fill=(32, 32, 32, 255))
        for strip in range(2):
            x = 20
            for image in images:
                preview.paste(image, (x, strip * 296 + 20 + (256 - image.height)), image)
                x += image.width + 20
        preview.save(sys.argv[1])


if __name__ == "__main__":
    main()
