#include <secrets.h>
#include "TelegramManager.h"
#include <UniversalTelegramBot.h>
#include <WiFi.h>
#include <WiFiClientSecure.h>
#include "nvs_flash.h"
#include "nvs.h"

#define NVS_NAMESPACE_TELEGRAM "telegram"
#define NVS_KEY_LAST_MSG_ID    "last_msg_id"

static WiFiClientSecure s_secured_client;
static UniversalTelegramBot s_bot(TELEGRAM_BOT_TOKEN, s_secured_client);
static int32_t s_last_message_id = 0;

static void telegram_nvs_load_last_id(void)
{
    nvs_handle_t nvs_handle;
    if (nvs_open(NVS_NAMESPACE_TELEGRAM, NVS_READONLY, &nvs_handle) != ESP_OK) {
        return;
    }
    nvs_get_i32(nvs_handle, NVS_KEY_LAST_MSG_ID, &s_last_message_id);
    nvs_close(nvs_handle);
}

static void telegram_nvs_save_last_id(int32_t message_id)
{
    nvs_handle_t nvs_handle;
    if (nvs_open(NVS_NAMESPACE_TELEGRAM, NVS_READWRITE, &nvs_handle) != ESP_OK) {
        return;
    }
    if (nvs_set_i32(nvs_handle, NVS_KEY_LAST_MSG_ID, message_id) == ESP_OK) {
        nvs_commit(nvs_handle);
    }
    nvs_close(nvs_handle);
}

static uint8_t telegram_delete_message(int32_t message_id)
{
    String command = s_bot.buildCommand("deleteMessage?chat_id=" + String(TELEGRAM_CHAT_ID) + "&message_id=" + String(message_id));
    String response = s_bot.sendGetToTelegram(command);
    return s_bot.checkForOkResponse(response) ? 1U : 0U;
}

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
    // Best-effort: el mensaje anterior puede haber sido borrado manualmente.
    if (s_last_message_id > 0) {
        (void)telegram_delete_message(s_last_message_id);
    }

    if (!s_bot.sendMessage(TELEGRAM_CHAT_ID, message, "HTML")) {
        return 0U;
    }

    s_last_message_id = s_bot.last_sent_message_id;
    telegram_nvs_save_last_id(s_last_message_id);
    return 1U;
}

void TelegramManager_init(void)
{
    WiFi.mode(WIFI_STA);
    WiFi.begin(WIFI_SSID, WIFI_PASSWORD);
    s_secured_client.setCACert(TELEGRAM_CERTIFICATE_ROOT);
    telegram_nvs_load_last_id();
}
