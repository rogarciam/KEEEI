#include <secrets.h>
#include "TelegramManager.h"
#include <UniversalTelegramBot.h>
#include <WiFi.h>
#include <WiFiClientSecure.h>


static WiFiClientSecure s_secured_client;
static UniversalTelegramBot s_bot(TELEGRAM_BOT_TOKEN, s_secured_client);


void TelegramManager_wifi_reconnect(void)
{
    
    WiFi.reconnect();
}

uint8_t TelegramManager_is_wifi_connected(void)
{
    return (WiFi.status() == WL_CONNECTED);
}

uint8_t TelegramManager_send_message(const char* message)
{
    if (s_bot.sendMessage(TELEGRAM_CHAT_ID, message, "Markdown")) {
        return 1U; // Mensaje enviado con éxito
    } else {
        return 0U; // Error al enviar el mensaje
    }
}

void TelegramManager_init(void)
{
    WiFi.mode(WIFI_STA);
    WiFi.begin(WIFI_SSID, WIFI_PASSWORD);
    s_secured_client.setCACert(TELEGRAM_CERTIFICATE_ROOT);
}