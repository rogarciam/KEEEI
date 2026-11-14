/**
 * @file TelegramManager.h
 * @brief Header file for TelegramManager module.
 * @author Romgm
 * @date 23-09-2026
 * @note This module provides functions to manage WiFi connection and send messages via Telegram bot.
 */
#ifndef TELEGRAM_MANAGER_H
#define TELEGRAM_MANAGER_H

#ifdef __cplusplus
extern "C" {
#endif

#include <stdint.h>

/**
 * @brief Reconnects to previously connected wifi
 * @note  Will disconnect and then try to connect again.
 */
void TelegramManager_wifi_reconnect(void);

/**
 * @brief Checks if the device is connected to WiFi.
 * @return 1U if connected, 0U otherwise.
 */
uint8_t TelegramManager_is_wifi_connected(void);

/**
 * @brief Publica un mensaje en el canal, borrando antes el mensaje anterior.
 * @param message Texto completo del mensaje, enviado tal cual en formato HTML.
 * @return 1U si el mensaje se envió correctamente, 0U en caso contrario.
 */
uint8_t TelegramManager_send_message(const char* message);

/**
 * @brief Initializes the TelegramManager module.
 * @note Configura WiFi en modo estación, el certificado TLS y recupera de NVS el ID del último mensaje.
 */
void TelegramManager_init(void);

#ifdef __cplusplus
}
#endif

#endif //TELEGRAM_MANAGER_H