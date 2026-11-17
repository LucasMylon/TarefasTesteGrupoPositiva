# Exemplos de Requisições da API de Tarefas

## Usando cURL

### 1. Criar uma Tarefa Pendente

```bash
curl -X POST "http://localhost:5000/api/tarefa" \
  -H "Content-Type: application/json" \
  -d '{
	"titulo": "Estudar C#",
	"descricao": "Aprender sobre async/await",
	"status": false
  }'
```

**Resposta (201 Created):**
```json
{
  "id": "550e8400-e29b-41d4-a716-446655440000",
  "titulo": "Estudar C#",
  "descricao": "Aprender sobre async/await",
  "status": false,
  "dataHoraCriacao": "2026-11-17T18:00:00Z"
}
```

---

### 2. Criar uma Tarefa Concluída

```bash
curl -X POST "http://localhost:5000/api/tarefa" \
  -H "Content-Type: application/json" \
  -d '{
	"titulo": "Tarefa concluída",
	"descricao": "Esta tarefa já foi feita",
	"status": true
  }'
```

**Resposta (201 Created):**
```json
{
  "id": "660e8400-e29b-41d4-a716-446655440001",
  "titulo": "Tarefa concluída",
  "descricao": "Esta tarefa já foi feita",
  "status": true,
  "dataHoraCriacao": "2026-11-17T18:01:00Z"
}
```

---

### 3. Listar Todas as Tarefas

```bash
curl -X GET "http://localhost:5000/api/tarefa"
```

**Resposta (200 OK):**
```json
[
  {
	"id": "550e8400-e29b-41d4-a716-446655440000",
	"titulo": "Estudar C#",
	"descricao": "Aprender sobre async/await",
	"status": false,
	"dataHoraCriacao": "2026-11-17T18:00:00Z"
  },
  {
	"id": "660e8400-e29b-41d4-a716-446655440001",
	"titulo": "Tarefa concluída",
	"descricao": "Esta tarefa já foi feita",
	"status": true,
	"dataHoraCriacao": "2026-11-17T18:01:00Z"
  }
]
```

---

### 4. Obter Tarefa Específica

```bash
curl -X GET "http://localhost:5000/api/tarefa/550e8400-e29b-41d4-a716-446655440000"
```

**Resposta (200 OK):**
```json
{
  "id": "550e8400-e29b-41d4-a716-446655440000",
  "titulo": "Estudar C#",
  "descricao": "Aprender sobre async/await",
  "status": false,
  "dataHoraCriacao": "2026-11-17T18:00:00Z"
}
```

---

### 5. Excluir Tarefa Pendente (Sucesso)

```bash
curl -X DELETE "http://localhost:5000/api/tarefa/550e8400-e29b-41d4-a716-446655440000"
```

**Resposta (204 No Content):**
```
(sem corpo)
```

---

### 6. Tentar Excluir Tarefa Concluída (Erro - Validação)

```bash
curl -X DELETE "http://localhost:5000/api/tarefa/660e8400-e29b-41d4-a716-446655440001"
```

**Resposta (400 Bad Request):**
```
Não é permitido excluir uma tarefa com status concluído (true).
```

---

### 7. Tentar Obter Tarefa Inexistente

```bash
curl -X GET "http://localhost:5000/api/tarefa/00000000-0000-0000-0000-000000000000"
```

**Resposta (404 Not Found):**
```
Tarefa não encontrada.
```

---

### 8. Tentar Criar Tarefa sem Título

```bash
curl -X POST "http://localhost:5000/api/tarefa" \
  -H "Content-Type: application/json" \
  -d '{
	"titulo": "",
	"descricao": "Descrição sem título",
	"status": false
  }'
```

**Resposta (400 Bad Request):**
```
O título da tarefa é obrigatório.
```

---

## Usando Postman

### Configuração Base

**Base URL:** `http://localhost:5000`

### Requisições Configuradas

#### 1. POST - Criar Tarefa

```
Método: POST
URL: http://localhost:5000/api/tarefa
Headers:
  Content-Type: application/json

Body (JSON):
{
  "titulo": "Minha tarefa",
  "descricao": "Descrição da tarefa",
  "status": false
}
```

#### 2. GET - Listar Tarefas

```
Método: GET
URL: http://localhost:5000/api/tarefa
```

#### 3. GET - Obter Tarefa por ID

```
Método: GET
URL: http://localhost:5000/api/tarefa/{{task_id}}
```

#### 4. DELETE - Excluir Tarefa

```
Método: DELETE
URL: http://localhost:5000/api/tarefa/{{task_id}}
```

---

## Usando VSCode com REST Client

Crie um arquivo `requests.rest` na raiz do projeto:

```rest
### Base URL
@baseUrl = http://localhost:5000/api/tarefa

### 1. Criar tarefa pendente
POST {{baseUrl}}
Content-Type: application/json

{
  "titulo": "Estudar API REST",
  "descricao": "Aprender sobre HTTP methods",
  "status": false
}

### 2. Criar tarefa concluída
POST {{baseUrl}}
Content-Type: application/json

{
  "titulo": "Tarefa concluída",
  "descricao": "Já foi finalizada",
  "status": true
}

### 3. Listar todas as tarefas
GET {{baseUrl}}

### 4. Obter tarefa específica (substitua o ID)
GET {{baseUrl}}/550e8400-e29b-41d4-a716-446655440000

### 5. Excluir tarefa pendente (substitua o ID)
DELETE {{baseUrl}}/550e8400-e29b-41d4-a716-446655440000

### 6. Tentar excluir tarefa concluída (vai retornar erro)
DELETE {{baseUrl}}/660e8400-e29b-41d4-a716-446655440001
```

---

## Testando no Swagger UI

1. **Inicie a aplicação:**
   ```bash
   cd TarefasTeste.Api
   dotnet run
   ```

2. **Abra o navegador:**
   ```
   http://localhost:5000/swagger
   ```

3. **Para cada endpoint:**
   - Clique no endpoint desejado
   - Clique em "Try it out"
   - Preencha os dados necessários
   - Clique em "Execute"
   - Veja o resultado

---

## Fluxo de Teste Recomendado

1. **Criar Tarefa 1 (Pendente)**
   - POST com `status: false`
   - Copie o ID retornado

2. **Criar Tarefa 2 (Concluída)**
   - POST com `status: true`
   - Copie o ID retornado

3. **Listar Tarefas**
   - GET sem parâmetros
   - Veja ambas as tarefas

4. **Obter Tarefa 1**
   - GET com ID da Tarefa 1
   - Confirme os dados

5. **Excluir Tarefa 1 (Deve Funcionar)**
   - DELETE com ID da Tarefa 1
   - Deve retornar 204

6. **Tentar Excluir Tarefa 2 (Deve Falhar)**
   - DELETE com ID da Tarefa 2
   - Deve retornar 400 com mensagem de validação

7. **Verificar Tarefa 1 Deletada**
   - GET com ID da Tarefa 1
   - Deve retornar 404

8. **Listar Tarefas Novamente**
   - GET sem parâmetros
   - Deve ter apenas a Tarefa 2

---

## Resposta Esperada para Cada Endpoint

### Sucesso

**POST /api/tarefa - 201 Created**
```json
{
  "id": "uuid-aqui",
  "titulo": "string",
  "descricao": "string",
  "status": boolean,
  "dataHoraCriacao": "2026-11-17T18:00:00Z"
}
```

**GET /api/tarefa - 200 OK**
```json
[
  {
	"id": "uuid",
	"titulo": "string",
	"descricao": "string",
	"status": boolean,
	"dataHoraCriacao": "2026-11-17T18:00:00Z"
  }
]
```

**GET /api/tarefa/{id} - 200 OK**
```json
{
  "id": "uuid",
  "titulo": "string",
  "descricao": "string",
  "status": boolean,
  "dataHoraCriacao": "2026-11-17T18:00:00Z"
}
```

**DELETE /api/tarefa/{id} - 204 No Content**
```
(sem corpo)
```

### Erros

**400 Bad Request**
```
"Mensagem de erro específica"
```

**404 Not Found**
```
"Tarefa não encontrada."
```

---

## Dicas Importantes

✅ **Sempre testse a validação especial:**
- Tente excluir uma tarefa com `status: true`
- Deve retornar erro 400

✅ **Teste o ID inválido:**
- Tente acessar ou deletar com UUID vazio
- Deve retornar erro 400

✅ **Teste o título obrigatório:**
- Tente criar tarefa sem título
- Deve retornar erro 400

✅ **Use o Swagger para explorar:**
- Interface gráfica facilita o teste
- Vê os schemas de request/response automaticamente
