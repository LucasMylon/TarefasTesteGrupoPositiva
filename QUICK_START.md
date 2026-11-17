# 🎯 GUIA RÁPIDO - API de Tarefas

## 📌 O Que Foi Feito

### ✅ 4 Métodos Implementados na Controller

```csharp
POST   /api/tarefa          → Criar nova tarefa
GET    /api/tarefa          → Listar todas as tarefas  
GET    /api/tarefa/{id}     → Obter uma tarefa específica
DELETE /api/tarefa/{id}     → Excluir tarefa (COM VALIDAÇÃO)
```

### ⭐ Validação Principal Implementada

**Não é permitido excluir tarefa com status = true (concluída)**

Exemplo:
```
DELETE /api/tarefa/550e8400-e29b-41d4-a716-446655440000
↓
Se status = true → Retorna HTTP 400 ❌
Se status = false → Retorna HTTP 204 ✅
```

---

## 🚀 Como Usar (3 Passos)

### Passo 1: Iniciar a API

```bash
cd C:\Users\Lucas\source\repos\TarefasTeste\TarefasTeste.Api
dotnet run
```

### Passo 2: Abrir o Swagger

Abra no navegador:
```
http://localhost:5000/swagger
```

### Passo 3: Testar os Endpoints

Clique em cada endpoint → "Try it out" → Preencha os dados → "Execute"

---

## 📝 Exemplos Rápidos

### Criar Tarefa

```json
POST /api/tarefa
Content-Type: application/json

{
  "titulo": "Estudar C#",
  "descricao": "Aprender async/await",
  "status": false
}

Resposta (201):
{
  "id": "550e8400-e29b-41d4-a716-446655440000",
  "titulo": "Estudar C#",
  "descricao": "Aprender async/await",
  "status": false
}
```

### Listar Todas as Tarefas

```json
GET /api/tarefa

Resposta (200):
[
  {
	"id": "550e8400-e29b-41d4-a716-446655440000",
	"titulo": "Estudar C#",
	"descricao": "Aprender async/await",
	"status": false
  },
  {
	"id": "660e8400-e29b-41d4-a716-446655440001",
	"titulo": "Tarefa concluída",
	"descricao": "Já foi feita",
	"status": true
  }
]
```

### Obter Uma Tarefa

```json
GET /api/tarefa/550e8400-e29b-41d4-a716-446655440000

Resposta (200):
{
  "id": "550e8400-e29b-41d4-a716-446655440000",
  "titulo": "Estudar C#",
  "descricao": "Aprender async/await",
  "status": false
}
```

### Deletar Tarefa Pendente ✅

```bash
DELETE /api/tarefa/550e8400-e29b-41d4-a716-446655440000

Resposta (204 No Content) ✅
```

### Tentar Deletar Tarefa Concluída ❌

```bash
DELETE /api/tarefa/660e8400-e29b-41d4-a716-446655440001

Resposta (400 Bad Request):
"Não é permitido excluir uma tarefa com status concluído (true)."
```

---

## 🧪 Teste Passo a Passo

### 1. Listar Tarefas Vazias
```
GET /api/tarefa
→ Resposta: []
```

### 2. Criar Primeira Tarefa
```
POST /api/tarefa
{
  "titulo": "Minha primeira tarefa",
  "status": false
}
→ Resposta: 201 Created com ID
→ Copie o ID
```

### 3. Criar Segunda Tarefa Concluída
```
POST /api/tarefa
{
  "titulo": "Tarefa concluída",
  "status": true
}
→ Resposta: 201 Created com ID
→ Copie este ID também
```

### 4. Listar Novamente
```
GET /api/tarefa
→ Resposta: [tarefa1, tarefa2]
```

### 5. Obter Primeira Tarefa
```
GET /api/tarefa/[ID_DA_TAREFA_1]
→ Resposta: tarefa1 com status: false
```

### 6. TESTE DA VALIDAÇÃO: Tentar Deletar Tarefa Concluída
```
DELETE /api/tarefa/[ID_DA_TAREFA_2]
→ Resposta: 400 Bad Request
→ Mensagem: "Não é permitido excluir uma tarefa com status concluído (true)."
✅ VALIDAÇÃO FUNCIONANDO!
```

### 7. Deletar Tarefa Pendente
```
DELETE /api/tarefa/[ID_DA_TAREFA_1]
→ Resposta: 204 No Content (sucesso)
```

### 8. Listar Novamente
```
GET /api/tarefa
→ Resposta: [tarefa2]
→ Tarefa 1 foi deletada com sucesso
```

---

## 📚 Documentação Disponível

| Arquivo | Descrição |
|---------|-----------|
| `README.md` | Documentação técnica completa |
| `STARTUP.md` | Como iniciar a aplicação |
| `API_EXAMPLES.md` | Exemplos de requisições |
| `VALIDATION_RULES.md` | Regras de validação |
| `DOCUMENTATION.md` | Índice geral |
| `QUICK_START.md` | Este arquivo (guia rápido) |

👉 **Comece pelo `README.md` para entendimento completo**

---

## 🔧 Usando cURL no Terminal

### Criar Tarefa
```bash
curl -X POST "http://localhost:5000/api/tarefa" \
  -H "Content-Type: application/json" \
  -d "{\"titulo\":\"Minha tarefa\",\"status\":false}"
```

### Listar Tarefas
```bash
curl -X GET "http://localhost:5000/api/tarefa"
```

### Deletar Tarefa (Teste a Validação)
```bash
# Primeiro crie uma tarefa com status true
curl -X POST "http://localhost:5000/api/tarefa" \
  -H "Content-Type: application/json" \
  -d "{\"titulo\":\"Concluída\",\"status\":true}"

# Pegue o ID retornado e tente deletar
# Vai retornar erro 400 ❌
curl -X DELETE "http://localhost:5000/api/tarefa/ID_AQUI"
```

---

## 📊 Respostas HTTP

| Status | Significado | Exemplo |
|--------|-----------|---------|
| `200` | Sucesso - Dados retornados | GET /api/tarefa |
| `201` | Criado - Novo recurso | POST /api/tarefa |
| `204` | Sem conteúdo - Deletado | DELETE /api/tarefa/{id} |
| `400` | Erro nos dados/validação | DELETE tarefa com status=true |
| `404` | Não encontrado | GET tarefa inexistente |

---

## ⚠️ Regras Importantes

### 1. Título Obrigatório
```json
❌ ERRO:
{
  "titulo": "",
  "status": false
}

✅ CORRETO:
{
  "titulo": "Uma tarefa",
  "status": false
}
```

### 2. Não Pode Deletar Tarefa Concluída
```
❌ NÃO FUNCIONA:
DELETE /api/tarefa/[id] quando status = true

✅ FUNCIONA:
DELETE /api/tarefa/[id] quando status = false
```

### 3. Tarefa Deve Existir
```
❌ NÃO FUNCIONA:
GET /api/tarefa/00000000-0000-0000-0000-000000000000

✅ FUNCIONA:
GET /api/tarefa/[um-id-valido-que-existe]
```

---

## 🎯 O Que Cada Status Significa

- **Status `false`** = Tarefa pendente (não feita)
  - ✅ Pode ser deletada

- **Status `true`** = Tarefa concluída (já feita)
  - ❌ Não pode ser deletada (validação implementada)

---

## 🛠️ Troubleshooting Rápido

### "Porta 5000 já está em uso"
```bash
dotnet run --urls "http://localhost:5001"
```

### "Erro de compilação"
```bash
dotnet clean
dotnet build
```

### "Swagger não aparece"
Certifique-se que a API está rodando em development:
```bash
dotnet run
```

---

## ✅ Checklist - Pronto para Usar?

- [ ] API iniciada com `dotnet run`
- [ ] Swagger acessível em `http://localhost:5000/swagger`
- [ ] Conseguir criar tarefa (POST)
- [ ] Conseguir listar tarefas (GET)
- [ ] Conseguir obter tarefa por ID (GET /{id})
- [ ] Conseguir deletar tarefa com status=false (DELETE)
- [ ] Receber erro 400 ao deletar tarefa com status=true (DELETE)

Se tudo funcionar → **API está pronta para uso! 🎉**

---

## 📞 Próximas Funcionalidades

Futuras implementações sugeridas:
- [ ] Autenticação (JWT)
- [ ] Atualização de tarefa (PUT/PATCH)
- [ ] Paginação
- [ ] Filtros (por status, data)
- [ ] Testes unitários
- [ ] Cache

---

## 🎓 Conceitos Usados

✅ REST API
✅ CRUD Operations
✅ Validações
✅ Tratamento de Exceções
✅ HTTP Status Codes
✅ Swagger/OpenAPI
✅ Dependency Injection
✅ Entity Framework Core

---

**Versão:** 1.0.0
**Status:** ✅ Completo e Funcional
**Data:** 2026-11-17

👉 **Comece agora! Execute `dotnet run` e abra o Swagger!** 🚀
