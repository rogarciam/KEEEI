# 0003. Una rama por opción de carcasa

- Estado: Aceptada
- Fecha: 2026-11-14

## Contexto
Hay tres diseños de carcasa en estudio y se elegirá uno. Se quiere que en
GitHub se vea que son **opciones** distintas.

## Decisión
- `main`: firmware y documentación común.
- `carcasa/a-ranura-vertical`: diseño actual (ranura vertical con leva lateral).
- `carcasa/b-…` y `carcasa/c-…`: se crearán con los nuevos diseños.

Cada rama añade su modelo en `modelos-3d/<opcion>/` con el código fuente
(FeatureScript), los STL, su README (orientación de impresión, piezas, montaje)
y su póster.

Cuando se elija una opción, se fusiona en `main` y las demás ramas se quedan
como histórico (no se borran).

## Alternativas descartadas
- Las tres opciones en carpetas de `main`: más simple, pero en GitHub no se ve
  que son alternativas en competencia.

## Consecuencias
- Los cambios que afecten a las tres (por ejemplo, las dimensiones de la
  protoboard) se hacen en `main` y luego se fusionan en cada rama.
