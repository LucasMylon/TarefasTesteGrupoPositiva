using TarefasTeste.Domain.Interfaces;
using TarefasTeste.Domain.Service;
using TarefasTeste.Domain.Interfaces.Repositories;
using TarefasApp.Infra.Data.Repositories;
using System.Reflection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using TarefasTeste.Domain.Settings;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddScoped<ITarefaService, TarefaService>();
builder.Services.AddScoped<ITarefaRepository, TarefaRepository>();

builder.Services.AddOpenApi();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new()
    {
        Title = "API de Tarefas",
        Version = "v1",
        Description = "API para gerenciamento de tarefas com autenticação JWT Bearer. Todos os endpoints requerem um token JWT válido."
    });

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }
});

// Carregar configurações JWT
var jwtSettings = builder.Configuration
    .GetRequiredSection("JwtSettings")
    .Get<JwtSettings>()!;

// Obter SecretKey da configuração (incluindo User Secrets) ou da variável de ambiente
var secretKey = builder.Configuration["JwtSettings:SecretKey"] 
    ?? Environment.GetEnvironmentVariable("JWT_SECRET_KEY")
    ?? throw new InvalidOperationException(
        "JWT_SECRET_KEY não foi configurada. Configure via variável de ambiente ou User Secrets.");

// Configurar autenticação JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Convert.FromBase64String(secretKey)),

            ValidateIssuer = true,
            ValidIssuer = jwtSettings.Issuer,

            ValidateAudience = true,
            ValidAudience = jwtSettings.Audience,

            ValidateLifetime = true,
            RequireExpirationTime = true,
            ClockSkew = TimeSpan.Zero
        };

        // Configurar para aceitar token apenas no header Authorization
        options.Events = new JwtBearerEvents
        {
            OnChallenge = context =>
            {
                context.HandleResponse();
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Response.ContentType = "application/json";
                return context.Response.WriteAsJsonAsync(new
                {
                    error = "Unauthorized",
                    message = "Token inválido, expirado ou ausente. Use o header: Authorization: Bearer <token>"
                });
            },
            OnForbidden = context =>
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                context.Response.ContentType = "application/json";
                return context.Response.WriteAsJsonAsync(new
                {
                    error = "Forbidden",
                    message = "Você não possui permissão para acessar este recurso."
                });
            }
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "API de Tarefas v1");
        c.DisplayOperationId();
    });
}

// Middleware de autenticação deve vir ANTES de autorização
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

try
{
    app.Run();
}
catch (Exception ex)
{
    Console.WriteLine($"❌ ERRO NA APLICAÇÃO: {ex.Message}");
    Console.WriteLine($"Stack: {ex.StackTrace}");
}

/// <summary>Entry point exposed for API integration tests.</summary>
public partial class Program { }
