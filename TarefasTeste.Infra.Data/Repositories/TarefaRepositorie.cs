using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TarefasTeste.Domain.Entities;
using TarefasTeste.Domain.Interfaces.Repositories;
using TarefasTeste.Infra.Data.Contexts;

namespace TarefasApp.Infra.Data.Repositories
{
    public class TarefaRepository : ITarefaRepository
    {
        public void Add(Tarefa tarefa)
        {
            using (var dataContext = new DataContext())
            {
                dataContext.Add(tarefa);
                dataContext.SaveChanges();
            }
        }

        public void Update(Tarefa tarefa)
        {
            using (var dataContext = new DataContext())
            {
                dataContext.Update(tarefa);
                dataContext.SaveChanges();
            }
        }

        public void Delete(Guid id)
        {
            using (var dataContext = new DataContext())
            {
                var tarefa = dataContext.Set<Tarefa>().Find(id);
                if (tarefa != null)
                {
                    dataContext.Remove(tarefa);
                    dataContext.SaveChanges();
                }
            }
        }

        public List<Tarefa> GetAll()
        {
            using (var dataContext = new DataContext())
            {
                return dataContext.Set<Tarefa>().ToList();
            }
        }
    }
}