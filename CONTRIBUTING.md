# Colaborar con KEEEI

Gracias por pasarte. Unas pocas normas para que esto siga siendo fácil de seguir:

- **Commits pequeños** y con prefijo: `feat(sensor): ...`, `fix(carcasa-b): ...`,
  `docs: ...`. Uno por idea.
- **Ramas:** el firmware y la documentación común van en `main`. Cada carcasa
  tiene su rama `carcasa/<opción>`. Si cambias algo que afecta a todas (el
  cartucho, las holguras...), hazlo en la A y llévalo a las demás.
- **Decisiones:** si cambias algo que no se entiende solo leyendo el código,
  añade una ficha en [docs/decisiones](docs/decisiones/README.md).
- **La máquina de estados no se toca a la ligera.** Los estados y los mensajes
  están en [docs/firmware.md](docs/firmware.md). Si añades uno, actualiza
  también el diagrama.
- **Carcasas:** el FeatureScript del repositorio es la fuente de la verdad.
  Onshape se actualiza desde él, nunca al revés. Antes de subir un cambio,
  comprueba que sigue imprimiéndose sin soportes.
- **Secretos:** `include/secrets.h` no se sube nunca. Si lo subes por error,
  cambia el token del bot en @BotFather enseguida.

¿Has impreso una carcasa? Lo que más nos ayuda son tus medidas y cómo te
salieron las piezas de prueba, apuntadas en
[docs/calibracion.md](docs/calibracion.md).
