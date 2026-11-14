#include <Arduino.h>
#include <esp_system.h>
#include <string.h>
#include <freertos/FreeRTOS.h>
#include <freertos/task.h>
#include <freertos/queue.h>
#include "SensorManager.h"
#include "LedManager.h"
#include "TelegramManager.h"

#define SENSOR_PIN 19
#define GREEN_LED  25
#define RED_LED    26

#define INTERVALO_RECONEXION 10000U
#define TIMEOUT_WIFI_INICIO_MS 15000U

typedef enum{
    INICIALIZANDO,
    REPOSO,
    LLAVES_PUESTAS,
    LLAVES_QUITADAS,
    TRAMITANDO_PUESTAS,
    TRAMITANDO_QUITADAS,
    ERROR
} estados_t;

typedef enum{
    ERROR_NINGUNO,
    ERROR_WIFI,
    ERROR_TELEGRAM
} tipo_error_t;

typedef struct {
    char texto[64];
    uint8_t estado_sensor;
} mensaje_telegram_t;

static tipo_error_t s_err = ERROR_NINGUNO;
static estados_t s_estado = INICIALIZANDO;
static uint32_t s_tiempo_inicio = 0U;

static QueueHandle_t s_telegram_queue = NULL;
static volatile uint8_t s_telegram_busy = 0U;
static volatile uint8_t s_telegram_result = 0xFFU;

void Task_Telegram(void *pvParameters)
{
    (void)pvParameters;
    
    mensaje_telegram_t msg;
    
    while(TelegramManager_is_wifi_connected() == 0U){
        vTaskDelay(pdMS_TO_TICKS(500));
    }
    
    while(1)
    {
        if(xQueueReceive(s_telegram_queue, &msg, portMAX_DELAY) == pdTRUE)
        {
            s_telegram_busy = 1U;
            s_telegram_result = 0xFFU;
            
            const uint8_t MAX_REINTENTOS = 3;
            uint8_t exito = 0U;
            
            for(uint8_t intento = 0; intento < MAX_REINTENTOS && exito == 0U; intento++)
            {
                if(TelegramManager_is_wifi_connected() == 0U){
                    vTaskDelay(pdMS_TO_TICKS(5000));
                    continue;
                }
                
                exito = TelegramManager_send_message(msg.texto);
                
                if(exito == 0U && intento < (MAX_REINTENTOS - 1)){
                    vTaskDelay(pdMS_TO_TICKS(2000));
                }
            }
            
            if(exito == 1U){
                sensor_manager_nvs_write(msg.estado_sensor);
                s_telegram_result = 1U;
            } else {
                s_telegram_result = 0U;
            }
            
            s_telegram_busy = 0U;
            vTaskDelay(pdMS_TO_TICKS(10));
        }
    }
}

void setup() {
    sensor_manager_init(SENSOR_PIN);
    led_manager_init(GREEN_LED, RED_LED);
    TelegramManager_init();
    
    s_tiempo_inicio = millis();
    s_estado = INICIALIZANDO;
    
    s_telegram_queue = xQueueCreate(5, sizeof(mensaje_telegram_t));
    
    if(s_telegram_queue == NULL){
        led_manager_set_mode(LED_MODE_BLINKING_BOTH);
        delay(5000);
        esp_restart();
    }
    
    BaseType_t resultado = xTaskCreatePinnedToCore(
        Task_Telegram,
        "TelegramTask",
        8192,
        NULL,
        1,
        NULL,
        0
    );
    
    if(resultado != pdPASS){
        led_manager_set_mode(LED_MODE_BLINKING_BOTH);
        delay(5000);
        esp_restart();
    }
}

void loop() {
    sensor_evento_t evento;
    sensor_manager_check_event(&evento);
    switch (s_estado){
        case INICIALIZANDO:
        {
            led_manager_set_mode(LED_MODE_SLOW_BLINKING_BOTH);

            if(TelegramManager_is_wifi_connected() == 1U){
                s_estado = REPOSO;
            }else if((millis() - s_tiempo_inicio) > TIMEOUT_WIFI_INICIO_MS){
                s_err = ERROR_WIFI;
                s_estado = ERROR;
            }
        }
        break;
        
        case REPOSO:
        {
            uint8_t estado_guardado = sensor_manager_nvs_read();
            if((evento.estado_actual == estado_guardado) && (evento.estado_actual == 1U)){
                s_estado = LLAVES_PUESTAS;
                led_manager_set_mode(LED_MODE_SOLID_GREEN);
            }else if((evento.estado_actual == estado_guardado) && (evento.estado_actual == 0U)){
                s_estado = LLAVES_QUITADAS;
                led_manager_set_mode(LED_MODE_SOLID_RED);
            }else if((evento.estado_actual != estado_guardado) && (evento.estado_actual == 1U)){
                s_estado = TRAMITANDO_PUESTAS;
            }else if((evento.estado_actual != estado_guardado) && (evento.estado_actual == 0U)){
                s_estado = TRAMITANDO_QUITADAS;
            }
        }
        break;

        case LLAVES_PUESTAS:
        {
            if((evento.evento_detectado != 0U) && (evento.estado_actual == 0U))
            {
                s_estado = TRAMITANDO_QUITADAS;
            }
        }
        break;

        case LLAVES_QUITADAS:
        {
            if((evento.evento_detectado != 0U) && (evento.estado_actual == 1U))
            {
                s_estado = TRAMITANDO_PUESTAS;
            }
        }
        break;

        case TRAMITANDO_PUESTAS:
        {
            static uint8_t mensaje_encolado = 0U;
            
            if(mensaje_encolado == 0U)
            {
                if(TelegramManager_is_wifi_connected() == 0U){
                    s_err = ERROR_WIFI;
                    s_estado = ERROR;
                    led_manager_set_mode(LED_MODE_BLINKING_BOTH);
                    mensaje_encolado = 0U;
                    break;
                }
                
                mensaje_telegram_t msg;
                strncpy(msg.texto, 
                        "🟢 Llaves colocadas en el sensor.", 
                        sizeof(msg.texto) - 1);
                msg.texto[sizeof(msg.texto) - 1] = '\0';
                msg.estado_sensor = 1U;
                
                s_telegram_result = 0xFFU;
                if(xQueueSend(s_telegram_queue, &msg, 0) == pdTRUE){
                    mensaje_encolado = 1U;
                    led_manager_set_mode(LED_MODE_BLINKING_GREEN);
                } else {
                    s_err = ERROR_TELEGRAM;
                    s_estado = ERROR;
                    led_manager_set_mode(LED_MODE_BLINKING_BOTH);
                    mensaje_encolado = 0U;
                    break;
                }
            }
            
            led_manager_update(millis());
            
            if((s_telegram_busy == 0U) && (s_telegram_result != 0xFFU))
            {
                if(s_telegram_result == 1U){
                    s_estado = LLAVES_PUESTAS;
                    led_manager_set_mode(LED_MODE_SOLID_GREEN);
                    mensaje_encolado = 0U;
                } else {
                    s_err = ERROR_TELEGRAM;
                    s_estado = ERROR;
                    led_manager_set_mode(LED_MODE_BLINKING_BOTH);
                    mensaje_encolado = 0U;
                }
            }
        }
        break;

        case TRAMITANDO_QUITADAS:
        {
            static uint8_t mensaje_encolado = 0U;
            
            if(mensaje_encolado == 0U)
            {
                if(TelegramManager_is_wifi_connected() == 0U){
                    s_err = ERROR_WIFI;
                    s_estado = ERROR;
                    led_manager_set_mode(LED_MODE_BLINKING_BOTH);
                    mensaje_encolado = 0U;
                    break;
                }
                
                mensaje_telegram_t msg;
                strncpy(msg.texto, 
                        "🔴 Llaves retiradas del sensor.", 
                        sizeof(msg.texto) - 1);
                msg.texto[sizeof(msg.texto) - 1] = '\0';
                msg.estado_sensor = 0U;
                
                s_telegram_result = 0xFFU;
                if(xQueueSend(s_telegram_queue, &msg, 0) == pdTRUE){
                    mensaje_encolado = 1U;
                    led_manager_set_mode(LED_MODE_BLINKING_RED);
                } else {
                    s_err = ERROR_TELEGRAM;
                    s_estado = ERROR;
                    led_manager_set_mode(LED_MODE_BLINKING_BOTH);
                    mensaje_encolado = 0U;
                    break;
                }
            }
            
            led_manager_update(millis());
            
            if((s_telegram_busy == 0U) && (s_telegram_result != 0xFFU))
            {
                if(s_telegram_result == 1U){
                    s_estado = LLAVES_QUITADAS;
                    led_manager_set_mode(LED_MODE_SOLID_RED);
                    mensaje_encolado = 0U;
                } else {
                    s_err = ERROR_TELEGRAM;
                    s_estado = ERROR;
                    led_manager_set_mode(LED_MODE_BLINKING_BOTH);
                    mensaje_encolado = 0U;
                }
            }
        }
        break;

        case ERROR:
        {
            static uint8_t retrys = 0U;
            static uint32_t last_time = 0U;
            
            led_manager_set_mode(LED_MODE_BLINKING_BOTH);
            led_manager_update(millis());
            
            if(s_err == ERROR_WIFI){
                // Serial.println("error wifi");
                if(TelegramManager_is_wifi_connected() == 0U){
                    uint32_t actual_time = millis();
                    
                    if(actual_time - last_time >= INTERVALO_RECONEXION){
                        last_time = actual_time;
                        
                        if(retrys < 5){
                            retrys++;
                        } else {
                            esp_restart();
                        }
                    }
                } else {
                    retrys = 0U;
                    s_err = ERROR_NINGUNO;
                    s_estado = REPOSO;
                }
            }
            else if(s_err == ERROR_TELEGRAM){
                // Serial.println("error telegram");
                uint32_t actual_time = millis();
                if(actual_time - last_time >= INTERVALO_RECONEXION){
                    last_time = actual_time;
                    retrys = 0U;
                    s_err = ERROR_NINGUNO;
                    s_estado = REPOSO;
                }
            }
        }
        break;
        
        default:
        {
            s_estado = REPOSO;
            s_err = ERROR_NINGUNO;
            led_manager_set_mode(LED_MODE_OFF);
        }
        break;
    }
    
    led_manager_update(millis());
}
