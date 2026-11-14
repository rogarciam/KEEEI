"""Render de una carcasa KEEEI a partir de sus STL (Blender 4.x / 5.x).

Uso:
  blender -b --python modelos-3d/render/render_carcasa.py -- <carpeta_stl> <salida.png> [explosion_mm]

Colores IEEE: azul eléctrico para el cuerpo, la tapa y el cartucho; blanco para el
frontal y el empujador. Los STL vienen de Onshape en mm y en posición de montaje
(X ancho, Y fondo con el frontal en +Y, Z alto).
"""
import math
import os
import sys

import bpy
from mathutils import Vector

argv = sys.argv[sys.argv.index("--") + 1:] if "--" in sys.argv else []
stl_dir = argv[0] if len(argv) > 0 else "modelos-3d/a-ranura-vertical/stl"
out_png = argv[1] if len(argv) > 1 else "modelos-3d/a-ranura-vertical/img/render.png"
explode = float(argv[2]) / 1000.0 if len(argv) > 2 else 0.0

AZUL = (0.0, 0.32, 1.0, 1.0)
BLANCO = (0.92, 0.93, 0.95, 1.0)
PIEZAS = {  # nombre de archivo: (color, desplazamiento de la vista explosionada)
    "cuerpo": (AZUL, (0, 0, 0)),
    "cartucho-mecanismo": (AZUL, (0, -1.0, 0.6)),
    "frontal": (BLANCO, (0, 1.0, 0)),
    "tapa-trasera": (AZUL, (0, -1.6, 0)),
    "empujador": (BLANCO, (0, -0.6, 0.6)),
}

bpy.ops.wm.read_factory_settings(use_empty=True)
scene = bpy.context.scene


def material(nombre, color):
    m = bpy.data.materials.new(nombre)
    m.use_nodes = True
    bsdf = m.node_tree.nodes.get("Principled BSDF")
    bsdf.inputs["Base Color"].default_value = color
    bsdf.inputs["Roughness"].default_value = 0.45
    return m


objs = []
for nombre, (color, off) in PIEZAS.items():
    path = os.path.join(stl_dir, nombre + ".stl")
    if not os.path.exists(path):
        print("Falta", path)
        continue
    bpy.ops.wm.stl_import(filepath=path, global_scale=0.001)
    ob = bpy.context.selected_objects[0]
    ob.name = nombre
    ob.data.materials.append(material(nombre, color))
    bpy.ops.object.shade_auto_smooth() if hasattr(bpy.ops.object, "shade_auto_smooth") else None
    ob.location += Vector(off) * explode
    objs.append(ob)

# Encuadre
mins = Vector((1e9, 1e9, 1e9))
maxs = Vector((-1e9, -1e9, -1e9))
for ob in objs:
    for c in ob.bound_box:
        w = ob.matrix_world @ Vector(c)
        mins = Vector(map(min, mins, w))
        maxs = Vector(map(max, maxs, w))
centro = (mins + maxs) / 2
tam = (maxs - mins).length

target = bpy.data.objects.new("objetivo", None)
target.location = centro
scene.collection.objects.link(target)

cam_data = bpy.data.cameras.new("cam")
cam_data.lens = 85
cam = bpy.data.objects.new("cam", cam_data)
cam.location = centro + Vector((0.55, 1.0, 0.45)).normalized() * tam * 3.2
scene.collection.objects.link(cam)
tc = cam.constraints.new("TRACK_TO")
tc.target = target
tc.track_axis = "TRACK_NEGATIVE_Z"
tc.up_axis = "UP_Y"
scene.camera = cam

# Luces de estudio
for nombre, pos, energia in [("clave", (0.6, 1.0, 1.2), 5.0), ("relleno", (-1.2, 0.6, 0.4), 1.5),
                             ("contra", (0.0, -1.2, 1.0), 3.0)]:
    ld = bpy.data.lights.new(nombre, "AREA")
    ld.energy = energia  # W; calibrado para objetos de ~10 cm a ~0,3 m
    ld.size = tam
    lo = bpy.data.objects.new(nombre, ld)
    lo.location = centro + Vector(pos) * tam * 2
    scene.collection.objects.link(lo)
    c = lo.constraints.new("TRACK_TO")
    c.target = target
    c.track_axis = "TRACK_NEGATIVE_Z"
    c.up_axis = "UP_Y"

# Suelo blanco infinito y fondo claro
bpy.ops.mesh.primitive_plane_add(size=tam * 30, location=(centro.x, centro.y, mins.z))
bpy.context.active_object.data.materials.append(material("suelo", (1, 1, 1, 1)))
world = bpy.data.worlds.new("mundo")
world.use_nodes = True
world.node_tree.nodes["Background"].inputs[0].default_value = (1, 1, 1, 1)
world.node_tree.nodes["Background"].inputs[1].default_value = 0.12
scene.world = world

scene.render.engine = "CYCLES"
scene.cycles.samples = 64
scene.cycles.use_denoising = True
scene.render.resolution_x = 1600
scene.render.resolution_y = 1600
scene.render.film_transparent = False
scene.view_settings.view_transform = "AgX"
os.makedirs(os.path.dirname(os.path.abspath(out_png)), exist_ok=True)
scene.render.filepath = os.path.abspath(out_png)
bpy.ops.render.render(write_still=True)
print("Render guardado en", scene.render.filepath)
