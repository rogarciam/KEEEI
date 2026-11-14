"""Une los PNG de gif_carcasa.py en un GIF animado en bucle.

Uso: python modelos-3d/render/frames_a_gif.py <carpeta_frames> <salida.gif> [ms_por_frame]
"""
import sys
from pathlib import Path

from PIL import Image

carpeta, salida = Path(sys.argv[1]), Path(sys.argv[2])
ms = int(sys.argv[3]) if len(sys.argv) > 3 else 80
frames = [Image.open(p).convert("RGB") for p in sorted(carpeta.glob("f*.png"))]
paleta = frames[len(frames) // 2].quantize(colors=128, method=Image.Quantize.MEDIANCUT)
cuadros = [f.quantize(palette=paleta, dither=Image.Dither.FLOYDSTEINBERG) for f in frames]
salida.parent.mkdir(parents=True, exist_ok=True)
cuadros[0].save(salida, save_all=True, append_images=cuadros[1:], duration=ms, loop=0, optimize=True)
print(f"{salida} ({len(cuadros)} frames, {salida.stat().st_size // 1024} kB)")
