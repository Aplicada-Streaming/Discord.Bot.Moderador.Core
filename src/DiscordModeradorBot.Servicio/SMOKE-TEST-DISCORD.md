# Smoke-test manual — Integración real con Discord (gateway + acciones)

Procedimiento para probar EN VIVO la integración real con Discord.Net contra un servidor de
prueba propio. Lo ejecuta una persona, con SU token, sobre un servidor descartable. Los tests
automáticos del repo NO usan token ni red; este smoke-test cubre lo que solo se puede verificar
contra la plataforma real.

> Seguridad (no negociable): el token NUNCA se escribe en el código, en `appsettings`, en logs ni
> en este documento. Se registra cifrado en reposo desde el panel (se descifra solo en memoria con
> la clave maestra del entorno, ADR-07, RN-14). No subas ningún token al repositorio.

---

## 1. Requisitos previos (portal de desarrolladores de Discord)

1. Crear una aplicación-bot en https://discord.com/developers/applications y, en la sección **Bot**,
   generar el **token** (se copia una sola vez; guardalo en un gestor de secretos, no en un archivo
   del repo).
2. Habilitar los **intents privilegiados** en la pestaña **Bot → Privileged Gateway Intents**:
   - **MESSAGE CONTENT INTENT** (para leer el contenido de los mensajes, CU-04).
   - **SERVER MEMBERS INTENT** (para leer los roles del autor y evaluar exenciones, CU-15/RN-07).
   - (GUILDS y GUILD MESSAGES no son privilegiados; el cliente ya los solicita.)
   Sin estos dos intents privilegiados el bot NO recibirá contenido/roles y la prueba de
   configuración (CU-12) marcará un chequeo bloqueante.
3. Invitar el bot al servidor de prueba con los **permisos necesarios** (OAuth2 → URL Generator →
   scope `bot`, y permisos):
   - Banear miembros (Ban Members)
   - Expulsar miembros (Kick Members)
   - Moderar miembros / timeout (Moderate Members)
   - Gestionar roles (Manage Roles)
   - Enviar mensajes (Send Messages) en el canal de salida
   - Ver canales (View Channels)
4. **Jerarquía**: arrastrá el rol del bot por ENCIMA de los roles de los usuarios que querés
   moderar (Discord no permite accionar sobre alguien con rol igual o superior, RN-01). Si quedan
   roles por encima del bot, la prueba lo marcará como **advertencia** (no bloquea, CU-12 CA-04).
5. **Canal de salida**: elegí (o creá) un canal privado donde el bot publicará los reportes de
   incidentes (CU-05) y anotá su **snowflake** (clic derecho → Copiar ID; requiere activar el modo
   desarrollador en Discord). Anotá también el **snowflake del servidor** (guild).

---

## 2. Proveer la clave maestra (cifrado del token en reposo, ADR-07, RN-14)

El token se guarda cifrado con AES-GCM; la clave maestra se deriva de una variable de entorno y
vive fuera de la base. Definila ANTES de arrancar:

- PowerShell (Windows):
  ```powershell
  $env:DISCORDMODERADOR_CLAVE_MAESTRA = "una-frase-larga-y-secreta-solo-para-esta-maquina"
  ```
- Bash (Linux/ARM de despliegue):
  ```bash
  export DISCORDMODERADOR_CLAVE_MAESTRA="una-frase-larga-y-secreta-solo-para-esta-maquina"
  ```

Para pruebas locales también podés guardar el token con `dotnet user-secrets` en lugar de tipearlo
en el panel, pero la vía soportada y recomendada es registrarlo cifrado desde el panel (paso 4).
La clave maestra NO se versiona; si no se define, el cifrado usa una clave de desarrollo (no usar
en producción).

---

## 3. Seleccionar el modo de gateway = Discord

El modo de gateway se elige con el flag **`Moderacion:Gateway`** (`Simulado` | `Discord`). El
default es `Simulado` (sin red ni token, para dev/tests). Para el smoke-test poné `Discord` por
cualquiera de estas vías (NO pongas el token aquí, solo el flag):

- Variable de entorno (recomendado, no toca archivos del repo):
  ```powershell
  $env:Moderacion__Gateway = "Discord"
  ```
  ```bash
  export Moderacion__Gateway=Discord
  ```
- O en un `appsettings` local NO versionado (p. ej. `appsettings.Production.json`):
  ```json
  { "Moderacion": { "Gateway": "Discord" } }
  ```

En modo `Discord` se registra el adaptador real (`AdaptadorGatewayDiscord`) + el gestor de
conexiones (`GestorConexionesGateway`), que abre una conexión por servidor activo y enruta los
mensajes al motor; NO se inyectan mensajes simulados. En modo `Simulado` corre el
`WalkingSkeletonHostedService` con el adaptador simulado.

---

## 4. Arrancar y registrar el servidor (queda cifrado)

1. Arrancar:
   ```bash
   dotnet run --project src/DiscordModeradorBot.Servicio
   ```
2. Abrir el panel, completar el primer ingreso / autenticarse (CU-08/CU-09).
3. Ir a **Servidores**, registrar el servidor de prueba: pegar el **snowflake del servidor**, el
   **token** (campo password; se guarda cifrado, nunca en claro, RN-14) y el **snowflake del canal
   de salida**. Al guardar, el servidor queda **registrado pero inactivo** (CU-10).

---

## 5. Probar la configuración (CU-12) y leer los chequeos

1. En **Servidores**, presionar **Probar** sobre el servidor recién registrado. El sistema hace un
   login efímero con el token (solo en memoria, RN-14) y verifica, contra la plataforma:
   - **Credencial válida** (token) — bloqueante si falla (`PRUEBA_TOKEN_INVALIDO`).
   - **Intents habilitados** (MessageContent, GuildMembers) — bloqueante si el portal los tiene
     deshabilitados.
   - **Recepción de eventos** (el bot está presente en el servidor) — bloqueante si no sincroniza.
   - **Permisos requeridos** (banear, expulsar, moderar, gestionar roles) — bloqueante si falta
     alguno (`PRUEBA_PERMISOS_FALTANTES`).
   - **Canal de salida disponible** (existe y el bot puede escribir) — bloqueante si no
     (`PRUEBA_CANAL_SALIDA_AUSENTE`).
   - **Jerarquía de roles** — **advertencia** (no bloquea) si hay roles por encima del bot (RN-01,
     CU-12 CA-04).
2. Leer la lista de verificaciones. Si hay algún chequeo **bloqueante**, la activación queda
   bloqueada (RN-16): corregir en el portal/servidor y volver a probar.
3. Cuando no haya bloqueantes, presionar **Probar y activar**: el servidor pasa a **Activo** y el
   gestor abre su conexión real al canal de eventos (CU-13).

---

## 6. Probar primero en SIMULACIÓN (RN-09), luego en ejecución

> Importante: probá SIEMPRE primero en modo simulación para ver el reporte sin accionar a nadie.

1. En **Configuración**, dejar la política (p. ej. ráfaga distribuida) en **modo Simulación**
   (RN-09). En simulación el motor evalúa y reporta "lo que se habría hecho", pero NO ejecuta
   baneos/timeouts reales.
2. Generar una **ráfaga real** en el servidor de prueba (postear el mismo usuario en varios canales
   dentro de la ventana de detección). Verificar en el **canal de salida** que aparece el reporte
   con la etiqueta `[SIMULACIÓN]` y que NADIE fue accionado.
3. Recién cuando el reporte sea el esperado, cambiar la política a **modo Ejecución** y repetir la
   ráfaga con un usuario descartable: ahora el bot debe **reportar** y luego **banear/timeoutear**
   de verdad, en el orden declarado (RN-05). Si el usuario tiene rol superior, el incidente queda
   **NoAccionable**, igual se reporta y el bot NO se cae (RN-01, ADR-08).

---

## 7. Leer el estado de conexión (CU-13)

- En **Servidores**, la columna **Conexión** muestra el estado en vivo del gestor de conexiones:
  `Conectado`, `Desconectado (DesconectadoTransitorio)` durante una caída (el SDK reconecta solo) o
  `Desconectado (DesconectadoTokenInvalido)` si la credencial se revoca durante la operación (hay
  que re-validar el token, CU-12/CU-13). El tooltip explica el motivo.
- Probar una caída: cortar la red unos segundos y verificar que el estado pasa a desconectado y
  vuelve a conectado al restablecerla (CU-13 CA-01). Revocar/rotar el token en el portal y verificar
  que el estado refleja token inválido (CU-13 CA-03).

---

## 8. Recordatorios

- Los **intents privilegiados** y los **permisos del bot** son condición necesaria para que la
  integración funcione; sin ellos la prueba de configuración bloquea la activación (CU-12, RN-16).
- El token NUNCA aparece en logs (RN-14); si alguna vez lo ves en un log, es un bug a reportar.
- Para volver a desarrollo sin red, poné de nuevo `Moderacion:Gateway=Simulado` (o quitá la
  variable de entorno) y reiniciá.
