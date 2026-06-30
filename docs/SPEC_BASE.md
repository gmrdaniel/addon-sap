# Spec base del proyecto — Add-on Modular SAP Business One

Especificación de la base técnica del proyecto: estructura de carpetas, herramientas, dependencias y convenciones para arrancar el desarrollo (cubre las Fases 0 y 1 del plan).

> Referencia de arquitectura: [`plan_addon_sap_b1.docx.md`](plan_addon_sap_b1.docx.md)

---

## 1. Objetivo de esta base

Dejar lista la **infraestructura mínima** sobre la que se construyen los módulos:

- Dos soluciones .NET separadas: el add-on (on-premise) y el backend (nube).
- Un **host modular** que registra el menú y carga módulos por contrato.
- Un **cliente de Service Layer** central con manejo de sesión.
- Creación de **UDT/UDF** vía metadatos del Service Layer.
- Esqueleto de **backend** (IA, licencias, medición).
- Herramientas de build, formato, logging y configuración.

---

## 2. Requisitos previos (entorno de desarrollo)

| Herramienta | Versión / Nota |
| :---- | :---- |
| **SAP Business One SDK** | 10.0 (UI API + DI API solo para tipos/metadatos en build, no en runtime) |
| **Cliente SAP B1** | 10.0 instalado en la estación de desarrollo (necesario para depurar el add-on) |
| **Service Layer** | Endpoint accesible (`https://<host>:50000/b1s/v2/`) |
| **.NET Framework** | 4.8 (Developer Pack) — requisito del SDK para el add-on |
| **.NET SDK** | 8.0 LTS — para el backend ASP.NET Core |
| **Visual Studio** | 2022 (workloads: .NET desktop, ASP.NET, .NET Framework 4.8) |
| **Base de datos** | PostgreSQL 16 o SQL Server 2022 (almacén del backend) |
| **Git** | control de versiones |
| **Docker Desktop** *(opcional)* | para correr backend + DB en local |

> El add-on (.NET Framework 4.8) solo compila/depura en Windows. El backend (.NET 8) es multiplataforma.

---

## 3. Estructura de carpetas del repositorio

```
addon-sap/
├─ README.md
├─ BITACORA.md
├─ .gitignore
├─ .editorconfig
├─ Directory.Build.props          · propiedades MSBuild comunes (versión, nullable, langversion)
├─ global.json                    · fija la versión del .NET SDK para el backend
│
├─ docs/
│  ├─ plan_addon_sap_b1.docx.md   · documento de arquitectura (fuente)
│  ├─ SPEC_BASE.md                · este documento
│  └─ adr/                         · Architecture Decision Records (1 archivo por decisión)
│
├─ src/
│  ├─ addon/                       · LN.AddOn.sln  (on-premise, .NET Framework 4.8)
│  │  ├─ LN.AddOn.sln
│  │  ├─ LN.Host/                  · arranque, conexión al cliente, registro de menús
│  │  ├─ LN.Core/                  · contratos de módulo, ServiceLayerClient, Auth,
│  │  │                              Config, LicenseClient, Logger
│  │  ├─ LN.Integrations.SL/       · modelos tipados de objetos de Service Layer (DTOs)
│  │  ├─ LN.Modules.Config/        · Módulo 1 — Administrador de Configuraciones
│  │  ├─ LN.Modules.ImagenesIA/    · Módulo 2 — Procesamiento de Imágenes con IA
│  │  └─ LN.Setup/                 · creación de UDT/UDF/UDO por metadatos de Service Layer
│  │
│  └─ backend/                     · LN.Backend.sln  (nube, .NET 8)
│     ├─ LN.Backend.sln
│     ├─ LN.Backend.Api/           · IA de visión, licencias y medición (ASP.NET Core)
│     ├─ LN.Backend.Billing/       · consolidación de consumo pospago
│     ├─ LN.Backend.Domain/        · entidades y reglas de dominio
│     └─ LN.Backend.Infrastructure/· EF Core, repositorios, proveedor de IA
│
├─ tests/
│  ├─ LN.Core.Tests/               · pruebas del núcleo del add-on
│  ├─ LN.Backend.Api.Tests/        · pruebas del backend
│  └─ LN.Backend.Domain.Tests/     · pruebas de dominio
│
├─ build/
│  ├─ ard/                         · plantilla y generación del Add-on Registration Data (.ard)
│  └─ installer/                   · script/proyecto del instalador del add-on
│
├─ deploy/
│  ├─ docker-compose.yml           · backend + base de datos para desarrollo local
│  └─ env/                         · plantillas .env por entorno (sin secretos)
│
└─ tools/
   └─ scripts/                     · scripts de utilidad (setup, seed de UDT, etc.)
```

> Los `.csproj` concretos se definen dentro de cada carpeta de proyecto. Los nombres `LN.*` siguen la convención del documento de arquitectura (ajustar el prefijo a la convención del equipo si cambia).

---

## 4. Solución 1 — Add-on (`src/addon`, .NET Framework 4.8)

### Proyectos y responsabilidades

| Proyecto | Tipo | Responsabilidad |
| :---- | :---- | :---- |
| `LN.Host` | WinExe / .NET FW 4.8 | Punto de entrada. Conecta a `SAPbouiCOM` (UI API), registra el menú raíz y descubre/carga módulos. |
| `LN.Core` | Class Library | Contratos (`IModule`), `ServiceLayerClient`, `AuthService` (login/sesión SL), `ConfigService`, `LicenseClient` (habla con el backend), `Logger`. |
| `LN.Integrations.SL` | Class Library | DTOs tipados de Service Layer: `Orders`, `Drafts`, `BusinessPartners`, `Items`, y los UDT `@LN_*`. |
| `LN.Modules.Config` | Class Library | Formularios y lógica del Módulo 1. Implementa `IModule`. |
| `LN.Modules.ImagenesIA` | Class Library | Formularios y lógica del Módulo 2. Implementa `IModule`. |
| `LN.Setup` | Class Library / Tool | Crea UDT/UDF/UDO vía `UserTablesMD`/`UserFieldsMD`/`UserObjectsMD` en la instalación. |

### Contrato de módulo (base del host modular)

```csharp
// LN.Core/Contracts/IModule.cs
public interface IModule
{
    string Id { get; }                 // ej. "config", "imagenes-ia"
    string MenuCaption { get; }        // texto del submenú
    void RegisterMenu(IMenuContext ctx);
    void OnMenuClick(string menuUid);  // abre formularios del módulo
}
```

El `LN.Host` mantiene un registro `IReadOnlyList<IModule>` y enruta los eventos de menú de la UI API al módulo correspondiente. Agregar un módulo nuevo = nuevo proyecto que implementa `IModule`, sin tocar el host ni los demás módulos.

### Cliente de Service Layer (en `LN.Core`)

Responsabilidades mínimas:
- Login `POST /b1s/v2/Login`, manejo de cookie de sesión y **ROUTEID** (alta disponibilidad).
- **Renovación/reintento automático** ante sesión vencida (riesgo identificado en arquitectura).
- Helpers `GetAsync<T>`, `PostAsync<T>`, `PatchAsync` sobre `HttpClient` + `System.Text.Json`.
- Sin DI API en runtime: todo por REST/OData v4.

### Dependencias (NuGet) del add-on

| Paquete | Uso |
| :---- | :---- |
| `System.Text.Json` | Serialización contra Service Layer |
| Referencias COM del SDK (`SAPbouiCOM`, `SAPbobsCOM`) | UI API + metadatos (vienen del SDK instalado, no de NuGet) |
| `Microsoft.Extensions.Configuration` *(opcional)* | Configuración local del add-on |
| `Serilog` o `NLog` | Logging local (alimenta `@LN_USOLOG`) |

> SDK de SAP B1: las DLLs de interop se referencian desde la instalación local del SDK (`..\SAP Business One SDK\...`), no se publican al repo.

---

## 5. Solución 2 — Backend (`src/backend`, .NET 8)

### Proyectos

| Proyecto | Responsabilidad |
| :---- | :---- |
| `LN.Backend.Api` | API REST: proxy de IA de visión, validación de cuota, registro de eventos de consumo, endpoints de licencias. |
| `LN.Backend.Domain` | Entidades (`Profile`, `UsageEvent`, `License`), reglas de cuota. |
| `LN.Backend.Infrastructure` | EF Core, repositorios, cliente del proveedor de IA (OpenAI/Azure OpenAI). |
| `LN.Backend.Billing` | Consolidación mensual de consumo pospago, generación de reportes. |

### Responsabilidades clave (proxy de IA)

1. Recibe la imagen del add-on.
2. Valida la cuota del perfil del usuario/empresa.
3. Llama al modelo de visión con las instrucciones (`@LN_PERSONA`).
4. Registra el evento facturable (medición).
5. Devuelve la interpretación estructurada (JSON).

> La **API key del modelo vive solo en el backend**, nunca en las estaciones (riesgo de seguridad mitigado).

### Dependencias (NuGet) del backend

| Paquete | Uso |
| :---- | :---- |
| `Microsoft.AspNetCore.*` | API (incluido en .NET 8) |
| `Microsoft.EntityFrameworkCore` + provider (`Npgsql.EntityFrameworkCore.PostgreSQL` o `Microsoft.EntityFrameworkCore.SqlServer`) | Persistencia de consumo/facturación |
| `Azure.AI.OpenAI` o `OpenAI` | Cliente del modelo de visión |
| `Serilog.AspNetCore` | Logging estructurado |
| `Swashbuckle.AspNetCore` | OpenAPI/Swagger |
| `FluentValidation` | Validación de requests |
| `xunit` + `Microsoft.NET.Test.Sdk` | Pruebas |

---

## 6. Herramientas base y archivos de configuración

| Archivo | Propósito |
| :---- | :---- |
| `.gitignore` | Ignora `bin/`, `obj/`, `*.user`, secretos, DLLs del SDK, `.env` |
| `.editorconfig` | Estilo de código y formato consistente entre proyectos |
| `Directory.Build.props` | Propiedades MSBuild comunes (langversion, nullable, treat warnings) |
| `global.json` | Fija la versión del .NET SDK para el backend (reproducibilidad) |
| `deploy/docker-compose.yml` | Backend + base de datos en local |
| `docs/adr/` | Architecture Decision Records (registra las "decisiones por confirmar") |

### Calidad y CI (recomendado para Fase 0/1)

- **Formato:** `dotnet format` (backend) + reglas `.editorconfig`.
- **Tests:** `dotnet test` (backend y `LN.Core`).
- **CI mínimo:** workflow que compila ambas soluciones, corre tests y valida formato.
- **Logging:** Serilog en backend; Serilog/NLog local en el add-on alimentando `@LN_USOLOG`.

---

## 7. Orden de arranque (Fase 1 — esqueleto)

1. Crear repositorio y archivos base (`.gitignore`, `.editorconfig`, `Directory.Build.props`, `global.json`).
2. Crear `LN.AddOn.sln` con `LN.Host`, `LN.Core`, `LN.Integrations.SL`.
3. Implementar conexión a la UI API y **registro del menú raíz** + un submenú de prueba.
4. Implementar `ServiceLayerClient` con login, manejo de sesión/ROUTEID y reintento.
5. Implementar `LN.Setup`: creación de UDT/UDF `@LN_*` por metadatos.
6. Crear `LN.Backend.sln` con `Api`, `Domain`, `Infrastructure`, `Billing`; endpoint de salud + esqueleto de proxy de IA.
7. Generar `docker-compose.yml` (backend + DB) para desarrollo local.
8. **Entregable Fase 1:** add-on instalable que arranca, registra el menú, conecta al cliente, autentica al Service Layer y crea los metadatos.

---

## 8. Convenciones

- **Namespaces/prefijo:** `LN.*` (ajustable a la convención del equipo).
- **UDT/UDF:** prefijo `@LN_` (ver tabla de modelo de datos en el README).
- **Idioma:** comentarios y documentación en español; identificadores de código en inglés.
- **Secretos:** nunca en el repo. API keys y cadenas de conexión por variables de entorno / `dotnet user-secrets`.
- **Ramas:** una rama por fase/feature; `main` siempre compilable.
- **Decisiones:** toda decisión arquitectónica relevante se documenta como ADR en `docs/adr/`.

---

## 9. Pendientes que bloquean la base (de Fase 0)

Estas confirmaciones afectan la estructura y deben cerrarse antes/durante el armado de la base:

- [ ] Proveedor de IA (OpenAI vs **Azure OpenAI**) → define el paquete NuGet del backend.
- [ ] Base de datos del backend (PostgreSQL vs SQL Server) → define el provider de EF Core.
- [ ] "Documento preliminar" = Drafts de SAP → define los DTOs de `LN.Integrations.SL`.
- [ ] Unidad de cobro (documento/imagen/llamada) → define el modelo de `UsageEvent`.
- [ ] Multiempresa → afecta el alcance de configuración y los UDT.

Ver `BITACORA.md` para el seguimiento de estos puntos.
