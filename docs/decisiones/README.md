# Registro de decisiones (ADR)

Cada decisión importante del proyecto KEEEI se apunta aquí en un archivo corto
numerado (`NNNN-titulo.md`). Así se sabe **qué** se decidió, **por qué** y qué
alternativas se descartaron, aunque pase tiempo.

| Estado | Significado |
|---|---|
| Propuesta | Pendiente de aprobar |
| Aceptada | En vigor |
| Sustituida | Reemplazada por otra ADR (se indica cuál) |
| Descartada | Se valoró y no se hará |

## Índice

| Nº | Decisión | Estado |
|---|---|---|
| [0001](0001-mensajes-hay-alguien-en-el-cubo.md) | Mensajes del canal: «Hay / No hay nadie en el cubo» | Aceptada |
| [0002](0002-canal-sin-bot-con-mensaje-fijado.md) | Canal de Telegram sin bot, con mensaje fijado | Aceptada |
| [0003](0003-ramas-para-opciones-de-carcasa.md) | Una rama por opción de carcasa | Aceptada |
| [0004](0004-licencias.md) | Licencias del repositorio | Propuesta |
| [0005](0005-carcasa-a-modular.md) | Carcasa A: dividirla en módulos según la orientación de impresión | Aceptada |
| [0006](0006-ramas-git-y-onshape.md) | Ramas de git y ramas de Onshape en paralelo | Aceptada |
| [0007](0007-carcasa-b-organica.md) | Carcasa B: forma de canto rodado partida por el plano del USB | Propuesta |

## Plantilla

```markdown
# NNNN. Título

- Estado: Propuesta | Aceptada | Sustituida por NNNN | Descartada
- Fecha: AAAA-MM-DD

## Contexto
## Decisión
## Alternativas descartadas
## Consecuencias
```
