# Plan visual de los pósteres KEEEI

Estado: **en uso**. Los pósteres salen de `hacer_poster.py` con los renders de `modelos-3d/render/poster_render.py`.

## 1. Colores: filamentos disponibles y fondo

Filamentos: **blanco**, **negro** y **azul violeta con purpurina** (lo llamaremos *galaxia*).

| Papel | Color | Por qué |
|---|---|---|
| Cuerpo principal de cada carcasa | Galaxia | Es la pieza más grande y la que más luce; la purpurina destaca con luz rasante |
| Frontal / QR / cara de Keko | Blanco | Contraste máximo, el QR se lee y los LED se ven |
| Piezas técnicas (tapa, cartucho, peana, pletina) | Negro | Ordena la figura y hace que el galaxia parezca más luminoso |

Póster:
- **Fondo:** degradado de azul noche `#0B0E2A` (arriba) a violeta muy oscuro `#1B1240` (abajo). Sobre fondo oscuro la purpurina brilla y los LED verde y rojo se convierten en el punto focal.
- **Texto:** blanco `#FFFFFF`; acento en azul eléctrico IEEE `#00A3FF` para flechas y números de paso.
- **Estados:** verde `#22E07A` (hay alguien) y rojo `#FF4D5E` (no hay nadie), los mismos tonos que el halo de los LED en el render.

## 2. Material de purpurina en Blender
Principled BSDF con base azul violeta `#3A3FD0`, metálico 0,3 y rugosidad 0,35. Una textura Voronoi muy fina (escala ~4000, en mm) controla puntos de metálico 1 y rugosidad 0,1: las «escamas» que destellan. El blanco y el negro van mate (rugosidad 0,55), como el PLA real.

## 3. Luces (iguales en los tres pósteres, para que formen serie)
| Luz | Posición | Función |
|---|---|---|
| Clave | Softbox arriba-izquierda, 45°, 5 500 K | Modela la forma y saca los destellos de la purpurina |
| Contraluz | Detrás, arriba-derecha, violeta `#7A5CFF` | Recorta la silueta contra el fondo oscuro |
| Relleno | Frente-derecha, muy suave | Que el negro no se pierda |
| Práctica | Emisión en el LED verde + halo | Cuenta la historia: «acaban de entrar las llaves» |

Suelo o pared «atrapasombras» invisible: solo se ve la sombra, el fondo es el del póster.

## 4. Planos de cámara («posturas»)
Cada figura en su contexto natural. La acción es la misma en las tres: **el llavero entra y el LED se pone verde**.

| Carcasa | Contexto | Cámara | Llavero |
|---|---|---|---|
| **A · ranura vertical** | Colgada en una pared | 3/4 frontal, desde un poco abajo (contrapicado de 10°), objetivo de 85 mm | Suspendido 3 cm por encima de la ranura, ligeramente girado (dinamismo) |
| **B · canto rodado** | Colgada en pared, de perfil a 3/4 | 3/4 desde arriba a la izquierda, 70 mm; se aprecia la curvatura y la «costura» del cartucho | Entrando, con la mitad del llavero ya dentro de la ranura |
| **C · Keko** | De pie sobre una mesa | Contrapicado heroico a la altura de la peana, 50 mm, Keko «mira» a cámara | Cayendo hacia su cabeza, con un pequeño giro |

Las **flechas no se renderizan**: se dibujan en el póster (vectores, nítidas a cualquier tamaño). Una flecha curva de trazo grueso en azul `#00A3FF`, del llavero a la ranura, más 2–3 líneas de movimiento.

## 5. Maquetación (A3 vertical, 297 × 420 mm, sangrado de 3 mm)
```
┌──────────────────────────────┐
│  ¿HAY ALGUIEN EN EL CUBO?    │  titular, 2 líneas, muy grande
│  KEEEI te lo dice.           │  subtítulo
│                              │
│        [ RENDER + FLECHA ]   │  60 % del alto
│        llavero → ranura      │
│                              │
│  ① Entras: mete el llavero   │  tira de 3 pasos con iconos
│  ② Sales: llévatelo          │
│  ③ Consulta Telegram         │
│ ──────────────────────────── │
│ [QR]  Únete al canal         │  QR grande, abajo a la izquierda
│       t.me/__________        │
│ [logo IEEE SB Unizar]  github│  pie
└──────────────────────────────┘
```

## 6. Textos (borrador)
- **Titular:** «¿Hay alguien en el cubo?»
- **Subtítulo:** «KEEEI te lo dice en Telegram.»
- **Problema (una línea):** «¿Vienes al cubo y te lo encuentras cerrado?»
- **Solución (una línea):** «Mientras sus llaves estén en KEEEI, hay alguien en el cubo. Lo verás en Telegram al momento.»
- **Pasos:**
  1. «Entras → mete el llavero. 🟢 *Hay alguien en el cubo.*»
  2. «Sales → sácalo y llévatelo. 🔴 *No hay nadie en el cubo.*»
  3. «Antes de venir, mira el canal.»
- **Llamada a la acción junto al QR:** «Únete al canal y no vuelvas a encontrarte la puerta cerrada.»
- **Pie:** «Proyecto abierto de la IEEE SB Unizar · github.com/rogarciam/KEEEI · Imprímelo tú también.»
- Variación por carcasa en una línea pequeña: A «Versión de pared», B «Canto rodado», C «Conoce a Keko».

## 7. Herramientas y archivos
- Render: `modelos-3d/render/render_carcasa.py` (Blender 5.3), con una escena por carcasa y PNG con transparencia.
- Póster: SVG en el repositorio (`docs/poster/`), exportado a PDF/PNG a 300 ppp. Se puede importar en Figma para retocarlo.

## 8. Lo que falta
- [x] Enlace del canal de Telegram: QR en `img/qr-telegram.svg`.
- [x] Logo de IEEE SB Unizar (JPEG; `img/logo-ieee-sb-unizar-blanco.png` es la versión en blanco para fondo oscuro). Si aparece el vectorial, se cambia.
- [x] STL de B y C exportados desde Onshape.
- [ ] Revisar colores, planos y textos sobre los pósteres impresos.

## 9. Cambios respecto al plan al hacerlo
- El llavero es una chapa de 22 × 40 × 3 mm con anilla, sin llave: con llave el dibujo se leía peor.
- El LED verde sale pálido en Blender (la curva AgX desatura los verdes intensos), así que el halo se refuerza en el póster con un degradado vectorial.
- En lugar de un «③ consulta Telegram» abstracto, una burbuja del canal junto a la carcasa enseña el mensaje que llega.
