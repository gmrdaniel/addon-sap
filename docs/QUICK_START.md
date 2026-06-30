# Guía de uso e instalación — Add-on Modular SAP Business One

Cómo preparar el entorno, compilar, ejecutar y desplegar este proyecto.
La solución tiene dos partes: el **add-on** (cliente nativo de SAP, .NET Framework 4.8) y el
**backend** (servicios en nube, .NET 8). Ver arquitectura completa en [`../README.md`](../README.md).

---

## ⚠️ Para Claude Code — leer ANTES de desarrollar

> Esta sección está dirigida al asistente **Claude Code** que vaya a trabajar en este
> repositorio. Si eres Claude y abres este proyecto, **lee esto primero** y actúa en consecuencia
> antes de tocar código.

### 1. Confirma las reglas básicas y escríbelas en el `CLAUDE.md` local

Al iniciar, **pregunta a la persona que va a ejecutar/desarrollar** y, con sus respuestas,
**escribe las reglas en el archivo `CLAUDE.md` de este proyecto** (créalo en la raíz si no existe),
para que queden persistentes en futuras sesiones. Confirma al menos estas reglas base:

- **No hacer `git push` sin preguntar.** Nunca subir cambios al remoto sin confirmación explícita
  del usuario en esa sesión. (También: no crear PRs ni `--force` sin permiso.)
- **Migraciones del esquema SQL: preguntar cómo generarlas.** Antes de crear o modificar el
  esquema de base de datos (las migraciones del modelo arquitectónico del software en SQL),
  **pregunta al usuario el mecanismo que quiere usar** (p. ej. EF Core `dotnet ef migrations add`,
  scripts SQL versionados, otra herramienta) y respétalo. No inventes el flujo de migraciones.
- **Reglas propias del ejecutor.** Pregunta directamente a quien va a ejecutar esto: *"¿Qué reglas
  adicionales quieres que siga?"* y agrégalas al `CLAUDE.md` local (estilo de commits, ramas,
  qué tocar y qué no, idioma, etc.).

> Plantilla mínima sugerida para el `CLAUDE.md` de este proyecto:
>
> ```markdown
> # Reglas del proyecto addon-sap
> - No hacer `git push` / PR / force-push sin preguntar al usuario en la sesión.
> - Antes de generar migraciones SQL, preguntar el mecanismo (EF Core / scripts / otro).
> - <reglas adicionales que indique el ejecutor>
> ```

### 2. Desarrolla con **superpowers** y guiándote por los SPEC

- Usa, de preferencia, el skill **superpowers** de Claude para el flujo de desarrollo
  (brainstorming → planes → TDD → revisión), en lugar de improvisar.
- Toma como **fuente de verdad** los documentos del repo y trabaja **con base en ellos**:
  - [`SPEC_BASE.md`](SPEC_BASE.md) — estructura, herramientas y orden de arranque.
  - [`plan_addon_sap_b1.docx.md`](plan_addon_sap_b1.docx.md) — arquitectura.
  - [`CUESTIONARIO_DECISIONES.md`](CUESTIONARIO_DECISIONES.md) — decisiones de Fase 0 (confírmalas
    antes de implementar lo que dependa de ellas: proveedor de IA, motor de BD, etc.).

### 3. Instala los MCP necesarios

Según la tarea, instala/activa los **MCP** que hagan falta y confírmalo con el usuario. En
particular, para el trabajo de base de datos y migraciones del backend:

- **MCP de SQL** (servidor de base de datos): el de **SQL Server / MSSQL** o el de **PostgreSQL**,
  según el motor que se confirme en Fase 0 (ver [`CUESTIONARIO_DECISIONES.md`](CUESTIONARIO_DECISIONES.md),
  B1). Úsalo para inspeccionar el esquema y validar las migraciones, **sin** ejecutar cambios
  destructivos sin permiso.

> Confirma con el usuario el nombre exacto del MCP a instalar y sus credenciales antes de
> conectarlo. Las claves nunca van al repositorio.

---

## Prerrequisitos

Antes de empezar, instala:

- **Windows** (obligatorio para el add-on; el SDK de SAP solo corre en Windows).
- **SAP Business One 10.0 + SDK** (UI API y DI API) — cliente nativo instalado.
- **Acceso al SAP Service Layer** (`https://<host>:50000/b1s/v2/`) con credenciales.
- **.NET Framework 4.8 Developer Pack** — requisito del SDK para el add-on.
- **.NET 8 SDK** — para el backend ([Descargar](https://dotnet.microsoft.com/download)).
- **Visual Studio 2022** (workloads: *.NET desktop*, *ASP.NET*, *.NET Framework 4.8*).
- **Git**.
- **Docker Desktop** *(opcional)* — para correr el backend + base de datos en local.
- **PostgreSQL 16** o **SQL Server 2022** *(según se confirme en Fase 0)*.

> El motor de base de datos del backend está pendiente de decidir (ver
> [`CUESTIONARIO_DECISIONES.md`](CUESTIONARIO_DECISIONES.md), bloque B1). El `docker-compose`
> usa PostgreSQL como placeholder.

---

## Instalación (entorno de desarrollo)

### 1. Clonar el repositorio

```powershell
git clone <url-del-repo> addon-sap
cd addon-sap
```

### 2. Configurar la ruta del SDK de SAP (solo add-on)

Los interop del SDK no se versionan. Define la variable de entorno `SAPB1_SDK_PATH`
apuntando a la carpeta donde están `Interop.SAPbouiCOM.dll` e `Interop.SAPbobsCOM.dll`:

```powershell
# Ejemplo (ajustar a la ruta real del SDK en tu estación)
setx SAPB1_SDK_PATH "C:\Program Files (x86)\SAP\SAP Business One SDK\DI API\Interop"
```

Reinicia Visual Studio / la terminal tras definir la variable.

### 3. Configurar variables del backend

```powershell
copy deploy\env\.env.example deploy\env\.env
# Editar deploy\env\.env con la clave de BD y la API key del proveedor de IA
```

---

## Compilar y ejecutar

### Backend (.NET 8)

```powershell
# Restaurar y compilar
dotnet build src\backend\LN.Backend.sln

# Ejecutar el API localmente
dotnet run --project src\backend\LN.Backend.Api

# Salud del servicio
# GET http://localhost:8080/health  ->  { "status": "ok" }
```

O con Docker (API + base de datos):

```powershell
cd deploy
docker compose --env-file env\.env up --build
```

### Add-on (.NET Framework 4.8)

1. Abrir `src\addon\LN.AddOn.sln` en Visual Studio 2022.
2. Verificar que `SAPB1_SDK_PATH` está definida (las referencias a `SAPbouiCOM`/`SAPbobsCOM`
   se resuelven con ella).
3. Compilar en **Release** (proyecto de arranque: `LN.Host`).
4. Tener el **cliente SAP B1 abierto** y con sesión en una compañía de prueba.
5. Ejecutar / depurar `LN.Host`: el add-on se conecta al cliente y registra el menú.

> En depuración, `SapConnection` usa una cadena de conexión por defecto. En instalación real,
> SAP entrega el connection string como argumento al ejecutable.

---

## Despliegue del add-on en SAP (instalación real)

```powershell
# 1. Publicar el host en Release
#    (genera binarios en src\addon\LN.Host\bin\Release\net48)

# 2. Generar el .ard y copiar binarios a la carpeta de instalación
cd build\installer
.\install.ps1 -PublishDir "..\..\src\addon\LN.Host\bin\Release\net48" `
              -InstallFolder "C:\Program Files\LaNeta\AddOn"

# 3. Registrar el .ard MANUALMENTE en SAP:
#    Administración > Add-Ons > Administración de add-ons > Registrar
#    -> seleccionar C:\Program Files\LaNeta\AddOn\LN.AddOn.ard
```

Verificación con la checklist: [`../build/installer/SMOKE_TEST.md`](../build/installer/SMOKE_TEST.md).

---

## Comandos útiles

```powershell
# Backend
dotnet build src\backend\LN.Backend.sln          # compilar
dotnet run --project src\backend\LN.Backend.Api  # ejecutar API
dotnet test src\backend\LN.Backend.sln           # pruebas (cuando existan)
dotnet format src\backend\LN.Backend.sln         # formato

# Docker (entorno local backend + BD)
docker compose --env-file env\.env up --build    # levantar
docker compose down                              # detener
docker compose logs -f api                       # ver logs del API
```

---

## Configuración de servicios (claves y endpoints)

| Servicio | Propósito | Dónde se configura |
| :---- | :---- | :---- |
| SAP Service Layer | Datos de SAP (órdenes, socios, artículos, UDT) | `ServiceLayerOptions` / config del add-on |
| Proveedor de IA (OpenAI/Azure) | Interpretación de imágenes | **Solo en el backend** (`.env` / appsettings) |
| Base de datos del backend | Consumo y facturación pospago | `ConnectionStrings__Billing` (`.env`) |

> **Seguridad:** la API key del modelo vive **únicamente en el backend**, nunca en las
> estaciones del add-on (decisión de arquitectura).

---

## Solución de problemas

### El add-on no compila: no encuentra `SAPbouiCOM` / `SAPbobsCOM`
- Verifica que `SAPB1_SDK_PATH` apunta a la carpeta con los `Interop.*.dll`.
- Reinicia Visual Studio tras definir la variable.

### El add-on no aparece en el menú de SAP
- Confirma que el cliente SAP estaba **abierto** al ejecutar `LN.Host`.
- Revisa el log: `%LocalAppData%\LN.AddOn\logs\addon-*.log`.
- Verifica que el `.ard` quedó registrado y el modo de arranque (Automatic).

### Falla la autenticación al Service Layer
- Revisa `BaseUrl`, `CompanyDb`, usuario y contraseña.
- En desarrollo, `AllowSelfSignedCertificate = true` permite certificados auto-firmados.
- Errores 401 intermitentes: el cliente reintenta y renueva sesión automáticamente; si
  persisten, revisa ROUTEID / alta disponibilidad.

### No se crean las UDT `@LN_*`
- Confirma que el usuario del Service Layer tiene permisos de metadatos.
- `MetadataInstaller` es idempotente: si ya existen, las omite (no es error).

### El backend no levanta en Docker
- Revisa que `deploy\env\.env` existe y tiene valores.
- Puerto 8080 o 5432 ocupados: cámbialos en `docker-compose.yml`.

---

## Próximos pasos

- **Cerrar Fase 0:** completar [`CUESTIONARIO_DECISIONES.md`](CUESTIONARIO_DECISIONES.md).
- **Spec base:** [`SPEC_BASE.md`](SPEC_BASE.md) — estructura, herramientas y orden de arranque.
- **Arquitectura:** [`plan_addon_sap_b1.docx.md`](plan_addon_sap_b1.docx.md).
- **Avances:** [`../BITACORA.md`](../BITACORA.md).

---

## Soporte

- **Contacto:** administracion@laneta.com
- **Documentación SAP Service Layer:** SAP Help Portal (Service Layer / B1).
- **SDK de SAP Business One:** documentación del SDK incluida en la instalación.
