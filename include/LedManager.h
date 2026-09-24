/**
 * @file LedManager.h
 * @brief Control y gestión no bloqueante de indicadores LED del sistema.
 * @author Romgm
 * @date 23-09-2026
 */

# ifndef LED_MANAGER_H
# define LED_MANAGER_H

#include <stdio.h>
#include <stdbool.h>

/**
 * @brief Modos de funcionamiento del LED.
 */
typedef enum {
    LED_MODE_OFF = 0,
    LED_MODE_SOLID_GREEN,
    LED_MODE_SOLID_RED,
    LED_MODE_BLINKING_GREEN,
    LED_MODE_BLINKING_RED,
    LED_MODE_BLINKING_BOTH,
} led_mode_t;

/**
 * @brief Inicializa los periféricos GPIO asociados a los LEDs de estado.
 * 
 * @param pin_green GPIO correspondiente al indicador verde.
 * @param pin_red   GPIO correspondiente al indicador rojo.
 */
void led_manager_init(uint8_t green_led_pin, uint8_t red_led_pin);

/**
 * @brief Establece el modo de trabajo del subsistema de LEDs.
 * 
 * @param mode Estado objetivo definido en led_mode_t.
 */
void led_manager_set_mode(led_mode_t mode);

/**
 * @brief Maquinaria de refresco no bloqueante para temporizaciones de parpadeo.
 *        Debe ser invocada en cada iteración del bucle principal.
 * 
 * @param current_time_ms Marca de tiempo actual en milisegundos.
 */
void led_manager_update(uint32_t current_time_ms);
#endif // LED_MANAGER_H