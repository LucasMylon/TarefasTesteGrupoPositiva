using System;
using System.Collections.Generic;
using System.Text;

namespace TarefasTeste.Domain.Entities
{
    /// <summary>
    /// Entidade que representa uma tarefa
    /// </summary>
    public class Tarefa
    {
        /// <summary>
        /// Identificador único da tarefa
        /// </summary>
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>
        /// Título da tarefa (obrigatório)
        /// </summary>
        public string Titulo { get; set; } = string.Empty;

        /// <summary>
        /// Descrição detalhada da tarefa
        /// </summary>
        public string Descricao { get; set; } = string.Empty;

        /// <summary>
        /// Status da tarefa: false = pendente, true = concluída
        /// </summary>
        public bool Status { get; set; } = false;

        /// <summary>
        /// Data e hora de criação em UTC
        /// </summary>
        public DateTime DataHoraCriacao { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Email do usuário proprietário da tarefa (extraído do token JWT); nulo para tarefas antigas sem proprietário.
        /// </summary>
        public string? UsuarioEmail { get; set; }
    }
}

