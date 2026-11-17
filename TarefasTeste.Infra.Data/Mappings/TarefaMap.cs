using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using TarefasTeste.Domain.Entities;

namespace TarefasTeste.Infra.Data.Mappings
{
    public class TarefaMap : IEntityTypeConfiguration<Tarefa>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Tarefa> builder)
        {
            builder.ToTable("Tarefas");
            builder.HasKey(t => t.Id);
            builder.Property(t => t.Titulo).IsRequired().HasMaxLength(100).HasColumnName("TITULO");
            builder.Property(t => t.Descricao).IsRequired().HasMaxLength(500).HasColumnName("DESCRICAO");
            builder.Property(t => t.Status).IsRequired().HasColumnName("STATUS");
            builder.Property(t => t.DataHoraCriacao)
                .IsRequired()
                .HasColumnName("DATAHORACRIACAO")
                .HasConversion(value => value, value => DateTime.SpecifyKind(value, DateTimeKind.Utc));

            builder.Property(t => t.UsuarioEmail)
                .HasColumnName("USUARIOEMAIL")
                .IsRequired(false);

            builder.HasIndex(t => t.Titulo).IsUnique();
        }
    }
}
