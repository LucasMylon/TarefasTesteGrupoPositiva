using System;
using System.Collections.Generic;
using System.Text;
using TarefasTeste.Domain.Entities;

namespace TarefasTeste.Domain.Interfaces.Repositories
{
    public interface ITarefaRepository
    {
        void Add(Tarefa tarefa);
        void Update(Tarefa tarefa);
        void Delete(Guid id);
        List<Tarefa> GetAll();
    }
}
