using TarefasTeste.Domain.Interfaces;
using TarefasTeste.Domain.Service;
using TarefasTeste.Domain.Interfaces.Repositories;
using TarefasApp.Infra.Data.Repositories;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddScoped<ITarefaService, TarefaService>();
builder.Services.AddScoped<ITarefaRepository, TarefaRepository>();

builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();

/// <summary>Entry point exposed for API integration tests.</summary>
public partial class Program { }
