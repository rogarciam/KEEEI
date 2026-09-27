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
 * @brief Sends a message via the Telegram bot.
 * @param message The message to be sent as a C-style string.
 * @return 1U if the message was sent successfully, 0U otherwise.
 */
uint8_t TelegramManager_send_message(const char* message);

/**
 * @brief Initializes the TelegramManager module.
 * @note This function sets the WiFi mode to station and connects to the WiFi network.
 *       It also sets the root certificate for secure communication with the Telegram API.
 */
void TelegramManager_init(void);

#ifdef __cplusplus
}
#endif

#endif //TELEGRAM_MANAGER_H