# 0006. Ramas de git y ramas de Onshape en paralelo

- Estado: Aceptada
- Fecha: 2026-11-14

## Contexto
Las carcasas B y C se van a diseñar en paralelo, a partir de la A modular, con
un subagente cada una. Cada opción necesita su línea de trabajo tanto en el
repositorio como en el documento de Onshape «KEEI», sin pisarse.

## Decisión
| Opción | Rama de git | Rama (workspace) de Onshape | Parte de |
|---|---|---|---|
| A · ranura vertical | `carcasa/a-ranura-vertical` | `Main` | — |
| B | `carcasa/b-pildora-pared` | `Carcasa B` | Versión V3 |
| C | `carcasa/c-totem-mesa` | `Carcasa C` | Versión V3 |

- **V3** («Carcasa A modular 5 piezas») corresponde al commit `49b8041`.
- **V2** guarda la carcasa A de 3 piezas, antes de la ADR 0005.
- El código fuente es el FeatureScript del repositorio. Onshape se actualiza
  desde él, nunca al revés, para que git sea la fuente de la verdad.
- Cada vez que una opción alcance un estado estable se crea una versión en su
  rama de Onshape y se anota el commit en la descripción.

## Consecuencias
- Si se corrige algo común (por ejemplo, el cartucho), se hace en la A y se
  lleva a B y C con `git merge` y subiendo de nuevo su FeatureScript.
- Con la cuenta gratuita de Onshape el documento es **público**: no se debe
  guardar en él nada privado.
