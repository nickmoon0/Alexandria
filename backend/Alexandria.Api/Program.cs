using Alexandria.Api.Infrastructure;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Register EF Core
var sqlConnString = builder.Configuration.GetConnectionString(nameof(AppDbContext));
var serverVersion = ServerVersion.AutoDetect(sqlConnString);
builder.Services.AddDbContext<AppDbContext>(options => 
    options.UseMySql(sqlConnString, serverVersion));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.Run();