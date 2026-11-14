# Calibración: medidas y resultados de las pruebas de impresión

> ⏰ **RECORDATORIO PENDIENTE:** rellena esta hoja con las medidas reales y con los
> resultados de las piezas de prueba de la **carcasa A**. Esos datos se aplican a
> la A y también a la **B** y la **C**, porque comparten cartucho, protoboard,
> insertos y holguras. Mientras esta hoja esté vacía, todos los modelos usan
> valores supuestos.

Impresora: ______  Material: ______  Boquilla: 0,4 mm  Capa: 0,2 mm

## 1. Componentes (medir con calibre)

| Medida | Parámetro | Supuesto | Medido |
|---|---|---|---|
| Protoboard: ancho × alto × grosor | `pcbW` × `pcbH` × `pcbT` | 40 × 50 × 1,6 mm | |
| Altura de las soldaduras bajo la protoboard | `pcbY` | 4 mm | |
| Altura de los pines hembra | `headerH` | 8,5 mm | |
| ESP32: componente más alto sobre la cara inferior de su PCB | `espFront` | 4,8 mm | |
| ESP32: eje del USB-C sobre la cara inferior de su PCB | `usbC` | 3,15 mm | |
| Clavija USB-C (cuerpo del conector): ancho × fondo | `usbOW` × `usbOT` | 13,5 × 8 mm | |
| Microswitch KW12-3: largo × grosor × alto | `swL` × `swT` × `swH` | 20 × 6 × 8,4 mm | |
| Microswitch: alto con la palanca en reposo / en el clic / pulsada | `hRest` / `hOP` / `hPressed` | 20 / 17 / 12,2 mm | |
| Microswitch: distancia entre agujeros / desde la cara de patillas | `holeS` / `holeY` | 9,5 / 2,5 mm | |
| Llavero: ancho × grosor | `tagW` × `tagT` | 22 × 3 mm | |
| LED: diámetro | `ledD` | 3,1 mm (LED de 3 mm) | |
| Inserto térmico M3: Ø exterior × largo | `insertD` × `insertL` | 4 × 6 mm | |

## 2. Resultados de las piezas de prueba

Marca cómo ha salido cada ajuste: **suelto**, **justo** (perfecto), **duro** o **no entra**.

| Prueba | Qué comprobar | Resultado | Notas |
|---|---|---|---|
| Prueba encaje cartucho | El cartucho entra por detrás y apoya en el nervio; la franja del techo queda enrasada | | |
| Cartucho + empujador | El empujador corre libre en su guía; el llavero entra y sale; se oye el clic | | |
| Cartucho + llavero | El llavero desliza por la ranura sin engancharse | | |
| Prueba esquina inserto M3 | El inserto entra con el soldador y queda recto | | |
| Prueba paso USB | La clavija entra sin forzar | | |
| Prueba tapa y protoboard | La protoboard encaja en las escuadras sin holgura excesiva | | |
| Frontal + pasadores | Los pasadores de filamento entran en ambos agujeros | | |
| Capa de sacrificio de la tapa | Se perfora bien con el tornillo o con una broca | | |

## 3. Holguras que se ajustarán con estos datos

| Holgura | Parámetro | Valor actual |
|---|---|---|
| Deslizante (llavero en la ranura, cartucho en la muesca) | `fitSlide` | 0,25 mm por lado |
| Ajuste firme (protoboard) | `fitFirm` | 0,15 mm por lado |
| Empujador en su guía | `fitGuide` | 0,25 mm por lado |
| Microswitch entre baldas | `fitSw` | 0,10 mm por lado |
| Agujero de los pasadores | `PIN_D` | Ø1,9 mm para filamento de 1,75 |
