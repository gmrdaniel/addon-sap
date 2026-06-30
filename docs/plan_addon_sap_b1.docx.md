  
**Add-on Modular para SAP Business One**

Plan técnico y arquitectura

*Módulo de configuración y generación de documentos a partir de imágenes con IA*

SAP Business One 10.0 sobre SAP HANA

Cliente nativo  ·  C\#  ·  SAP Service Layer  ·  Despliegue híbrido

Junio 2026  ·  Documento de arquitectura v1

**Contenido**

# **1\. Resumen ejecutivo**

Este documento define la arquitectura y el plan de implementación de un add-on modular para SAP Business One que se integra como un menú adicional dentro del cliente nativo. El add-on agrupa varias soluciones bajo un mismo punto de entrada y cada una se construye como un módulo independiente conectado a un núcleo común.

La primera solución es un administrador de configuraciones, permisos, licencias, datos maestros y parámetros del modelo de IA. La segunda lee una imagen o un repositorio de imágenes, las interpreta con un modelo de visión, valida que cumplan los requisitos definidos y, cuando corresponde, genera una orden de venta o un documento preliminar en SAP.

Toda la operación de datos contra SAP se realiza sobre el Service Layer, el add-on se desarrolla en C\# y el modelo de servicio es híbrido: la interfaz vive dentro del cliente y los servicios de IA, licenciamiento y medición se exponen como un backend en la nube. El licenciamiento contempla tres perfiles y facturación pospago por consumo.

# **2\. Contexto y decisiones base**

El proyecto parte de una instalación existente y de un conjunto de decisiones ya definidas. La tabla siguiente las resume.

| Decisión | Definición |
| :---- | :---- |
| **Plataforma SAP** | SAP Business One 10.0 sobre SAP HANA (instalación existente) |
| **Cliente** | Cliente nativo de Windows, no Web Client |
| **Lenguaje** | C\# |
| **Acceso a datos** | SAP Service Layer (REST/OData), sin DI API en tiempo de ejecución |
| **Modelo de despliegue** | Híbrido: cliente on-premise más servicios en nube |
| **Licenciamiento** | Tres perfiles, servicio pospago por consumo |
| **Integración de IA** | Modelo de visión para interpretar imágenes, con instrucciones configurables |

La elección de HANA no limita el alcance. SAP mantiene el soporte para HANA y SQL en las próximas versiones, y el Service Layer es nativo y más completo sobre HANA, lo que favorece este diseño. Al usar el cliente nativo, la interfaz se construye con el SDK de SAP Business One (UI API) y la capa de datos se resuelve por Service Layer.

# **3\. Arquitectura general**

La solución se organiza en cinco capas. La interfaz vive dentro del cliente nativo mediante el SDK de SAP Business One. La lógica y la orquestación corren en C\#. El acceso a datos de SAP se hace por Service Layer. Los servicios de IA, licenciamiento y medición se exponen como un backend propio en la nube. La configuración y la trazabilidad se guardan en tablas definidas por el usuario dentro de SAP.

| Capa | Tecnología | Responsabilidad |
| :---- | :---- | :---- |
| Presentación | SAP B1 SDK (UI API), C\# | Menú, formularios y eventos dentro del cliente nativo |
| Orquestación y negocio | C\# (.NET Framework 4.8) | Host modular, carga de módulos, reglas y validación |
| Datos SAP | Service Layer (REST/OData v4) | Lectura y escritura de objetos: órdenes, borradores, socios, artículos |
| Servicios en nube | Backend propio (ASP.NET Core) y modelo de visión | IA de visión, licenciamiento y medición de consumo |
| Persistencia de config. | Tablas y campos de usuario (UDT/UDF) en SAP | Configuración, mapeos, perfiles, bitácora y trazabilidad |

### **Punto clave de diseño: el backend como proxy de IA**

Las llamadas al modelo de visión no se hacen desde cada estación. El add-on envía la imagen a un backend propio que guarda la llave de la API, valida la cuota del perfil, llama al modelo, registra el evento de consumo para la facturación pospago y devuelve la interpretación. Esto resuelve en un solo lugar la seguridad de las credenciales, el control de licencias y la medición, y encaja con el modelo híbrido.

# **4\. Módulos del add-on**

## **4.1 Menú principal**

El add-on registra un menú de primer nivel en el cliente. De ese menú se despliegan las soluciones. La estructura inicial contempla dos, con espacio para crecer porque cada solución se implementa como un módulo independiente que se conecta al host común.

* **Menú raíz:** punto de entrada del add-on en el cliente nativo.

* **Solución 1:** Administrador de Configuraciones.

* **Solución 2:** Procesamiento de Imágenes con IA.

* **Futuras:** nuevas soluciones se enchufan al mismo host sin tocar las anteriores.

## **4.2 Módulo 1: Administrador de Configuraciones**

Centraliza la parametrización del add-on y el control de uso.

| Función | Descripción |
| :---- | :---- |
| **Permisos y licencias** | Asignación de perfil por usuario, visualización de cuota y consumo, estado de la cuenta pospago |
| **Selección de documento** | Definición del tipo de documento que produce el sistema (orden de venta, documento preliminar u otro) y mapeo de campos |
| **Alta de datos maestros** | Creación y edición de socios de negocio, artículos u otros maestros desde el add-on, vía Service Layer |
| **Uso por usuario** | Registro de qué usuario ejecuta cada operación y con qué frecuencia |
| **Personalidad del modelo** | Editor del mensaje de sistema (la personalidad) y parámetros del modelo (temperatura, versión), por empresa y por usuario |

## **4.3 Módulo 2: Procesamiento de Imágenes con IA**

Convierte imágenes en documentos de SAP con revisión del usuario.

* **Fuente configurable:** una sola imagen, o un repositorio (carpeta local, recurso de red o almacenamiento en nube).

* **Interpretación:** por cada imagen, el sistema obtiene una lectura estructurada del modelo de visión, guiada por las instrucciones configuradas.

* **Validación de requisitos:** antes de crear cualquier documento se revisa que se cumplan las reglas (campos obligatorios presentes, el artículo existe en SAP, el socio de negocio es válido, entre otras).

* **Formularios de revisión:** el usuario ve la interpretación, la ajusta si hace falta y confirma. En lote, cada imagen tiene un estado: pendiente, validada, creada o rechazada.

* **Creación del documento:** al confirmar, el add-on arma el JSON conforme a la estructura del documento y lo crea en SAP por Service Layer.

* **Trazabilidad:** se guarda la relación entre cada imagen y el documento generado.

# **5\. Flujo imagen a documento**

El recorrido completo, desde la selección de la imagen hasta la creación del documento, es el siguiente.

1. El usuario abre el módulo de imágenes y selecciona una imagen o un repositorio.

2. El add-on lee las imágenes y las envía, una a una, al backend.

3. El backend valida la cuota del perfil, llama al modelo de visión con las instrucciones configuradas y registra el evento de consumo.

4. El modelo devuelve una interpretación estructurada en formato JSON.

5. El add-on valida los requisitos básicos contra las reglas definidas y contra los datos de SAP (existencia de artículo y socio).

6. El formulario muestra la interpretación para revisión. El usuario corrige y confirma.

7. El add-on construye el documento y lo crea en SAP por Service Layer (orden de venta o documento preliminar).

8. Se actualiza el consumo del perfil y se guarda la trazabilidad imagen a documento.

**Nota sobre revisión humana.** Se recomienda mantener la revisión del usuario al menos en los perfiles Básico y Pro, para evitar documentos creados a partir de interpretaciones erróneas. La creación automática se puede habilitar de forma controlada cuando los requisitos se cumplan con alta confianza.

# **6\. Modelo de datos**

La configuración, los perfiles y la trazabilidad se guardan en tablas definidas por el usuario dentro de SAP. Estas se crean en la instalación mediante los puntos de metadatos del Service Layer (UserTablesMD, UserFieldsMD y UserObjectsMD), de modo que el runtime se mantiene por completo sobre Service Layer.

| Tabla (UDT) | Propósito | Campos clave (ejemplo) |
| :---- | :---- | :---- |
| **@LN\_CONFIG** | Configuración general del add-on | Clave, valor, ámbito (empresa o usuario) |
| **@LN\_PERFILES** | Asignación de perfil por usuario | Usuario, perfil, cuota, vigencia |
| **@LN\_DOCMAP** | Mapeo del documento a generar | Campo origen, campo destino, tipo de documento |
| **@LN\_PERSONA** | Instrucciones del modelo por ámbito | Ámbito, mensaje de sistema, parámetros |
| **@LN\_USOLOG** | Bitácora de uso y medición local | Usuario, operación, fecha, costo estimado |
| **@LN\_TRAZA** | Trazabilidad imagen a documento | Ruta o ID de imagen, tipo y número de documento, estado |

Los nombres de tabla son una propuesta para ajustar según la convención del equipo. El mismo esquema sostiene los reportes de uso y la conciliación del cobro pospago.

# **7\. Licenciamiento y cobro pospago**

El add-on define tres perfiles y se factura por consumo sobre una base mensual. La asignación de perfil se administra desde el módulo de configuración y el consumo se mide en el backend.

| Perfil | Alcance | Cuota o lote | Usuarios |
| :---- | :---- | :---- | :---- |
| **Básico** | Configuración y procesamiento de una imagen a la vez | Cuota mensual baja de documentos por IA | 1 usuario |
| **Pro** | Repositorio y procesamiento en lote, mapeos avanzados | Cuota media | Varios |
| **Enterprise** | Lote alto, reglas avanzadas, multiempresa, soporte prioritario | Cuota alta o sin tope con tarifa por consumo | Sin límite definido |

### **Cómo funciona el cobro pospago**

* **Unidad de cobro:** se define en la fase de diseño (por documento generado, por imagen procesada o por llamada al modelo).

* **Medición:** el backend registra cada evento facturable y consolida el consumo del mes.

* **Facturación:** al cierre del periodo se genera el reporte de consumo por empresa y por usuario.

**Nota.** Los límites y las tarifas son una propuesta inicial para ajustar según el modelo comercial.

# **8\. Stack técnico y estructura del proyecto**

| Componente | Tecnología |
| :---- | :---- |
| **Add-on (interfaz y lógica)** | C\# sobre .NET Framework 4.8 (requisito del SDK de SAP B1) |
| **Interfaz en el cliente** | SAP Business One SDK (UI API) |
| **Acceso a datos SAP** | HttpClient y System.Text.Json contra Service Layer (OData v4) |
| **IA de visión** | OpenAI o Azure OpenAI (recomendado Azure OpenAI por manejo de datos empresarial) |
| **Backend de IA, licencias y medición** | ASP.NET Core (.NET 8\) |
| **Almacén del backend** | Base de datos para consumo y facturación (PostgreSQL o SQL Server) |
| **Empaque e instalación** | Add-on Registration Data (.ard) e instalador del add-on |

### **Estructura de la solución (propuesta)**

| LN.AddOn.sln ├─ Host              · arranque, conexión al cliente, registro de menús ├─ Core              · contratos de módulo, ServiceLayerClient, AuthService, │                      ConfigService, LicenseClient, Logger ├─ Modules.Config    · formularios y lógica del Módulo 1 ├─ Modules.ImagenesIA· formularios y lógica del Módulo 2 ├─ Integrations.SL   · modelos tipados de los objetos de Service Layer └─ Setup             · creación de UDT/UDF/UDO por metadatos de Service Layer LN.Backend.sln (separado, en nube) ├─ Api               · IA de visión, licencias y medición └─ Billing           · consolidación de consumo pospago |
| :---- |

# **9\. Plan de implementación por fases**

El desarrollo se ordena en fases con un entregable concreto cada una. Las duraciones son estimaciones para un equipo pequeño y se ajustan según el alcance final.

| Fase | Objetivo | Entregable | Duración |
| :---- | :---- | :---- | :---- |
| 0 | Diseño y preparación | Especificación detallada, esquema de UDT/UDF, contrato de módulos, perfiles y eventos de medición | 1 a 2 sem |
| 1 | Esqueleto del add-on | Add-on instalable que arranca, registra el menú, conecta al cliente, autentica al Service Layer y crea los metadatos | 1 a 2 sem |
| 2 | Módulo de configuración | Formularios de configuración, perfiles y licencias, selección de documento, alta de maestros, editor de personalidad | 2 a 3 sem |
| 3 | IA y creación de documento | Backend de IA, interpretación estructurada, validación y creación de orden o documento preliminar para una imagen | 2 a 3 sem |
| 4 | Repositorio y lote | Manejo de múltiples imágenes, estados y formularios de revisión en lote | 1 a 2 sem |
| 5 | Licenciamiento y pospago | Backend de licencias y medición, tres perfiles, reportes de consumo y consolidación | 2 sem |
| 6 | QA, seguridad y rendimiento | Manejo de sesión del Service Layer, errores, seguridad de credenciales y pruebas | 1 a 2 sem |
| 7 | Empaque, instalador y piloto | Archivo .ard, instalador, documentación y puesta en marcha del piloto | 1 sem |

**Duración estimada total:** 12 a 18 semanas, según el alcance final y la disponibilidad del equipo.

# **10\. Riesgos y consideraciones**

| Riesgo o consideración | Mitigación |
| :---- | :---- |
| Sesión del Service Layer: vencimientos y ROUTEID en alta disponibilidad | Manejo central de sesión con reintento y renovación automática |
| Seguridad de la llave del modelo de IA | La llave vive solo en el backend, nunca en las estaciones |
| Costo por token de visión | Medición por imagen y cuotas por perfil, alineadas al cobro pospago |
| Documentos erróneos por mala interpretación | Revisión humana obligatoria en los perfiles base |
| Consistencia de datos maestros | Validación de existencia de artículo y socio antes de crear el documento |
| Actualización del add-on en muchas estaciones | Versionado y despliegue controlado del instalable |

# **11\. Decisiones por confirmar**

Estos puntos quedan abiertos y conviene cerrarlos en la fase de diseño, porque afectan la arquitectura y el modelo comercial.

1. Alcance de documento preliminar: confirmar que se refiere a los borradores de SAP (servicio Drafts) frente a una orden posteada.

2. Proveedor de IA: OpenAI directo o Azure OpenAI. Recomendación: Azure OpenAI por manejo de datos.

3. Unidad de cobro pospago: por documento generado, por imagen procesada o por llamada al modelo.

4. Fuente del repositorio de imágenes: carpeta local, recurso de red o almacenamiento en nube, para definir los conectores.

5. Nivel de automatización: revisión humana obligatoria o creación automática cuando se cumplen los requisitos.

6. Multiempresa: si una instalación atiende a varias bases o empresas de SAP.