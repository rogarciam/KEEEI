#include <Arduino.h>
#include "SensorManager.h"
#include "LedManager.h"
#include "TelegramManager.h"

#define SENSOR_PIN 34    // ← GPIO34 (input-only, ideal para sensores),  No tiene pull-up interno, requiere pull-up externo
#define GREEN_LED  25    // ← GPIO25 (DAC2, seguro para salidas)
#define RED_LED    26    // ← GPIO26 (DAC1, seguro para salidas)

#define INTERVALO_RECONEXION 10000U

typedef enum{
  INICIALIZANDO,
  REPOSO,
  LLAVES_PUESTAS,
  LLAVES_QUITADAS,
  TRAMITANDO_PUESTAS,
  TRAMITANDO_QUITADAS,
  ERROR
}estados_t;

typedef enum{
  ERROR_NINGUNO,
  ERROR_WIFI,
  ERROR_TELEGRAM
}tipo_error_t;

static tipo_error_t err = ERROR_NINGUNO;
static estados_t estado = INICIALIZANDO;
static uint32_t tiempo_inicio = 0U;

void setup() {
    SensorManager_init(SENSOR_PIN);
    led_manager_init(GREEN_LED, RED_LED);
    
    // Iniciar conexión WiFi
    TelegramManager_init();
    
    // Guardar timestamp de inicio para timeout
    tiempo_inicio = millis();
    
    // Estado inicial: INICIALIZANDO (permitir estabilización)
    estado = INICIALIZANDO;
    
    //printf("[SETUP] Sistema inicializado, esperando estabilización de red...\n");
}

void loop() {
  sensor_evento_t evento;
  sensor_manager_check_event(&evento);
  switch (estado){
    case INICIALIZANDO:
    {
        // Indicador visual: parpadeo de ambos LEDs
        led_manager_set_mode(LED_MODE_BLINKING_BOTH);
        led_manager_update(millis());
        
        // Esperar mínimo 5 segundos O hasta que WiFi esté conectado
        uint32_t tiempo_transcurrido = millis() - tiempo_inicio;
        
        if(tiempo_transcurrido > 5000U){  // Timeout de 5 segundos
            if(TelegramManager_is_wifi_connected() == 1U){
                // WiFi estable: transicionar a REPOSO
                estado = REPOSO;
                //printf("[INIT] Sistema estabilizado con WiFi conectado\n");
            }else{
                // WiFi no conectado tras timeout: entrar en modo ERROR
                err = ERROR_WIFI;
                estado = ERROR;
                //printf("[INIT] Timeout de conexión WiFi\n");
            }
        }
    }
    break;
    
    case REPOSO:
    {
      uint8_t estado_guardado = sensor_manager_nvs_read();

      if((evento.estado_actual == estado_guardado) && (evento.estado_actual== 1U)){
        estado = LLAVES_PUESTAS;
      }else if((evento.estado_actual == estado_guardado) && (evento.estado_actual == 0U)){
        estado = LLAVES_QUITADAS;
      }else if((evento.estado_actual != estado_guardado) && (evento.estado_actual == 1U)){
        estado = TRAMITANDO_PUESTAS;
      }else if((evento.estado_actual != estado_guardado) && (evento.estado_actual == 0U)){
        estado = TRAMITANDO_QUITADAS;
      }
    }
    break;

    case LLAVES_PUESTAS:
    {
      if((evento.evento_detectado != 0U) && (evento.estado_actual == 0U))
      {
        estado = TRAMITANDO_QUITADAS;
      }else if((evento.evento_detectado != 0U) && (evento.estado_actual == 1U)){
        estado = LLAVES_PUESTAS;
      }
      break;

      case LLAVES_QUITADAS:
      {
      if((evento.evento_detectado != 0U) && (evento.estado_actual == 0U))
      {
        estado = LLAVES_QUITADAS;
      }else if((evento.evento_detectado != 0U) && (evento.estado_actual == 1U)){
        estado = TRAMITANDO_PUESTAS;
      }
      }
    }
    break;

    case TRAMITANDO_PUESTAS:
    {
    led_manager_set_mode(LED_MODE_BLINKING_GREEN);
    led_manager_update(millis());   //modificar para que parpadee mientras se envía el mensaje de telegram(RTOS?)

    if(TelegramManager_is_wifi_connected() == 0U){
      err = ERROR_WIFI;
      estado = ERROR; //la wifi no está conectada
      led_manager_set_mode(LED_MODE_BLINKING_BOTH);
      break;
    }
    err = TelegramManager_send_message("Las llaves han sido depositadas en el sensor, están en el cubo");
    if(err == 0U){
      err = ERROR_TELEGRAM;
      estado = ERROR;
      led_manager_set_mode(LED_MODE_BLINKING_BOTH);
      break;
    }
    sensor_manager_nvs_write(1U);
    estado = LLAVES_PUESTAS;
    led_manager_set_mode(LED_MODE_SOLID_GREEN);
    break;      
    }


    case TRAMITANDO_QUITADAS:
    led_manager_set_mode(LED_MODE_BLINKING_RED);
    led_manager_update(millis());   //modificar para que parpadee mientras se envía el mensaje de telegram(RTOS?)
    if(TelegramManager_is_wifi_connected() == 0U){
      //la wifi no está conectada
      err = ERROR_WIFI;
      estado = ERROR; 
      led_manager_set_mode(LED_MODE_BLINKING_BOTH);
      break;
    }

    if(TelegramManager_send_message("Las llaves se han retirado del sensor , se han devuelto a secretaría") == 0U){
      err = ERROR_TELEGRAM;
      estado = ERROR;
      led_manager_set_mode(LED_MODE_BLINKING_BOTH);
      break;
    }
    sensor_manager_nvs_write(0U);
    estado = LLAVES_QUITADAS;
    led_manager_set_mode(LED_MODE_SOLID_RED);
    break;

    case ERROR:
    {
    static uint8_t retrys = 0U;
    static uint32_t last_time = 0U;
    if(err == ERROR_WIFI){
      // el error ha sido por la wifi
      if(TelegramManager_is_wifi_connected()  == 0U){
        //no está conectado a la wifi
        uint32_t actual_time = millis();

        //esperamos un tiempo de reconexión
        if(actual_time - last_time >= INTERVALO_RECONEXION){
          last_time = actual_time;

          //ponemos una cantidad máxima de reintentos
          if(retrys < 5){
            retrys++;
          }else{
            //aqui tendría que poner algo con estilo ya que ha superado la cantidad de reintentos, no se si reiniciar el dispositivo es una buena práctica.
          }
        }
      }else{
        //se ha conectado a la wifi
        retrys = 0U;
        estado = REPOSO;
      }
    }else if(err == ERROR_TELEGRAM){
    // Reintentar envío tras detectar error de Telegram
    if(TelegramManager_is_wifi_connected() == 1U){  // ← WiFi OK, error es solo de Telegram
        uint32_t actual_time = millis();
        
        if(actual_time - last_time >= INTERVALO_RECONEXION){
            last_time = actual_time;
            
            if(retrys < 5){
                // Reintentar el envío del mensaje pendiente
                const char* mensaje = (evento.estado_actual == 1U) 
                    ? "Las llaves han sido depositadas en el sensor, están en el cubo"
                    : "Las llaves se han retirado del sensor , se han devuelto a secretaría";
                
                if(TelegramManager_send_message(mensaje) == 1U){
                    // Éxito: actualizar NVS y salir del estado ERROR
                    sensor_manager_nvs_write(evento.estado_actual);
                    retrys = 0U;
                    estado = (evento.estado_actual == 1U) ? LLAVES_PUESTAS : LLAVES_QUITADAS;
                    led_manager_set_mode((evento.estado_actual == 1U) ? LED_MODE_SOLID_GREEN : LED_MODE_SOLID_RED);
                }
                retrys++;
            }else{
                // Máximo de reintentos alcanzado: acción de contingencia
                // OPCIÓN A: Reset suave y volver a REPOSO
                retrys = 0U;
                err = ERROR_NINGUNO;
                estado = REPOSO;
                // OPCIÓN B (más agresiva): esp_restart();
            }
        }
    }else{
        // Si WiFi se cayó, delegar al manejador de ERROR_WIFI
        err = ERROR_WIFI;
    }
    }
    }
    break;

  // ═══════════════════════════════════════════════════════════
  // Actualización de LEDs (común a todos los estados)
  // ═══════════════════════════════════════════════════════════
  led_manager_update(millis());
}