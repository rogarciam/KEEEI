#include "LedManager.h"
#include <Arduino.h>

#define BLINK_INTERVAL_FAST_MS 100U   // Semiperiodo: parpadeo a 5 Hz
#define BLINK_INTERVAL_SLOW_MS 500U   // Semiperiodo: parpadeo a 1 Hz

static uint8_t s_green_led_pin = 0U;
static uint8_t s_red_led_pin = 0U;
static led_mode_t s_current_mode = LED_MODE_OFF;
static uint32_t s_last_toggle_ms = 0U;
static bool s_blink_state = false;

static void set_physical_leds(bool green, bool red)
{
    digitalWrite(s_green_led_pin, green ? HIGH : LOW);
    digitalWrite(s_red_led_pin, red ? HIGH : LOW);
}

static bool is_blinking_mode(led_mode_t mode)
{
    return (mode == LED_MODE_BLINKING_GREEN) ||
           (mode == LED_MODE_BLINKING_RED) ||
           (mode == LED_MODE_BLINKING_BOTH) ||
           (mode == LED_MODE_SLOW_BLINKING_BOTH);
}

static uint32_t blink_interval_ms(led_mode_t mode)
{
    return (mode == LED_MODE_SLOW_BLINKING_BOTH) ? BLINK_INTERVAL_SLOW_MS : BLINK_INTERVAL_FAST_MS;
}

static void set_blink_output(led_mode_t mode, bool on)
{
    switch (mode) {
        case LED_MODE_BLINKING_GREEN:
            set_physical_leds(on, false);
            break;
        case LED_MODE_BLINKING_RED:
            set_physical_leds(false, on);
            break;
        case LED_MODE_BLINKING_BOTH:
        case LED_MODE_SLOW_BLINKING_BOTH:
            set_physical_leds(on, on);
            break;
        default:
            break;
    }
}

void led_manager_init(uint8_t green_led_pin, uint8_t red_led_pin)
{
    s_green_led_pin = green_led_pin;
    s_red_led_pin = red_led_pin;

    pinMode(s_green_led_pin, OUTPUT);
    pinMode(s_red_led_pin, OUTPUT);

    s_current_mode = LED_MODE_OFF;
    s_last_toggle_ms = 0U;
    s_blink_state = false;
    set_physical_leds(false, false);
}

void led_manager_set_mode(led_mode_t mode)
{
    if (mode == s_current_mode) {
        return;
    }

    s_current_mode = mode;
    s_blink_state = true;
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
        default:
            set_blink_output(s_current_mode, s_blink_state);
            break;
    }
}

void led_manager_update(uint32_t current_time_ms)
{
    if (!is_blinking_mode(s_current_mode)) {
        return;
    }

    if ((current_time_ms - s_last_toggle_ms) >= blink_interval_ms(s_current_mode)) {
        s_last_toggle_ms = current_time_ms;
        s_blink_state = !s_blink_state;
        set_blink_output(s_current_mode, s_blink_state);
    }
}
