using System;
using System.Collections.Generic;
using System.Text;

namespace TarefasTeste.Domain.Entities
{
    public class Tarefa
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Titulo { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public bool Status { get; set; } = false;
        public DateTime DataHoraCriacao { get; set; } = DateTime.UtcNow;

    }
}
