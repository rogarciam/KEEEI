# 0005. Carcasa A: dividirla en módulos según la orientación de impresión

- Estado: Aceptada (2026-11-14). Implementada y compilada en Onshape (regenera sin errores)
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
| 2. **Cartucho del mecanismo** | Tubo cerrado de la ranura, guía del empujador, las dos baldas del switch y la franja central del techo | **Tumbado sobre su costado -X**: las caras de la ranura quedan verticales, con las capas a lo largo del deslizamiento; puentes de 3,5 mm (techo del tubo) y 6,2 mm (guía) | Entra por detrás y la franja de techo rellena una muesca del cuerpo. Apoya en un nervio de la pared frontal, que fija su altura; se pega, y la tapa trasera le impide salir hacia atrás |
| 3. **Frontal** | Placa de 1,6 mm con el rebaje del QR (0,4 mm) y los LED | Cara trasera sobre la cama (cara vista arriba) | Pegada **sobre** la cara frontal, centrada con 2 pasadores de filamento de 1,75 mm |
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

## Cambios respecto al diseño anterior
- El fondo crece 1,2 mm, porque el cartucho cierra la ranura por delante con su propia pared.
- El techo del cuerpo tiene una muesca abierta hacia atrás, en lugar de una ventana; así no hay puente al imprimirlo.
- Las baldas del switch pasan a medir lo mismo que el cartucho (24,9 mm); el switch, de 20 mm, sigue cabiendo, y hay una comprobación nueva para ello.
- Modo de pruebas: «Prueba mecanismo» pasa a ser «Prueba encaje cartucho» (muesca, pared frontal y nervio). El mecanismo se prueba con el cartucho real.

## Correcciones tras la auditoría automática
Un rebaje en una cara que apoya en la cama también se imprime como puente. Por eso:
- El frontal ya no va en un rebaje del cuerpo, que hacía un puente de 33,6 mm, sino pegado encima.
- Se imprime con la cara vista hacia arriba, para que el rebaje del QR no sea un puente de 30,4 mm.
- Camisa de 1,6 mm y chaflán de entrada de 0,5 mm, para que el labio no baje de 0,8 mm.
- Muesca del techo con holgura deslizante (0,25 mm por lado), y la franja de techo del cartucho a 0,2 mm de la tapa.
- Capa de sacrificio de 0,2 mm en los avellanados de la tapa.

## Pendiente
1. ~~Compilar en Onshape y comprobar interferencias~~ (hecho, sin solapes).
2. Imprimir la prueba de encaje y el cartucho.
3. Exportar los STL de las 5 piezas.
