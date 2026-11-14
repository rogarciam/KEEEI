# Carcasa B · Canto rodado

La B mantiene **todo lo funcional de la A**: cartucho del mecanismo, empujador
con leva, microswitch KW12-3, protoboard de 40 × 50 con el ESP32, paso del
USB-C, insertos M3 y holguras. Cambia la forma exterior, el montaje en la pared
y la posición de los LED.

- Fuente: [`carcasa_b.fs`](carcasa_b.fs) (FeatureScript 3112, feature `carcasaKeeeiB`)
- Medidas exteriores con los valores por defecto: **64 × 48,8 × 93 mm**
  (ancho × fondo × alto, con el frontal; la pletina queda escondida detrás)
- Material de referencia: PETG, boquilla de 0,4 mm, capa de 0,2 mm
- Holguras: los mismos parámetros y valores que la A (`fitSlide`, `fitFirm`,
  `fitSw`, `fitGuide`, pasadores de Ø1,9 mm). Al calibrar la A
  ([docs/calibracion.md](../../docs/calibracion.md)), los valores sirven también para la B.

## Forma

Un canto rodado ovalado, algo más ancho abajo, como un huevo o una semilla.

- **Vista frontal:** la envolvente de dos círculos tangentes, uno inferior de R32
  y otro superior de R27, unidos por rectas tangentes. El contorno es continuo
  en tangencia y no tiene esquinas.
- **Perfil lateral:** cada cara plana (la delantera y la de la pared) pasa al
  costado por un **chaflán de 45°**, que hace de chaflán de cama, y luego por un
  **redondeo grande** que acaba en vertical. Delante: chaflán de 8 mm y R14.
  Detrás: chaflán de 3 mm y R6.
- Así el perfil nunca supera los 45° de voladizo si cada mitad se imprime sobre
  su cara plana. El borde del frontal es una arista de 135°, no viva.

```
        vista frontal                         perfil (corte por el centro)

            ___                                   pared        frente
          /  ‖  \     <- boca de la ranura         |  ___________  .
         | o ‖ o |    <- LED verde / rojo          | /           \ .
        /         \                                |/  concha  |  \.
       |  .-----.  |                               |  trasera  |   |  <- frontal (QR)
       |  | QR  |  |                               |    :      |   |
       |  |     |  |                               |\   :      |  /
        \ '-----' /                                | \__:______|_/
         \_______/                                 y=-3  ↑ unión  y=44
              ↑ USB-C (abajo)                          (labio)
```

## Piezas y orientación de impresión

Se decidió la orientación **antes** de modelar. El canto rodado se parte por
un plano paralelo a la pared, por el centro del paso del USB, donde el perfil
lateral es vertical. Cada mitad tiene así una cara plana que apoya en la cama y
crece en voladizos de 45° o menos.

| Pieza | Color sugerido | Orientación | Unión |
|---|---|---|---|
| Concha frontal | Azul eléctrico | Cara delantera sobre la cama | Labio de 1,2 mm que entra 2 mm en la concha trasera + 4 tornillos M3 a insertos |
| Concha trasera | Azul eléctrico | Cara de pared sobre la cama | Tornillos M3 desde la cara de pared, con avellanado plano y capa de sacrificio |
| Cartucho mecanismo | Blanco | Tumbado sobre su costado izquierdo (−X), como en la A | Entra por detrás en la concha frontal; su franja superior rellena una muesca de la cúpula y forma parte de la superficie exterior. Apoya en un nervio de la pared frontal |
| Frontal (QR) | Blanco | Cara trasera sobre la cama (cara vista arriba) | Pegado sobre la cara delantera con 2 pasadores de filamento de 1,75 mm |
| Empujador | Cualquiera | De pie sobre la cara trasera del tope | Suelto, en su guía |
| Pletina de pared | Cualquiera | Cara delantera sobre la cama | 2 tornillos de 3,5 mm avellanados a la pared |

El cartucho blanco dibuja una **franja que recorre la cúpula** de delante a
atrás. Es la «costura» de la semilla, y en ella están la boca de la ranura y los
dos LED.

## Montaje en la pared: pletina de colgar (tipo *French cleat*)

1. Se atornilla la **pletina** a la pared (24,8 × 9,75 × 4 mm, 2 tornillos de 3,5 mm
   avellanados a 14 mm). Su canto superior tiene un bisel de 45° de 2 mm que baja
   hacia la pared.
2. La concha trasera tiene en su cara de pared un **bolsillo** con el bisel
   contrario. Se apoya la carcasa contra la pared unos 2,25 mm por encima y se deja
   **deslizar hacia abajo**: el labio del bolsillo entra detrás del bisel y el
   peso empuja la carcasa contra la pared.
3. Para descolgarla se levanta 2,25 mm y se separa de la pared. La uña del
   bolsillo engancha 1,4 mm: sus primeros 0,6 mm junto a la pared son rectos para
   que no acabe en filo sobre la cama.

El bolsillo es un rebaje en la cara que apoya en la cama. Su techo es un puente
de 9,5 mm (el máximo admitido es 10), y su esquina inferior lleva un chaflán de
45° que coincide con el de la pletina. Por dentro, una placa de 1,2 mm cierra el
bolsillo por encima de la protoboard.

## LED

Los dos LED de 3 mm ya no van en el frontal. Están **en lo alto de la cúpula,
detrás de la boca de la ranura**, sobre la franja blanca del cartucho, separados
16 mm. Entran desde dentro con la pestaña haciendo de tope. Las patillas se
doblan hacia atrás por encima de la balda superior del switch, por donde van
también sus cables.

## Montaje

1. Insertos M3 en los 4 tetones de la concha frontal.
2. Empujador en su guía y microswitch atornillado al cartucho (como en la A).
   LED en sus agujeros del cartucho.
3. Cartucho dentro de la concha frontal, entrando por detrás hasta que apoye en
   el nervio y en la pared frontal. Se pega.
4. Frontal pegado sobre la cara delantera, centrado con los 2 pasadores.
5. Protoboard en las escuadras de la concha trasera.
6. Concha trasera sobre la frontal (el labio centra), con 4 tornillos M3 × 25.
7. Pletina en la pared y carcasa colgada.

## Medidas por pieza (valores por defecto)

| Pieza | Exterior (X × Y × Z) | Volumen |
|---|---|---|
| Concha frontal | 64 × 38,6 × 89,6 mm | 23,5 cm³ |
| Concha trasera | 64 × 20,3 × 93 mm | 24,2 cm³ |
| Cartucho mecanismo | 25,7 × 33,2 × 31,6 mm | 5,9 cm³ |
| Frontal | 33,6 × 1,6 × 35,5 mm | 1,5 cm³ |
| Empujador | 12 × 10,4 × 5,7 mm | 0,5 cm³ |
| Pletina de pared | 24,8 × 4 × 9,75 mm | 0,7 cm³ |

Plano de unión a 17,25 mm de la cara interior trasera, por el eje del paso USB.
Cada concha se queda con media abertura del USB, abierta hacia el plano de
unión, así que no hay puente.

## Auditoría automática de imprimibilidad (2026-11-14)

Se hizo con FeatureScript en Onshape (lambda con `test_featurescript`), pieza a
pieza y en su orientación de impresión. Se midieron las normales de cada cara
en una rejilla de 4 × 4, los puentes, los espesores por rayos, los solapes entre
las 15 parejas de piezas, la inserción del cartucho (66 pasos de 0,5 mm) y el
gesto de colgar la carcasa en la pletina.

| Pieza | Resultado |
|---|---|
| Concha frontal | ✅ Superficie exterior: 45° como máximo. El labio nace de la pared con un chaflán de 45°. Solo hay 2 puentes de Ø1,9 mm, en los pasadores del frontal (en la cama) |
| Concha trasera | ✅ Techo del bolsillo de la pletina: puente de 10,0 mm (el máximo admitido). Avellanados: puente circular de Ø6,5 mm con capa de sacrificio de 0,2 mm (como en la A). Ventilación: 1,6 mm |
| Cartucho | ✅ La franja de la cúpula no tiene voladizos (impresa de lado, el peor punto queda a 28°). Puentes de 3,6 mm (ranura) y de 4 a 6,2 mm (guía), como en la A. Agujeros de los LED de Ø3,1 mm en horizontal |
| Frontal | ✅ Igual que en la A (0,8 mm sobre los pasadores) |
| Empujador | ✅ Sin voladizos; espesor mínimo de 2 mm |
| Pletina | ✅ Biseles y avellanados a 45° exactos, impresa con la cara delantera en la cama |
| Montaje | ✅ Volumen de solape 0 en todas las parejas. Solo se tocan las caras previstas: el cartucho con el nervio y la pared delantera, el frontal con la cara delantera, las dos conchas en el plano de unión y el tope del empujador con la guía |
| Inserción del cartucho | ✅ Entra 33 mm por detrás sin ningún choque |
| Colgar en la pletina | ✅ Bajando 2,25 mm y separándola 5 mm, nada choca |

Espesores: los únicos valores por debajo de 0,8 mm son las capas de sacrificio
(0,2 mm, a propósito) y lecturas falsas de los rayos al cruzar en oblicuo la
cúpula o el labio junto a los cortes. El espesor real es el de la pared: 1,2 a 2 mm.

Correcciones hechas durante la auditoría:
- El labio tenía la base en voladizo (cornisa de 1,35 mm). Ahora nace con un chaflán de 45°.
- La uña del bolsillo acababa en filo de 45° sobre la cama (0,06 mm). Ahora tiene un tramo recto de 0,6 mm.
- Los chaflanes de cama del cartucho y de la pletina solo se aplican a las aristas entre caras a 90°: en las inclinadas dejaban un pequeño voladizo.
- El enganche de la pletina pasa de 1,5 a 2 mm, para que la uña siga enganchando 1,4 mm.

También regeneran estas variantes: radios R36/R30 con redondeo delantero de
10 + R18 (72 × 91 mm), y paredes de 1,6 mm con holguras de 0,4 / 0,3 mm. Con un
llavero de 26 mm el modelo se detiene con un mensaje claro: el cartucho más
ancho no cabe bajo la cúpula y hay que aumentar el radio superior.

## Piezas de prueba (`testMode`)

- **Prueba encaje cartucho**: la parte alta de la concha frontal (muesca, nervio
  y pared delantera). Se prueba con el cartucho real.
- **Prueba inserto M3**: el tetón lateral izquierdo con su trozo de labio.
- **Prueba pletina**: el bolsillo de la concha trasera. Se prueba con la pletina real.

## Tornillería

- 4 × M3 × 25 de cabeza cilíndrica, desde la cara de pared, con 4 insertos
  térmicos M3 (Ø4 × 6) en la concha frontal.
- 2 × M2 × 10 para el microswitch (como en la A).
- 2 tornillos de 3,5 mm de cabeza avellanada, con tacos, para la pletina.
- 4 trozos de filamento de 1,75 mm: 2 para el frontal y 2 de repuesto.
