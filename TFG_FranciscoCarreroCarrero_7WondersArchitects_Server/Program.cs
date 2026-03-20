using TFG_FranciscoCarreroCarrero_7WondersArchitects_Server.Hubs;

// constructor para app
var builder = WebApplication.CreateBuilder(args);


//azure
builder.Services.AddCors(options => {
    options.AddDefaultPolicy(policy => {
        policy.AllowAnyMethod()
              .AllowAnyHeader()
              .SetIsOriginAllowed(origin => true) // para que MAUI conecte desde cualquier sitio
              .AllowCredentials();               // para SignalR 
    });
});




//configuro api//
// habilitamos signalR como servicio
builder.Services.AddSignalR();
// habilitamos controladores de API estandar
builder.Services.AddControllers();
//creo el servidor(api) con todo configurado
var app = builder.Build();


//activo cors por azure
app.UseCors();


//middleware, permisos y seguridad
app.UseAuthorization();
app.MapControllers();

// ruta de acceso
app.MapHub<GameHub>("/gamehub");

// para hacer ping
app.MapGet("/", () => "7 Wonders Server Online");

app.Run();