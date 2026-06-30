# Smoke test — Entregable Fase 1 (Paso 8)

Checklist de verificación del esqueleto del add-on. Se ejecuta en una estación con
**SAP Business One 10.0 + SDK** instalados y acceso al **Service Layer**.

> Objetivo de la Fase 1: un add-on instalable que arranca, registra el menú, conecta al
> cliente, autentica al Service Layer y crea los metadatos.

## Pre-requisitos
- [ ] Cliente SAP B1 10.0 instalado y con sesión abierta en una compañía de prueba.
- [ ] Variable `SAPB1_SDK_PATH` apuntando a los interop del SDK.
- [ ] Service Layer accesible (`https://<host>:50000/b1s/v2/`) y credenciales de prueba.
- [ ] Backend local corriendo (`docker compose up`) y respondiendo en `/health` *(opcional para Fase 1)*.

## Build y despliegue
- [ ] `LN.AddOn.sln` compila en Release sin errores.
- [ ] `install.ps1` copia binarios y genera el `LN.AddOn.ard`.
- [ ] El `.ard` se registra en *Administración > Add-Ons > Administración de add-ons*.

## Verificación funcional
- [ ] El add-on **arranca** al iniciar el cliente (o al asignarlo manualmente).
- [ ] Aparece el menú raíz **"Add-on LaNeta"** bajo *Módulos*.
- [ ] Cuelgan los dos submenús: **Administrador de Configuraciones** y **Procesamiento de Imágenes con IA**.
- [ ] Al hacer clic en cada submenú no se produce excepción (los formularios aún son stub: Fase 2/3).
- [ ] El add-on **autentica** al Service Layer (login correcto; sesión y ROUTEID en log).
- [ ] `MetadataInstaller` crea las **6 UDT `@LN_*`** y sus UDF (verificar en SAP).
- [ ] Re-ejecutar el add-on **no duplica** metadatos (idempotencia).

## Evidencia / log
- [ ] Revisar `%LocalAppData%\LN.AddOn\logs\addon-*.log`: arranque, módulos registrados, login SL, metadatos.
- [ ] Capturas del menú y de las UDT creadas.

## Resultado
- [ ] **Fase 1 aceptada** cuando todos los puntos anteriores pasan.
