# Guía práctica — Configurar un bot de Discord para DiscordModeradorBot

**Proyecto:** discord-bots-admin
**Versión del documento:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-22
**Audiencia:** Administrador / desarrollador que da de alta un servidor en el panel.

Guía paso a paso para crear la aplicación y el bot en el portal de desarrolladores de Discord,
obtener el **token correcto**, habilitar los **intents** y **permisos** que necesita
DiscordModeradorBot, **invitar** el bot al servidor y darlo de alta en el panel hasta dejarlo
**activo**. Incluye ejemplos, enlaces y una sección de **solución de problemas** con los errores
reales más comunes.

> TL;DR: creá la app → pestaña **Bot** → **Reset Token** y copiá el token (¡no lo reseteés de nuevo!) →
> activá **MESSAGE CONTENT** y **SERVER MEMBERS** → invitá el bot con permisos de **moderación** →
> copiá el **ID del servidor** y del **canal** → cargá todo en `/servidores` → **Probar y activar**.

---

## 0. Qué vas a necesitar

- Una cuenta de Discord con **permiso de administrador** en el servidor que querés moderar.
- Acceso al **Portal de Desarrolladores**: <https://discord.com/developers/applications>
- El servicio corriendo en **modo Discord** (ver §8). En **modo Simulado** el bot NO se conecta a
  Discord de verdad (solo registra en el log).

---

## 1. Los 4 valores que NO hay que confundir

Es el error #1. Tu aplicación de Discord tiene varios valores y **solo uno es el token del bot**:

| Valor | Dónde está | ¿Sirve como "Token del bot"? | Pinta |
| --- | --- | --- | --- |
| **Application ID** | General Information | ❌ No | Solo dígitos (~19) |
| **Public Key** | General Information | ❌ No | Hex de 64, **sin puntos** |
| **Client Secret** | OAuth2 | ❌ No | Cadena corta |
| **Bot Token** | **pestaña Bot** | ✅ **SÍ** | **~70 chars, con 2 puntos** (3 partes) |

Un **bot token** tiene **3 partes separadas por puntos** y unos ~70 caracteres. Su forma
(ejemplo ficticio, no es un token real):

```
Mxxxxxxxxxxxxxxxxxxxxxxxxx.xxxxxx.xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
└── parte 1 (~26) ──────┘ └─(6)─┘ └────────── parte 3 (~38) ──────────┘
```

Si lo que copiaste **no tiene puntos** (p. ej. 64 caracteres hexadecimales seguidos), es la
**Public Key**, no el token.

> ⚠️ Nunca pegues un token real en documentos, capturas ni el repo: GitHub (secret scanning) lo
> detecta y, además, queda expuesto. Si pasó, **reseteá** el token.

---

## 2. Crear la aplicación

1. Entrá a <https://discord.com/developers/applications>.
2. **New Application** → ponele un nombre (ej. `TUP Programación Moderador`) → **Create**.
3. Quedás en **General Information**. Acá ves el **Application ID** y la **Public Key**
   (recordá: ninguno es el token).

---

## 3. Obtener el Bot Token (la parte sensible)

1. Menú izquierdo → **Bot**.
2. En la sección **Token**, hacé clic en **Reset Token** (Discord no muestra el token viejo; solo
   permite resetear para ver uno nuevo).
3. Confirmá (puede pedir 2FA). **El token aparece UNA sola vez** → **Copy**.

> ⚠️ **Reglas de oro del token (evitan el 90% de los problemas):**
> - El token **vive para siempre** hasta que hagas **Reset** de nuevo.
> - **Cada "Reset Token" invalida el anterior.** Si reseteás después de copiar, el copiado queda **muerto**.
> - Reseteá **una vez**, copialo y **pegalo directo en el panel**. No lo reseteés "para chequear".
> - **Nunca** lo compartas (chat, captura, repo). Si se expuso, **reseteá** y cargá el nuevo.

---

## 4. Habilitar los Intents privilegiados (obligatorio)

DiscordModeradorBot necesita leer el **contenido** de los mensajes y los **roles** de los autores.
Esos son *privileged intents* y vienen **apagados** por defecto.

En la misma pestaña **Bot** → sección **Privileged Gateway Intents** → activá y **Save Changes**:

- ✅ **MESSAGE CONTENT INTENT** — para leer el texto de los mensajes (detección de contenido).
- ✅ **SERVER MEMBERS INTENT** — para leer los roles del autor (exenciones, jerarquía).

> Si están apagados, el bot **no sincroniza** el servidor: Discord cierra el gateway con
> **close code 4014 "Disallowed intent(s)"** y la prueba marca *Intents habilitados* y
> *Recepción de eventos* como **bloqueantes** (ver §9).

Intents exactos que pide el servicio: `Guilds`, `GuildMessages`, `MessageContent` (priv.),
`GuildMembers` (priv.), `GuildBans`.

---

## 5. Permisos que necesita el bot

Para **activarse** (no solo para escribir), el bot debe tener estos permisos en el servidor:

| Permiso (Discord) | Para qué (en el bot) | ¿Bloquea activación si falta? |
| --- | --- | --- |
| **View Channel** + **Send Messages** | Publicar el reporte en el canal de salida (CU-05) | El canal de reportes queda bloqueante |
| **Ban Members** | Banear al emisor (CU-02/CU-03) | ✅ Sí |
| **Kick Members** | Expulsar (R6) | ✅ Sí |
| **Moderate Members** (Timeout) | Silenciar/timeout (R6) | ✅ Sí |
| **Manage Roles** | Asignar/quitar rol (R6) | ✅ Sí |
| **Read Message History** | Borrado retroactivo de mensajes (CU-03) | Recomendado |

> 💡 **Atajo:** darle **Administrator** cubre todos de una. Para mínimo privilegio, activá solo los de
> arriba. La **jerarquía de roles** (que haya roles por encima del bot) es solo **advertencia**: no
> bloquea, pero el bot no podrá accionar sobre quienes tengan esos roles más altos.

---

## 6. Invitar el bot al servidor

Hay dos caminos en el portal; cualquiera sirve.

### Opción A — OAuth2 → URL Generator (clásica)

1. Menú izquierdo → **OAuth2** → **URL Generator**.
2. En **Scopes** marcá **`bot`**.
3. Abajo aparece **Bot Permissions** → marcá los de §5 (o **Administrator**).
4. Discord arma la **URL** al pie (con el `permissions=` ya calculado). Copiala, abrila en el
   navegador, elegí tu servidor y **Authorize**.

Ejemplo de URL (reemplazá `APPLICATION_ID`; `permissions=8` es **Administrator**):

```
https://discord.com/api/oauth2/authorize?client_id=APPLICATION_ID&scope=bot&permissions=8
```

### Opción B — Installation (nueva)

1. Menú izquierdo → **Installation**.
2. En **Install Link** elegí **Discord Provided Link** (o el método que prefieras).
3. En **Default Install Settings** → **Guild Install** → **Scopes**: `bot` → **Permissions**: los de §5.
4. Copiá el **Install Link** y abrilo para agregar el bot a tu servidor.

> Verificá que el bot quedó en el servidor: en **Server Settings → Members** debería figurar (aparece
> como `NombreBot#1234` o con su usuario). Si ya está pero faltan permisos, ajustá su **rol** en
> **Server Settings → Roles → (rol del bot) → Permissions**.

---

## 7. Obtener los IDs del servidor y del canal

El panel pide el **ID del servidor** (guild) y el **ID del canal** de reportes — son **snowflakes
numéricos**, no nombres.

1. En Discord (cliente de escritorio/web): **Ajustes de usuario → Avanzado → Modo Desarrollador: ON**.
2. **ID del servidor:** clic derecho sobre el ícono del servidor → **Copiar ID del servidor**.
3. **ID del canal:** clic derecho sobre el canal de reportes → **Copiar ID del canal**.

Ejemplo: `743672323122528307` (servidor), `743672323122528310` (canal).

---

## 8. Dar de alta el servidor en el panel

> El servicio debe correr en **modo Discord** para que el envío/activación sean reales. Usá
> `scripts/local/run-discord.bat` (setea `Moderacion__Gateway=Discord`). El `run-all.bat` corre en
> **Simulado** (no envía a Discord; solo log). Ver `scripts/local/README.md`.

1. Abrí el panel: `http://localhost:5072/servidores` (login admin de desarrollo: ver
   `scripts/local/README.md`).
2. **Registrar un servidor** y completá:
   - **Nombre descriptivo** (opcional): etiqueta libre, ej. `TUP Programación`.
   - **ID del servidor (Discord, numérico):** el guild ID del §7.
   - **Token del bot:** el **Bot Token** del §3 (con 2 puntos).
   - **ID del canal de reportes (opcional):** el canal ID del §7.
3. **Registrar**. El token se guarda **cifrado** (nunca en texto plano, RN-14).
4. **Probar** (sin activar): el panel muestra el panel **"Verificaciones"** con cada chequeo
   (token, intents, recepción, permisos, canal, jerarquía).
5. **Probar y activar**: si **no hay chequeos bloqueantes**, el servidor pasa a **Activo** (RN-16).
6. **Enviar prueba** ✈️: publica un mensaje de prueba en el canal de reportes para confirmar de
   punta a punta.

> Para **editar** (nombre/token/canal): botón ✏️. En el campo token, **en blanco conserva el actual**;
> para cambiarlo, **pegá uno nuevo**.

---

## 9. Solución de problemas (errores reales)

| Síntoma / mensaje | Causa | Solución |
| --- | --- | --- |
| `Desconectado (DesconectadoTokenInvalido)` / `401 Unauthorized` / "el token no validó" | El token guardado **no es un bot token** (pegaste la Public Key) o está **muerto** por un reset posterior | Usá el **Bot Token** de la pestaña Bot (§1, §3). Reset **una vez**, copiá, pegá en el panel, **no reseteés más** |
| "Probar y activar" → **2 chequeos bloqueantes** (Intents + Recepción) | **Intents privilegiados apagados** (o el bot no está en el servidor) | Activá **MESSAGE CONTENT** y **SERVER MEMBERS** (§4) y confirmá que el bot esté invitado (§6) |
| "Probar y activar" → **1+ bloqueante: Permisos requeridos** | Al bot le faltan permisos de **moderación** (Kick/Moderate/Manage Roles) aunque pueda escribir | Dale esos permisos al **rol del bot** (§5) o **Administrator** |
| "Enviar prueba" dice OK pero **no llega** al canal | Estás en **modo Simulado** | Corré en **modo Discord** (`run-discord.bat`, §8) |
| Chequeo de **canal** bloqueante | El canal no existe / no es de texto / el bot no puede escribir | Verificá el **ID del canal** y el permiso **Send Messages** del bot en ese canal |
| **Advertencia** de jerarquía ("N roles por encima del bot") | El rol del bot está por debajo de otros | No bloquea; subí el **rol del bot** en **Server Settings → Roles** si querés que accione sobre esos usuarios |
| La consola se llena de `401` en bucle | Un servidor **activo** quedó con token inválido (p. ej. uno de demo) | Eliminá/editá ese servidor en `/servidores`; el bot deja de reintentar ante token inválido |

### Cómo ver el motivo exacto del gateway

En modo Discord, el log del servicio incluye los eventos internos de Discord.Net (prefijo
`[Discord.Net:Gateway]`). Ahí vas a ver el motivo real:
- `401: Unauthorized` → **token inválido**.
- `4004` / "Authentication failed" → **token inválido** (autenticación).
- `4014` / "Disallowed intent(s)" → **intents privilegiados apagados** (§4).

---

## 10. Seguridad del token (importante)

- El token **da control total del bot**. Tratalo como una contraseña.
- **No lo pegues** en chats, issues, capturas ni lo commitees. Si se expuso, **Reset Token** y cargá
  el nuevo en el panel.
- En reposo, el servicio lo guarda **cifrado** (AES-GCM, RN-14). En **Development** usa una clave
  maestra por defecto; en **producción** definí `DISCORDMODERADOR_CLAVE_MAESTRA` (ver `09_devops`).
  ⚠️ Si cambiás la clave maestra, los tokens ya guardados **dejan de descifrar**: hay que recargarlos.

---

## 11. Checklist final

- [ ] App creada en el portal.
- [ ] **Bot Token** copiado de la pestaña **Bot** (3 partes, con puntos) — sin reset posterior.
- [ ] **MESSAGE CONTENT** y **SERVER MEMBERS** activados (+ Save).
- [ ] Bot **invitado** al servidor con permisos de moderación (o Administrator).
- [ ] **ID del servidor** y **ID del canal** copiados (Modo Desarrollador).
- [ ] Servicio en **modo Discord** (`run-discord.bat`).
- [ ] Servidor dado de alta en `/servidores` con token + IDs.
- [ ] **Probar** sin chequeos bloqueantes → **Probar y activar** → **Activo**.
- [ ] **Enviar prueba** llega al canal.

---

## 12. Enlaces útiles

- Portal de desarrolladores: <https://discord.com/developers/applications>
- Documentación de bots: <https://discord.com/developers/docs/intro>
- Gateway Intents (y privilegiados): <https://discord.com/developers/docs/topics/gateway#gateway-intents>
- Permisos (bitwise): <https://discord.com/developers/docs/topics/permissions>
- OAuth2 (invitar bots): <https://discord.com/developers/docs/topics/oauth2#bots>
- Discord.Net (SDK usado): <https://docs.discordnet.dev/>
- Cómo obtener IDs (Modo Desarrollador): <https://support.discord.com/hc/articles/206346498>

---

**Relacionado:** CU-05 (reporte a canal), CU-10 (registro de servidor), CU-12 (prueba de
configuración), CU-13 (conexión del gateway), RN-14 (token cifrado), RN-16 (activación sin
bloqueantes). Scripts: `scripts/local/run-discord.bat`, `scripts/local/README.md`.
