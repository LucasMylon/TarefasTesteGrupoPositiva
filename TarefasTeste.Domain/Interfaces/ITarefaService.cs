using System;
using System.Collections.Generic;
using System.Text;
using TarefasTeste.Domain.Dtos;

namespace TarefasTeste.Domain.Interfaces
{
    public interface ITarefaService
    {
        TarefaResponse AddTarefa(TarefaRequest request);
        TarefaResponse UpdateTarefa(Guid id, TarefaRequest request);
        TarefaResponse DeleteTarefa(Guid id);
        List<TarefaResponse> GetAllTarefas();
        TarefaResponse GetTarefaById(Guid id);
    }
}
