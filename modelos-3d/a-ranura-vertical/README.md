# Carcasa A · Ranura vertical con leva lateral

El llavero entra por arriba en una ranura vertical. Cerca del fondo empuja de lado
un **empujador** con cuña doble que corre en una guía horizontal y acciona el
rodillo del microswitch KW12-3. Un tope en el empujador protege la palanca
(margen de reposo de 1,5 mm sobre el clic y sobrecarrera de 0,8 mm).

- Fuente: [`carcasa_llaves_esp32.fs`](carcasa_llaves_esp32.fs) (FeatureScript, Onshape)
- Medidas exteriores con los valores por defecto: unos **47 × 48 × 90 mm** (ancho × fondo × alto, con el frontal)
- Material de referencia: PETG, boquilla de 0,4 mm, capa de 0,2 mm
- Montaje: dos ojales en la tapa trasera para colgarla en la pared

## Piezas y orientación de impresión (ADR 0005)

| Pieza | Color sugerido | Orientación | ¿Soportes? |
|---|---|---|---|
| Cuerpo | Azul eléctrico | Cara frontal sobre la cama | No |
| Cartucho mecanismo | Azul o blanco | Tumbado sobre su costado izquierdo | No (puentes ≤ 6,2 mm) |
| Frontal | Blanco | Cara trasera sobre la cama (cara vista arriba) | No |
| Tapa trasera | Azul eléctrico | Cara de pared sobre la cama | No |
| Empujador | Cualquiera | De pie, sobre la cara trasera del tope | No |

**Montaje:** 1) insertos M3 en el cuerpo; 2) empujador en la guía y switch
atornillado en el cartucho; 3) cartucho dentro del cuerpo por detrás hasta que
apoye en el nervio, pegado; 4) frontal pegado sobre la cara frontal, centrado con 2 pasadores de filamento, y los LED desde dentro;
5) protoboard en la tapa y tapa atornillada.

## Auditoría automática de imprimibilidad (2026-11-14)

Se hizo con FeatureScript en Onshape, pieza a pieza y en su orientación de impresión:
normales de todas las caras, puentes, espesores mínimos, solapes entre piezas y
la inserción del cartucho, simulada en pasos de 0,5 mm.

| Pieza | Resultado |
|---|---|
| Cuerpo | ✅ Ventilación con puente de 1,6 mm; pasadores con puente de 1,9 mm. ⚠️ USB con puente de 13,5 mm, aceptado: un chaflán no deja pasar la clavija |
| Cartucho | ✅ Puentes de 3,5 y 6,2 mm; espesor mínimo 1,2 mm |
| Frontal | ✅ Sin voladizos con la cara vista hacia arriba; 0,8 mm sobre los pasadores |
| Tapa trasera | ✅ Avellanados como puente circular con capa de sacrificio de 0,2 mm, que se perfora al montar |
| Empujador | ✅ Sin voladizos; espesor mínimo 1,9 mm |
| Montaje | ✅ Ningún solape entre las 10 parejas de piezas. El cartucho entra sin chocar, con 0,25 mm de holgura por lado y 0,2 mm hasta la tapa |

Medidas exteriores con los valores por defecto: unos 47 × 48 × 90 mm, con el frontal.
Límite conocido: con la protoboard de 40 mm, el llavero puede medir como mucho
unos 27 mm de ancho.

Montaje del frontal: corta 2 trozos de 1,6 mm de filamento de 1,75 mm y úsalos
como pasadores para centrarlo antes de pegarlo.

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
