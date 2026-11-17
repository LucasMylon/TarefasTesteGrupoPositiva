using System;
using System.Collections.Generic;
using System.Text;
using TarefasTeste.Domain.Dtos;
using TarefasTeste.Domain.Entities;
using TarefasTeste.Domain.Interfaces;
using TarefasTeste.Domain.Interfaces.Repositories;

namespace TarefasTeste.Domain.Service
{
    public class TarefaService : ITarefaService
    {

        private readonly ITarefaRepository _tarefaRepository;

        public TarefaService(ITarefaRepository tarefaRepository)
        {
            _tarefaRepository = tarefaRepository;
        }

        public TarefaResponse AddTarefa(TarefaRequest request, string usuarioEmail)
        {
            if (string.IsNullOrWhiteSpace(usuarioEmail))
                throw new ArgumentException("Email do usuário é obrigatório.");

            var tarefa = new Tarefa
            {
                Titulo = request.Titulo,
                Descricao = request.Descricao,
                Status = request.Status,
                UsuarioEmail = usuarioEmail
            };
            _tarefaRepository.Add(tarefa);
            return new TarefaResponse
            {
                Id = tarefa.Id,
                Titulo = tarefa.Titulo,
                Descricao = tarefa.Descricao,
                Status = tarefa.Status,
                DataHoraCriacao = tarefa.DataHoraCriacao,
                UsuarioEmail = tarefa.UsuarioEmail
            };
        }

        public TarefaResponse UpdateTarefa(Guid id, TarefaRequest request, string usuarioEmail)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("ID inválido.");
            if (string.IsNullOrWhiteSpace(usuarioEmail))
                throw new ArgumentException("Email do usuário é obrigatório.");

            var tarefa = _tarefaRepository.GetAll().FirstOrDefault(t => t.Id == id && t.UsuarioEmail == usuarioEmail);
            if (tarefa == null)
                throw new KeyNotFoundException("Tarefa não encontrada.");

            tarefa.Titulo = request.Titulo;
            tarefa.Descricao = request.Descricao;
            tarefa.Status = request.Status;
            _tarefaRepository.Update(tarefa);
            return new TarefaResponse
            {
                Id = tarefa.Id,
                Titulo = tarefa.Titulo,
                Descricao = tarefa.Descricao,
                Status = tarefa.Status,
                DataHoraCriacao = tarefa.DataHoraCriacao,
                UsuarioEmail = tarefa.UsuarioEmail
            };

        }

        public TarefaResponse DeleteTarefa(Guid id, string usuarioEmail)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("ID inválido.");
            if (string.IsNullOrWhiteSpace(usuarioEmail))
                throw new ArgumentException("Email do usuário é obrigatório.");

            var tarefas = _tarefaRepository.GetAll();
            var tarefa = tarefas.FirstOrDefault(t => t.Id == id && t.UsuarioEmail == usuarioEmail);

            if (tarefa == null)
                throw new KeyNotFoundException("Tarefa não encontrada.");

            if (tarefa.Status)
                throw new InvalidOperationException("Não é permitido excluir uma tarefa com status concluído (true).");

            _tarefaRepository.Delete(id);
            return new TarefaResponse
            {
                Id = id,
                DataHoraCriacao = tarefa.DataHoraCriacao
            };
        }

        public List<TarefaResponse> GetAllTarefas(string usuarioEmail)
        {
            if (string.IsNullOrWhiteSpace(usuarioEmail))
                throw new ArgumentException("Email do usuário é obrigatório.");

            var tarefas = _tarefaRepository.GetAll()
                .Where(t => t.UsuarioEmail == usuarioEmail)
                .ToList();

            return tarefas.Select(tarefa => new TarefaResponse
            {
                Id = tarefa.Id,
                Titulo = tarefa.Titulo,
                Descricao = tarefa.Descricao,
                Status = tarefa.Status,
                DataHoraCriacao = tarefa.DataHoraCriacao,
                UsuarioEmail = tarefa.UsuarioEmail
            }).ToList();
        }

        public TarefaResponse GetTarefaById(Guid id, string usuarioEmail)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("ID inválido.");

            if (string.IsNullOrWhiteSpace(usuarioEmail))
                throw new ArgumentException("Email do usuário é obrigatório.");

            var tarefas = _tarefaRepository.GetAll();
            var tarefa = tarefas.FirstOrDefault(t => t.Id == id && t.UsuarioEmail == usuarioEmail);

            if (tarefa == null)
                throw new KeyNotFoundException("Tarefa não encontrada.");

            return new TarefaResponse
            {
                Id = tarefa.Id,
                Titulo = tarefa.Titulo,
                Descricao = tarefa.Descricao,
                Status = tarefa.Status,
                DataHoraCriacao = tarefa.DataHoraCriacao,
                UsuarioEmail = tarefa.UsuarioEmail
            };
        }
    }
}

