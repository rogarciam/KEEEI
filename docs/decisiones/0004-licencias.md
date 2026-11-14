# 0004. Licencias del repositorio

- Estado: Propuesta
- Fecha: 2026-11-14

## Contexto
El repositorio será público. Sin licencia, legalmente nadie puede reutilizar
el código ni los modelos, aunque estén a la vista.

## Propuesta
| Contenido | Licencia | Qué permite |
|---|---|---|
| Firmware (`src/`, `include/`) | **MIT** | Cualquiera puede usar y modificar el código, incluso con fines comerciales, siempre que mantenga el aviso de autoría. |
| Modelos 3D, pósteres y documentación | **CC BY-SA 4.0** | Se puede imprimir y modificar citando a KEEEI / IEEE SB Unizar; las versiones modificadas deben publicarse con la misma licencia. Es la licencia habitual en Printables y Thingiverse. |

El logo y el nombre de IEEE son marcas registradas: ninguna de estas licencias
autoriza a usarlos. Hay que indicarlo en el README.

## Alternativas
- GPL-3.0 para el firmware: obliga a quien lo modifique a publicar su código.
  Es más restrictiva y para un proyecto de rama no aporta mucho.
- CERN-OHL-S/W/P: licencias pensadas para hardware (PCB, mecánica). Son más
  formales; tendrían sentido si hubiera PCB propia.
