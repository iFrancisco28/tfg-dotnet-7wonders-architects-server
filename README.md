# 7 Wonders Architects - Server (ASP.NET Core & SignalR)

![.NET](https://img.shields.io/badge/.NET-9.0-512BD4?style=for-the-badge&logo=dotnet)
![C#](https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=c-sharp&logoColor=white)
![SignalR](https://img.shields.io/badge/SignalR-00599C?style=for-the-badge&logo=microsoft&logoColor=white)
![Azure](https://img.shields.io/badge/Microsoft_Azure-0089D6?style=for-the-badge&logo=microsoft-azure&logoColor=white)

Servidor backend y API en tiempo real encargada de gestionar la lógica de emparejamiento, creación de salas y sincronización del estado para la adaptación digital multijugador del juego de mesa *7 Wonders Architects*.

## Arquitectura y Características Técnicas

Este proyecto opera como un Hub de comunicaciones P2P (Peer-to-Peer) de baja latencia. Sus principales características a nivel de ingeniería son:

* **Gestión de Concurrencia:** Implementación de colecciones Thread-Safe (`ConcurrentDictionary`) para el manejo seguro de las salas activas en memoria (`RoomInfo`). Garantiza la integridad de los datos frente a accesos y colisiones simultáneas desde múltiples hilos.
* **Comunicación P2P en Tiempo Real:** Uso de WebSockets a través de SignalR para la sincronización bidireccional entre el Host y el Visitante.
* **Pasarela de Estado:** El servidor actúa como intermediario para la distribución y actualización del `GameState` serializado en formato JSON.
* **Despliegue Cloud:** Alojamiento en Azure App Service, con políticas CORS explícitamente configuradas para permitir conexiones globales desde clientes MAUI.

## Métodos del Hub (`GameHub.cs`)

El servidor expone los siguientes métodos a través de SignalR:

| Método | Descripción | Retorno |
|--------|-------------|---------|
| `CreateRoom` | Registra una nueva sala y asigna un código único alfanumérico (ej. `A7X2`). | `string` (Código de la sala) |
| `JoinRoom` | Valida la existencia de la sala y evita la colisión de Maravillas elegidas. | `string` (`OK`, `NOT_FOUND`, `WONDER_TAKEN`) |
| `SendGameState` | Recibe el JSON con el estado de la partida y lo retransmite al rival. | `void` |
| `SendGameNotification`| Envía alertas (ej. resoluciones de guerra) a todos los miembros de la sala. | `void` |

## Despliegue Local

Para ejecutar el servidor en tu máquina local:

1. Clona este repositorio.
2. Abre la solución con Visual Studio 2022 o ejecuta en terminal: `dotnet build`.
3. Ejecuta el proyecto: `dotnet run`.
4. El servidor mapeará el endpoint principal en `/gamehub` y un test de salud en `/`.

---

## Aviso de Propiedad Intelectual y Uso Educativo

Este proyecto utiliza material gráfico, nombres y mecánicas basadas en el juego de mesa *7 Wonders Architects*, cuya propiedad intelectual pertenece a **Repos Production** y **Asmodee**. 

No se reclama ningún derecho sobre el material gráfico, diseño o marca registrada. Este proyecto es **exclusivamente académico y educativo**, no tiene ningún fin comercial y se publica públicamente de buena fe y únicamente como portafolio para demostrar habilidades de desarrollo de software, arquitectura de sistemas y programación concurrente.

## Licencia
El código fuente de este proyecto (excluyendo los assets y mecánicas mencionadas en el aviso superior) está bajo la Licencia MIT.
