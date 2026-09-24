/**
 * @file LedManager.cpp
 * @brief Archivo de implementación del controlador de leds.
 * @author Romgm
 */

 #include "LedManager.h"
 #include <Arduino.h>

 #define BLINK_INTERVAL_MS 500U //tiempo de parpadeo.

 static uint8_t s_green_led_pin = 0U;
 static uint8_t s_red_led_pin = 0U;
 static led_mode_t s_current_mode = LED_MODE_OFF;
 static uint32_t s_last_toggle_ms = 0U;
 static bool s_blink_state = false;

/**
 * @brief Activa o desactiva los leds físicos segun los parámetros.
 * @param green Estado del led verde (true = encendido, false = apagado).
 * @param red Estado del led rojo (true = encendido, false = apagado).
 */
 void set_physical_leds(bool green, bool red) {
     digitalWrite(s_green_led_pin, green ? HIGH : LOW);
     digitalWrite(s_red_led_pin, red ? HIGH : LOW);
 }

 void led_manager_init(uint8_t green_led_pin, uint8_t red_led_pin) {
     s_green_led_pin = green_led_pin;
     s_red_led_pin = red_led_pin;

     pinMode(s_green_led_pin, OUTPUT);
     pinMode(s_red_led_pin, OUTPUT);

     s_current_mode = LED_MODE_OFF;
     s_last_toggle_ms = 0U;
     s_blink_state = false;
     set_physical_leds(false, false); // Inicialmente apagados
 }

 void led_manager_set_mode(led_mode_t mode) {
    if (mode != s_current_mode) {
        s_current_mode = mode;
        s_blink_state = false;
        s_last_toggle_ms = millis();
        switch (s_current_mode) {
            case LED_MODE_OFF:
                set_physical_leds(false, false);
                break;
            case LED_MODE_SOLID_GREEN:
                set_physical_leds(true, false);
                break;
            case LED_MODE_SOLID_RED:
                set_physical_leds(false, true);
                break;
            case LED_MODE_BLINKING_GREEN:
            case LED_MODE_BLINKING_RED:
            case LED_MODE_BLINKING_BOTH:
                s_blink_state = true;
                break;
        }
    }
 }
 void led_manager_update(uint32_t current_time_ms) {
    if (s_current_mode == LED_MODE_BLINKING_GREEN ||
        s_current_mode == LED_MODE_BLINKING_RED ||
        s_current_mode == LED_MODE_BLINKING_BOTH)
    {
        if((current_time_ms - s_last_toggle_ms)>= BLINK_INTERVAL_MS) {
            s_last_toggle_ms = current_time_ms;
            s_blink_state = !s_blink_state;

            switch (s_current_mode) {
                case LED_MODE_BLINKING_GREEN:
                    set_physical_leds(s_blink_state, false);
                    break;
                case LED_MODE_BLINKING_RED:
                    set_physical_leds(false, s_blink_state);
                    break;
                case LED_MODE_BLINKING_BOTH:
                    set_physical_leds(s_blink_state, s_blink_state);
                    break;
                default:
                    break;
            }
        }
    }

 }