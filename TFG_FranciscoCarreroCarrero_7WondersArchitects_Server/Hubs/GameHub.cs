using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;

namespace TFG_FranciscoCarreroCarrero_7WondersArchitects_Server.Hubs {
    public class GameHub : Hub {
        //un diccionario al que pueden acceder desde distintos hilos a la vez
        private static readonly ConcurrentDictionary<string, bool> SalasActivas = new();

        //creamos sala devolvemos codigo
        public async Task<string> CreateRoom() {
            //creo el codigo de sala (le quito los guiones, cojo 4 y a mayusculas)
            string code = Guid.NewGuid().ToString("N").Substring(0, 4).ToUpper();

            //registramos sala
            SalasActivas.TryAdd(code, true);

            //metemos al creador en el grupo de signalR
            await Groups.AddToGroupAsync(Context.ConnectionId, code);

            return code;
        }

        //intentamos unirnos, false si no existe la sala pasada por args
        public async Task<bool> JoinRoom(string roomCode) {
            string code = roomCode.ToUpper();
            
            if (SalasActivas.ContainsKey(code)) {
                //si el codigo existe unimos al usuario al grupo
                await Groups.AddToGroupAsync(Context.ConnectionId, code);

                //y se avisa
                await Clients.Group(code).SendAsync("ReceiveMessage", "Sistema", "Un nuevo jugador se ha unido a la sala");

                return true;
            }

            return false; //sala no encontrada, el jugador no se une
        }

        // metodo generico
        public async Task SendMessageToRoom(string roomCode, string message) {
            //clients es todos los miembros de Groups
            await Clients.Group(roomCode.ToUpper()).SendAsync("ReceiveMessage", message);
        }
    }
}