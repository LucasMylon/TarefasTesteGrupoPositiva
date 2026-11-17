using System;
using System.Collections.Generic;
using System.Text;
using TarefasTeste.Domain.Dtos;

namespace TarefasTeste.Domain.Interfaces
{
    public interface ITarefaService
    {
        /// <summary>
        /// Adiciona uma nova tarefa associada ao usuário
        /// </summary>
        TarefaResponse AddTarefa(TarefaRequest request, string usuarioEmail);

        /// <summary>
        /// Atualiza uma tarefa existente do usuário
        /// </summary>
        TarefaResponse UpdateTarefa(Guid id, TarefaRequest request, string usuarioEmail);

        /// <summary>
        /// Deleta uma tarefa do usuário
        /// </summary>
        TarefaResponse DeleteTarefa(Guid id, string usuarioEmail);

        /// <summary>
        /// Obtém todas as tarefas do usuário
        /// </summary>
        List<TarefaResponse> GetAllTarefas(string usuarioEmail);

        /// <summary>
        /// Obtém uma tarefa específica do usuário
        /// </summary>
        TarefaResponse GetTarefaById(Guid id, string usuarioEmail);
    }
}

