/**
 * @file SensorManager.h
 * @brief Control y gestión del sensor final de carrera, memoria NVS e interrupción por hardware.
 * @author Romgm
 * @date 23-09-2026
 */
# ifndef SENSOR_MANAGER_H
#define SENSOR_MANAGER_H

# include <stdio.h>

typedef struct {
    uint8_t evento_detectado;   // 1 si hay evento, 0 si no
    uint8_t estado_actual;      // 1 = sensor activo, 0 = inactivo
} sensor_evento_t;

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
 * @note Configura el pin como entrada SIN resistencia pull-up interna 
 *       (requerida para GPIOs input-only como GPIO 34, 36, 39).
 *       CRÍTICO: Añadir resistencia pull-up externa de 10kΩ entre el pin y VCC (3.3V).
 *       Establece la interrupción por cambio de estado e inicializa la memoria NVS.
 */
void sensor_manager_init(uint8_t sensor_pin);

/**
 * @brief Verifica si ha ocurrido un evento en el sensor y actualiza la variable estado global del sensor.
 * @param[out] evento Puntero a estructura donde se almacenará el resultado.
 * @note esta función implementa un mecanismo de debounce para evitar lecturas erróneas debido a rebotes del sensor.
 *       Debe ser llamada periódicamente en el bucle principal para detectar cambios de estado.
 */
void sensor_manager_check_event(sensor_evento_t* evento);

/**
 * @brief Obtiene el estado actual del sensor.
 * @return 1U si el sensor está activo, 0U en caso contrario.
 */
uint8_t sensor_manager_get_state(void);

# endif
