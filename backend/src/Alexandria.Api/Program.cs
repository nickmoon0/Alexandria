using Alexandria.Api;

var builder = WebApplication.CreateBuilder(args);

builder.AddServices();

var app = builder.Build();

app.AddApp();
app.Run();