# Compilar y flashear

Hace falta [PlatformIO](https://platformio.org/): la extensión de VS Code o la
CLI. Las librerías (UniversalTelegramBot y ArduinoJson) las baja solo la
primera vez.

```bash
pio run                 # compila
pio run -t upload       # compila y flashea por USB
pio device monitor      # abre el puerto serie
```

Desde VS Code: el icono de la hormiga → `esp32dev` → *Upload*.

## Si algo va mal

| Síntoma | Lo más probable |
|---|---|
| `secrets.h: No such file` | No has copiado `include/secrets.h.example` a `include/secrets.h` |
| Los dos LED parpadean lento y luego rápido | No llega al wifi: revisa el SSID y la contraseña, y que la red sea de 2,4 GHz |
| Los dos LED parpadean rápido justo después de meter las llaves | El wifi va pero Telegram no responde: token, `chat_id`, permisos del bot o el certificado ([telegram.md](telegram.md)) |
| No detecta las llaves | Mira el cableado del microswitch ([hardware.md](hardware.md)) y que el empujador del cartucho llegue a pulsarlo |
| No se ve el puerto COM | Falta el driver USB-serie de la placa o el cable solo sirve para cargar |

Referencia de memoria con la versión actual: unos 45 kB de RAM (14 %) y 874 kB
de flash (67 %).
