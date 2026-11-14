# 0007. Carcasa B: forma de canto rodado partida por el plano del USB

- Estado: Propuesta
- Fecha: 2026-11-14

## Contexto
Para la opción B se pidió abandonar la caja rectangular de la A por una forma
natural y fluida, sin aristas vivas a la vista. También se pidió otro enganche
de pared y los LED en otra posición. Hay que mantener todo lo funcional de la
A: cartucho, empujador, microswitch, protoboard de 40 × 50 con el ESP32,
USB-C, insertos M3 y holguras. Siguen vigentes las reglas de la ADR 0005:
- sin soportes;
- voladizos de 45° como máximo y puentes de 10 mm como máximo;
- piezas que se pegan y se centran entre sí;
- chaflán de 45° en la cama;
- paredes de 1,2, 1,6 o 2,0 mm.

Una forma orgánica cerrada no se puede imprimir entera sin soportes: la mitad
inferior de cualquier superficie curva queda en voladizo.

## Decisión
1. **Forma:** un canto rodado ovalado. En la vista frontal es la envolvente de dos
   círculos (R32 abajo y R27 arriba). En el perfil, cada cara plana pasa al costado
   con un chaflán de 45° y un redondeo grande (delante 8 mm + R14, detrás
   3 mm + R6). Así el perfil nunca supera los 45° si se imprime sobre esa cara,
   y el chaflán sirve también de chaflán de cama.
2. **División:** un plano paralelo a la pared, en la zona en que el costado es
   vertical, a la altura del eje del USB. Cada concha se imprime sobre su cara plana.
   El USB queda como dos medias muescas sin puente.
3. **Unión de las conchas:** un labio de 1,2 mm que nace a 45° de la concha frontal y entra 2 mm
   en la trasera con ajuste firme (`fitFirm`). Además, 4 tornillos M3 desde la
   pared a insertos térmicos: 2 tetones a los lados de la protoboard y 2 junto al cartucho.
4. **Cartucho:** el de la A, con una franja de la cúpula unida a él. Esa franja
   rellena una muesca de la concha frontal y forma parte de la superficie
   exterior: es una «costura» blanca sobre el cuerpo azul.
5. **LED:** en la franja de la cúpula, detrás de la boca de la ranura (separados 16 mm).
6. **Pared:** una pletina tipo *French cleat* (bisel de 45° de 2 mm) atornillada a la
   pared. La carcasa la recibe en un bolsillo de su cara trasera y se cuelga
   deslizando 2,25 mm hacia abajo.
7. **Holguras:** los mismos parámetros y valores que la A, para que una sola
   calibración ([docs/calibracion.md](../calibracion.md)) sirva para las dos.

## Alternativas descartadas
- **Loft entre perfiles con splines:** daría curvatura continua (G2), pero no hay
  forma sencilla de garantizar los 45° en cada punto ni de calcular el interior
  a pared constante. El contorno de arcos y rectas tangentes (G1) se desplaza
  hacia dentro de forma exacta.
- **Partir por un plano horizontal (arriba / abajo):** las dos mitades tendrían una
  cúpula con el interior en voladizo.
- **Ojales en la cara trasera (como la A):** se pidió otro sistema. Además, la
  pletina deja la carcasa centrada y apretada contra la pared.
- **Bolsillo en cola de milano:** su techo sería un puente de más de 20 mm.
- **Tetones en las esquinas inferiores:** el redondeo trasero deja poco sitio y
  los avellanados quedarían a menos de 0,8 mm del borde.

## Consecuencias
- Es más grande que la A: 64 × 48,8 × 93 mm, frente a 47 × 48 × 90 mm.
- 6 piezas en lugar de 5. El cartucho es más complejo, aunque se imprime igual:
  tumbado de lado y sin soportes.
- Un llavero de más de 22 mm de ancho obliga a aumentar el radio superior; el
  modelo lo avisa.
- Puentes en el límite: el techo del bolsillo de la pletina mide 10,0 mm.
- La auditoría automática (README de la B) no encuentra voladizos de más de 45°,
  solapes ni choques al insertar el cartucho o al colgar la carcasa.

## Pendiente
1. Elegir entre la B y la C.
2. Imprimir las piezas de prueba (`testMode`) y la pletina.
3. Exportar los STL de las 6 piezas.
