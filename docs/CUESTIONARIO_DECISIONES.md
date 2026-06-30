# Cuestionario de decisiones finales — Add-on Modular SAP Business One

Este cuestionario recoge las decisiones que deben cerrarse en la **Fase 0 (diseño)** porque
afectan la arquitectura, el modelo comercial y la configuración de la base del proyecto.

**Instrucciones:** marca una opción por pregunta (o escribe en "Otra"), y agrega comentarios
donde se pida. Al terminar, devuelve el documento al equipo técnico.

- **Responde:** ______________________________
- **Rol / área:** ______________________________
- **Fecha:** ______________________________

---

## Bloque A — Decisiones de producto/arquitectura

### A1. Tipo de documento "preliminar"
¿Qué debe generar el add-on cuando no se crea una orden de venta posteada?

- [ ] Borrador de SAP (servicio **Drafts** del Service Layer)
- [ ] Orden de venta **posteada** directamente
- [ ] Ambos, configurable por perfil/empresa
- [ ] Otra: ______________________________

**Comentarios:** ______________________________

---

### A2. Proveedor de IA de visión
¿Con qué proveedor se interpretan las imágenes? *(Recomendación técnica: Azure OpenAI por manejo de datos empresarial.)*

- [ ] **Azure OpenAI** (recomendado)
- [ ] OpenAI directo
- [ ] Otra: ______________________________

**Comentarios (región/datos/contrato):** ______________________________

---

### A3. Unidad de cobro pospago
¿Sobre qué se factura el consumo?

- [ ] Por **documento generado** en SAP
- [ ] Por **imagen procesada**
- [ ] Por **llamada al modelo** de visión
- [ ] Otra: ______________________________

**Comentarios (precio por unidad, mínimos):** ______________________________

---

### A4. Fuente del repositorio de imágenes
¿De dónde se leen las imágenes? *(Define los conectores a desarrollar; se puede marcar más de una.)*

- [ ] Carpeta **local**
- [ ] Recurso de **red** (carpeta compartida)
- [ ] Almacenamiento en **nube** (¿cuál? OneDrive / SharePoint / S3 / otro): ____________
- [ ] Otra: ______________________________

**Comentarios:** ______________________________

---

### A5. Nivel de automatización
¿Se permite crear documentos sin revisión humana?

- [ ] **Revisión humana obligatoria** siempre
- [ ] Revisión obligatoria en perfiles Básico y Pro; automática en Enterprise
- [ ] Creación **automática** cuando la confianza supera un umbral (indicar umbral): ______ %
- [ ] Otra: ______________________________

**Comentarios:** ______________________________

---

### A6. Multiempresa
¿Una misma instalación atiende varias bases/empresas de SAP?

- [ ] No, una sola empresa
- [ ] Sí, varias empresas (indicar cuántas / cuáles): ______________________________
- [ ] Por confirmar

**Comentarios:** ______________________________

---

## Bloque B — Decisiones técnicas de la base

### B1. Motor de base de datos del backend
Afecta `docker-compose`, el provider de EF Core y la cadena de conexión.

- [ ] **PostgreSQL** (recomendado para el backend en nube)
- [ ] **SQL Server**
- [ ] Otra: ______________________________

**Comentarios (infraestructura existente):** ______________________________

---

### B2. Hospedaje del backend
¿Dónde se despliega el backend de IA/licencias/medición?

- [ ] Azure (App Service / Container Apps)
- [ ] AWS
- [ ] Servidor propio / on-premise
- [ ] Por definir
- [ ] Otra: ______________________________

**Comentarios:** ______________________________

---

### B3. Modo de arranque del add-on en SAP
Cómo se carga el add-on en las estaciones.

- [ ] **Automatic** (arranca con el cliente, el usuario puede desactivarlo)
- [ ] **Mandatory** (obligatorio)
- [ ] **Manual**
- [ ] Otra: ______________________________

**Comentarios:** ______________________________

---

### B4. Convención de nombres de UDT/UDF
Prefijo de las tablas y campos de usuario.

- [ ] Mantener `@LN_` (propuesta actual)
- [ ] Otro prefijo: ______________________________

**Comentarios:** ______________________________

---

## Bloque C — Alcance del piloto

### C1. Empresa y usuarios del piloto
- Empresa / base de datos de prueba: ______________________________
- Número de usuarios y perfiles asignados: ______________________________

### C2. Tipo de imágenes y documento objetivo del piloto
- Tipo de imagen (factura, ticket, remisión, etc.): ______________________________
- Documento que se generará en el piloto: ______________________________

### C3. Volumen estimado mensual
- Imágenes/documentos por mes (aprox.): ______________________________

**Comentarios generales:** ______________________________

---

> Una vez respondido, el equipo técnico actualiza `BITACORA.md` con las decisiones tomadas y
> ajusta la configuración (provider de IA, motor de BD, conectores y reglas de validación).
