# Bitácora del proyecto — Add-on Modular SAP Business One

Registro cronológico de avances, decisiones y pendientes del proyecto.
Las entradas más recientes van arriba.

> Convención: cada entrada incluye fecha, autor/responsable, resumen de lo realizado, decisiones tomadas y pendientes detectados.

---

## 2026-06-30 — Publicación en GitHub y comunicación

**Responsable:** Daniel

**Realizado:**
- Repositorio inicializado y publicado en GitHub: https://github.com/gmrdaniel/addon-sap (rama `main`).
- Commits subidos: estructura base (`a9db83b`) y sección "Para Claude Code" en la guía (`3af9be2`).
- Correo de avance enviado desde y hacia `daniel@laneta.com` (dirigido a Alejandro, firma de Daniel) con resumen de lo hecho y los pasos para continuar. Message ID: `19f1a002ad24520d`.

**Pendientes detectados:**
- [ ] Confirmar si el correo debe reenviarse directamente a Alejandro (p. ej. `chogi033@gmail.com`) además de a la cuenta de Daniel.

---

## 2026-06-30 — Estructura base del proyecto (pasos 1–6)

**Responsable:** —

**Realizado:**
- **Paso 1.** Archivos base del repo: `.gitignore`, `.editorconfig`, `Directory.Build.props`, `global.json` (.NET SDK 8.0.100).
- **Paso 2.** Solución del add-on `src/addon/LN.AddOn.sln` con 6 proyectos (net48): `LN.Core`, `LN.Integrations.SL`, `LN.Modules.Config`, `LN.Modules.ImagenesIA`, `LN.Setup`, `LN.Host`.
- **Paso 3.** Host modular: `Program.cs`, `SapConnection`, `AddOnApplication` (menú raíz + ruteo de eventos), `MenuContext` (UI API), contratos `IModule`/`IMenuContext` y dos módulos stub.
- **Paso 4.** `ServiceLayerClient` + `AuthService` con login, manejo de sesión/ROUTEID y reintento ante 401; `ConfigService`, `LicenseClient`, `AppLogger`, `ODataList`.
- **Paso 5.** `LN.Setup`: `MetadataDefinitions` (6 UDT `@LN_*` con sus UDF) y `MetadataInstaller` idempotente vía `UserTablesMD`/`UserFieldsMD` del Service Layer (sin DI API).
- **Paso 6.** Solución del backend `src/backend/LN.Backend.sln` (.NET 8): `Domain` (License, UsageEvent, contratos de visión), `Infrastructure` (`StubVisionProvider`, `BillingDbContext`), `Billing` (`ConsumptionConsolidator`), `Api` (endpoint `/health` + esqueleto de proxy `/api/vision/interpret` y `/api/licenses/{userKey}`).

**Notas / decisiones técnicas:**
- Add-on en proyectos SDK-style con `TargetFramework=net48`; interop del SDK de SAP referenciado por `$(SAPB1_SDK_PATH)` (no se versiona).
- Creación de metadatos por Service Layer (no DI API), conforme a la arquitectura.
- Proveedor de visión y motor de BD del backend quedan como **stub/comentados** hasta confirmar las decisiones de Fase 0.

**Pendientes detectados:**
- [ ] No se pudo compilar en este entorno: **no hay .NET SDK instalado**. Verificar build en estación con VS 2022 + .NET 8 SDK.
- [ ] Definir `$(SAPB1_SDK_PATH)` (variable de entorno o `Directory.Build.props` local) apuntando a los interop del SDK de SAP B1.
- [ ] Pasos pendientes de Fase 1: **paso 7** (docker-compose backend + BD) y **paso 8** (entregable instalable).

---

## 2026-06-30 — Inicio de documentación

**Responsable:** —

**Realizado:**
- Recepción y lectura del documento de arquitectura `docs/plan_addon_sap_b1.docx.md` (v1, Junio 2026).
- Creación de `README.md` con resumen ejecutivo, arquitectura, módulos, modelo de datos, licenciamiento, stack técnico y plan por fases.
- Creación de esta bitácora.
- Creación de `docs/SPEC_BASE.md`: spec base con estructura de carpetas, herramientas, dependencias, contrato de módulo, orden de arranque (Fase 1) y convenciones.

**Decisiones tomadas:**
- (ninguna nueva; se mantienen las decisiones base del documento de arquitectura)

**Pendientes / decisiones por confirmar (de la fase 0):**
- [ ] Alcance de "documento preliminar": ¿Drafts de SAP vs. orden posteada?
- [ ] Proveedor de IA: OpenAI directo o Azure OpenAI (recomendado Azure OpenAI).
- [ ] Unidad de cobro pospago: por documento, por imagen o por llamada al modelo.
- [ ] Fuente del repositorio de imágenes: carpeta local, red o nube.
- [ ] Nivel de automatización: revisión humana obligatoria vs. creación automática.
- [ ] Multiempresa: ¿una instalación atiende varias bases/empresas de SAP?

---

## 2026-06-30 — Pasos 7–8, documentación y cuestionario

**Responsable:** —

**Realizado:**
- **Paso 7.** Entorno local del backend: `src/backend/LN.Backend.Api/Dockerfile`, `deploy/docker-compose.yml` (API + PostgreSQL como placeholder) y `deploy/env/.env.example`.
- **Paso 8 (plantillas).** `build/ard/LN.AddOn.ard.template.xml` (registro del add-on), `build/installer/install.ps1` (copia binarios + genera `.ard`) y `build/installer/SMOKE_TEST.md` (checklist de aceptación de Fase 1).
- `docs/CUESTIONARIO_DECISIONES.md`: cuestionario para cerrar las decisiones de Fase 0 (bloques A producto/arquitectura, B técnicas, C piloto).
- `docs/QUICK_START.md`: guía de uso e instalación (prerrequisitos, compilar add-on/backend, desplegar `.ard`, comandos, troubleshooting), adaptada del formato de un proyecto previo.
- `README.md`: sección de documentación y estado del proyecto actualizado a "Fase 1 — esqueleto en curso".

**Decisiones tomadas:**
- Motor de BD del backend: **decidir después** → `docker-compose` y `BillingDbContext` quedan con PostgreSQL como placeholder, marcado para cambiar al confirmar Fase 0 (ver cuestionario, B1).

**Pendientes detectados:**
- [ ] Completar `docs/CUESTIONARIO_DECISIONES.md` con un responsable de negocio/técnico.
- [ ] Paso 8 real (build + registrar `.ard` + smoke test) requiere entorno Windows con SAP B1 + SDK.

---

## 2026-06-30 — Sección "Para Claude Code" en la guía

**Responsable:** —

**Realizado:**
- `docs/QUICK_START.md`: nueva sección dirigida a Claude Code con (1) reglas básicas a confirmar con el ejecutor y escribir en el `CLAUDE.md` local (no `push` sin preguntar; preguntar el mecanismo de migraciones SQL; pedir reglas propias del ejecutor), (2) uso de **superpowers** + los SPEC como fuente de verdad, y (3) instalación de MCP necesarios (MCP de SQL: MSSQL/PostgreSQL según Fase 0).

**Pendientes detectados:**
- [ ] Confirmar el nombre exacto del MCP de base de datos a instalar (depende del motor de Fase 0).

---

## Seguimiento de fases

| Fase | Objetivo | Estado | Notas |
| :----: | :---- | :----: | :---- |
| 0 | Diseño y preparación | 🟡 En curso | Arquitectura v1 + spec base listos; cuestionario de decisiones pendiente de responder |
| 1 | Esqueleto del add-on | 🟡 En curso | Estructura base (pasos 1–8) generada; falta build/instalación en entorno SAP |
| 2 | Módulo de configuración | ⚪ Pendiente | |
| 3 | IA y creación de documento | ⚪ Pendiente | |
| 4 | Repositorio y lote | ⚪ Pendiente | |
| 5 | Licenciamiento y pospago | ⚪ Pendiente | |
| 6 | QA, seguridad y rendimiento | ⚪ Pendiente | |
| 7 | Empaque, instalador y piloto | ⚪ Pendiente | |

**Leyenda:** ⚪ Pendiente · 🟡 En curso · 🟢 Completada · 🔴 Bloqueada

---

## Plantilla de entrada (copiar para nuevas anotaciones)

```
## AAAA-MM-DD — Título corto

**Responsable:**

**Realizado:**
-

**Decisiones tomadas:**
-

**Pendientes detectados:**
- [ ]
```
