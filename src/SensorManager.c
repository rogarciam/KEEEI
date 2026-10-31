# include <Arduino.h>
#include "esp_attr.h"  //libería para acceso rapido a función(guardada en ram)
#include "SensorManager.h"
#include "nvs_flash.h" //librerías para el manejo de la memoria NVS
#include "nvs.h"       //librerías para el manejo de la memoria NVS

#define SENSOR_DEBOUNCE_MS 50U //tiempo de rebote del sensor.


static uint8_t s_sensor_pin = 0U;
static uint8_t s_sensor_state = 0U;
static volatile uint8_t s_sensor_state_changed = 0U;

//inicio de memoria nvms
void sensor_manager_nvs_init(void)
{
    esp_err_t err = nvs_flash_init();
    if (err == ESP_ERR_NVS_NO_FREE_PAGES || err == ESP_ERR_NVS_NEW_VERSION_FOUND) {
        ESP_ERROR_CHECK(nvs_flash_erase()); // Borra si está corrupta o llena
        err = nvs_flash_init();             // Re-inicializa
    }
    ESP_ERROR_CHECK(err);
}

void sensor_manager_nvs_write(uint8_t estado)
{
    nvs_handle_t memoria_nvs;
    
    // Abrimos el espacio de nombres
    esp_err_t err = nvs_open("config_space", NVS_READWRITE, &memoria_nvs);
    if (err == ESP_OK) {
        // Escribimos en el buffer de la RAM
        err = nvs_set_u8(memoria_nvs, "estado_sensor", estado);
        
        if (err == ESP_OK) {
            // Guardamos físicamente en la memoria Flash
            nvs_commit(memoria_nvs);
        }
        
        // Cerramos el manejador para liberar memoria
        nvs_close(memoria_nvs);
    }
}

uint8_t sensor_manager_nvs_read(void)
{
    uint8_t salida = 0U;
    nvs_handle_t memoria_nvs;
    
    esp_err_t err = nvs_open("config_space", NVS_READONLY, &memoria_nvs); // Usamos READONLY por seguridad
    if (err == ESP_OK) {
        err = nvs_get_u8(memoria_nvs, "estado_sensor", &salida);
        
        if (err == ESP_ERR_NVS_NOT_FOUND) {
            //printf("La variable no existe aún. Valor por defecto: %d\n", salida);
        } else if (err != ESP_OK) {
            //printf("Error al leer NVS: %s\n", esp_err_to_name(err));
        } else {
            //printf("Variable leída con éxito: %d\n", salida);
        }
        
        nvs_close(memoria_nvs);
    }
    return salida;
}

static void IRAM_ATTR sensor_manager_isr(void)
{
    s_sensor_state_changed = 1U;
}

void sensor_manager_init(uint8_t sensor_pin)
{
    s_sensor_pin = sensor_pin;

    // IMPORTANTE: GPIO 34 no tiene pull-up interno
    // Configurar como INPUT (sin PULLUP interno)
    pinMode(s_sensor_pin, INPUT);
    
    // CRÍTICO: Añadir resistencia pull-up externa de 10kΩ en hardware
    // entre GPIO34 y VCC (3.3V)

    attachInterrupt(digitalPinToInterrupt(s_sensor_pin), sensor_manager_isr, CHANGE);
    sensor_manager_nvs_init();

}


void sensor_manager_check_event(sensor_evento_t* evento)
{
    static uint32_t s_last_valid_event = 0U;
    static uint8_t s_event_time_saved = 0U;
    static uint8_t s_sensor_propper_event = 0U;
    
    if(s_sensor_state_changed)
    {
        if(s_event_time_saved == 0U)
        {
            s_last_valid_event = millis();
            s_event_time_saved = 1U;
        }
        else if(millis() - s_last_valid_event >= SENSOR_DEBOUNCE_MS)
        {
            // Lectura atómica con deshabilitación temporal de interrupciones
            noInterrupts();  // Deshabilitar interrupciones globalmente
            s_sensor_state = digitalRead(s_sensor_pin);
            s_sensor_state_changed = 0U;
            interrupts();    // Re-habilitar interrupciones
            
            s_event_time_saved = 0U;
            s_sensor_propper_event = 1U;
        }
    }
    
    // Llenar estructura de salida de forma atómica
    evento->evento_detectado = s_sensor_propper_event;
    evento->estado_actual = s_sensor_state;
    
    // Reset del flag de evento después de leerlo
    s_sensor_propper_event = 0U;
}

uint8_t sensor_manager_get_state(void)
{
    return s_sensor_state;
}