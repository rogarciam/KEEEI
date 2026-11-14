# Hardware

Todo va soldado en una protoboard de 40 × 50 mm, la que esperan las tres carcasas.

## Lista de materiales

| Cant. | Pieza | Nota |
|---:|---|---|
| 1 | ESP32 NodeMCU-32S (USB-C) | Va pinchado en tiras de pines hembra, así se puede cambiar |
| 1 | Protoboard para soldar de 40 × 50 mm | |
| 1 | Microswitch KW12-3 con rodillo | Se atornilla al cartucho con 2 tornillos M2 × 10 |
| 1 | LED verde de 3 mm + resistencia | |
| 1 | LED rojo de 3 mm + resistencia | |
| 4 | Insertos térmicos M3 + tornillos M3 | Cierran la tapa |
| — | Filamento blanco, negro y azul violeta | PETG o PLA, boquilla de 0,4 mm |
| — | Un trozo de filamento de 1,75 mm | Pasadores para centrar las piezas pegadas |

Cada carcasa añade su propia tornillería (pletina, tuercas de lastre...). Está
en el README de su rama.

## Conexiones

```
            ESP32
  GPIO19 ───────────── NO  ┐
                           │ microswitch
  GND ──────────────── COM ┘

  GPIO25 ── R ── ▶|── GND   LED verde
  GPIO26 ── R ── ▶|── GND   LED rojo
```

- El microswitch cierra a GND cuando entran las llaves. El firmware usa el
  pull-up interno, así que no hace falta resistencia externa.
- La resistencia de los LED depende del LED que tengas. Con 3,3 V, entre 100 y
  220 Ω suele ir bien. **Apunta la que hayas puesto** en
  [calibracion.md](calibracion.md), para que quede documentada.
- El ESP32 se alimenta por su USB-C. Las carcasas dejan paso para la clavija.
