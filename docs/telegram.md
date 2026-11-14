# Telegram

KEEEI publica en un **canal** con un bot. No responde a comandos: solo escribe
(ver [ADR 0002](decisiones/0002-canal-sin-bot-con-mensaje-fijado.md)).

## Prepararlo

1. Habla con [@BotFather](https://t.me/BotFather), usa `/newbot` y guarda el
   **token**.
2. Crea el canal (o usa el de la rama) y añade el bot como **administrador**,
   con permiso para publicar y borrar mensajes. Lo de borrar no es opcional: el
   firmware elimina el aviso anterior antes de enviar el nuevo.
3. Saca el `chat_id`: publica algo en el canal y abre
   `https://api.telegram.org/bot<TOKEN>/getUpdates`. El número que empieza por
   `-100` es el que buscas.
4. Ponlos en `include/secrets.h` (copia de `secrets.h.example`).

## Qué escribe

| Evento | Mensaje |
|---|---|
| Entran las llaves | 🟢 **Hay alguien en el cubo.** |
| Salen las llaves | 🔴 **No hay nadie en el cubo.** |

Usa el formato HTML de Telegram (`<b>`) y cada mensaje cabe en 64 bytes.

## Mensaje fijado

El texto para fijar en el canal, con instrucciones y contacto, está en
[telegram/mensaje-fijado.md](telegram/mensaje-fijado.md). El firmware solo
borra sus propios mensajes, así que el fijado no se toca.

## Certificado

`secrets.h.example` trae el certificado raíz de Baltimore, que caducó en mayo de
2025. Si la conexión HTTPS falla, usa el `TELEGRAM_CERTIFICATE_ROOT` que trae la
librería UniversalTelegramBot (`TelegramCertificate.h`) o el certificado raíz
actual de `api.telegram.org`.
