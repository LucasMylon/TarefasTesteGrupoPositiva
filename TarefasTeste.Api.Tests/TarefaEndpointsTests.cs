using System.Collections.Concurrent;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Tokens;
using TarefasTeste.Domain.Dtos;
using TarefasTeste.Domain.Entities;
using TarefasTeste.Domain.Interfaces.Repositories;
using TarefasTeste.Domain.Service;
using Xunit;

namespace TarefasTeste.Api.Tests;

public class TarefaEndpointsTests
{
    private const string TestEmail = "usuario@example.com";
    private static readonly byte[] TestKey = Enumerable.Range(1, 32).Select(value => (byte)value).ToArray();

    [Fact]
    public async Task Criar_tarefa_retorna_created_e_dados_da_tarefa()
    {
        using var factory = new TarefaApiFactory();
        using var client = factory.CreateClient();
        var request = new TarefaRequest
        {
            Titulo = "Estudar xUnit",
            Descricao = "Criar testes de integração",
            Status = false
        };

        var inicio = DateTime.UtcNow;
        var response = await client.PostAsJsonAsync("/api/tarefa", request);
        var fim = DateTime.UtcNow;

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var tarefa = await response.Content.ReadFromJsonAsync<TarefaResponse>();
        Assert.NotNull(tarefa);
        Assert.NotEqual(Guid.Empty, tarefa.Id);
        Assert.Equal(request.Titulo, tarefa.Titulo);
        Assert.Equal(request.Descricao, tarefa.Descricao);
        Assert.False(tarefa.Status);
        Assert.Equal(DateTimeKind.Utc, tarefa.DataHoraCriacao.Kind);
        Assert.InRange(tarefa.DataHoraCriacao, inicio, fim);
        Assert.Equal(tarefa.DataHoraCriacao, Assert.Single(factory.Repository.GetAll()).DataHoraCriacao);
        Assert.EndsWith($"/api/Tarefa/{tarefa.Id}", response.Headers.Location!.ToString());
    }

    [Fact]
    public async Task Data_enviada_pelo_cliente_nao_substitui_a_data_gerada_pelo_servidor()
    {
        using var factory = new TarefaApiFactory();
        using var client = factory.CreateClient();
        var inicio = DateTime.UtcNow;

        var response = await client.PostAsJsonAsync("/api/tarefa", new
        {
            Titulo = "Tarefa",
            Descricao = "Descrição",
            Status = false,
            DataHoraCriacao = new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        });
        var fim = DateTime.UtcNow;

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var criada = await response.Content.ReadFromJsonAsync<TarefaResponse>();
        Assert.NotNull(criada);
        Assert.InRange(criada.DataHoraCriacao, inicio, fim);
        var consultada = await client.GetFromJsonAsync<TarefaResponse>($"/api/tarefa/{criada.Id}");
        Assert.NotNull(consultada);
        Assert.Equal(criada.DataHoraCriacao, consultada.DataHoraCriacao);
        Assert.Equal(DateTimeKind.Utc, consultada.DataHoraCriacao.Kind);
    }

    [Fact]
    public async Task Obter_todas_as_tarefas_retorna_lista_cadastrada()
    {
        using var factory = new TarefaApiFactory();
        using var client = factory.CreateClient();
        factory.Repository.Add(new Tarefa
        {
            Titulo = "Tarefa de teste",
            Descricao = "Descrição",
            Status = false,
            DataHoraCriacao = new DateTime(2026, 1, 2, 3, 4, 5, DateTimeKind.Utc)
        });

        var tarefas = await client.GetFromJsonAsync<List<TarefaResponse>>("/api/tarefa");

        var tarefa = Assert.Single(tarefas!);
        Assert.Equal("Tarefa de teste", tarefa.Titulo);
        Assert.Equal("Descrição", tarefa.Descricao);
        Assert.False(tarefa.Status);
        Assert.Equal(new DateTime(2026, 1, 2, 3, 4, 5, DateTimeKind.Utc), tarefa.DataHoraCriacao);
        Assert.Equal(DateTimeKind.Utc, tarefa.DataHoraCriacao.Kind);
    }

    [Fact]
    public async Task Obter_tarefa_por_id_retorna_tarefa_cadastrada()
    {
        using var factory = new TarefaApiFactory();
        using var client = factory.CreateClient();
        var cadastrada = new Tarefa
        {
            Titulo = "Tarefa específica",
            Descricao = "Buscar por identificador",
            Status = true,
            DataHoraCriacao = new DateTime(2026, 2, 3, 4, 5, 6, DateTimeKind.Utc)
        };
        factory.Repository.Add(cadastrada);

        var tarefa = await client.GetFromJsonAsync<TarefaResponse>($"/api/tarefa/{cadastrada.Id}");

        Assert.NotNull(tarefa);
        Assert.Equal(cadastrada.Id, tarefa.Id);
        Assert.Equal(cadastrada.Titulo, tarefa.Titulo);
        Assert.Equal(cadastrada.Descricao, tarefa.Descricao);
        Assert.True(tarefa.Status);
        Assert.Equal(cadastrada.DataHoraCriacao, tarefa.DataHoraCriacao);
        Assert.Equal(DateTimeKind.Utc, tarefa.DataHoraCriacao.Kind);
    }

    [Fact]
    public async Task Excluir_tarefa_pendente_retorna_no_content_e_remove_tarefa()
    {
        using var factory = new TarefaApiFactory();
        using var client = factory.CreateClient();
        var cadastrada = new Tarefa
        {
            Titulo = "Tarefa pendente",
            Descricao = "Pode ser excluída",
            Status = false
        };
        factory.Repository.Add(cadastrada);

        var response = await client.DeleteAsync($"/api/tarefa/{cadastrada.Id}");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        Assert.Empty(factory.Repository.GetAll());
    }

    [Fact]
    public async Task Excluir_tarefa_concluida_retorna_bad_request_e_preserva_tarefa()
    {
        using var factory = new TarefaApiFactory();
        using var client = factory.CreateClient();
        var cadastrada = new Tarefa
        {
            Titulo = "Tarefa concluída",
            Descricao = "Não pode ser excluída",
            Status = true
        };
        factory.Repository.Add(cadastrada);

        var response = await client.DeleteAsync($"/api/tarefa/{cadastrada.Id}");
        var mensagem = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Contains("Não é permitido excluir uma tarefa com status concluído (true).", mensagem);
        var tarefaPreservada = Assert.Single(factory.Repository.GetAll());
        Assert.Equal(cadastrada.Id, tarefaPreservada.Id);
        Assert.True(tarefaPreservada.Status);
    }

    [Fact]
    public async Task Atualizar_tarefa_por_endpoint_preserva_data_e_atualiza_status()
    {
        using var factory = new TarefaApiFactory();
        using var client = factory.CreateClient();
        var cadastrada = new Tarefa
        {
            Titulo = "Original",
            Descricao = "Descrição original",
            DataHoraCriacao = new DateTime(2020, 1, 2, 3, 4, 5, DateTimeKind.Utc)
        };
        factory.Repository.Add(cadastrada);

        var response = await client.PutAsJsonAsync($"/api/tarefa/{cadastrada.Id}", new TarefaRequest
        {
            Titulo = "Atualizada",
            Descricao = "Nova descrição",
            Status = true
        });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var atualizada = await response.Content.ReadFromJsonAsync<TarefaResponse>();
        Assert.NotNull(atualizada);
        Assert.Equal(cadastrada.Id, atualizada.Id);
        Assert.Equal("Atualizada", atualizada.Titulo);
        Assert.Equal("Nova descrição", atualizada.Descricao);
        Assert.True(atualizada.Status);
        Assert.Equal(cadastrada.DataHoraCriacao, atualizada.DataHoraCriacao);
        var consultada = await client.GetFromJsonAsync<TarefaResponse>($"/api/tarefa/{cadastrada.Id}");
        Assert.NotNull(consultada);
        Assert.Equal("Atualizada", consultada.Titulo);
        Assert.True(consultada.Status);
    }

    [Fact]
    public async Task Atualizar_tarefa_rejeita_id_inexistente_e_titulo_vazio()
    {
        using var factory = new TarefaApiFactory();
        using var client = factory.CreateClient();
        var cadastrada = new Tarefa { Titulo = "Original" };
        factory.Repository.Add(cadastrada);

        var inexistente = await client.PutAsJsonAsync($"/api/tarefa/{Guid.NewGuid()}",
            new TarefaRequest { Titulo = "Atualizada" });
        var tituloVazio = await client.PutAsJsonAsync($"/api/tarefa/{cadastrada.Id}",
            new TarefaRequest { Titulo = "  " });

        Assert.Equal(HttpStatusCode.NotFound, inexistente.StatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, tituloVazio.StatusCode);
        Assert.Equal("Original", Assert.Single(factory.Repository.GetAll()).Titulo);
    }

    [Fact]
    public void Atualizar_tarefa_preserva_data_original()
    {
        var repository = new InMemoryTarefaRepository();
        var cadastrada = new Tarefa
        {
            Titulo = "Original",
            Descricao = "Descrição original",
            DataHoraCriacao = new DateTime(2020, 1, 2, 3, 4, 5, DateTimeKind.Utc)
        };
        repository.Add(cadastrada);
        var service = new TarefaService(repository);

        var resultado = service.UpdateTarefa(cadastrada.Id, new TarefaRequest
        {
            Titulo = "Atualizada",
            Descricao = "Nova descrição",
            Status = true
        }, TestEmail);

        Assert.Equal(cadastrada.DataHoraCriacao, resultado.DataHoraCriacao);
        var atualizada = Assert.Single(repository.GetAll());
        Assert.Equal(cadastrada.DataHoraCriacao, atualizada.DataHoraCriacao);
        Assert.Equal("Atualizada", atualizada.Titulo);
        Assert.Equal("Nova descrição", atualizada.Descricao);
        Assert.True(atualizada.Status);
    }

    [Fact]
    public void Atualizar_tarefa_inexistente_retorna_erro()
    {
        var service = new TarefaService(new InMemoryTarefaRepository());

        Assert.Throws<KeyNotFoundException>(() => service.UpdateTarefa(Guid.NewGuid(), new TarefaRequest(), TestEmail));
    }

    [Fact]
    public void Excluir_tarefa_retorna_data_original_no_servico()
    {
        var repository = new InMemoryTarefaRepository();
        var cadastrada = new Tarefa
        {
            Titulo = "Pendente",
            Descricao = "Descrição",
            DataHoraCriacao = new DateTime(2020, 1, 2, 3, 4, 5, DateTimeKind.Utc)
        };
        repository.Add(cadastrada);

        var resultado = new TarefaService(repository).DeleteTarefa(cadastrada.Id, TestEmail);

        Assert.Equal(cadastrada.DataHoraCriacao, resultado.DataHoraCriacao);
        Assert.Empty(repository.GetAll());
    }

    [Fact]
    public async Task Tarefas_sao_restritas_ao_proprietario_e_registros_legados_nao_sao_expostos()
    {
        using var factory = new TarefaApiFactory();
        using var owner = factory.CreateClient();
        using var other = factory.CreateClientFor("outro@example.com");
        using var anonymous = factory.CreateAnonymousClient();
        var legacy = new Tarefa { Titulo = "Legada", Descricao = "Sem dono" };
        factory.Repository.AddLegacy(legacy);

        var created = await owner.PostAsJsonAsync("/api/tarefa", new TarefaRequest { Titulo = "Privada", Descricao = "Descrição" });
        Assert.Equal(HttpStatusCode.Created, created.StatusCode);
        var task = await created.Content.ReadFromJsonAsync<TarefaResponse>();
        Assert.NotNull(task);
        Assert.Equal(TestEmail, task.UsuarioEmail);
        Assert.Equal(TestEmail, factory.Repository.GetAll().Single(t => t.Id == task.Id).UsuarioEmail);
        Assert.Single((await owner.GetFromJsonAsync<List<TarefaResponse>>("/api/tarefa"))!);
        Assert.Empty((await other.GetFromJsonAsync<List<TarefaResponse>>("/api/tarefa"))!);
        Assert.Equal(HttpStatusCode.Unauthorized, (await anonymous.GetAsync("/api/tarefa")).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await other.GetAsync($"/api/tarefa/{task.Id}")).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await owner.GetAsync($"/api/tarefa/{legacy.Id}")).StatusCode);
        var update = new TarefaRequest { Titulo = "Alterada", Descricao = "Descrição" };
        Assert.Equal(HttpStatusCode.NotFound, (await other.PutAsJsonAsync($"/api/tarefa/{task.Id}", update)).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await owner.PutAsJsonAsync($"/api/tarefa/{legacy.Id}", update)).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await other.DeleteAsync($"/api/tarefa/{task.Id}")).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await owner.DeleteAsync($"/api/tarefa/{legacy.Id}")).StatusCode);
        Assert.Equal("Privada", factory.Repository.GetAll().Single(t => t.Id == task.Id).Titulo);
        Assert.Equal(2, factory.Repository.GetAll().Count);
    }

    private sealed class TarefaApiFactory : WebApplicationFactory<global::Program>
    {
        private static readonly object ConfigurationLock = new();
        public InMemoryTarefaRepository Repository { get; } = new();

        public new HttpClient CreateClient() => CreateClientFor(TestEmail);

        public HttpClient CreateAnonymousClient() => CreateClientWithTestConfiguration();

        private HttpClient CreateClientWithTestConfiguration()
        {
            lock (ConfigurationLock)
            {
                var previousKey = Environment.GetEnvironmentVariable("JWT_SECRET_KEY");
                try
                {
                    Environment.SetEnvironmentVariable("JWT_SECRET_KEY", Convert.ToBase64String(TestKey));
                    return base.CreateClient();
                }
                finally
                {
                    Environment.SetEnvironmentVariable("JWT_SECRET_KEY", previousKey);
                }
            }
        }

        public HttpClient CreateClientFor(string email)
        {
            var client = CreateClientWithTestConfiguration();
            var token = new JwtSecurityToken(
                issuer: "Usuarios.App.API",
                audience: "Usuarios.App.Clients",
                claims: new[] { new Claim(ClaimTypes.Name, email) },
                expires: DateTime.UtcNow.AddMinutes(10),
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(TestKey), SecurityAlgorithms.HmacSha256));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", new JwtSecurityTokenHandler().WriteToken(token));
            return client;
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureAppConfiguration((_, configuration) => configuration.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["JwtSettings:SecretKey"] = Convert.ToBase64String(TestKey),
                ["JwtSettings:Issuer"] = "Usuarios.App.API",
                ["JwtSettings:Audience"] = "Usuarios.App.Clients"
            }));
            builder.ConfigureServices(services =>
            {
                services.RemoveAll<ITarefaRepository>();
                services.AddSingleton<ITarefaRepository>(Repository);
                services.PostConfigure<Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerOptions>(
                    Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme,
                    options => options.TokenValidationParameters.IssuerSigningKey = new SymmetricSecurityKey(TestKey));
            });
        }
    }

    private sealed class InMemoryTarefaRepository : ITarefaRepository
    {
        private readonly ConcurrentDictionary<Guid, Tarefa> _tarefas = new();

        public void Add(Tarefa tarefa)
        {
            tarefa.UsuarioEmail ??= TestEmail;
            _tarefas[tarefa.Id] = Clone(tarefa);
        }

        public void AddLegacy(Tarefa tarefa) => _tarefas[tarefa.Id] = Clone(tarefa);

        public void Update(Tarefa tarefa) => _tarefas[tarefa.Id] = Clone(tarefa);

        public void Delete(Guid id) => _tarefas.TryRemove(id, out _);

        public List<Tarefa> GetAll() => _tarefas.Values.Select(Clone).ToList();

        private static Tarefa Clone(Tarefa tarefa) => new()
        {
            Id = tarefa.Id,
            Titulo = tarefa.Titulo,
            Descricao = tarefa.Descricao,
            Status = tarefa.Status,
            DataHoraCriacao = tarefa.DataHoraCriacao,
            UsuarioEmail = tarefa.UsuarioEmail
        };
    }
}
