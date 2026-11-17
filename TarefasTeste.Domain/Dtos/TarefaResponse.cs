using System;
using System.Collections.Generic;
using System.Text;

namespace TarefasTeste.Domain.Dtos
{
    /// <summary>
    /// DTO para retornar informações de uma tarefa
    /// </summary>
    public class TarefaResponse
    {
        /// <summary>
        /// Identificador único da tarefa
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Título da tarefa
        /// </summary>
        public string? Titulo { get; set; }

        /// <summary>
        /// Descrição detalhada da tarefa
        /// </summary>
        public string? Descricao { get; set; }

        /// <summary>
        /// Status da tarefa: false = pendente, true = concluída
        /// </summary>
        public bool Status { get; set; }

        /// <summary>
        /// Data e hora de criação da tarefa em UTC, geradas pelo servidor
        /// </summary>
        public DateTime DataHoraCriacao { get; set; }
    }
}
