# Carcasa C · Keko, el robotito tragallaves

**Keko** es un robotito-hucha de sobremesa, un personaje **original** del proyecto KEEEI.
Las llaves se le echan por la **ranura de la cabeza**, como a una hucha, y entonces se
le encienden los **ojos**, que son los 2 LED. Lleva el **QR en la barriga**, dos
**orejas-antena** (es un aparato IoT) y está de pie sobre una **peana con dos pies**.
Tiene esas formas sencillas (bloque, ojos y antenas) para que se reconozca de lejos y se pueda
imprimir sin soportes.

- Fuente: [`carcasa_c.fs`](carcasa_c.fs) (FeatureScript 3112, feature `carcasaKeeeiC`)
- Onshape: Feature Studio «Carcasa C» (eid `e88b5fa6c0eeabe6c83a1ad5`) en la rama «Carcasa C» del documento. Tiene el mismo código sin los comentarios
- Colores: **azul eléctrico** en el cuerpo, la tapa y la peana; **blanco** en la cara, la barriga, las orejas y el cartucho
- Material de referencia: PETG, boquilla de 0,4 mm, capa de 0,2 mm
- Medidas con los valores por defecto: cuerpo de 47,3 × 50,0 × 90,1 mm; peana de 59,3 × 69,2 mm; **alto total de unos 118 mm**

```
                o               o        <- bolas de las antenas
                |               |
              (( ))  ______  (( ))       <- orejas (disco + botón)
               |   |[==  ==]|   |        <- ranura de la cabeza (hucha)
               |   |        |   |
               |   | (O)‿(O) |   |        <- cara blanca: ojos LED (verde y rojo) y sonrisa
               |   |        |   |
               |   | ┌────┐ |   |
               |   | │ QR │ |   |        <- barriga blanca con el QR
               |   | └────┘ |   |
               |   |________|   |
              _|________________|_
             (____peana____________)     <- peana azul con 2 pies, lastre M8 y canal del cable
              (pie)      (pie)
```

## Qué se conserva de la carcasa A

Todo lo funcional, con los **mismos parámetros y valores por defecto**, para que la
calibración de la A ([docs/calibracion.md](../../docs/calibracion.md)) sirva también aquí:
el cartucho del mecanismo (ranura vertical, empujador con leva, microswitch KW12-3 y
baldas), la protoboard de 40 × 50 con el ESP32, el paso del USB-C, los insertos M3 de la
tapa y todas las holguras (`fitSlide`, `fitFirm`, `fitSw`, `fitGuide`…). También se
conservan las piezas de prueba (`testMode`).

Lo que cambia:
- Ya no se cuelga en la pared, así que la tapa trasera no lleva ojales.
- Los LED pasan a ser los ojos, en la cabeza (a 72 mm de altura y separados 32 mm), a los dos lados del cartucho.
- El QR baja a la barriga y el frontal de la A pasa a llamarse **Barriga**.

## Piezas y orientación de impresión

La orientación se decidió antes de modelar. Ninguna pieza necesita soportes.

| Pieza | Color | Orientación en la cama | Medidas (mm) | Unión |
|---|---|---|---|---|
| Cuerpo | Azul | Cara frontal abajo (como la A) | 47,3 × 44,2 × 90,1 | — |
| Cartucho mecanismo | Blanco o azul | Tumbado sobre su costado −X (como la A) | 25,7 × 42 × 31,6 | Entra por detrás y se pega (como la A) |
| Empujador | Cualquiera | De pie sobre la cara trasera del tope | 12 × 10,4 × 5,7 | Suelto en su guía |
| Tapa trasera | Azul | Cara exterior abajo | 47,3 × 8,6 × 90,1 | 4 tornillos M3 a insertos |
| Cara (visor) | Blanco | Cara trasera abajo: anillos y sonrisa hacia arriba | 43,4 × 2,8 × 11,4 | Pegada, 2 pasadores de filamento |
| Barriga | Blanco | Cara trasera abajo | 33,6 × 1,6 × 35,9 | Pegada, 2 pasadores de filamento |
| Oreja izquierda y derecha | Blanco | Tumbadas sobre la cara que se pega | 3,6 × 14 × 37,6 | Pegadas, 2 pasadores cada una |
| Peana | Azul | Cara inferior abajo | 59,3 × 69,2 × 15 | Pegada bajo el cuerpo, 2 pasadores |

Los pasadores son trozos de **filamento de 1,75 mm**, de 1,8 mm de largo, en agujeros de
Ø 1,9 mm (1 mm en el cuerpo y 0,8 mm en la pieza). Con la impresora de un solo color,
basta con cambiar de bobina entre piezas para tener a Keko en azul y blanco.

## Montaje

1. Insertos M3 en el cuerpo; empujador y switch en el cartucho; cartucho en el cuerpo, pegado (igual que en la A).
2. LED desde dentro por los agujeros de los ojos: verde en uno y rojo en el otro.
3. Pega la cara, la barriga y las orejas sobre el cuerpo, centradas con los pasadores.
4. Mete **4 tuercas M8** en la peana, 2 apiladas en cada alojamiento hexagonal (unos 20 g de lastre).
5. Enchufa el cable USB-C al ESP32 por el paso de la base del cuerpo y llévalo por el canal de la peana hacia atrás.
6. Pega el cuerpo sobre la peana, centrado con los 2 pasadores. Las tuercas quedan encerradas.
7. Protoboard en la tapa y tapa atornillada.

**Cable:** el canal mide 15 × 13,8 mm. Va mejor un cable USB-C con **clavija acodada**
(90°). Con una clavija recta y larga, sube el alto de la peana (`baseH`).

## Parámetros nuevos (grupos «Mascota» y «Peana»)

| Parámetro | Defecto | Origen |
|---|---|---|
| `ledS` / `ledZ` (separación y altura de los ojos) | 32 / 72 mm | Diseño: no tocan el cartucho (quedan 0,75 mm hasta su pared) |
| `earR`, `earZ` (oreja) | 7 / 72 mm | Diseño |
| `antH`, `antBall` (antena) | 9 / 3,5 mm | Diseño |
| `baseH` (alto de la peana) | 15 mm | Supuesto: clavija USB-C acodada |
| `baseMS` / `baseMF` / `baseMB` (márgenes de la peana) | 6 / 6 / 6 mm | Diseño |
| `baseR`, `toeR` (esquinas y pies) | 10 / 10 mm | Diseño |
| `nutAF` (entrecaras de la tuerca de lastre) | 13 mm (M8) | Norma DIN 934 (NO VERIFICADO con calibre) |
| `qrZ` (centro del QR) | 24 mm | Diseño (en la A eran 19 mm) |

La holgura de los alojamientos de las tuercas es `fitSlide` (0,25 mm por lado), la misma de la A.

## Auditoría automática (2026-11-14)

Hecha con FeatureScript en Onshape, pieza a pieza y en su orientación de impresión,
con los valores por defecto.

| Pieza | Resultado |
|---|---|
| Cuerpo | ✅ Puentes de 1,6 mm (ventilación) y 1,9 mm (pasadores). ⚠️ Paso USB con puente de 10,5 mm, heredado de la A y aceptado |
| Cartucho y empujador | ✅ Sin cambios respecto a la A: puentes de 6,2 mm como máximo y espesor mínimo de 1,2 mm |
| Tapa trasera | ✅ Igual que la A: capa de sacrificio de 0,2 mm en los avellanados |
| Cara | ✅ Sin voladizos; espesor mínimo de 0,8 mm sobre los pasadores |
| Barriga | ✅ Sin voladizos. El borde junto a los pasadores medía 0,58 mm por el chaflán de la cama; se ha alargado la placa 0,4 mm |
| Orejas | ✅ Prisma plano sin voladizos; espesor mínimo de 1,6 mm |
| Peana | ✅ Todos los huecos están abiertos hacia arriba, sin puentes; suelo mínimo de 1,06 mm junto al chaflán |
| Solapes | ✅ Volumen 0 entre todas las parejas de las 9 piezas, que solo se tocan cara con cara |
| Inserción del cartucho | ✅ Entra sin chocar en los 46 mm de recorrido, con 0,25 mm de holgura |

**Vuelco.** La estimación supone el plástico al 50 % de relleno efectivo (unos 66 g), 20 g de
electrónica y 20 g de lastre, con 40 g de llaves:

| Llaves | Con lastre: margen hasta el borde de la peana | Sin lastre |
|---|---|---|
| En la ranura | 23,2 mm | 23,7 mm |
| Colgando 20 mm por delante de la cara | 16,2 mm | 15,7 mm |
| Colgando 20 mm por un lado | 17,2 mm | 15,3 mm |

El centro de masas siempre cae dentro de la peana. El lastre sobre todo baja el centro de
masas, que pasa de unos 44 a 37 mm de altura, y así Keko aguanta mejor los tirones al sacar las llaves.
