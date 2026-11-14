# 0005. Carcasa A: dividirla en módulos según la orientación de impresión

- Estado: Propuesta
- Fecha: 2026-11-14

## Contexto
Nueva restricción para todos los modelos: diseñar pensando en la dirección de
impresión, con los mínimos soportes posibles, y dividir en piezas que se impriman
por separado y se peguen. La revisión de la carcasa A
([README](../../modelos-3d/a-ranura-vertical/README.md)) encuentra un puente de
22,5 mm sobre la ranura del llavero y una superficie de deslizamiento impresa en
vertical.

La pieza mide 47 × 45 × 91 mm y cabe en cualquier impresora: **no hace falta
dividirla por tamaño**. Solo conviene dividirla donde eso mejora la impresión, el
ajuste o el aspecto.

## Propuesta: de 3 piezas a 5

| Módulo | Contenido | Orientación | Unión |
|---|---|---|---|
| 1. Cuerpo | Carcasa sin el mecanismo: paredes, tetones, postes, USB y ventilación | Cara frontal abajo (como ahora) | — |
| 2. **Cartucho del mecanismo** | Camisa de la ranura, guía del empujador y las dos baldas del switch | **Tumbado, con una cara de la ranura sobre la cama**: sin puente y con la superficie de deslizamiento lisa | Entra por detrás en unos carriles del cuerpo; pegado + 2 pines de centrado |
| 3. **Frontal** | Placa con el rebaje del QR y los agujeros de los LED | Sobre la cama | Pegada en un rebaje del cuerpo, con un reborde de centrado de 0,6 mm |
| 4. Tapa trasera | Sin cambios | Cara de pared abajo | 4 tornillos M3 + insertos (desmontable) |
| 5. Empujador | Sin cambios | De pie | Suelto |

Ventajas:
- Desaparece el puente de 22,5 mm.
- Para recalibrar el mecanismo solo se reimprime el cartucho (unos 10 g, en lugar de la carcasa entera).
- Frontal **blanco** sobre cuerpo **azul eléctrico** (colores de IEEE) sin necesidad de impresora multicolor.

Inconvenientes:
- Dos uniones pegadas más. Las de carga (el cartucho) llevan pines de centrado y
  se pegan a lo largo de toda la superficie de contacto.
- Hay que añadir la holgura de ajuste firme (0,10–0,15 mm por lado) como parámetro.

## Alternativas
- **Mantener 3 piezas** y cambiar el techo de la ranura por un arco: no se
  puede, porque el llavero es plano y necesita una ranura rectangular.
- **Imprimir con soportes dentro de la ranura**: quedan restos en la superficie por la que desliza el llavero. Descartada.

## Pendiente
Aprobación. Después: modificar el FeatureScript en Onshape, hacer una pieza de
prueba del encaje del cartucho y exportar los STL.
