# Add-on Modular para SAP Business One

Add-on modular para **SAP Business One 10.0 sobre SAP HANA** que se integra como un menú adicional dentro del cliente nativo de Windows. Agrupa varias soluciones bajo un mismo punto de entrada; cada solución es un módulo independiente conectado a un núcleo (host) común.

> Documento de arquitectura de referencia: [`docs/plan_addon_sap_b1.docx.md`](docs/plan_addon_sap_b1.docx.md) — v1, Junio 2026.

## Documentación

| Documento | Contenido |
| :---- | :---- |
| [`docs/QUICK_START.md`](docs/QUICK_START.md) | **Guía de uso e instalación**: prerrequisitos, compilar, ejecutar, desplegar y troubleshooting |
| [`docs/SPEC_BASE.md`](docs/SPEC_BASE.md) | Spec base: estructura de carpetas, herramientas y orden de arranque |
| [`docs/plan_addon_sap_b1.docx.md`](docs/plan_addon_sap_b1.docx.md) | Documento de arquitectura (fuente) |
| [`docs/CUESTIONARIO_DECISIONES.md`](docs/CUESTIONARIO_DECISIONES.md) | Cuestionario de decisiones finales (Fase 0) para responder |
| [`BITACORA.md`](BITACORA.md) | Registro cronológico de avances |

---

## Visión general

| Aspecto | Definición |
| :---- | :---- |
| **Plataforma SAP** | SAP Business One 10.0 sobre SAP HANA (instalación existente) |
| **Cliente** | Cliente nativo de Windows (no Web Client) |
| **Lenguaje** | C# |
| **Acceso a datos** | SAP Service Layer (REST/OData v4), sin DI API en runtime |
| **Modelo de despliegue** | Híbrido: cliente on-premise + servicios en la nube |
| **Licenciamiento** | Tres perfiles, facturación pospago por consumo |
| **Integración de IA** | Modelo de visión para interpretar imágenes, con instrucciones configurables |

---

## Arquitectura (5 capas)

| Capa | Tecnología | Responsabilidad |
| :---- | :---- | :---- |
| Presentación | SAP B1 SDK (UI API), C# | Menú, formularios y eventos dentro del cliente nativo |
| Orquestación y negocio | C# (.NET Framework 4.8) | Host modular, carga de módulos, reglas y validación |
| Datos SAP | Service Layer (REST/OData v4) | Lectura/escritura de órdenes, borradores, socios, artículos |
| Servicios en nube | Backend propio (ASP.NET Core .NET 8) + modelo de visión | IA de visión, licenciamiento y medición de consumo |
| Persistencia de config. | UDT/UDF en SAP | Configuración, mapeos, perfiles, bitácora y trazabilidad |

### Punto clave: el backend como proxy de IA

Las llamadas al modelo de visión **no** se hacen desde cada estación. El add-on envía la imagen a un backend propio que: guarda la API key, valida la cuota del perfil, llama al modelo, registra el evento de consumo (facturación pospago) y devuelve la interpretación. Centraliza seguridad de credenciales, control de licencias y medición.

---

## Módulos

### Menú principal
Menú de primer nivel en el cliente nativo del que se despliegan las soluciones. Diseñado para crecer: nuevas soluciones se "enchufan" al host común sin tocar las anteriores.

### Módulo 1 — Administrador de Configuraciones
Centraliza la parametrización del add-on y el control de uso:
- Permisos y licencias (perfil por usuario, cuota, consumo, estado pospago)
- Selección y mapeo del tipo de documento a generar
- Alta/edición de datos maestros (socios, artículos) vía Service Layer
- Registro de uso por usuario
- Editor de "personalidad" del modelo (system prompt + parámetros) por empresa/usuario

### Módulo 2 — Procesamiento de Imágenes con IA
Convierte imágenes en documentos de SAP con revisión del usuario:
- Fuente configurable: una imagen o repositorio (carpeta local, red o nube)
- Interpretación estructurada por modelo de visión
- Validación de requisitos contra reglas y datos de SAP (artículo/socio existen)
- Formularios de revisión con estados: pendiente, validada, creada, rechazada
- Creación del documento (orden de venta o borrador) vía Service Layer
- Trazabilidad imagen → documento

---

## Modelo de datos (UDTs propuestas)

| Tabla (UDT) | Propósito |
| :---- | :---- |
| `@LN_CONFIG` | Configuración general del add-on |
| `@LN_PERFILES` | Asignación de perfil por usuario |
| `@LN_DOCMAP` | Mapeo del documento a generar |
| `@LN_PERSONA` | Instrucciones del modelo por ámbito |
| `@LN_USOLOG` | Bitácora de uso y medición local |
| `@LN_TRAZA` | Trazabilidad imagen → documento |

Se crean en la instalación mediante metadatos del Service Layer (`UserTablesMD`, `UserFieldsMD`, `UserObjectsMD`).

---

## Licenciamiento (cobro pospago)

| Perfil | Alcance | Usuarios |
| :---- | :---- | :---- |
| **Básico** | Configuración y una imagen a la vez, cuota mensual baja | 1 |
| **Pro** | Repositorio y lote, mapeos avanzados, cuota media | Varios |
| **Enterprise** | Lote alto, reglas avanzadas, multiempresa, soporte prioritario | Sin límite |

---

## Stack técnico

| Componente | Tecnología |
| :---- | :---- |
| Add-on (interfaz y lógica) | C# / .NET Framework 4.8 |
| Interfaz en el cliente | SAP B1 SDK (UI API) |
| Acceso a datos SAP | HttpClient + System.Text.Json contra Service Layer |
| IA de visión | OpenAI o Azure OpenAI (recomendado Azure OpenAI) |
| Backend de IA, licencias y medición | ASP.NET Core (.NET 8) |
| Almacén del backend | PostgreSQL o SQL Server |
| Empaque e instalación | Add-on Registration Data (.ard) + instalador |

### Estructura de la solución (propuesta)

```
LN.AddOn.sln
├─ Host               · arranque, conexión al cliente, registro de menús
├─ Core               · contratos de módulo, ServiceLayerClient, AuthService,
│                       ConfigService, LicenseClient, Logger
├─ Modules.Config     · formularios y lógica del Módulo 1
├─ Modules.ImagenesIA · formularios y lógica del Módulo 2
├─ Integrations.SL    · modelos tipados de objetos de Service Layer
└─ Setup              · creación de UDT/UDF/UDO por metadatos de Service Layer

LN.Backend.sln (separado, en nube)
├─ Api                · IA de visión, licencias y medición
└─ Billing            · consolidación de consumo pospago
```

---

## Plan de implementación por fases

| Fase | Objetivo | Duración |
| :---- | :---- | :---- |
| 0 | Diseño y preparación | 1–2 sem |
| 1 | Esqueleto del add-on (menú, conexión, auth SL, metadatos) | 1–2 sem |
| 2 | Módulo de configuración | 2–3 sem |
| 3 | IA y creación de documento (una imagen) | 2–3 sem |
| 4 | Repositorio y lote | 1–2 sem |
| 5 | Licenciamiento y pospago | 2 sem |
| 6 | QA, seguridad y rendimiento | 1–2 sem |
| 7 | Empaque, instalador y piloto | 1 sem |

**Duración estimada total:** 12 a 18 semanas.

---

## Decisiones por confirmar

1. Alcance de "documento preliminar" → ¿Drafts de SAP vs. orden posteada?
2. Proveedor de IA → OpenAI directo o Azure OpenAI (recomendado).
3. Unidad de cobro pospago → por documento, por imagen o por llamada al modelo.
4. Fuente del repositorio de imágenes → carpeta local, red o nube.
5. Nivel de automatización → revisión humana obligatoria vs. creación automática.
6. Multiempresa → ¿una instalación atiende varias bases/empresas de SAP?

---

## Estado del proyecto

**Fase 1 — esqueleto en curso.** Estructura base generada (pasos 1–8): ambas soluciones
(`src/addon/LN.AddOn.sln` y `src/backend/LN.Backend.sln`), host modular, `ServiceLayerClient`,
setup de metadatos, esqueleto de backend, `docker-compose` y plantillas de instalación (`.ard`,
instalador, smoke test).

Pendiente: cerrar decisiones de Fase 0 ([`docs/CUESTIONARIO_DECISIONES.md`](docs/CUESTIONARIO_DECISIONES.md))
y verificar el build/instalación en un entorno Windows con SAP B1 + SDK.

Para arrancar, ver [`docs/QUICK_START.md`](docs/QUICK_START.md). Ver [`BITACORA.md`](BITACORA.md)
para el registro de avances.
