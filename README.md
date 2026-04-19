# 7 Wonders Architects - Game Server (ASP.NET Core & SignalR)

![.NET](https://img.shields.io/badge/.NET-9.0-512BD4?style=for-the-badge&logo=dotnet)
![C#](https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=c-sharp&logoColor=white)
![SignalR](https://img.shields.io/badge/SignalR-00599C?style=for-the-badge&logo=microsoft&logoColor=white)
![Azure](https://img.shields.io/badge/Microsoft_Azure-0089D6?style=for-the-badge&logo=microsoft-azure&logoColor=white)

Servidor backend desarrollado en ASP.NET Core 9 para orquestar las conexiones y el flujo de datos del cliente multiplataforma de *7 Wonders Architects*. Su diseño se centra en la baja latencia, la gestión segura del estado en un entorno concurrente y la escalabilidad de sesiones de juego aisladas mediante WebSockets.

## Arquitectura y Lógica de Concurrencia

El servidor no calcula las mecánicas físicas del juego (las cuales residen en el cliente de MAUI para minimizar la carga computacional del backend), sino que actúa como un nodo de orquestación y validación transaccional altamente optimizado.

### 1. Gestión de Memoria Segura (Thread-Safety)
En una API estándar de ASP.NET, múltiples peticiones HTTP o de WebSockets pueden intentar leer o escribir el estado simultáneamente desde hilos distintos. Para mitigar colisiones y corrupciones en memoria (*Race Conditions*), el estado global de las partidas se almacena en un diccionario concurrente:
```csharp
private static readonly ConcurrentDictionary<string, RoomInfo> SalasActivas = new();
```
Esto garantiza que la creación de salas (`TryAdd`) y la lectura del estado (`TryGetValue`) sean operaciones atómicas y bloqueantes a nivel de hilo durante el proceso de *Matchmaking*.

### 2. Aislamiento de Red mediante SignalR Groups
Para garantizar que el intercambio del voluminoso JSON del `GameState` sea seguro y privado, el servidor prescinde del *broadcasting* global. Al instanciar una sala mediante la generación de un token GUID (ej. `A7X2`), los clientes se asocian lógicamente a un grupo nativo de SignalR:
```csharp
await Groups.AddToGroupAsync(Context.ConnectionId, code);
```
Cuando un jugador finaliza su turno, la actualización del estado se enruta exclusivamente a la conexión activa del oponente usando `Clients.OthersInGroup(roomCode)`, minimizando la latencia y el ancho de banda.

### 3. Validaciones de Integridad del Lobby
El Hub implementa reglas de validación pre-partida antes de permitir la unión a un grupo (`JoinRoom`). El servidor analiza la clase de datos `RoomInfo` para asegurar que el jugador invitado (*Guest*) no pueda seleccionar una Maravilla ya instanciada por el anfitrión (*Host*), devolviendo códigos de respuesta transaccionales (`WONDER_TAKEN`, `NOT_FOUND`, `OK`).

### 4. Políticas CORS y Despliegue en la Nube
El clúster está configurado específicamente en la canalización (`Program.cs`) para integrarse sin fricción con clientes nativos distribuidos. Se han establecido políticas explícitas (`AllowAnyMethod`, `AllowAnyHeader`, `SetIsOriginAllowed`, `AllowCredentials`) para garantizar que la conexión persistente por WebSockets a través de **Azure App Service** no sea bloqueada por los protocolos de seguridad entre dominios.

## Métodos del Endpoint (`/gamehub`)

El Hub principal expone las siguientes invocaciones P2P:

| Método RPC | Comportamiento en Servidor |
|------------|----------------------------|
| `CreateRoom` | Registra el host, instancia la maravilla solicitada, inicializa el `ConcurrentDictionary` y devuelve un código GUID recortado. |
| `JoinRoom` | Evalúa la existencia de la sala y ejecuta verificaciones de colisión de entidad. Si tiene éxito, empareja a los jugadores mediante SignalR Groups y alerta al Host de la unión. |
| `SendGameState` | Actúa como túnel pasarela. Recibe el payload JSON del estado del juego local y lo inyecta a través de la red al cliente remoto. |
| `SendGameNotification`| Protocolo de alerta global utilizado para notificaciones asíncronas dentro de la sala (ej. resultados de resoluciones militares y avisos). |

## Despliegue Local

1.  Asegúrate de tener instalado el **SDK de .NET 9**.
2.  Clona este repositorio y restaura las dependencias mediante CLI: `dotnet restore`.
3.  Ejecuta el proyecto: `dotnet run`.
4.  El servidor inicializará el middleware y levantará el endpoint de WebSockets en `/gamehub` y un test de salud HTTP en la raíz `/`.

---

## Aviso de Propiedad Intelectual y Uso Educativo

Este proyecto hace referencia a mecánicas y nombres basados en el juego de mesa *7 Wonders Architects*, cuya propiedad intelectual pertenece a **Repos Production** y **Asmodee**. 

Este proyecto es **exclusivamente académico y educativo**, no tiene ningún fin comercial y se publica públicamente de buena fe y únicamente como portafolio para demostrar habilidades avanzadas de desarrollo de backend, concurrencia en C# y sincronización distribuida con SignalR.

## Licencia
El código fuente de este proyecto está bajo la Licencia MIT.
