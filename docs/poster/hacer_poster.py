"""Monta el póster A3 de una carcasa: SVG autocontenido + PDF y PNG con Edge.

Uso: python docs/poster/hacer_poster.py <a|b|c>

Lee docs/poster/img/render-<x>.png y render-<x>.json (los genera
modelos-3d/render/poster_render.py en la rama de cada carcasa) y sigue
docs/poster/plan-visual.md: degradado noche → violeta, titular blanco, flecha
azul eléctrico del llavero a la ranura, halo verde en el LED, burbuja de
Telegram, tres pasos, QR del canal y pie con el logo.
"""
import base64
import json
import shutil
import subprocess
import sys
import tempfile
import time
from pathlib import Path

from PIL import Image

AQUI = Path(__file__).resolve().parent
IMG = AQUI / "img"
modelo = sys.argv[1]

W, H = 2970, 4200  # décimas de milímetro: A3 vertical
AZUL, VERDE, ROJO = "#00A3FF", "#22E07A", "#FF4D5E"
FUENTE = "'Segoe UI', 'Inter', 'Helvetica Neue', Arial, sans-serif"

TEXTOS = {
    "a": dict(etiqueta="KEEEI · VERSIÓN DE PARED",
              sub="Mientras haya unas llaves dentro de KEEEI, hay alguien en el cubo."),
    "b": dict(etiqueta="KEEEI · CANTO RODADO",
              sub="Una piedra en la pared que sabe si hay alguien en el cubo."),
    "c": dict(etiqueta="KEEEI · CONOCE A KEKO",
              sub="Keko se come las llaves y lo cuenta en Telegram."),
}[modelo]


def b64(ruta, tipo):
    return f"data:{tipo};base64," + base64.b64encode(Path(ruta).read_bytes()).decode()


render = IMG / f"render-{modelo}.png"
puntos = json.loads((IMG / f"render-{modelo}.json").read_text())
rw, rh = Image.open(render).size

# Zona del render: centrada, entre el titular y los pasos
RH = 2250
RW = RH * rw / rh
RX, RY = (W - RW) / 2, 820


def en_poster(nombre):
    u, v = puntos[nombre]
    return RX + u * RW, RY + v * RH


bx, by = en_poster("boca")
lx, ly = en_poster("llavero")

# Flecha: arco limpio desde la derecha del llavero hasta la boca de la ranura
x0, y0 = lx + 260, min(ly - 40, by - 420)  # aunque el llavero ya esté dentro (B)
x1, y1 = bx + 130, by - 40
cx, cy = max(x0, x1) + 320, (y0 + y1) / 2
flecha = (f'<path d="M {x0:.0f} {y0:.0f} Q {cx:.0f} {cy:.0f} {x1:.0f} {y1:.0f}" fill="none" '
          f'stroke="{AZUL}" stroke-width="26" stroke-linecap="round" marker-end="url(#punta)"/>')
# Líneas de movimiento: estela vertical a la izquierda del llavero que cae
movimiento = "".join(
    f'<line x1="{lx + dx:.0f}" y1="{ly + y0:.0f}" x2="{lx + dx:.0f}" y2="{ly + y1:.0f}" '
    f'stroke="{AZUL}" stroke-width="14" stroke-linecap="round" opacity="{op}"/>'
    for dx, y0, y1, op in [(-190, -330, -80, 0.85), (-260, -260, -120, 0.55), (-330, -300, -200, 0.35)])

halo = ""
if "led" in puntos:
    ex, ey = en_poster("led")
    halo = (f'<circle cx="{ex:.0f}" cy="{ey:.0f}" r="170" fill="url(#halo)"/>'
            f'<circle cx="{ex:.0f}" cy="{ey:.0f}" r="16" fill="{VERDE}" opacity="0.9"/>')

# Burbuja de Telegram: lo que pasa en el canal cuando el llavero entra
BX, BY, BW, BH = 1830, 2650, 1000, 190
burbuja = f'''
<g>
  <rect x="{BX}" y="{BY}" width="{BW}" height="{BH}" rx="56" fill="#FFFFFF"/>
  <path d="M {BX + 30} {BY + BH - 40} q -40 60 -110 70 q 70 -50 60 -110 z" fill="#FFFFFF"/>
  <text x="{BX + 60}" y="{BY + 84}" font-family="{FUENTE}" font-size="46" font-weight="600"
        fill="#2AABEE">Canal KEEEI</text>
  <circle cx="{BX + 82}" cy="{BY + 140}" r="24" fill="{VERDE}"/>
  <text x="{BX + 124}" y="{BY + 156}" font-family="{FUENTE}" font-size="52" font-weight="700"
        fill="#0B0E2A">Hay alguien en el cubo.</text>
</g>'''

PASOS = [
    ("1", "Entras", "Mete el llavero.", VERDE, "Hay alguien en el cubo."),
    ("2", "Sales", "Llévatelo contigo.", ROJO, "No hay nadie en el cubo."),
    ("3", "¿Vienes?", "Mira antes el canal.", AZUL, "Sin preguntar en el grupo."),
]
pasos = ""
for i, (num, titulo, texto, color, estado) in enumerate(PASOS):
    x = 200 + i * 880
    pasos += f'''
<g transform="translate({x},3200)">
  <circle cx="60" cy="60" r="60" fill="none" stroke="{AZUL}" stroke-width="10"/>
  <text x="60" y="84" text-anchor="middle" font-family="{FUENTE}" font-size="68"
        font-weight="800" fill="{AZUL}">{num}</text>
  <text x="160" y="58" font-family="{FUENTE}" font-size="64" font-weight="800" fill="#FFFFFF">{titulo}</text>
  <text x="160" y="128" font-family="{FUENTE}" font-size="50" fill="#FFFFFF" opacity="0.85">{texto}</text>
  <circle cx="176" cy="190" r="15" fill="{color}"/>
  <text x="206" y="206" font-family="{FUENTE}" font-size="42" fill="{color}">{estado}</text>
</g>'''

qr = b64(IMG / "qr-telegram.svg", "image/svg+xml")
logo = b64(IMG / "logo-ieee-sb-unizar-blanco.png", "image/png")

svg = f'''<svg xmlns="http://www.w3.org/2000/svg" xmlns:xlink="http://www.w3.org/1999/xlink"
     width="297mm" height="420mm" viewBox="0 0 {W} {H}">
<defs>
  <linearGradient id="fondo" x1="0" y1="0" x2="0" y2="1">
    <stop offset="0" stop-color="#0B0E2A"/><stop offset="1" stop-color="#1B1240"/>
  </linearGradient>
  <radialGradient id="foco" cx="0.5" cy="0.45" r="0.5">
    <stop offset="0" stop-color="#3A3FD0" stop-opacity="0.35"/>
    <stop offset="1" stop-color="#3A3FD0" stop-opacity="0"/>
  </radialGradient>
  <radialGradient id="halo">
    <stop offset="0" stop-color="{VERDE}" stop-opacity="0.95"/>
    <stop offset="0.35" stop-color="{VERDE}" stop-opacity="0.35"/>
    <stop offset="1" stop-color="{VERDE}" stop-opacity="0"/>
  </radialGradient>
  <radialGradient id="borde" cx="0.5" cy="0.5" r="0.5">
    <stop offset="0.72" stop-color="#FFFFFF"/><stop offset="1" stop-color="#000000"/>
  </radialGradient>
  <mask id="difumina" maskContentUnits="objectBoundingBox">
    <rect width="1" height="1" fill="url(#borde)"/>
  </mask>
  <marker id="punta" viewBox="0 0 10 10" refX="5" refY="5" markerWidth="3.2" markerHeight="3.2"
          orient="auto-start-reverse">
    <path d="M 0 0 L 10 5 L 0 10 z" fill="{AZUL}"/>
  </marker>
</defs>
<rect width="{W}" height="{H}" fill="url(#fondo)"/>
<ellipse cx="{W / 2}" cy="{RY + RH * 0.5:.0f}" rx="1500" ry="1300" fill="url(#foco)"/>

<text x="200" y="260" font-family="{FUENTE}" font-size="52" font-weight="700"
      letter-spacing="10" fill="{AZUL}">{TEXTOS["etiqueta"]}</text>
<text x="190" y="480" font-family="{FUENTE}" font-size="200" font-weight="900"
      fill="#FFFFFF">¿Hay alguien</text>
<text x="190" y="690" font-family="{FUENTE}" font-size="200" font-weight="900"
      fill="#FFFFFF">en el cubo?</text>
<text x="200" y="830" font-family="{FUENTE}" font-size="60" fill="#FFFFFF"
      opacity="0.8">{TEXTOS["sub"]}</text>

<image mask="url(#difumina)" x="{RX:.0f}" y="{RY}" width="{RW:.0f}" height="{RH}" xlink:href="{b64(render, "image/png")}"/>
{halo}
{movimiento}
{flecha}
{burbuja}
{pasos}

<line x1="200" y1="3540" x2="{W - 200}" y2="3540" stroke="#FFFFFF" stroke-opacity="0.18" stroke-width="4"/>

<rect x="200" y="3620" width="440" height="440" rx="36" fill="#FFFFFF"/>
<image x="220" y="3640" width="400" height="400" xlink:href="{qr}"/>
<text x="720" y="3740" font-family="{FUENTE}" font-size="80" font-weight="800" fill="#FFFFFF">Únete al canal</text>
<text x="720" y="3830" font-family="{FUENTE}" font-size="50" fill="#FFFFFF" opacity="0.85">y no vuelvas a encontrarte</text>
<text x="720" y="3895" font-family="{FUENTE}" font-size="50" fill="#FFFFFF" opacity="0.85">la puerta cerrada.</text>
<text x="720" y="4010" font-family="{FUENTE}" font-size="40" fill="{AZUL}">github.com/rogarciam/KEEEI · imprímelo tú también</text>

<image x="{W - 200 - 260}" y="3600" width="220" height="307" xlink:href="{logo}"/>
<text x="{W - 200 - 150}" y="4010" text-anchor="middle" font-family="{FUENTE}" font-size="34"
      fill="#FFFFFF" opacity="0.7">Proyecto abierto</text>
<text x="{W - 200 - 150}" y="4055" text-anchor="middle" font-family="{FUENTE}" font-size="34"
      fill="#FFFFFF" opacity="0.7">IEEE SB Unizar</text>
</svg>
'''

svg_ruta = AQUI / f"poster-{modelo}.svg"
svg_ruta.write_text(svg, encoding="utf-8")

# Exportar con Edge (Chromium) en modo headless: PDF A3 y PNG de vista previa
edge = shutil.which("msedge") or r"C:\Program Files (x86)\Microsoft\Edge\Application\msedge.exe"
html = AQUI / f".poster-{modelo}.html"
html.write_text(f'''<!doctype html><html><head><meta charset="utf-8"><style>
@page {{ size: 297mm 420mm; margin: 0 }}
html, body {{ margin: 0; padding: 0; background: #0B0E2A }}
svg {{ display: block; width: 297mm; height: 420mm }}
</style></head><body>{svg}</body></html>''', encoding="utf-8")
url = html.as_uri()
comunes = [edge, "--headless", "--disable-gpu", "--hide-scrollbars", "--no-pdf-header-footer"]


def con_edge(*args, sale):
    """Lanza Edge con un perfil propio (no se cruza con otro Edge abierto).
    En Windows el lanzador vuelve antes de que el archivo esté escrito, así
    que se espera a que aparezca y deje de crecer."""
    perfil = tempfile.mkdtemp(prefix="keeei-edge-")
    subprocess.run(comunes + [f"--user-data-dir={perfil}", *args, url], check=True)
    tam = -1
    for _ in range(120):
        if Path(sale).exists() and Path(sale).stat().st_size == tam > 0:
            break
        tam = Path(sale).stat().st_size if Path(sale).exists() else -1
        time.sleep(0.5)
    else:
        raise RuntimeError(f"Edge no ha generado {sale}")
    time.sleep(1)
    shutil.rmtree(perfil, ignore_errors=True)


pdf = AQUI / f"poster-{modelo}.pdf"
pdf.unlink(missing_ok=True)
con_edge(f"--print-to-pdf={pdf}", sale=pdf)
html.write_text(html.read_text(encoding="utf-8").replace(
    "width: 297mm; height: 420mm", "width: 1188px; height: 1680px"), encoding="utf-8")
grande = AQUI / f".poster-{modelo}-grande.png"
con_edge("--window-size=1188,1680", "--force-device-scale-factor=2", f"--screenshot={grande}",
         sale=grande)
Image.open(grande).convert("RGB").resize((1188, 1680), Image.LANCZOS).save(
    AQUI / f"poster-{modelo}.png", optimize=True)
grande.unlink()
html.unlink()
print("Listo:", svg_ruta.name, f"poster-{modelo}.pdf", f"poster-{modelo}.png")
