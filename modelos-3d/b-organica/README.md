# Carcasa B · Canto rodado

La B mantiene **todo lo funcional de la A**: cartucho del mecanismo, empujador
con leva, microswitch KW12-3, protoboard de 40 × 50 con el ESP32, paso del
USB-C, insertos M3 y holguras. Cambia la forma exterior, el montaje en la pared
y la posición de los LED.

- Fuente: [`carcasa_b.fs`](carcasa_b.fs) (FeatureScript 3112, feature `carcasaKeeeiB`)
- Medidas exteriores con los valores por defecto: unos **64 × 49 × 93 mm**
  (ancho × fondo × alto, con el frontal y sin la pletina)
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

1. Se atornilla la **pletina** a la pared (24,8 × 9,75 × 4 mm). Su canto superior
   tiene un bisel de 45° que baja hacia la pared.
2. La concha trasera tiene en su cara de pared un **bolsillo** con el bisel
   contrario. Se apoya la carcasa contra la pared unos 2 mm por encima y se deja
   **deslizar hacia abajo**: el labio del bolsillo entra detrás del bisel y el
   peso empuja la carcasa contra la pared.
3. Para descolgarla se levanta 2 mm y se separa de la pared.

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

## Auditoría

Los resultados de la auditoría automática están más abajo (sección
«Auditoría automática»).
