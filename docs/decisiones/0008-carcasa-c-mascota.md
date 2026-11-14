# 0008. Carcasa C: mascota de sobremesa «Keko, el robotito tragallaves»

- Estado: Propuesta
- Fecha: 2026-11-14

## Contexto
Para la opción C (rama `carcasa/c-mascota`, ADR 0006) se pide fusionar la carcasa
con una figurita graciosa y conservar todo lo funcional de la carcasa A: el cartucho del
mecanismo, la protoboard con el ESP32, el USB-C, los insertos M3, las holguras, el QR
y los 2 LED. El repositorio y el póster son públicos, así que el personaje debe ser
**original**. Además, todas las piezas deben imprimirse sin soportes, divididas en
módulos y en dos colores (azul eléctrico y blanco).

## Decisión
- **Personaje:** Keko, un robotito-hucha original. La ranura de las llaves es la
  ranura de hucha de su cabeza, los 2 LED son sus ojos y el QR va en su barriga. Tiene
  dos orejas-antena.
- **Montaje:** de pie sobre la mesa, encima de una **peana** con dos pies, un canal para
  el cable USB-C que sale por detrás y 2 alojamientos para 4 tuercas M8 de lastre.
- **9 piezas:** cuerpo, cartucho, empujador y tapa (las de la A, cambiando solo los
  agujeros), más cara, barriga, 2 orejas y peana. Las piezas nuevas se pegan centradas
  con pasadores de filamento de 1,75 mm.
- Se usan los **mismos parámetros y valores por defecto** que en la A para todo lo que
  se calibra.

## Alternativas descartadas
- **Cuerpo redondeado (elipsoide o loft):** no deja sitio al cartucho rectangular ni a la
  protoboard de 40 × 50 sin agrandar mucho la pieza. Un robot de formas cuadradas
  encaja mejor con el mecanismo.
- **Ranura como boca en la cara frontal:** obligaría a rehacer el cartucho, que es la
  pieza ya auditada y la que más se recalibra.
- **Antenas cilíndricas de pie con bola:** la bola tendría voladizos de más de 45°. Por eso
  son piezas planas que se imprimen tumbadas.
- **Lastre en huecos abiertos por debajo de la peana:** el techo del hueco sería un puente
  de 13–15 mm. Se abren por arriba y el cuerpo los tapa.

## Consecuencias
- Hay 4 uniones pegadas más (cara, barriga y orejas) y la peana. Ninguna soporta carga.
- La peana mide 15 mm de alto. Con una clavija USB-C recta y larga, conviene usar un
  cable acodado o subir `baseH`.
- Si se pega el cuerpo a la peana, para desenchufar el USB hay que sacar la clavija
  hacia abajo por el canal.
- Auditoría con los valores por defecto: sin voladizos nuevos, solapes de volumen 0
  entre las 9 piezas, el cartucho entra sin chocar (0,25 mm de holgura) y, con 40 g de
  llaves, el centro de masas queda a 15 mm o más del borde de la peana
  ([README](../../modelos-3d/c-mascota/README.md)).

## Pendiente
1. Revisión del equipo: ¿gusta el personaje? ¿Se aprueba esta ADR?
2. Imprimir la peana y la cara para comprobar los pasadores y el canal del cable.
3. Exportar los STL de las 9 piezas.
