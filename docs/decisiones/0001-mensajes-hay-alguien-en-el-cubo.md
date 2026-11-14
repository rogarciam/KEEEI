# 0001. Mensajes del canal: «Hay / No hay nadie en el cubo»

- Estado: Aceptada
- Fecha: 2026-11-14

## Contexto
En la rama IEEE SB Unizar la pregunta habitual es «¿Hay alguien en el cubo?».
El firmware publicaba «Llaves colocadas/retiradas del sensor», un texto que
obliga a interpretar qué significa.

## Decisión
- Llaves puestas en KEEEI → `🟢 <b>Hay alguien en el cubo.</b>`
- Llaves retiradas → `🔴 <b>No hay nadie en el cubo.</b>`

Solo cambia el texto de `src/main.cpp`; la máquina de estados no cambia.
Los mensajes ocupan 36 bytes como máximo (el búfer es de 64, y cada emoji ocupa 4).
La hora la muestra Telegram en cada mensaje. Como el dispositivo borra el
mensaje anterior, el canal muestra siempre **solo el estado actual**.

## Alternativas descartadas
- Mensajes largos con hora y ubicación: no caben en 64 bytes sin ampliar el
  búfer, y Telegram ya muestra la hora.

## Consecuencias
- Si en el futuro se quiere añadir el nombre del local, hay que comprobar
  que el texto siga cabiendo en `mensaje_telegram_t.texto`.
