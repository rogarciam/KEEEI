# Carcasa A · Ranura vertical con leva lateral

El llavero entra por arriba en una ranura vertical. Cerca del fondo empuja de lado
un **empujador** con cuña doble que corre en una guía horizontal y acciona el
rodillo del microswitch KW12-3. Un tope en el empujador protege la palanca
(margen de reposo de 1,5 mm sobre el clic y sobrecarrera de 0,8 mm).

- Fuente: [`carcasa_llaves_esp32.fs`](carcasa_llaves_esp32.fs) (FeatureScript, Onshape)
- Medidas exteriores con los valores por defecto: **47,3 × 45,2 × 91,1 mm** (ancho × fondo × alto)
- Material de referencia: PETG, boquilla de 0,4 mm, capa de 0,2 mm
- Montaje: dos ojales en la tapa trasera para colgarla en la pared

## Piezas actuales y orientación de impresión

| Pieza | Orientación | ¿Soportes? | Observaciones |
|---|---|---|---|
| Carcasa frontal | Cara frontal sobre la cama | No | Ver el puente crítico más abajo |
| Tapa trasera | Cara de pared sobre la cama | No | Ojales, avellanados y escuadras sin voladizos |
| Empujador | De pie, sobre la cara trasera del tope | No | Las rampas se estrechan hacia arriba |

## Revisión con las reglas de FDM (2026-11-14)

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
recalibra. La propuesta para modularizarla está en la
[ADR 0005](../../docs/decisiones/0005-carcasa-a-modular.md).
