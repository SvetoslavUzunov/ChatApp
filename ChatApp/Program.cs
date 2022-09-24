using ChatApp;
using ChatApp.Configurations;
using ChatApp.Hubs;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSignalR();

builder.Services.AddConfigurations();

builder.Services.AddSingleton<IDictionary<string, UserConnection>>(options => new Dictionary<string, UserConnection>());

var app = builder.Build();

app.UseCors();

app.MapHub<ChatHub>("/chat");

app.Run();
