using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using TarefasTeste.Domain.Dtos;
using TarefasTeste.Domain.Interfaces;
using System.Security.Claims;

namespace TarefasTeste.Api.Controllers
{
    /// <summary>
    /// Controller responsável pelo gerenciamento de tarefas com autenticação JWT Bearer.
    /// Todos os endpoints requerem autenticação e isolam dados por usuário.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TarefaController : ControllerBase
    {
        private readonly ITarefaService _tarefaService;

        /// <summary>
        /// Inicializa uma nova instância do TarefaController
        /// </summary>
        /// <param name="tarefaService">Serviço de tarefas injetado</param>
        public TarefaController(ITarefaService tarefaService)
        {
            _tarefaService = tarefaService;
        }

        /// <summary>
        /// Extrai o email do usuário do token JWT
        /// </summary>
        /// <returns>Email do usuário autenticado</returns>
        private string GetUsuarioEmail()
        {
            var emailClaim = User.FindFirst(ClaimTypes.Name);
            if (emailClaim == null || string.IsNullOrWhiteSpace(emailClaim.Value))
                throw new InvalidOperationException("Email do usuário não encontrado no token.");
            return emailClaim.Value;
        }

        /// <summary>
        /// Cria uma nova tarefa para o usuário autenticado
        /// </summary>
        /// <param name="request">Dados da tarefa a ser criada</param>
        /// <returns>A tarefa criada com seu ID</returns>
        /// <remarks>
        /// Requer autenticação via JWT Bearer Token.
        /// 
        /// A data e hora de criação são geradas pelo servidor em UTC.
        /// A tarefa é automaticamente associada ao email do usuário extraído do token.
        /// 
        /// Exemplo de header:
        /// Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
        /// 
        /// Exemplo de request:
        /// 
        ///     POST /api/tarefa
        ///     {
        ///       "titulo": "Minha primeira tarefa",
        ///       "descricao": "Descrição detalhada da tarefa",
        ///       "status": false
        ///     }
        /// 
        /// Resposta de sucesso (201):
        ///     {
        ///       "id": "550e8400-e29b-41d4-a716-446655440000",
        ///       "titulo": "Minha primeira tarefa",
        ///       "descricao": "Descrição detalhada da tarefa",
        ///       "status": false,
        ///       "dataHoraCriacao": "2026-11-17T14:30:00Z",
        ///       "usuarioEmail": "usuario@example.com"
        ///     }
        /// </remarks>
        [HttpPost]
        [ProducesResponseType(typeof(TarefaResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult Criar([FromBody] TarefaRequest request)
        {
            try
            {
                if (request == null)
                    return BadRequest("Request não pode ser nulo.");

                if (string.IsNullOrWhiteSpace(request.Titulo))
                    return BadRequest("O título da tarefa é obrigatório.");

                var usuarioEmail = GetUsuarioEmail();
                var resultado = _tarefaService.AddTarefa(request, usuarioEmail);
                return CreatedAtAction(nameof(ObterPorId), new { id = resultado.Id }, resultado);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized("Token inválido ou expirado.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Atualiza os dados e o status de uma tarefa existente do usuário autenticado
        /// </summary>
        /// <param name="id">ID da tarefa a ser atualizada</param>
        /// <param name="request">Dados atualizados da tarefa</param>
        /// <returns>A tarefa atualizada</returns>
        /// <remarks>
        /// Requer autenticação via JWT Bearer Token.
        /// 
        /// O usuário só pode atualizar suas próprias tarefas.
        /// Sem restrições em relação ao status (pode atualizar de true para false e vice-versa).
        /// 
        /// Exemplo de header:
        /// Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
        /// 
        /// Exemplo de request:
        ///     PUT /api/tarefa/550e8400-e29b-41d4-a716-446655440000
        ///     {
        ///       "titulo": "Tarefa atualizada",
        ///       "descricao": "Nova descrição",
        ///       "status": true
        ///     }
        /// </remarks>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(TarefaResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult Atualizar(Guid id, [FromBody] TarefaRequest request)
        {
            try
            {
                if (id == Guid.Empty)
                    return BadRequest("ID inválido.");

                if (request == null)
                    return BadRequest("Request não pode ser nulo.");

                if (string.IsNullOrWhiteSpace(request.Titulo))
                    return BadRequest("O título da tarefa é obrigatório.");

                var usuarioEmail = GetUsuarioEmail();
                return Ok(_tarefaService.UpdateTarefa(id, request, usuarioEmail));
            }
            catch (KeyNotFoundException)
            {
                return NotFound("Tarefa não encontrada.");
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized("Token inválido ou expirado.");
            }
        }

        /// <summary>
        /// Exclui uma tarefa do usuário autenticado
        /// </summary>
        /// <param name="id">ID da tarefa a ser excluída</param>
        /// <returns>Sem conteúdo se bem-sucedido</returns>
        /// <remarks>
        /// Requer autenticação via JWT Bearer Token.
        /// 
        /// **IMPORTANTE:** Não é permitido excluir tarefas com status concluído (true).
        /// Se tentar excluir uma tarefa concluída, receberá um erro 400 com mensagem descritiva.
        /// 
        /// O usuário só pode excluir suas próprias tarefas.
        /// 
        /// Exemplo de header:
        /// Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
        /// 
        /// Resposta de sucesso (204):
        /// Sem conteúdo
        /// 
        /// Erro ao tentar deletar tarefa concluída (400):
        ///     {
        ///       "message": "Não é permitido excluir tarefas com status concluído."
        ///     }
        /// </remarks>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult Excluir(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                    return BadRequest("ID inválido.");

                var usuarioEmail = GetUsuarioEmail();
                _tarefaService.DeleteTarefa(id, usuarioEmail);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound("Tarefa não encontrada.");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized("Token inválido ou expirado.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Obtém uma tarefa específica do usuário autenticado
        /// </summary>
        /// <param name="id">ID da tarefa a ser obtida</param>
        /// <returns>A tarefa encontrada</returns>
        /// <remarks>
        /// Requer autenticação via JWT Bearer Token.
        /// 
        /// O usuário só pode visualizar suas próprias tarefas.
        /// Retorna os detalhes completos de uma tarefa específica incluindo email do proprietário.
        /// 
        /// Exemplo de header:
        /// Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
        /// 
        /// Resposta de sucesso (200):
        ///     {
        ///       "id": "550e8400-e29b-41d4-a716-446655440000",
        ///       "titulo": "Minha tarefa",
        ///       "descricao": "Descrição da tarefa",
        ///       "status": false,
        ///       "dataHoraCriacao": "2026-11-17T14:30:00Z",
        ///       "usuarioEmail": "usuario@example.com"
        ///     }
        /// </remarks>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(TarefaResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult ObterPorId(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                    return BadRequest("ID inválido.");

                var usuarioEmail = GetUsuarioEmail();
                var tarefa = _tarefaService.GetTarefaById(id, usuarioEmail);
                return Ok(tarefa);
            }
            catch (KeyNotFoundException)
            {
                return NotFound("Tarefa não encontrada.");
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized("Token inválido ou expirado.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Obtém todas as tarefas do usuário autenticado
        /// </summary>
        /// <returns>Lista de tarefas do usuário</returns>
        /// <remarks>
        /// Requer autenticação via JWT Bearer Token.
        /// 
        /// Retorna apenas as tarefas do usuário autenticado. Isolamento de dados é garantido no servidor.
        /// Se não houver tarefas, retorna um array vazio.
        /// 
        /// Exemplo de header:
        /// Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
        /// 
        /// Resposta de sucesso (200):
        ///     [
        ///       {
        ///         "id": "550e8400-e29b-41d4-a716-446655440000",
        ///         "titulo": "Primeira tarefa",
        ///         "descricao": "Descrição",
        ///         "status": false,
        ///         "dataHoraCriacao": "2026-11-17T14:30:00Z",
        ///         "usuarioEmail": "usuario@example.com"
        ///       },
        ///       {
        ///         "id": "660e8400-e29b-41d4-a716-446655440001",
        ///         "titulo": "Segunda tarefa",
        ///         "descricao": "Outra descrição",
        ///         "status": true,
        ///         "dataHoraCriacao": "2026-11-17T15:00:00Z",
        ///         "usuarioEmail": "usuario@example.com"
        ///       }
        ///     ]
        /// </remarks>
        [HttpGet]
        [ProducesResponseType(typeof(List<TarefaResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult ObterTodos()
        {
            try
            {
                var usuarioEmail = GetUsuarioEmail();
                var tarefas = _tarefaService.GetAllTarefas(usuarioEmail);
                return Ok(tarefas);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized("Token inválido ou expirado.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
