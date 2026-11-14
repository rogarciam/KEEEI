# include <Arduino.h>
#include "SensorManager.h"
#include "nvs_flash.h" //librerías para el manejo de la memoria NVS
#include "nvs.h"       //librerías para el manejo de la memoria NVS

#define SENSOR_DEBOUNCE_MS 80U

static uint8_t s_sensor_pin = 0U;
static uint8_t s_sensor_state = 0U;
static uint8_t s_lectura_candidata = 0U;
static uint32_t s_candidata_desde_ms = 0U;

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

static uint8_t sensor_leer_pin(void)
{
    // INPUT_PULLUP: LOW = switch cerrado a GND = llave puesta
    return (digitalRead(s_sensor_pin) == LOW) ? 1U : 0U;
}

void sensor_manager_init(uint8_t sensor_pin)
{
    s_sensor_pin = sensor_pin;
    pinMode(s_sensor_pin, INPUT_PULLUP);

    s_sensor_state = sensor_leer_pin();
    s_lectura_candidata = s_sensor_state;
    s_candidata_desde_ms = millis();

    sensor_manager_nvs_init();
}

void sensor_manager_check_event(sensor_evento_t* evento)
{
    uint8_t lectura = sensor_leer_pin();
    uint32_t ahora = millis();

    evento->evento_detectado = 0U;

    if (lectura != s_lectura_candidata) {
        s_lectura_candidata = lectura;
        s_candidata_desde_ms = ahora;
    } else if ((lectura != s_sensor_state) &&
               ((ahora - s_candidata_desde_ms) >= SENSOR_DEBOUNCE_MS)) {
        s_sensor_state = lectura;
        evento->evento_detectado = 1U;
    }

    evento->estado_actual = s_sensor_state;
}

uint8_t sensor_manager_get_state(void)
{
    return s_sensor_state;
}
