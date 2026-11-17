# API de Tarefas - Documentação

## Visão Geral

API REST para gerenciamento de tarefas com validações de status. A API permite criar, consultar, atualizar e excluir tarefas, com uma validação especial que impede a exclusão de tarefas concluídas.

## Tecnologias

- .NET 10.0
- ASP.NET Core
- Entity Framework Core
- Swagger/OpenAPI
- SQL Server

## Data de criação e banco de dados

`dataHoraCriacao` é gerada pelo servidor em UTC, retornada nas respostas de criação e consulta e preservada em atualizações. Não é necessário enviá-la no corpo da requisição. As tarefas existentes recebem a data UTC da aplicação da migração, pois a data original não foi registrada.

Aplique as migrações no banco configurado antes de iniciar a versão atualizada. Se necessário, instale antes a ferramenta `dotnet-ef` da versão 10.0.15:

```powershell
dotnet tool install --global dotnet-ef --version 10.0.15
dotnet ef database update --project TarefasTeste.Infra.Data --startup-project TarefasTeste.Infra.Data
```

## Endpoints da API

### Base URL
```
http://localhost:5000/api/tarefa
```

### 1. Criar Tarefa (POST)

**Endpoint:** `POST /api/tarefa`

**Descrição:** Cria uma nova tarefa no sistema.

**Request Body:**
```json
{
  "titulo": "Minha primeira tarefa",
  "descricao": "Descrição detalhada da tarefa",
  "status": false
}
```

**Parâmetros:**
- `titulo` (string, obrigatório): Título da tarefa
- `descricao` (string, opcional): Descrição detalhada
- `status` (boolean): false = pendente, true = concluída

**Response (201 Created):**
```json
{
  "id": "550e8400-e29b-41d4-a716-446655440000",
  "titulo": "Minha primeira tarefa",
  "descricao": "Descrição detalhada da tarefa",
  "status": false,
  "dataHoraCriacao": "2026-11-17T18:00:00Z"
}
```

**Erros:**
- `400 Bad Request`: Titulo vazio ou request nulo

---

### 2. Obter Todas as Tarefas (GET)

**Endpoint:** `GET /api/tarefa`

**Descrição:** Retorna a lista de todas as tarefas cadastradas.

**Response (200 OK):**
```json
[
  {
	"id": "550e8400-e29b-41d4-a716-446655440000",
	"titulo": "Minha primeira tarefa",
	"descricao": "Descrição detalhada da tarefa",
	"status": false,
	"dataHoraCriacao": "2026-11-17T18:00:00Z"
  },
  {
	"id": "660e8400-e29b-41d4-a716-446655440001",
	"titulo": "Segunda tarefa",
	"descricao": "Outra descrição",
	"status": true,
	"dataHoraCriacao": "2026-11-17T18:01:00Z"
  }
]
```

---

### 3. Obter Tarefa por ID (GET)

**Endpoint:** `GET /api/tarefa/{id}`

**Descrição:** Retorna os detalhes de uma tarefa específica.

**Parâmetros de Path:**
- `id` (UUID): ID da tarefa

**Response (200 OK):**
```json
{
  "id": "550e8400-e29b-41d4-a716-446655440000",
  "titulo": "Minha primeira tarefa",
  "descricao": "Descrição detalhada da tarefa",
  "status": false,
  "dataHoraCriacao": "2026-11-17T18:00:00Z"
}
```

**Erros:**
- `400 Bad Request`: ID inválido
- `404 Not Found`: Tarefa não encontrada

---

### 4. Excluir Tarefa (DELETE)

**Endpoint:** `DELETE /api/tarefa/{id}`

**Descrição:** Exclui uma tarefa do sistema.

**⚠️ VALIDAÇÃO IMPORTANTE:**
Não é permitido excluir tarefas com status concluído (`status = true`). 

Se tentar excluir uma tarefa concluída, receberá o erro:
```json
{
  "error": "Não é permitido excluir uma tarefa com status concluído (true)."
}
```

**Parâmetros de Path:**
- `id` (UUID): ID da tarefa

**Response (204 No Content):** 
Sem corpo na resposta (sucesso)

**Erros:**
- `400 Bad Request`: 
  - ID inválido
  - Tarefa tem status concluído (true)
- `404 Not Found`: Tarefa não encontrada

---

## Testando a API no Swagger

### Passo 1: Iniciar a Aplicação

```bash
cd TarefasTeste.Api
dotnet run
```

A aplicação será iniciada em: `http://localhost:5000`

### Passo 2: Acessar o Swagger UI

Navegue até:
```
http://localhost:5000/swagger
```

Você verá a interface do Swagger com todos os endpoints documentados.

### Passo 3: Testar os Endpoints

#### Exemplo 1: Criar uma Tarefa
1. Clique em **POST /api/tarefa**
2. Clique em "Try it out"
3. Preencha o Request Body:
```json
{
  "titulo": "Estudar C#",
  "descricao": "Aprender sobre async/await",
  "status": false
}
```
4. Clique em "Execute"
5. Veja a resposta 201 Created com o ID gerado

#### Exemplo 2: Listar Todas as Tarefas
1. Clique em **GET /api/tarefa**
2. Clique em "Try it out"
3. Clique em "Execute"
4. Veja a lista de todas as tarefas

#### Exemplo 3: Obter Tarefa por ID
1. Clique em **GET /api/tarefa/{id}**
2. Clique em "Try it out"
3. Preencha o parâmetro `id` com um UUID válido (da tarefa criada no exemplo 1)
4. Clique em "Execute"

#### Exemplo 4: Tentar Excluir uma Tarefa Concluída (Validação)
1. Primeiro, crie uma tarefa com `status: true`
```json
{
  "titulo": "Tarefa já concluída",
  "descricao": "Esta tarefa está concluída",
  "status": true
}
```
2. Copie o ID da tarefa criada
3. Clique em **DELETE /api/tarefa/{id}**
4. Clique em "Try it out"
5. Preencha o `id` com o ID copiado
6. Clique em "Execute"
7. Veja o erro 400 Bad Request com a mensagem de validação

#### Exemplo 5: Excluir uma Tarefa Pendente
1. Crie uma tarefa com `status: false` (ou use uma existente)
2. Copie o ID da tarefa
3. Clique em **DELETE /api/tarefa/{id}**
4. Clique em "Try it out"
5. Preencha o `id`
6. Clique em "Execute"
7. Veja o sucesso 204 No Content

---

## Fluxo de Exemplo Completo

```bash
# 1. Criar uma tarefa pendente
curl -X POST "http://localhost:5000/api/tarefa" \
  -H "Content-Type: application/json" \
  -d '{
	"titulo": "Implementar autenticação",
	"descricao": "Adicionar JWT à API",
	"status": false
  }'

# Resposta: 201 Created
# {
#   "id": "550e8400-e29b-41d4-a716-446655440000",
#   "titulo": "Implementar autenticação",
#   "descricao": "Adicionar JWT à API",
#   "status": false
# }

# 2. Listar todas as tarefas
curl -X GET "http://localhost:5000/api/tarefa"

# 3. Obter tarefa específica
curl -X GET "http://localhost:5000/api/tarefa/550e8400-e29b-41d4-a716-446655440000"

# 4. Excluir a tarefa (sucesso - status é false)
curl -X DELETE "http://localhost:5000/api/tarefa/550e8400-e29b-41d4-a716-446655440000"

# Resposta: 204 No Content
```

---

## Códigos de Status HTTP

| Código | Descrição |
|--------|-----------|
| `200 OK` | Requisição bem-sucedida |
| `201 Created` | Recurso criado com sucesso |
| `204 No Content` | Recurso excluído com sucesso |
| `400 Bad Request` | Dados inválidos ou validação falhou |
| `404 Not Found` | Recurso não encontrado |
| `500 Internal Server Error` | Erro no servidor |

---

## Validações Implementadas

### Criar Tarefa
- ✅ Titulo é obrigatório (não pode ser vazio ou nulo)
- ✅ Request não pode ser nulo

### Excluir Tarefa
- ✅ **ID não pode ser vazio**
- ✅ **Tarefa deve existir**
- ✅ **Tarefa não pode ter status concluído (true)** ⚠️ VALIDAÇÃO ESPECIAL

### Obter Tarefa por ID
- ✅ ID não pode ser vazio
- ✅ Tarefa deve existir

---

## Estrutura do Projeto

```
TarefasTeste/
├── TarefasTeste.Api/
│   ├── Controllers/
│   │   └── TarefaController.cs
│   ├── Program.cs
│   └── TarefasTeste.Api.csproj
├── TarefasTeste.Domain/
│   ├── Entities/
│   │   └── Tarefa.cs
│   ├── Dtos/
│   │   ├── TarefaRequest.cs
│   │   └── TarefaResponse.cs
│   ├── Interfaces/
│   │   ├── ITarefaService.cs
│   │   └── Repositories/
│   │       └── ITarefaRepository.cs
│   └── Service/
│       └── TarefaService.cs
└── TarefasTeste.Infra.Data/
	├── Repositories/
	│   └── TarefaRepository.cs
	├── Contexts/
	│   └── DataContext.cs
	└── Migrations/
```

---

## Próximas Melhorias

- [ ] Adicionar autenticação e autorização (JWT)
- [ ] Implementar paginação nas listas
- [ ] Adicionar filtros (por status, data, etc)
- [ ] Implementar atualização parcial (PATCH)
- [ ] Adicionar logging
- [ ] Implementar rate limiting
- [ ] Adicionar testes unitários e de integração
- [ ] Adicionar cache

---

## Suporte

Para dúvidas ou problemas, entre em contato com o time de desenvolvimento.

**Versão da API:** 1.0.0  
**Última atualização:** 2026-11-17
