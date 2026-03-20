using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;

namespace TFG_FranciscoCarreroCarrero_7WondersArchitects_Server.Hubs {
    public class RoomInfo {
        public string HostName { get; set; }
        public string HostWonder { get; set; }
    }
    public class GameHub : Hub {
        //un diccionario al que pueden acceder desde distintos hilos a la vez
        private static readonly ConcurrentDictionary<string, RoomInfo> SalasActivas = new();

        //creamos sala devolvemos codigo
        public async Task<string> CreateRoom(string hostName, string hostWonder) {
            //creo el codigo de sala (le quito los guiones, cojo 4 y a mayusculas)
            string code = Guid.NewGuid().ToString("N").Substring(0, 4).ToUpper();

            //registramos sala
            var roomInfo = new RoomInfo { HostName = hostName, HostWonder = hostWonder };
            SalasActivas.TryAdd(code, roomInfo);

            //metemos al creador en el grupo de signalR
            await Groups.AddToGroupAsync(Context.ConnectionId, code);

            return code;
        }

        //intentamos unirnos, false si no existe la sala pasada por args
        public async Task<string> JoinRoom(string roomCode, string guestName, string guestWonder) {
            string code = roomCode.ToUpper();
            
            if (SalasActivas.TryGetValue(code, out RoomInfo room)) {

                //si la maravilla esta repetida
                if (room.HostWonder == guestWonder) {
                    return "WONDER_TAKEN";
                }

                //si el codigo existe unimos al usuario al grupo
                await Groups.AddToGroupAsync(Context.ConnectionId, code);

                //y se avisa
                await Clients.Group(code).SendAsync("ReceiveMessage", "Sistema", $"{guestName} esta listo para construir {guestWonder}");
                
                await Clients.OthersInGroup(code).SendAsync("PlayerJoined", guestName, guestWonder);

                return "OK";
            }

            return "NOT_FOUND"; //sala no encontrada, el jugador no se une
        }

        //pasar gameState al otro jugador
        public async Task SendGameState(string roomCode, string jsonState) {
            // "OthersInGroup" asegura que el JSON le llega al rival, no a ti mismo
            await Clients.OthersInGroup(roomCode.ToUpper()).SendAsync("ReceiveGameState", jsonState);
        }

        // metodo generico
        public async Task SendMessageToRoom(string roomCode, string message) {
            //clients es todos los miembros de Groups
            await Clients.Group(roomCode.ToUpper()).SendAsync("ReceiveMessage", message);
        }
    }
}