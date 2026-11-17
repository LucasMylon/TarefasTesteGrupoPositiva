using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using TarefasTeste.Domain.Entities;
using TarefasTeste.Infra.Data.Contexts;
using Xunit;

namespace TarefasTeste.Api.Tests;

public class DataHoraCriacaoMappingTests
{
    [Fact]
    public void Mapeamento_restaura_kind_utc_ao_ler_do_sql_server()
    {
        using var context = new DataContext();
        var entity = context.Model.FindEntityType(typeof(Tarefa));
        Assert.NotNull(entity);
        var property = entity.FindProperty(nameof(Tarefa.DataHoraCriacao));
        Assert.NotNull(property);
        Assert.False(property.IsNullable);
        Assert.Equal("DATAHORACRIACAO", property.GetColumnName());
        Assert.Equal("datetime2", property.GetColumnType());
        var converter = property.GetTypeMapping().Converter;
        Assert.NotNull(converter);
        var valorDoBanco = new DateTime(2020, 1, 2, 3, 4, 5, DateTimeKind.Unspecified);

        var valorLido = Assert.IsType<DateTime>(converter.ConvertFromProvider(valorDoBanco));

        Assert.Equal(valorDoBanco.Ticks, valorLido.Ticks);
        Assert.Equal(DateTimeKind.Utc, valorLido.Kind);
    }

    [Fact]
    public void Proprietario_e_persistido_em_coluna_nullable_para_tarefas_legadas()
    {
        using var context = new DataContext();
        var entity = context.Model.FindEntityType(typeof(Tarefa));
        Assert.NotNull(entity);
        var property = entity.FindProperty(nameof(Tarefa.UsuarioEmail));
        Assert.NotNull(property);
        Assert.True(property.IsNullable);
        Assert.Equal("USUARIOEMAIL", property.GetColumnName());
    }

    [Fact]
    public void Migracao_de_proprietario_preserva_tarefas_legadas_sem_atribuicao()
    {
        using var context = new DataContext();
        var script = context.GetService<IMigrator>().GenerateScript(
            "20261117181950_AdicionarDataHoraCriacao", "20261117220216_AdicionarProprietario");

        Assert.Contains("ADD [USUARIOEMAIL] nvarchar(max) NULL", script);
        Assert.DoesNotContain("UPDATE [Tarefas]", script);
    }

    [Fact]
    public void Migracao_preenche_registros_existentes_antes_de_tornar_coluna_obrigatoria()
    {
        using var context = new DataContext();
        var script = context.GetService<IMigrator>().GenerateScript(
            "20261117165039_Initial", "20261117181950_AdicionarDataHoraCriacao");

        var adicionar = script.IndexOf("ADD [DATAHORACRIACAO] datetime2 NULL", StringComparison.Ordinal);
        var preencher = script.IndexOf("UPDATE [Tarefas] SET [DATAHORACRIACAO] = SYSUTCDATETIME()", StringComparison.Ordinal);
        var obrigatorio = script.IndexOf("ALTER COLUMN [DATAHORACRIACAO] datetime2 NOT NULL", StringComparison.Ordinal);
        Assert.True(adicionar >= 0, script);
        Assert.True(preencher > adicionar, script);
        Assert.True(obrigatorio > preencher, script);
    }
}
