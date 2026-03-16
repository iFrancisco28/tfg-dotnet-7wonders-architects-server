using TFG_FranciscoCarreroCarrero_7WondersArchitects_Server.Hubs;

// constructor para app
var builder = WebApplication.CreateBuilder(args);

//configuro api//
// habilitamos signalR como servicio
builder.Services.AddSignalR();
// habilitamos controladores de API estandar
builder.Services.AddControllers();
//creo el servidor(api) con todo configurado
var app = builder.Build();

//middleware, permisos y seguridad
app.UseAuthorization();
app.MapControllers();

// ruta de acceso
app.MapHub<GameHub>("/gamehub");

// para hacer ping
app.MapGet("/", () => "7 Wonders Server Online");

app.Run();