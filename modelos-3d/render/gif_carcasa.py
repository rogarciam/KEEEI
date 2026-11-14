"""Animación giratoria de una carcasa KEEEI: gira 360° mientras se despieza y se vuelve a montar.

Uso:
  blender -b --python modelos-3d/render/gif_carcasa.py -- <carpeta_stl> <carpeta_frames> [frames] [px]
Después: python modelos-3d/render/frames_a_gif.py <carpeta_frames> <salida.gif>

Colores del plan visual (docs/poster/plan-visual.md): galaxia (azul violeta con
purpurina), negro y blanco. Cada STL se asigna por nombre en PIEZAS; las piezas
que no estén en la tabla salen en galaxia. Desplazamientos de despiece en mm.
"""
import math
import os
import sys

import bpy
from mathutils import Vector

argv = sys.argv[sys.argv.index("--") + 1:] if "--" in sys.argv else []
stl_dir, out_dir = argv[0], argv[1]
FRAMES = int(argv[2]) if len(argv) > 2 else 36
PX = int(argv[3]) if len(argv) > 3 else 640

GALAXIA, NEGRO, BLANCO = "galaxia", "negro", "blanco"
PIEZAS = {  # nombre: (material, despiece x, y, z en mm)
    "cuerpo": (GALAXIA, (0, 0, 0)),
    "concha-frontal": (GALAXIA, (0, 0, 0)),
    "cartucho-mecanismo": (NEGRO, (0, -30, 12)),
    "frontal": (BLANCO, (0, 25, 0)),
    "tapa-trasera": (NEGRO, (0, -40, 0)),
    "concha-trasera": (NEGRO, (0, -40, 0)),
    "empujador": (BLANCO, (0, -22, 12)),
    "pletina-de-pared": (NEGRO, (0, -60, 0)),
    "cara": (BLANCO, (0, 20, 0)),
    "barriga": (BLANCO, (0, 20, 0)),
    "oreja-izquierda": (BLANCO, (-20, 0, 0)),
    "oreja-derecha": (BLANCO, (20, 0, 0)),
    "peana": (NEGRO, (0, 0, -25)),
}

bpy.ops.wm.read_factory_settings(use_empty=True)
scene = bpy.context.scene


def mat_basico(nombre, color, rough):
    m = bpy.data.materials.new(nombre)
    m.use_nodes = True
    b = m.node_tree.nodes["Principled BSDF"]
    b.inputs["Base Color"].default_value = color
    b.inputs["Roughness"].default_value = rough
    return m


def mat_galaxia():
    m = bpy.data.materials.new("galaxia")
    m.use_nodes = True
    nt = m.node_tree
    b = nt.nodes["Principled BSDF"]
    b.inputs["Base Color"].default_value = (0.03, 0.025, 0.42, 1)  # galaxia algo más oscuro y violeta
    vor = nt.nodes.new("ShaderNodeTexVoronoi")
    vor.inputs["Scale"].default_value = 4000.0  # escamas de ~0,25 mm (escena en m)
    ramp = nt.nodes.new("ShaderNodeValToRGB")
    ramp.color_ramp.elements[0].position = 0.0
    ramp.color_ramp.elements[1].position = 0.12
    nt.links.new(vor.outputs["Distance"], ramp.inputs["Fac"])
    inv = nt.nodes.new("ShaderNodeInvert")
    nt.links.new(ramp.outputs["Color"], inv.inputs["Color"])
    nt.links.new(inv.outputs["Color"], b.inputs["Metallic"])
    rmap = nt.nodes.new("ShaderNodeMapRange")
    rmap.inputs["To Min"].default_value = 0.42
    rmap.inputs["To Max"].default_value = 0.08
    nt.links.new(inv.outputs["Color"], rmap.inputs["Value"])
    nt.links.new(rmap.outputs["Result"], b.inputs["Roughness"])
    return m


MATS = {GALAXIA: mat_galaxia(),
        NEGRO: mat_basico("negro", (0.012, 0.012, 0.014, 1), 0.55),
        BLANCO: mat_basico("blanco", (0.85, 0.86, 0.88, 1), 0.55)}

objs = []
for f in sorted(os.listdir(stl_dir)):
    if not f.lower().endswith(".stl"):
        continue
    nombre = f[:-4]
    mat, off = PIEZAS.get(nombre, (GALAXIA, (0, 0, 0)))
    bpy.ops.wm.stl_import(filepath=os.path.join(stl_dir, f), global_scale=0.001)
    ob = bpy.context.selected_objects[0]
    ob.data.materials.append(MATS[mat])
    if hasattr(bpy.ops.object, "shade_auto_smooth"):
        bpy.ops.object.shade_auto_smooth()
    ob["base"] = ob.location.copy()
    ob["off"] = Vector(off) / 1000.0
    objs.append(ob)

bpy.context.view_layer.update()
mins = Vector((1e9,) * 3)
maxs = Vector((-1e9,) * 3)
for ob in objs:
    for c in ob.bound_box:
        w = ob.matrix_world @ Vector(c)
        mins = Vector(map(min, mins, w))
        maxs = Vector(map(max, maxs, w))
centro = (mins + maxs) / 2
tam = (maxs - mins).length

# Las piezas cuelgan de un pivote que gira; la cámara y las luces quedan fijas
pivote = bpy.data.objects.new("pivote", None)
pivote.location = centro
scene.collection.objects.link(pivote)
bpy.context.view_layer.update()  # sin esto la matriz del pivote es la identidad
for ob in objs:
    ob.parent = pivote
    ob.matrix_parent_inverse = pivote.matrix_world.inverted()

cam = bpy.data.objects.new("cam", bpy.data.cameras.new("cam"))
cam.data.lens = 70
cam.location = centro + Vector((0.0, 1.0, 0.32)).normalized() * tam * 4.3
scene.collection.objects.link(cam)
tc = cam.constraints.new("TRACK_TO")
tc.target = pivote
tc.track_axis, tc.up_axis = "TRACK_NEGATIVE_Z", "UP_Y"
scene.camera = cam

for nombre, pos, energia, color in [("clave", (-0.8, 0.8, 1.2), 6.0, (1, 0.97, 0.92)),
                                    ("contra", (0.7, -1.2, 0.9), 9.0, (0.48, 0.36, 1.0)),
                                    ("relleno", (1.0, 0.8, 0.2), 1.2, (1, 1, 1))]:
    ld = bpy.data.lights.new(nombre, "AREA")
    ld.energy, ld.size, ld.color = energia, tam * 0.8, color
    lo = bpy.data.objects.new(nombre, ld)
    lo.location = centro + Vector(pos) * tam * 2
    scene.collection.objects.link(lo)
    c = lo.constraints.new("TRACK_TO")
    c.target = pivote
    c.track_axis, c.up_axis = "TRACK_NEGATIVE_Z", "UP_Y"

world = bpy.data.worlds.new("mundo")
world.use_nodes = True
world.node_tree.nodes["Background"].inputs[0].default_value = (0.0035, 0.0045, 0.022, 1)  # #0B0E2A
world.node_tree.nodes["Background"].inputs[1].default_value = 1.0
scene.world = world

scene.render.engine = "CYCLES"
scene.cycles.samples = 24
scene.cycles.use_denoising = True
scene.render.resolution_x = scene.render.resolution_y = PX
scene.view_settings.view_transform = "AgX"
os.makedirs(out_dir, exist_ok=True)

for i in range(FRAMES):
    t = i / FRAMES
    pivote.rotation_euler = (0, 0, 2 * math.pi * t)
    despiece = math.sin(math.pi * t) ** 2  # 0 -> 1 -> 0: se abre a mitad de vuelta
    for ob in objs:
        ob.location = Vector(ob["base"]) + Vector(ob["off"]) * despiece
    scene.render.filepath = os.path.join(os.path.abspath(out_dir), f"f{i:03d}.png")
    bpy.ops.render.render(write_still=True)
print("Frames listos en", out_dir)
