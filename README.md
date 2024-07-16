Esto es un repositorio Asp.net core API 8 con service worker de prueba que esta hecho con .net 8.

El Worker Service se ejecuta en segundo plano y pueda manejar tareas de procesamiento de órdenes (pago o cobranzas).

## Estado del Sistema
Accede en /health-check para verificar si el api esta funcionando correctamente.

## Ejecución
- Configurar la cadena de base de datos en los proyectos `Order__processing` y `WorkerService`.
- Ejecutar la migración en el proyecto `Database`.
- Ejecutar el proyecto `Order__processing` para la API.
- Ejecutar el proyecto `WorkerService` para la service worker (Tareas en segundo plano).
- Registrate en /register y Conectate en /login para recuperar el token.
- Post en /Orders para registrar un Order con estado `Pendiente` `{
  "status": "Pendiente"
}`.
- Controlar el número de órdenes a procesar en paralelo poniendo la configuración `tareasMax` en `Appsettings.json` del proyecto `WorkerService`.