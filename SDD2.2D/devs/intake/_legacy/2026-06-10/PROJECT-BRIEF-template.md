# PROJECT BRIEF — Plantilla de intake de negocio

```yaml
Nombre de la solución: [Nombre comercial o interno de la solución, kebab-case sugerido]
Cliente / Stakeholder principal: [Nombre de la persona, área u organización que lo impulsa]
Documento: PROJECT-BRIEF-<nombre-solucion-kebab>_v1.0.md
Versión: 1.0
Fecha: [YYYY-MM-DD]
Autor del brief: [Nombre y rol de quien redacta el brief]
Estado: Borrador
```

> Este documento captura **lo que el cliente quiere** en lenguaje de negocio.
> NO incluye decisiones técnicas (stack, arquitectura, deployment, etc.).
> Para eso ver el documento PROJECT-README.
>
> El brief es de nivel solución: hay uno por solución, no uno por proyecto. El negocio
> del cliente es uno solo, aunque la solución se construya como una jerarquía de varios
> proyectos. La descomposición en proyectos y su tipado D8 viven en el SOLUTION-MANIFEST
> y en el PROJECT-README, no acá. Para una solución de un solo proyecto, el nombre de la
> solución y el del proyecto coinciden.

---

## Cómo usar esta plantilla

_Cada sección trae cuatro bloques: una instrucción sobre qué responder, un set de preguntas guía (las marcadas con `(*)` son bloqueantes para avanzar), un ejemplo aplicado a un dominio distinto y una nota explícita de qué NO va en esa sección. Reemplazá los placeholders entre corchetes con la información real del proyecto. El brief no se considera listo hasta que el checklist de §13 esté completo._

---

## §1 Idea y problema

**Instrucción:** _Describir en lenguaje del cliente qué dolor concreto motiva este proyecto y por qué ahora. Dos a cuatro párrafos. Debe responder qué pasa hoy, a quién le duele y cuál es la consecuencia si no se resuelve._

**Preguntas guía:**
- (*) ¿Qué problema concreto resuelve el sistema y a quién le pasa hoy?
- (*) ¿Qué pasa si NO se construye este sistema en los próximos meses?
- ¿Por qué ahora y no el año pasado o el próximo?
- ¿Hay algún disparador externo (regulación, competencia, crecimiento) que lo motive?

**Ejemplo genérico (sistema de turnos médicos):**
> Hoy los pacientes solo pueden sacar turno llamando por teléfono al consultorio en horario de atención, lo que satura la línea y genera tiempos de espera de quince a treinta minutos. La recepcionista anota en una planilla y a veces se duplican turnos. Si no resolvemos esto, vamos a perder pacientes contra clínicas que ya tienen reserva online, y la recepcionista no llega a cubrir las llamadas mientras atiende presencialmente.

**Lo que NO va en esta sección:**
- Stack tecnológico, frameworks, lenguaje de programación.
- Modelo de datos, diagramas técnicos, decisiones de arquitectura.
- Estimaciones de esfuerzo o sprint plan.

---

## §2 Audiencia y stakeholders

**Instrucción:** _Identificar a las personas o roles que interactúan con el sistema, distinguiendo quién lo usa día a día, quién paga por él y quién decide su rumbo. Completar la tabla con al menos un representante por categoría (propietario, implementador, beneficiario). No usar genéricos como "los usuarios" o "el equipo"._

**Preguntas guía:**
- (*) ¿Quién es el propietario del problema y va a aprobar el brief?
- (*) ¿Quiénes son los usuarios finales y qué rol cumplen cada uno?
- ¿Quién financia el proyecto y a quién hay que rendir resultados?
- ¿Hay actores indirectos (auditoría, legal, soporte) que deban estar al tanto?

**Ejemplo genérico (e-commerce de productos artesanales):**

| Rol | Nombre o cargo | Categoría | Responsabilidad principal |
|---|---|---|---|
| Dueña de la tienda | [Nombre] | Propietario | Define catálogo y aprueba el brief |
| Equipo de desarrollo externo | [Nombre del estudio] | Implementador | Construye y mantiene la plataforma |
| Clientes finales | Compradores online | Beneficiario | Compran productos artesanales |
| Artesanos proveedores | Red de talleres aliados | Beneficiario | Cargan productos y reciben pagos |
| Contadora | [Nombre] | Beneficiario indirecto | Recibe reportes fiscales |

**Lo que NO va en esta sección:**
- Roles técnicos internos del equipo de desarrollo (eso va en PROJECT-README).
- Permisos, perfiles de seguridad o niveles de acceso (eso es parte de la especificación funcional).
- Personas físicas anónimas: si no se puede nombrar el rol, no es un stakeholder.

---

## §3 Propuesta de valor y diferenciación

**Instrucción:** _Explicar qué hace este sistema mejor o distinto que la alternativa actual del cliente (sea esa alternativa un sistema viejo, una planilla manual, un producto de la competencia o no hacer nada). Una o dos frases por diferenciador, máximo cinco diferenciadores._

**Preguntas guía:**
- (*) ¿Qué hace hoy el cliente para resolver este problema y por qué no le alcanza?
- (*) ¿Cuál es la promesa central que justifica el proyecto?
- ¿Qué pasaría si el cliente comprara una solución existente en lugar de construirla?
- ¿Cuál es el diferenciador defendible si la competencia copia la funcionalidad?

**Ejemplo genérico (CLI de migración de datos):**
> Hoy el equipo migra datos entre bases con scripts SQL ad-hoc que cada analista escribe a mano. La promesa de esta CLI es ejecutar migraciones declarativas con un solo comando, generar reportes de filas migradas y filas con error, y permitir reintentar solo lo fallido sin volver a correr todo. Frente a herramientas comerciales como las suites ETL pagas, el diferenciador es que se integra al pipeline de CI/CD existente y no requiere licencias por usuario.

**Lo que NO va en esta sección:**
- Listas exhaustivas de funcionalidades (eso va en §4).
- Comparativas técnicas de performance o benchmarks (eso va en PROJECT-README).
- Lenguaje de marketing vacío: si la frase aplica a cualquier producto, no es propuesta de valor.

---

## §4 Alcance funcional pretendido (alto nivel)

**Instrucción:** _Listar las capacidades funcionales que el sistema debería tener, sin entrar en detalles de implementación. Cada ítem se etiqueta con MoSCoW provisorio. La etiqueta se ajustará luego en el refinamiento del backlog, pero acá quedan las prioridades iniciales del cliente._

> **MoSCoW** es una técnica de priorización con cuatro categorías:
> - **Must Have**: imprescindible para considerar al sistema funcional. Sin esto, no hay MVP.
> - **Should Have**: importante, pero el sistema puede salir sin esto si urge.
> - **Could Have**: deseable, agrega valor pero puede esperar a una versión siguiente.
> - **Won't Have v1**: declarado fuera de alcance para esta versión, registrado para no olvidarlo.
>
> Regla práctica: si todo es Must Have, no hay priorización real. Forzá la lista a que el Must Have sea el mínimo con el que el cliente ya resuelve su problema central.

**Preguntas guía:**
- (*) ¿Cuál es el conjunto mínimo de capacidades sin el cual el sistema no resuelve el problema?
- (*) ¿Qué capacidades son importantes pero pueden esperar uno o dos meses?
- ¿Hay capacidades pedidas que sirven para una versión futura y no para la inicial?
- ¿Hay alguna capacidad cuya ausencia bloquearía la aprobación del cliente?

**Ejemplo genérico (librería utilitaria de parsing CSV):**

| ID | Capacidad | MoSCoW provisorio |
|---|---|---|
| F-01 | Leer un archivo CSV y devolver una lista de registros tipados | **Must Have** |
| F-02 | Soportar delimitadores configurables (coma, punto y coma, tab) | **Must Have** |
| F-03 | Reportar filas con error sin frenar la lectura completa | **Must Have** |
| F-04 | Escribir CSV desde una colección de objetos | **Should Have** |
| F-05 | Detectar el encoding automáticamente | **Could Have** |
| F-06 | Soporte para archivos remotos por URL | **Won't Have v1** |

**Lo que NO va en esta sección:**
- Algoritmos, estructuras de datos o decisiones de diseño.
- Endpoints concretos, nombres de clases o firmas de métodos.
- Detalles de UI específicos (colores, layouts, componentes visuales).

---

## §5 Historias de usuario / experiencias deseadas

**Instrucción:** _Describir el sistema desde la perspectiva de quien lo usa, en formato `Como [rol], quiero [acción], para [valor]`. Mínimo tres historias, cubriendo al menos dos roles distintos si el sistema tiene más de un actor. No reemplazan la especificación funcional; son input inicial._

**Preguntas guía:**
- (*) ¿Quiénes son los roles que van a operar el sistema y qué quiere lograr cada uno?
- (*) ¿Cuáles son las tres acciones más frecuentes que va a hacer un usuario típico?
- ¿Hay alguna experiencia que el cliente describió con la frase "y sería genial que..."?
- ¿Hay acciones que aparecen rara vez pero son críticas (ej: cierre de mes, baja de cuenta)?

**Ejemplo genérico (app móvil de inventario de almacén):**
> - Como **encargado de depósito**, quiero **escanear un código de barras con la cámara del celular**, para **registrar la entrada de mercadería sin tipear**.
> - Como **dueño del almacén**, quiero **recibir una notificación cuando el stock de un producto cae por debajo del mínimo**, para **generar el pedido al proveedor a tiempo**.
> - Como **vendedor**, quiero **consultar el stock actual de un producto desde el mostrador**, para **decirle al cliente si tenemos disponibilidad sin ir al depósito**.
> - Como **auditor externo**, quiero **exportar los movimientos del mes a una planilla**, para **conciliar contra los comprobantes físicos**.

**Lo que NO va en esta sección:**
- Criterios de aceptación detallados (eso va en el refinamiento de cada US).
- Wireframes o diseños de pantalla.
- Restricciones técnicas de la implementación.

---

## §6 Flujos típicos

**Instrucción:** _Narrar dos o tres recorridos representativos del uso normal del sistema, en lenguaje coloquial. Cada flujo en cuatro a ocho pasos. No usar diagramas formales (BPMN, secuencia, actividad); eso vendrá después. La idea es que el equipo técnico entienda el ritmo de uso real._

**Preguntas guía:**
- (*) ¿Cuál es el flujo más frecuente, el que se ejecuta el 80% del tiempo?
- ¿Hay un flujo crítico que rara vez pasa pero que no puede fallar?
- ¿Cuál es el flujo de inicio de un usuario nuevo (onboarding)?

**Ejemplo genérico (API de pagos):**
> **Flujo 1 — Cobro exitoso con tarjeta:**
> 1. El comercio recibe un pedido en su sistema y le pide a la API que cobre cien pesos con la tarjeta del cliente.
> 2. La API valida los datos de la tarjeta y los envía al procesador.
> 3. El procesador responde "aprobado" y devuelve un identificador de transacción.
> 4. La API guarda la transacción, marca el pedido como pagado y notifica al comercio por webhook.
> 5. El cliente ve un comprobante en pantalla y recibe un email con el detalle.
>
> **Flujo 2 — Reverso de un cobro:**
> 1. El comercio detecta un error y le pide a la API que devuelva el cobro de la transacción X.
> 2. La API verifica que la transacción está en estado "aprobada" y no fue revertida antes.
> 3. La API envía el pedido de reverso al procesador.
> 4. Si el procesador confirma, la API marca la transacción como "revertida" y notifica al comercio.
> 5. El cliente recibe el dinero en su tarjeta entre uno y diez días hábiles según el banco emisor.

**Lo que NO va en esta sección:**
- Diagramas UML, BPMN o de secuencia.
- Nombres de endpoints, payloads JSON o códigos de respuesta HTTP.
- Implementación interna del flujo (cola de mensajes, retries, etc.).

---

## §7 Casos límite y "qué pasa si"

**Instrucción:** _Listar las preguntas incómodas que el cliente tiene que responder antes de que el equipo técnico empiece a construir. No son requisitos: son disparadores para que el cliente piense en escenarios excepcionales. Mínimo cinco preguntas. Cada pregunta queda con la respuesta del cliente cuando se valide el brief._

**Preguntas guía:**
- (*) ¿Qué pasa si dos personas hacen la misma operación al mismo tiempo?
- (*) ¿Qué pasa si se pierde la conexión a internet en medio de una operación?
- ¿Qué pasa si un dato obligatorio llega vacío o en formato incorrecto?
- ¿Qué pasa si una persona quiere borrar su cuenta o sus datos?

**Ejemplo genérico (plataforma educativa SaaS):**
> - ¿Qué pasa si un alumno paga el curso y la pasarela confirma, pero la plataforma cae antes de habilitar el acceso? → Respuesta del cliente: [Pendiente].
> - ¿Qué pasa si un instructor borra un curso al que ya hay alumnos inscriptos? → Respuesta del cliente: [Pendiente].
> - ¿Qué pasa si el video que el alumno está mirando se interrumpe a la mitad? → Respuesta del cliente: [Pendiente].
> - ¿Qué pasa si un alumno comparte su contraseña con otra persona? → Respuesta del cliente: [Pendiente].
> - ¿Qué pasa si el alumno reclama un reembolso después de haber visto el setenta por ciento del curso? → Respuesta del cliente: [Pendiente].

**Lo que NO va en esta sección:**
- Soluciones técnicas (retries, locks, idempotencia). Acá va el "qué", no el "cómo".
- Códigos de error o mensajes de UI exactos.
- Acuerdos de SLA o niveles de servicio (eso va en PROJECT-README y en §10).

---

## §8 Métricas de éxito desde el negocio

**Instrucción:** _Establecer métricas SMART (Específicas, Medibles, Alcanzables, Relevantes, Temporales) que validen que el sistema cumple su objetivo de negocio. Mínimo tres métricas. Cada métrica debe tener criterio, unidad de medida, valor objetivo y plazo para alcanzarlo._

**Preguntas guía:**
- (*) ¿Cómo sabemos, en seis meses, si el proyecto fue exitoso?
- (*) ¿Qué número concreto le va a hacer decir al cliente "valió la pena"?
- ¿Qué métricas tiene hoy el negocio que vamos a poder comparar antes y después?
- ¿Hay algún indicador que el cliente quiera evitar empeorar (ej: tasa de quejas)?

**Ejemplo genérico (portal de trámites municipales):**

| Criterio | Métrica | Target | Plazo |
|---|---|---|---|
| Adopción del canal digital | Porcentaje de trámites iniciados online sobre el total | **≥ 40%** | 6 meses post-lanzamiento |
| Reducción de filas presenciales | Cantidad de vecinos atendidos en mesa de entrada por día | **≤ 80** (vs 150 actual) | 9 meses post-lanzamiento |
| Tiempo de resolución de un trámite simple | Días promedio desde inicio a finalización | **≤ 3 días hábiles** | 12 meses post-lanzamiento |
| Satisfacción del vecino | Promedio de calificación en encuesta post-trámite (1 a 5) | **≥ 4.0** | continuo, revisión trimestral |

**Lo que NO va en esta sección:**
- Métricas técnicas como latencia p99, throughput de la API, uptime (eso va en PROJECT-README).
- Métricas de proceso de desarrollo (velocity, story points, cobertura de tests).
- Aspiraciones sin número: "que sea rápido", "que sea fácil de usar".

---

## §9 Lo que NO es este sistema (exclusiones)

**Instrucción:** _Declarar explícitamente qué cosas el sistema NO va a hacer, con justificación. Sin exclusiones declaradas, el alcance se infla solo. Mínimo tres exclusiones. Cada una debe explicar por qué queda afuera y, si aplica, cuándo podría incorporarse._

**Preguntas guía:**
- (*) ¿Qué pidió el cliente o algún stakeholder y se decidió dejar afuera de esta versión?
- (*) ¿Qué supuestos podría hacer el equipo de desarrollo que serían incorrectos?
- ¿Hay funcionalidades de la competencia que el cliente NO quiere copiar?
- ¿Hay integraciones que están fuera de alcance aunque parezcan obvias?

**Ejemplo genérico (sistema de turnos médicos, continuación):**

| Funcionalidad excluida | Justificación | Versión futura |
|---|---|---|
| Videoconsulta integrada | Requiere certificación médica y firma digital, fuera del scope inicial | Posible v2.0 |
| Historia clínica electrónica | Es un sistema en sí mismo, escapa al objetivo del MVP de turnos | No planificado |
| Cobro online del turno | El consultorio cobra en efectivo en mostrador, no es prioridad | Backlog v1.5 |
| App móvil nativa | El portal web responsive cubre los casos de uso primarios | Posible v2.0 |
| Integración con obras sociales | Cada obra social tiene API distinta, requiere proyecto dedicado | No planificado |

**Lo que NO va en esta sección:**
- Capacidades que sí están dentro del alcance (eso va en §4).
- Exclusiones sin justificación (si no hay razón, no es una exclusión deliberada).
- Comentarios despectivos sobre la competencia o sobre soluciones existentes.

---

## §10 Restricciones del cliente

**Instrucción:** _Documentar las restricciones externas que condicionan el proyecto: presupuesto orientativo, fecha objetivo, restricciones legales o regulatorias, e integraciones obligatorias con sistemas ya existentes. Estas restricciones no son negociables a nivel técnico; el equipo tiene que trabajar dentro de ellas._

**Preguntas guía:**
- (*) ¿Hay una fecha objetivo concreta y, si la hay, qué la motiva (regulación, evento, contrato)?
- (*) ¿Cuál es el presupuesto orientativo o el rango con el que se va a trabajar?
- ¿Hay normativas legales o regulatorias que el sistema debe cumplir?
- ¿Hay sistemas existentes con los que el nuevo sistema debe integrarse sí o sí?

**Ejemplo genérico (app móvil de inventario, continuación):**

| Tipo de restricción | Detalle | Origen |
|---|---|---|
| Presupuesto | Hasta 30.000 USD para la primera versión | Aprobación del directorio |
| Fecha objetivo | MVP en producción antes de la temporada alta (15 de noviembre) | Estacionalidad del negocio |
| Legal | Cumplimiento de Ley de Protección de Datos Personales | Marco regulatorio nacional |
| Integración obligatoria | Conexión al sistema contable existente (vía exportación CSV diaria) | Sistema legacy en uso desde hace 8 años |
| Operativa | Debe funcionar sin conexión a internet durante toda una jornada laboral | Mala conectividad en el depósito |

**Lo que NO va en esta sección:**
- Restricciones técnicas autoimpuestas por el equipo de desarrollo (eso va en PROJECT-README).
- Decisiones de arquitectura derivadas de la restricción (eso va en los ADR).
- Restricciones inventadas o supuestas sin confirmación del cliente.

---

## §11 Riesgos detectados desde el negocio

**Instrucción:** _Listar los riesgos que el cliente ya conoce o intuye, con su probabilidad cualitativa (alta / media / baja), impacto cualitativo y mitigación propuesta desde el negocio. Mínimo tres riesgos. No incluir riesgos técnicos generales del tipo "el equipo se enferma": foco en riesgos del dominio._

**Preguntas guía:**
- (*) ¿Qué le quita el sueño al cliente sobre este proyecto?
- (*) ¿Hubo algún intento previo de resolver este problema y por qué falló?
- ¿Hay cambios regulatorios, de mercado o de competencia esperables en los próximos meses?
- ¿Qué supuesto crítico del proyecto, si se rompe, hace inviable el resultado?

**Ejemplo genérico (API de pagos, continuación):**

| ID | Riesgo | Probabilidad | Impacto | Mitigación propuesta |
|---|---|---|---|---|
| R-01 | El procesador de pagos cambia su contrato y exige certificación PCI nivel 1 | Media | **Alto** | Revisar el contrato vigente al inicio del proyecto y reservar buffer para certificación |
| R-02 | Fraude masivo con tarjetas robadas en los primeros meses | Media | **Alto** | Activar reglas anti-fraude del procesador desde el día uno y revisar semanalmente |
| R-03 | El volumen real de transacciones supera ampliamente la proyección | Baja | Medio | Pruebas de carga antes del lanzamiento y plan de escalado documentado |
| R-04 | Cambios regulatorios sobre comisiones | Baja | Medio | Monitoreo del marco regulatorio del banco central y ajustes contractuales semestrales |

**Lo que NO va en esta sección:**
- Riesgos técnicos puros (deuda técnica, refactor, performance). Eso va en PROJECT-README.
- Riesgos genéricos del estilo "puede haber bugs". No agregan valor.
- Mitigaciones sin responsable ni plazo concreto (deben quedar accionables).

---

## §12 Glosario del dominio del cliente

**Instrucción:** _Definir los términos específicos del dominio del cliente que el equipo técnico necesita aprender. Mínimo cinco términos. Cada definición en una o dos frases, en lenguaje accesible. Si un término tiene sinónimos en el negocio, anotarlos._

**Preguntas guía:**
- (*) ¿Qué palabras usa el cliente que el equipo técnico no entendería de entrada?
- ¿Hay términos que parecen comunes pero significan algo distinto en este negocio?
- ¿Hay siglas o acrónimos del rubro que se mencionan habitualmente?

**Ejemplo genérico (e-commerce de productos artesanales, continuación):**

| Término | Definición | Sinónimos / Notas |
|---|---|---|
| **Lote** | Conjunto de piezas producidas en la misma tanda, comparten color, técnica y fecha | "Tirada", "serie" |
| **Pieza única** | Producto sin réplica, no se puede reponer si se vende | "Obra", "one-off" |
| **Comisión** | Pedido especial de un cliente con personalización | "Encargo", "pedido custom" |
| **Reposición** | Producción nueva de un producto agotado con las mismas características | — |
| **Curaduría** | Proceso de seleccionar qué artesanos y productos entran al catálogo | Decisión exclusiva del propietario |
| **Embalaje firmado** | Empaque con etiqueta a mano del artesano que produjo la pieza | Es parte del producto, no un servicio adicional |

**Lo que NO va en esta sección:**
- Términos técnicos universales del software (API, base de datos, framework).
- Definiciones que el equipo ya conoce sin contexto del dominio.
- Documentación duplicada del glosario general del proyecto (si existe en PROJECT-README).

---

## §13 Checklist de completitud del brief

_El brief se considera listo para ser usado como input del flujo SDD (Visión → NB → CU) solo cuando todos los ítems están tildados. Cada ítem debe poder ser verificado objetivamente por una persona que lea el documento._

- [ ] El bloque de cabecera tiene nombre de solución, cliente, fecha, autor y estado completos.
- [ ] §1 describe un problema concreto en lenguaje del cliente, no una solución técnica.
- [ ] §1 responde explícitamente qué pasa si NO se construye el sistema.
- [ ] §2 contiene al menos un stakeholder por categoría (propietario, implementador, beneficiario), con rol explícito.
- [ ] §3 articula una propuesta de valor que no aplica a "cualquier producto".
- [ ] §4 tiene al menos un ítem en cada categoría MoSCoW (Must, Should, Could, Won't Have v1).
- [ ] §4 no contiene más Must Have que el mínimo razonable para el problema central.
- [ ] §5 incluye al menos 3 historias de usuario en formato `Como [rol], quiero [acción], para [valor]`.
- [ ] §5 cubre al menos 2 roles distintos si el sistema tiene más de un actor.
- [ ] §6 describe al menos 2 flujos típicos en lenguaje coloquial, sin diagramas formales.
- [ ] §7 lista al menos 5 preguntas de caso límite con espacio para respuesta del cliente.
- [ ] §8 tiene al menos 3 métricas SMART con criterio, target y plazo numérico.
- [ ] §9 lista al menos 3 exclusiones con justificación explícita.
- [ ] §10 tiene definidos presupuesto orientativo (o rango) y fecha objetivo (o "sin fecha límite" justificado).
- [ ] §11 lista al menos 3 riesgos con probabilidad, impacto y mitigación accionable.
- [ ] §12 define al menos 5 términos del dominio del cliente.
- [ ] El documento no contiene referencias a stack tecnológico, frameworks ni decisiones de arquitectura.
- [ ] El estado del documento se actualizó de "Borrador" a "En revisión" o "Aprobado" cuando corresponde.
- [ ] La tabla de control de cambios refleja la versión actual del documento.

---

## Control de cambios

| Versión | Fecha | Cambios | Autor |
|---|---|---|---|
| 1.0 | [YYYY-MM-DD] | Plantilla inicial generada durante bootstrap SDD 2.2 | Bootstrap SDD 2.2 |
| 1.1 | 2026-06-09 | Reformulación a nivel solución (ST-03): la cabecera declara "Nombre de la solución" y el documento se identifica como `PROJECT-BRIEF-<nombre-solucion-kebab>_v1.0.md`; nota de que el brief es uno por solución. El cuerpo de negocio (§1 a §13) no cambia. | Reformulación SDD 2.2D |

---

**Fin de la plantilla**
