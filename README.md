# 7Wonders-Architects-Server

Descripción:
Este repositorio contiene la API y el servidor en tiempo real encargado de gestionar la lógica de emparejamiento y sincronización de estado para la adaptación multijugador de 7 Wonders Architects.

Stack Técnico: .NET 9 | ASP.NET Core | SignalR | Azure App Service

Arquitectura y Soluciones Implementadas:

Gestión de Concurrencia: Implementación de ConcurrentDictionary para el manejo seguro de las salas activas en memoria, garantizando la integridad de los datos frente a accesos simultáneos desde múltiples hilos.

Comunicación P2P en Tiempo Real: Sincronización bidireccional de baja latencia mediante SignalR, operando como Hub central para la distribución y actualización del GameState en formato JSON.

Despliegue y Configuración: Alojamiento en Microsoft Azure con políticas de CORS configuradas explícitamente para permitir la conexión segura y sin restricciones de clientes multiplataforma.
