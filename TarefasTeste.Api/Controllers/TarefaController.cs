using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TarefasTeste.Domain.Dtos;
using TarefasTeste.Domain.Interfaces;

namespace TarefasTeste.Api.Controllers
{
    /// <summary>
    /// Controller responsável pelo gerenciamento de tarefas
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
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
        /// Cria uma nova tarefa
        /// </summary>
        /// <param name="request">Dados da tarefa a ser criada</param>
        /// <returns>A tarefa criada com seu ID</returns>
        /// <remarks>
        /// A data e hora de criação são geradas pelo servidor em UTC e retornadas na resposta.
        /// Exemplo de request:
        /// 
        ///     POST /api/tarefa
        ///     {
        ///       "titulo": "Minha primeira tarefa",
        ///       "descricao": "Descrição detalhada da tarefa",
        ///       "status": false
        ///     }
        /// </remarks>
        [HttpPost]
        [ProducesResponseType(typeof(TarefaResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult Criar([FromBody] TarefaRequest request)
        {
            try
            {
                if (request == null)
                    return BadRequest("Request não pode ser nulo.");

                if (string.IsNullOrWhiteSpace(request.Titulo))
                    return BadRequest("O título da tarefa é obrigatório.");

                var resultado = _tarefaService.AddTarefa(request);
                return CreatedAtAction(nameof(ObterPorId), new { id = resultado.Id }, resultado);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Exclui uma tarefa pelo ID
        /// </summary>
        /// <param name="id">ID da tarefa a ser excluída</param>
        /// <returns>Sem conteúdo se bem-sucedido</returns>
        /// <remarks>
        /// **IMPORTANTE:** Não é permitido excluir tarefas com status concluído (true).
        /// Se tentar excluir uma tarefa concluída, receberá um erro 400.
        /// </remarks>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult Excluir(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                    return BadRequest("ID inválido.");

                var resultado = _tarefaService.DeleteTarefa(id);
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
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Obtém uma tarefa específica pelo ID
        /// </summary>
        /// <param name="id">ID da tarefa a ser obtida</param>
        /// <returns>A tarefa encontrada</returns>
        /// <remarks>
        /// Retorna os detalhes completos de uma tarefa específica.
        /// </remarks>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(TarefaResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult ObterPorId(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                    return BadRequest("ID inválido.");

                var tarefa = _tarefaService.GetTarefaById(id);
                return Ok(tarefa);
            }
            catch (KeyNotFoundException)
            {
                return NotFound("Tarefa não encontrada.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Obtém todas as tarefas cadastradas
        /// </summary>
        /// <returns>Lista de todas as tarefas</returns>
        /// <remarks>
        /// Retorna um array com todas as tarefas cadastradas no sistema.
        /// Se não houver tarefas, retorna um array vazio.
        /// </remarks>
        [HttpGet]
        [ProducesResponseType(typeof(List<TarefaResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult ObterTodos()
        {
            try
            {
                var tarefas = _tarefaService.GetAllTarefas();
                return Ok(tarefas);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
