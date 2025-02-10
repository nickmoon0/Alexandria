using Alexandria.FileApi;

var builder = WebApplication.CreateBuilder(args);

builder.AddServices();

var app = builder.Build();

app.AddApp();
app.Run();