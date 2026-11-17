using System;
using System.Collections.Generic;
using System.Text;

namespace TarefasTeste.Domain.Dtos
{
    /// <summary>
    /// DTO para criar ou atualizar uma tarefa; a data de criação é gerada pelo servidor
    /// </summary>
    public class TarefaRequest
    {
        /// <summary>
        /// Título da tarefa (obrigatório)
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
    }
}
