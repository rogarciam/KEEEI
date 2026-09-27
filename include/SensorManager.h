/**
 * @file SensorManager.h
 * @brief Control y gestión del sensor final de carrera, memoria NVS e interrupción por hardware.
 * @author Romgm
 * @date 23-09-2026
 */
# ifndef SENSOR_MANAGER_H
#define SENSOR_MANAGER_H

# include <stdio.h>

/**
 * @brief Escribe un valor en la memoria NVS.
 * @param estado El valor a escribir de tipo uint8_t.
 * @note  Sobreescribe el valor existente y debe ejecutarse al final ya que interrumple acceso a memoria flash.
 */
void sensor_manager_nvs_write(uint8_t estado);

/**
 * @brief Lee un valor de la memoria NVS.
 * @return El valor leído de tipo uint8_t. Si no se encuentra, devuelve 0U por defecto.
 */
uint8_t sensor_manager_nvs_read(void);

/**
 * @brief Inicializa el gestor de sensores.
 * @param sensor_pin El pin GPIO del sensor final de carrera.
 * @note Configura el pin como entrada con resistencia pull-up y establece la interrupción por cambio de estado.
 *       la función también inicializa la memoria NVS.
 */
void sensor_manager_init(uint8_t sensor_pin);

/**
 * @brief Verifica si ha ocurrido un evento en el sensor y actualiza la variable estado global del sensor.
 * @return 1U si ha ocurrido un evento válido, 0U en caso contrario.
 * @note esta función implementa un mecanismo de debounce para evitar lecturas erróneas debido a rebotes del sensor.
 *       Debe ser llamada periódicamente en el bucle principal para detectar cambios de estado.
 */
uint8_t sensor_manager_check_event(void);

/**
 * @brief Obtiene el estado actual del sensor.
 * @return 1U si el sensor está activo, 0U en caso contrario.
 */
uint8_t sensor_manager_get_state(void);

# endif
