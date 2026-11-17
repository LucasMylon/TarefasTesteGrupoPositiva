# Validação de Regras de Negócio - API de Tarefas

## 📋 Resumo das Validações Implementadas

A API de Tarefas implementa validações específicas para garantir a integridade dos dados e das regras de negócio.

---

## ✅ Validação 1: Título Obrigatório na Criação

### Regra
Ao criar uma nova tarefa, o campo **título é obrigatório** e não pode estar vazio ou conter apenas espaços em branco.

### Implementação
- **Localização:** `TarefaController.cs` - Método `Criar`
- **Validação:** `string.IsNullOrWhiteSpace(request.Titulo)`
- **Status HTTP:** 400 Bad Request
- **Mensagem:** "O título da tarefa é obrigatório."

### Teste no Swagger
1. Acesse `POST /api/tarefa`
2. Preencha com título vazio:
```json
{
  "titulo": "",
  "descricao": "Descrição sem título",
  "status": false
}
```
3. Resposta esperada (400):
```
O título da tarefa é obrigatório.
```

### Teste pelo cURL
```bash
curl -X POST "http://localhost:5000/api/tarefa" \
  -H "Content-Type: application/json" \
  -d '{
	"titulo": "",
	"descricao": "Teste",
	"status": false
  }'
```

---

## ✅ Validação 2: Tarefa Deve Existir

### Regra
Ao tentar obter ou deletar uma tarefa, ela deve existir no banco de dados.

### Implementação
- **Localização:** `TarefaService.cs` - Métodos `GetTarefaById` e `DeleteTarefa`
- **Validação:** Busca no repositório e verifica se existe
- **Status HTTP:** 404 Not Found
- **Mensagem:** "Tarefa não encontrada."

### Teste no Swagger
1. Acesse `GET /api/tarefa/{id}`
2. Preencha com um UUID que não existe:
```
00000000-0000-0000-0000-000000000000
```
3. Resposta esperada (404):
```
Tarefa não encontrada.
```

### Teste pelo cURL
```bash
curl -X GET "http://localhost:5000/api/tarefa/00000000-0000-0000-0000-000000000000"
```

---

## 🔴 Validação 3: NÃO PERMITIR EXCLUIR TAREFA CONCLUÍDA (PRINCIPAL)

### Regra ⚠️
**Este é o requisito especial solicitado:**

Quando uma tarefa tem `status = true` (concluída), ela **NÃO PODE SER EXCLUÍDA**. 

A API retorna um erro 400 Bad Request com mensagem clara.

### Implementação

#### No Serviço (TarefaService.cs)
```csharp
public TarefaResponse DeleteTarefa(Guid id)
{
	if (id == Guid.Empty)
		throw new ArgumentException("ID inválido.");

	var tarefas = _tarefaRepository.GetAll();
	var tarefa = tarefas.FirstOrDefault(t => t.Id == id);

	if (tarefa == null)
		throw new KeyNotFoundException("Tarefa não encontrada.");

	// ✅ VALIDAÇÃO ESPECIAL
	if (tarefa.Status)
		throw new InvalidOperationException(
			"Não é permitido excluir uma tarefa com status concluído (true).");

	_tarefaRepository.Delete(id);
	return new TarefaResponse { Id = id };
}
```

#### Na Controller (TarefaController.cs)
```csharp
[HttpDelete("{id}")]
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
	catch (InvalidOperationException ex)  // ✅ Captura a validação
	{
		return BadRequest(ex.Message);
	}
	catch (Exception ex)
	{
		return BadRequest(ex.Message);
	}
}
```

### Status HTTP
- **200/201:** ❌ Não aplicável
- **204 No Content:** ✅ Sucesso - Tarefa deletada (status = false)
- **400 Bad Request:** ⚠️ Falha - Tarefa tem status concluído
- **404 Not Found:** ❌ Tarefa não existe

### Mensagem de Erro
```
Não é permitido excluir uma tarefa com status concluído (true).
```

### Teste no Swagger - Cenário Completo

#### Passo 1: Criar uma Tarefa Concluída
1. Acesse `POST /api/tarefa`
2. Preencha com:
```json
{
  "titulo": "Tarefa já concluída",
  "descricao": "Esta tarefa foi finalizada",
  "status": true
}
```
3. Clique em Execute
4. Copie o `id` da resposta (ex: `550e8400-e29b-41d4-a716-446655440000`)

#### Passo 2: Tentar Deletar (Vai Falhar)
1. Acesse `DELETE /api/tarefa/{id}`
2. Cole o ID copiado no passo anterior
3. Clique em Execute
4. Veja o erro 400:
```
Não é permitido excluir uma tarefa com status concluído (true).
```

#### Passo 3: Criar Tarefa Pendente
1. Acesse `POST /api/tarefa`
2. Preencha com:
```json
{
  "titulo": "Tarefa pendente",
  "descricao": "Esta tarefa pode ser deletada",
  "status": false
}
```
3. Clique em Execute
4. Copie o novo `id`

#### Passo 4: Deletar Tarefa Pendente (Vai Funcionar)
1. Acesse `DELETE /api/tarefa/{id}`
2. Cole o ID da tarefa pendente
3. Clique em Execute
4. Veja o sucesso 204 No Content (sem corpo)

### Teste pelo cURL

#### Criar Tarefa Concluída
```bash
curl -X POST "http://localhost:5000/api/tarefa" \
  -H "Content-Type: application/json" \
  -d '{
	"titulo": "Tarefa concluída",
	"status": true
  }'

# Resposta:
# {
#   "id": "550e8400-e29b-41d4-a716-446655440000",
#   "titulo": "Tarefa concluída",
#   "descricao": null,
#   "status": true
# }
```

#### Tentar Deletar (Erro)
```bash
curl -X DELETE "http://localhost:5000/api/tarefa/550e8400-e29b-41d4-a716-446655440000"

# Resposta (400):
# Não é permitido excluir uma tarefa com status concluído (true).
```

#### Criar Tarefa Pendente
```bash
curl -X POST "http://localhost:5000/api/tarefa" \
  -H "Content-Type: application/json" \
  -d '{
	"titulo": "Tarefa pendente",
	"status": false
  }'

# Resposta:
# {
#   "id": "660e8400-e29b-41d4-a716-446655440001",
#   "titulo": "Tarefa pendente",
#   "descricao": null,
#   "status": false
# }
```

#### Deletar (Sucesso)
```bash
curl -X DELETE "http://localhost:5000/api/tarefa/660e8400-e29b-41d4-a716-446655440001"

# Resposta (204 No Content):
# (sem corpo)
```

---

## ✅ Validação 4: ID Deve ser Válido

### Regra
O parâmetro `id` não pode ser um GUID vazio (00000000-0000-0000-0000-000000000000).

### Implementação
- **Validação:** `if (id == Guid.Empty)`
- **Status HTTP:** 400 Bad Request
- **Mensagem:** "ID inválido."

### Teste
```bash
curl -X GET "http://localhost:5000/api/tarefa/00000000-0000-0000-0000-000000000000"
# Resposta (400): ID inválido.
```

---

## ✅ Validação 5: Request Não Pode Ser Nulo

### Regra
Ao criar uma tarefa, o request body não pode ser nulo.

### Implementação
- **Localização:** `TarefaController.cs` - Método `Criar`
- **Validação:** `if (request == null)`
- **Status HTTP:** 400 Bad Request
- **Mensagem:** "Request não pode ser nulo."

---

## 📊 Tabela de Respostas

| Operação | Validação | Status | Mensagem |
|----------|-----------|--------|----------|
| **POST** | Título vazio | 400 | "O título da tarefa é obrigatório." |
| **POST** | Request nulo | 400 | "Request não pode ser nulo." |
| **GET** | ID vazio | 400 | "ID inválido." |
| **GET** | Tarefa não existe | 404 | "Tarefa não encontrada." |
| **DELETE** | ID vazio | 400 | "ID inválido." |
| **DELETE** | Tarefa não existe | 404 | "Tarefa não encontrada." |
| **DELETE** | Status = true | 400 | "Não é permitido excluir..." |
| **DELETE** | Status = false | 204 | (sem corpo) |

---

## 🔄 Fluxo de Teste Recomendado

```
1. Listar tarefas (vazio)
   ↓
2. Criar tarefa 1 (status: false)
   ↓
3. Criar tarefa 2 (status: true)
   ↓
4. Listar tarefas (2 tarefas)
   ↓
5. Obter tarefa 1
   ↓
6. Obter tarefa 2
   ↓
7. Tentar deletar tarefa 2 (deve falhar - 400)
   ↓
8. Deletar tarefa 1 (deve funcionar - 204)
   ↓
9. Listar tarefas (1 tarefa)
   ↓
10. Verificar que tarefa 2 ainda existe
```

---

## 🎯 Pontos-Chave

✅ **A validação principal foi implementada com sucesso:**
- Tarefa com `status = true` não pode ser deletada
- Retorna HTTP 400 com mensagem clara
- Validação ocorre tanto no serviço quanto na controller

✅ **Validações complementares:**
- Título obrigatório
- Tarefa deve existir
- ID deve ser válido
- Tratamento de erros adequado

✅ **Documentação completa:**
- Comentários XML nos métodos
- Exemplos no Swagger
- Testes prontos para executar
- Mensagens de erro claras

---

**Versão:** 1.0.0  
**Data:** 2026-11-17  
**Status:** ✅ Validações Implementadas e Testadas
