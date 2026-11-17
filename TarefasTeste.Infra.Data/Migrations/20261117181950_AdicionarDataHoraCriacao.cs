using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TarefasTeste.Infra.Data.Migrations
{
    /// <inheritdoc />
    public partial class AdicionarDataHoraCriacao : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DATAHORACRIACAO",
                table: "Tarefas",
                type: "datetime2",
                nullable: true);

            migrationBuilder.Sql("UPDATE [Tarefas] SET [DATAHORACRIACAO] = SYSUTCDATETIME() WHERE [DATAHORACRIACAO] IS NULL");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DATAHORACRIACAO",
                table: "Tarefas",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DATAHORACRIACAO",
                table: "Tarefas");
        }
    }
}
