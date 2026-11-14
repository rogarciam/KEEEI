# Carcasa A · Ranura vertical con leva lateral

El llavero entra por arriba en una ranura vertical. Cerca del fondo empuja de lado
un **empujador** con cuña doble que corre en una guía horizontal y acciona el
rodillo del microswitch KW12-3. Un tope en el empujador protege la palanca
(margen de reposo de 1,5 mm sobre el clic y sobrecarrera de 0,8 mm).

- Fuente: [`carcasa_llaves_esp32.fs`](carcasa_llaves_esp32.fs) (FeatureScript, Onshape)
- Medidas exteriores con los valores por defecto: **47,3 × 45,2 × 91,1 mm** (ancho × fondo × alto)
- Material de referencia: PETG, boquilla de 0,4 mm, capa de 0,2 mm
- Montaje: dos ojales en la tapa trasera para colgarla en la pared

## Piezas y orientación de impresión (ADR 0005)

| Pieza | Color sugerido | Orientación | ¿Soportes? |
|---|---|---|---|
| Cuerpo | Azul eléctrico | Cara frontal sobre la cama | No |
| Cartucho mecanismo | Azul o blanco | Tumbado sobre su costado izquierdo | No (puentes ≤ 6,2 mm) |
| Frontal | Blanco | Cara vista sobre la cama | No |
| Tapa trasera | Azul eléctrico | Cara de pared sobre la cama | No |
| Empujador | Cualquiera | De pie, sobre la cara trasera del tope | No |

**Montaje:** 1) insertos M3 en el cuerpo; 2) empujador en la guía y switch
atornillado en el cartucho; 3) cartucho dentro del cuerpo por detrás hasta que
apoye en el nervio, pegado; 4) frontal pegado y los LED desde dentro;
5) protoboard en la tapa y tapa atornillada.

> ⚠️ Esta versión modular **todavía no se ha compilado en Onshape**. Los STL de
> `stl/pruebas/` son de la versión anterior, de 3 piezas.

## Revisión con las reglas de FDM de la versión de 3 piezas (2026-11-14)

Orientación de la carcasa frontal: el eje Y del modelo es vertical en la impresora
y la cavidad queda abierta hacia arriba.

| # | Elemento | Resultado | Valoración |
|---|---|---|---|
| 1 | Paredes, tetones, postes, nervios, baldas | Crecen en vertical desde la cara frontal | ✅ |
| 2 | **Pared trasera de la camisa de la ranura** | **Puente de 22,5 mm** (ancho de la ranura) sobre un hueco de 3,5 mm | ⚠️ Supera la regla (≤ 10 mm; 15 mm en PLA). En PETG, si cae 0,3 mm se come la holgura del llavero (0,25 mm por lado). |
| 3 | Abertura USB en la base | Puente de 13,5 mm (algo menos por los redondeos R1,5) | ⚠️ Aceptable, pero mejor terminarla en chaflán de 45° |
| 4 | Agujeros M2 de las baldas | Horizontales, Ø 1,7 / 2,4 mm | ℹ️ Saldrán un poco ovalados: repasar con broca o hacerlos en gota |
| 5 | Ranuras de ventilación | Puente de 1,6 mm | ✅ |
| 6 | LED, QR, tetones de los insertos | En vertical o sobre la cama | ✅ |
| 7 | Superficie por la que desliza el llavero | Queda en vertical (con escalones de capa) | ⚠️ Roza más que una cara impresa sobre la cama |

**Conclusión:** la carcasa ya se imprime casi sin soportes. Los únicos puntos
débiles están en el **mecanismo** (puntos 2 y 7), que además es lo que más se
recalibra. La solución adoptada se explica en la
[ADR 0005](../../docs/decisiones/0005-carcasa-a-modular.md).
