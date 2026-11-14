# 0002. Canal de Telegram sin bot, con mensaje fijado

- Estado: Aceptada
- Fecha: 2026-11-14

## Contexto
Se valoró un bot con comandos (`/estado`, `/info`…). El firmware solo envía
mensajes (no lee `getUpdates`), así que un bot exigiría un servicio aparte
funcionando siempre.

## Decisión
No hay bot. El canal tiene **un mensaje fijado** con la información de siempre:
qué es KEEEI, cómo leer los mensajes, qué significan los LED, el contacto, el
repositorio y el correo. El texto está en [`docs/telegram/mensaje-fijado.md`](../telegram/mensaje-fijado.md).

## Alternativas descartadas
- Bot con comandos en un servidor o Raspberry: más mantenimiento a cambio de
  muy poco.

## Consecuencias
- El firmware borra solo el último mensaje que **él** envió (`last_msg_id` en
  NVS), así que el mensaje fijado no se borra.
