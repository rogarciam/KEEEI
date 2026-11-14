"""Render de póster: la carcasa en su «postura» con el llavero entrando en la ranura.

Uso: blender -b --python modelos-3d/render/poster_render.py -- <stl_dir> <salida.png> <a|b|c> [px]

Sigue docs/poster/plan-visual.md: galaxia/negro/blanco, luz clave cálida, contraluz
violeta, relleno suave y un atrapasombras invisible (pared para A y B, mesa para C).
El PNG sale con fondo transparente; el degradado y las flechas los pone el póster.
La ranura se localiza sola lanzando rayos hacia abajo sobre el cartucho.
"""
import math
import os
import sys

import bmesh
import bpy
from mathutils import Vector

argv = sys.argv[sys.argv.index("--") + 1:]
stl_dir, salida, modelo = argv[0], argv[1], argv[2]
PX = int(argv[3]) if len(argv) > 3 else 2000
MM = 0.001

GALAXIA, NEGRO, BLANCO = "galaxia", "negro", "blanco"
COLORES = {
    "a": {"cuerpo": GALAXIA, "cartucho-mecanismo": NEGRO, "frontal": BLANCO,
          "tapa-trasera": NEGRO, "empujador": BLANCO},
    "b": {"concha-frontal": GALAXIA, "concha-trasera": NEGRO, "cartucho-mecanismo": BLANCO,
          "frontal": BLANCO, "empujador": BLANCO, "pletina-de-pared": NEGRO},
    "c": {"cuerpo": GALAXIA, "cartucho-mecanismo": NEGRO, "tapa-trasera": NEGRO,
          "cara": BLANCO, "barriga": BLANCO, "oreja-izquierda": BLANCO,
          "oreja-derecha": BLANCO, "peana": NEGRO, "empujador": BLANCO},
}[modelo]

# Postura por carcasa: dirección de cámara (desde el centro), objetivo, distancia
# en tamaños de pieza, cuánto asoma el llavero sobre la ranura (mm, negativo =
# ya dentro), giro del llavero y tipo de atrapasombras.
POSTURA = {
    "a": dict(dir=(0.55, 1.0, -0.12), lens=85, dist=2.6, llavero=12, giro=14, sombra="pared"),
    "b": dict(dir=(-0.75, 1.0, 0.55), lens=70, dist=2.5, llavero=-14, giro=-8, sombra="pared"),
    "c": dict(dir=(0.35, 1.0, -0.02), lens=50, dist=1.55, llavero=18, giro=22, sombra="mesa"),
}[modelo]

bpy.ops.wm.read_factory_settings(use_empty=True)
scene = bpy.context.scene


def mat_basico(nombre, color, rough, emision=None, fuerza=0.0):
    m = bpy.data.materials.new(nombre)
    m.use_nodes = True
    b = m.node_tree.nodes["Principled BSDF"]
    b.inputs["Base Color"].default_value = color
    b.inputs["Roughness"].default_value = rough
    if emision:
        b.inputs["Emission Color"].default_value = emision
        b.inputs["Emission Strength"].default_value = fuerza
    return m


def mat_galaxia():
    m = bpy.data.materials.new("galaxia")
    m.use_nodes = True
    nt = m.node_tree
    b = nt.nodes["Principled BSDF"]
    b.inputs["Base Color"].default_value = (0.03, 0.025, 0.42, 1)
    tc = nt.nodes.new("ShaderNodeTexCoord")
    vor = nt.nodes.new("ShaderNodeTexVoronoi")
    vor.inputs["Scale"].default_value = 4000.0
    nt.links.new(tc.outputs["Object"], vor.inputs["Vector"])
    ramp = nt.nodes.new("ShaderNodeValToRGB")
    ramp.color_ramp.elements[0].position = 0.06
    ramp.color_ramp.elements[0].color = (1, 1, 1, 1)
    ramp.color_ramp.elements[1].position = 0.12
    ramp.color_ramp.elements[1].color = (0, 0, 0, 1)
    nt.links.new(vor.outputs["Distance"], ramp.inputs["Fac"])
    met = nt.nodes.new("ShaderNodeMapRange")
    met.inputs["To Min"].default_value, met.inputs["To Max"].default_value = 0.3, 1.0
    nt.links.new(ramp.outputs["Color"], met.inputs["Value"])
    nt.links.new(met.outputs["Result"], b.inputs["Metallic"])
    rou = nt.nodes.new("ShaderNodeMapRange")
    rou.inputs["To Min"].default_value, rou.inputs["To Max"].default_value = 0.35, 0.1
    nt.links.new(ramp.outputs["Color"], rou.inputs["Value"])
    nt.links.new(rou.outputs["Result"], b.inputs["Roughness"])
    return m


MATS = {GALAXIA: mat_galaxia(),
        NEGRO: mat_basico("negro", (0.012, 0.012, 0.014, 1), 0.55),
        BLANCO: mat_basico("blanco", (0.82, 0.82, 0.84, 1), 0.55)}

objs = {}
for f in sorted(os.listdir(stl_dir)):
    if not f.lower().endswith(".stl"):
        continue
    nombre = os.path.splitext(f)[0]
    bpy.ops.wm.stl_import(filepath=os.path.join(stl_dir, f), global_scale=MM)
    ob = bpy.context.selected_objects[0]
    ob.data.materials.append(MATS[COLORES.get(nombre, GALAXIA)])
    if hasattr(bpy.ops.object, "shade_auto_smooth"):
        bpy.ops.object.shade_auto_smooth()
    objs[nombre] = ob
bpy.context.view_layer.update()


def caja(obs):
    lo, hi = Vector((1e9,) * 3), Vector((-1e9,) * 3)
    for ob in obs:
        for c in ob.bound_box:
            w = ob.matrix_world @ Vector(c)
            lo, hi = Vector(map(min, lo, w)), Vector(map(max, hi, w))
    return lo, hi


mins, maxs = caja(objs.values())
centro, tam = (mins + maxs) / 2, (maxs - mins).length

# --- Localizar la ranura: rayos verticales sobre el cartucho; donde el primer
# impacto queda muy por debajo del techo, estamos sobre la boca de la ranura.
deps = bpy.context.evaluated_depsgraph_get()
clo, chi = caja([objs["cartucho-mecanismo"]])
techo = maxs.z + 0.01
hits = []
N = 60
for i in range(N + 1):
    for j in range(N + 1):
        x = clo.x + (chi.x - clo.x) * i / N
        y = clo.y + (chi.y - clo.y) * j / N
        ok, loc, *_ = scene.ray_cast(deps, Vector((x, y, techo)), Vector((0, 0, -1)))
        if ok:
            hits.append((x, y, loc.z))
ztop = max(h[2] for h in hits)
prof = {(round(h[0], 5), round(h[1], 5)): h[2] for h in hits}
dy = (chi.y - clo.y) / N


def arriba(x, y, zh):  # ¿el rayo en (x, y) da en techo, 10 mm por encima de zh?
    z = prof.get((round(x, 5), round(y, 5)))
    return z is not None and z > zh + 10 * MM


# Boca = puntos hondos con techo delante y detrás (la ranura es estrecha en un
# eje); así no cuentan los bordes de la carcasa ni los escalones del techo.
k = max(1, round(4 * MM / dy))
hondos = [h for h in hits if h[2] < ztop - 15 * MM
          and arriba(h[0], h[1] + k * dy, h[2]) and arriba(h[0], h[1] - k * dy, h[2])]
if not hondos:  # la ranura va en el otro sentido
    dx = (chi.x - clo.x) / N
    k = max(1, round(4 * MM / dx))
    hondos = [h for h in hits if h[2] < ztop - 15 * MM
              and arriba(h[0] + k * dx, h[1], h[2]) and arriba(h[0] - k * dx, h[1], h[2])]
sx = sum(h[0] for h in hondos) / len(hondos)
sy = sum(h[1] for h in hondos) / len(hondos)
ancho_x = max(h[0] for h in hondos) - min(h[0] for h in hondos)
ancho_y = max(h[1] for h in hondos) - min(h[1] for h in hondos)
zboca = max(h[2] for h in hits if abs(h[0] - sx) < 6 * MM and abs(h[1] - sy) < 6 * MM)
boca = Vector((sx, sy, zboca))  # a ras de los labios de la ranura
eje_largo_x = ancho_x >= ancho_y
print(f"Ranura en {boca * 1000} mm, {ancho_x * 1000:.1f} x {ancho_y * 1000:.1f} mm")


# --- LED verde encendido: busca agujeros pequeños rodeados de superficie
# (rayos desde delante y desde arriba), se queda con la pareja más cercana y
# enciende el de la izquierda.
def agujeros(eje):
    paso = 0.5 * MM
    if eje == "y":
        us = [mins.x + k * paso for k in range(int((maxs.x - mins.x) / paso))]
        vs = [mins.z + k * paso for k in range(int((maxs.z - mins.z) / paso))]
        org = lambda u, v: Vector((u, maxs.y + 0.01, v))
        dirv, prof = Vector((0, -1, 0)), lambda loc: loc.y
    else:
        us = [mins.x + k * paso for k in range(int((maxs.x - mins.x) / paso))]
        vs = [mins.y + k * paso for k in range(int((maxs.y - mins.y) / paso))]
        org = lambda u, v: Vector((u, v, maxs.z + 0.01))
        dirv, prof = Vector((0, 0, -1)), lambda loc: loc.z
    mapa = {}
    for i, u in enumerate(us):
        for j, v in enumerate(vs):
            ok, loc, *_ = scene.ray_cast(deps, org(u, v), dirv)
            if ok:
                mapa[i, j] = (prof(loc), loc)
    r = 8  # 4 mm
    pts = []
    for (i, j), (d, loc) in mapa.items():
        vec = [mapa.get(k) for k in ((i + r, j), (i - r, j), (i, j + r), (i, j - r))]
        if all(n and n[0] > d + 1.5 * MM for n in vec):
            sup = min(n[0] for n in vec)
            q = loc.copy()
            if eje == "y":
                q.y = sup
            else:
                q.z = sup
            pts.append(q)
    grupos = []
    for p in pts:
        for g in grupos:
            if (g[0] - p).length < 6 * MM:
                g.append(p)
                break
        else:
            grupos.append([p])
    res = []
    for g in grupos:
        c = sum(g, Vector()) / len(g)
        diam = 2 * math.sqrt(len(g) * paso * paso / math.pi)
        if 2.5 * MM < diam < 6.5 * MM:
            res.append((c, diam, eje))
    return res


cands = agujeros("y") + agujeros("z")
par = min(((a, b) for i, a in enumerate(cands) for b in cands[i + 1:] if a[2] == b[2]),
          key=lambda ab: (ab[0][0] - ab[1][0]).length, default=None)
if par:
    # Las cámaras miran desde +Y, así que la izquierda de la imagen es +X
    verde = max(par, key=lambda c: c[0].x)
    rojo = min(par, key=lambda c: c[0].x)
    print(f"LED verde en {verde[0] * 1000} mm (Ø{verde[1] * 1000:.1f}), rojo en {rojo[0] * 1000}")
    hacia = Vector((0, 1, 0)) if verde[2] == "y" else Vector((0, 0, 1))
    bpy.ops.mesh.primitive_uv_sphere_add(radius=verde[1] / 2 * 0.95,
                                         location=verde[0] - hacia * 1.2 * MM)
    bola = bpy.context.active_object
    bola.data.materials.append(mat_basico("led", (0.02, 1, 0.15, 1), 0.3,
                                          (0.02, 1, 0.15, 1), 0.7))
    luz = bpy.data.objects.new("halo", bpy.data.lights.new("halo", "POINT"))
    luz.data.energy, luz.data.color = 0.015, (0.13, 0.88, 0.48)
    luz.data.shadow_soft_size = 1 * MM
    luz.location = verde[0] + hacia * 3 * MM
    scene.collection.objects.link(luz)
else:
    print("No encuentro los LED")

# --- Llavero: chapa 22 x 40 x 3 mm con agujero, anilla y una llave sencilla.
TAG_W, TAG_H, TAG_T = 22 * MM, 40 * MM, 3 * MM


def chapa():
    bm = bmesh.new()
    bmesh.ops.create_cube(bm, size=1.0)
    for v in bm.verts:
        v.co = Vector((v.co.x * TAG_W, v.co.y * TAG_T, v.co.z * TAG_H))
    me = bpy.data.meshes.new("chapa")
    bm.to_mesh(me)
    ob = bpy.data.objects.new("chapa", me)
    scene.collection.objects.link(ob)
    bev = ob.modifiers.new("bisel", "BEVEL")
    bev.width, bev.segments = 2.5 * MM, 6
    agujero = bpy.data.objects.new("agujero", bpy.data.meshes.new("aguj"))
    bm2 = bmesh.new()
    bmesh.ops.create_cone(bm2, cap_ends=True, segments=32, radius1=2.6 * MM,
                          radius2=2.6 * MM, depth=10 * MM)
    bm2.to_mesh(agujero.data)
    agujero.rotation_euler = (math.pi / 2, 0, 0)
    agujero.location = (0, 0, TAG_H / 2 - 5 * MM)
    scene.collection.objects.link(agujero)
    agujero.hide_render = True
    boo = ob.modifiers.new("agujero", "BOOLEAN")
    boo.object, boo.operation = agujero, "DIFFERENCE"
    ob.data.materials.append(MATS[BLANCO if modelo != "b" else GALAXIA])
    return ob, agujero


tag, aguj = chapa()
bpy.ops.mesh.primitive_torus_add(major_radius=11 * MM, minor_radius=1.1 * MM,
                                 major_segments=64, minor_segments=16)
anilla = bpy.context.active_object
anilla.rotation_euler = (0, math.pi / 2 - 0.5, 0)
anilla.location = (0, 0, TAG_H / 2 + 6 * MM)
metal = mat_basico("metal", (0.8, 0.8, 0.82, 1), 0.18)
metal.node_tree.nodes["Principled BSDF"].inputs["Metallic"].default_value = 1.0
anilla.data.materials.append(metal)

for o in (anilla, aguj):
    o.parent = tag

# Colocar el llavero: base de la chapa a «llavero» mm sobre la boca, alineado
# con la ranura y con un pequeño giro para dar movimiento.
tag.rotation_euler = (0, math.radians(POSTURA["giro"]) if eje_largo_x else 0,
                      0 if eje_largo_x else math.pi / 2)
tag.location = boca + Vector((0, 0, TAG_H / 2 + POSTURA["llavero"] * MM))
bpy.context.view_layer.update()

# --- Atrapasombras: pared detrás (A, B) o mesa debajo (C)
bpy.ops.mesh.primitive_plane_add(size=tam * 8)
plano = bpy.context.active_object
if POSTURA["sombra"] == "pared":
    plano.rotation_euler = (math.pi / 2, 0, 0)
    plano.location = (centro.x, mins.y - 0.2 * MM, centro.z)
else:
    plano.location = (centro.x, centro.y, mins.z - 0.2 * MM)
plano.is_shadow_catcher = True
plano.visible_diffuse = False  # no rebota luz; solo recoge sombra

# --- Cámara
cam = bpy.data.objects.new("cam", bpy.data.cameras.new("cam"))
cam.data.lens = POSTURA["lens"]
mira = bpy.data.objects.new("mira", None)
tlo, thi = caja([tag, anilla])
tlo, thi = Vector(map(min, tlo, mins)), Vector(map(max, thi, maxs))
mira.location = (tlo + thi) / 2  # encuadra carcasa y llavero juntos
tam_vista = (thi - tlo).length
scene.collection.objects.link(mira)
cam.location = mira.location + Vector(POSTURA["dir"]).normalized() * tam_vista * POSTURA["dist"]
scene.collection.objects.link(cam)
tc = cam.constraints.new("TRACK_TO")
tc.target = mira
tc.track_axis, tc.up_axis = "TRACK_NEGATIVE_Z", "UP_Y"
scene.camera = cam

# --- Luces (iguales en la serie)
for nombre, pos, energia, color in [("clave", (-0.9, 0.9, 1.2), 4.0, (1, 0.95, 0.88)),
                                    ("contra", (1.2, 0.05, 1.1) if POSTURA["sombra"] == "pared" else (0.8, -1.0, 1.0), 11.0, (0.48, 0.36, 1.0)),
                                    ("relleno", (1.0, 0.9, 0.1), 1.5, (1, 1, 1))]:
    ld = bpy.data.lights.new(nombre, "AREA")
    ld.energy, ld.size, ld.color = energia, tam * 0.8, color
    lo = bpy.data.objects.new(nombre, ld)
    lo.location = centro + Vector(pos) * tam * 2
    if nombre == "contra" and POSTURA["sombra"] == "mesa":
        ld.use_shadow = False  # su sombra cruzaría la mesa hacia la cámara
    scene.collection.objects.link(lo)
    c = lo.constraints.new("TRACK_TO")
    c.target = mira
    c.track_axis, c.up_axis = "TRACK_NEGATIVE_Z", "UP_Y"

world = bpy.data.worlds.new("mundo")
world.use_nodes = True
world.node_tree.nodes["Background"].inputs[0].default_value = (0.012, 0.012, 0.05, 1)
world.node_tree.nodes["Background"].inputs[1].default_value = 1.0
scene.world = world

scene.render.engine = "CYCLES"
scene.cycles.samples = 96
scene.cycles.use_denoising = True
scene.render.film_transparent = True
scene.render.resolution_x = PX
scene.render.resolution_y = int(PX * 1.15)
scene.view_settings.view_transform = "AgX"
scene.render.image_settings.file_format = "PNG"
scene.render.image_settings.color_mode = "RGBA"
scene.render.filepath = os.path.abspath(salida)
bpy.ops.render.render(write_still=True)

# Posición de la boca en la imagen (0..1), para que el póster dibuje la flecha
from bpy_extras.object_utils import world_to_camera_view  # noqa: E402

puntos = [("boca", boca), ("llavero", tag.matrix_world.translation)]
if par:
    puntos.append(("led", verde[0]))
import json  # noqa: E402

pix = {}
for nombre, p in puntos:
    v = world_to_camera_view(scene, cam, p)
    pix[nombre] = [round(v.x, 4), round(1 - v.y, 4)]  # 0..1 desde arriba a la izquierda
    print(f"PUNTO {nombre} {pix[nombre][0]} {pix[nombre][1]}")
with open(os.path.splitext(os.path.abspath(salida))[0] + ".json", "w") as f:
    json.dump(pix, f)
