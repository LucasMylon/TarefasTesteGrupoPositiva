# 📚 Documentação Completa - API de Tarefas

## 🎯 O que foi implementado

### ✅ 4 Métodos na TarefaController

1. **CREATE (POST)** - `Criar` - Criar nova tarefa
2. **READ (GET)** - `ObterPorId` - Obter tarefa específica
3. **READ (GET)** - `ObterTodos` - Listar todas as tarefas
4. **DELETE (DELETE)** - `Excluir` - Deletar tarefa com validação

### ✅ Validação Especial Implementada

**🔴 Regra Principal: NÃO PERMITIR EXCLUIR TAREFA COM STATUS CONCLUÍDO**

- Status `true` = Tarefa concluída
- Status `false` = Tarefa pendente
- Ao tentar deletar tarefa com `status: true`, retorna **HTTP 400 Bad Request**
- Mensagem clara: "Não é permitido excluir uma tarefa com status concluído (true)."

---

## 📖 Documentação Gerada

### 1. **README.md** ⭐ COMECE AQUI
Documentação completa da API com:
- Visão geral
- Todos os endpoints
- Parâmetros e respostas
- Fluxo de exemplo
- Instruções de teste no Swagger

**Como ler:** 
```
Este é o documento principal. Leia-o para entender toda a API.
```

### 2. **STARTUP.md** 🚀 COMO INICIAR
Guia passo a passo para executar a API:
- Pré-requisitos
- 3 opções de execução (Terminal, VS, VSCode)
- Primeira requisição
- Troubleshooting
- Checklist de verificação

**Como usar:**
```
Siga este guia para iniciar a aplicação e acessar o Swagger.
```

### 3. **API_EXAMPLES.md** 📝 EXEMPLOS PRÁTICOS
Exemplos prontos para copiar e colar:
- Requisições cURL
- Requisições Postman
- Requisições VSCode REST Client
- Fluxo de teste recomendado

**Como usar:**
```
Copie os exemplos e teste no Swagger ou terminal.
```

### 4. **VALIDATION_RULES.md** ✅ REGRAS DE NEGÓCIO
Detalhe completo das validações:
- Regra 1: Título obrigatório
- Regra 2: Tarefa deve existir
- **Regra 3: NÃO deletar tarefa concluída** ⭐ PRINCIPAL
- Regra 4: ID deve ser válido
- Regra 5: Request não nulo
- Tabela de respostas
- Fluxo de teste recomendado

**Como usar:**
```
Consulte para entender cada validação e como testá-la.
```

### 5. **Este Arquivo (DOCUMENTATION.md)**
Índice de toda a documentação.

---

## 🌐 Como Testar a API

### Opção 1: Swagger UI (Recomendado) ⭐

1. **Inicie a API:**
   ```bash
   cd TarefasTeste.Api
   dotnet run
   ```

2. **Abra no navegador:**
   ```
   http://localhost:5000/swagger
   ```

3. **Teste os endpoints:**
   - Clique em cada endpoint
   - Clique em "Try it out"
   - Preencha os dados
   - Clique em "Execute"

### Opção 2: Terminal (cURL)

```bash
# Criar tarefa
curl -X POST "http://localhost:5000/api/tarefa" \
  -H "Content-Type: application/json" \
  -d '{
	"titulo": "Minha tarefa",
	"status": false
  }'

# Listar tarefas
curl -X GET "http://localhost:5000/api/tarefa"
```

### Opção 3: Postman

Importe a coleção ou use os exemplos em `API_EXAMPLES.md`.

---

## 🔄 Fluxo Recomendado de Testes

### 1️⃣ Teste Básico (5 minutos)

```
Criar tarefa pendente → Listar → Obter por ID → Deletar
```

### 2️⃣ Teste da Validação Principal (3 minutos)

```
Criar tarefa concluída (status: true) → Tentar deletar → Ver erro 400
```

### 3️⃣ Teste Completo (10 minutos)

```
Criar tarefa 1 (status: false)
  ↓
Criar tarefa 2 (status: true)
  ↓
Listar (2 tarefas)
  ↓
Obter tarefa 1
  ↓
Tentar deletar tarefa 2 → Erro 400 ✅
  ↓
Deletar tarefa 1 → Sucesso 204 ✅
  ↓
Listar (1 tarefa) → Apenas tarefa 2
```

---

## 📊 Endpoints Rápido

| Método | Endpoint | Função | Status Sucesso |
|--------|----------|--------|---|
| POST | `/api/tarefa` | Criar tarefa | 201 |
| GET | `/api/tarefa` | Listar todas | 200 |
| GET | `/api/tarefa/{id}` | Obter por ID | 200 |
| DELETE | `/api/tarefa/{id}` | Deletar tarefa | 204 |

---

## 🛠️ Tecnologias Utilizadas

- **Framework:** ASP.NET Core 10.0
- **Padrão:** REST API
- **Documentação:** Swagger/OpenAPI
- **ORM:** Entity Framework Core
- **Banco:** SQL Server
- **Linguagem:** C#

---

## 📁 Estrutura de Arquivos

```
TarefasTeste/
│
├── 📄 README.md ⭐ Documentação principal
├── 📄 STARTUP.md 🚀 Como iniciar
├── 📄 API_EXAMPLES.md 📝 Exemplos de requisições
├── 📄 VALIDATION_RULES.md ✅ Regras de negócio
├── 📄 DOCUMENTATION.md 📚 Este arquivo
│
├── TarefasTeste.Api/
│   ├── Controllers/
│   │   └── TarefaController.cs ← 4 Métodos + Validações
│   ├── Program.cs ← Configuração Swagger
│   └── ...
│
├── TarefasTeste.Domain/
│   ├── Entities/Tarefa.cs
│   ├── Dtos/
│   │   ├── TarefaRequest.cs ← Documentado
│   │   └── TarefaResponse.cs ← Documentado
│   ├── Interfaces/
│   │   ├── ITarefaService.cs ← Novo método GetTarefaById
│   │   └── Repositories/ITarefaRepository.cs
│   └── Service/TarefaService.cs ← Validação implementada
│
└── TarefasTeste.Infra.Data/
	├── Repositories/
	│   └── TarefaRepository.cs
	└── Contexts/DataContext.cs
```

---

## ✨ Destaques Implementados

### 1. Documentação XML Completa
- ✅ Comentários em todos os métodos
- ✅ Documentação de parâmetros
- ✅ Documentação de retornos
- ✅ Exemplos nos comentários

### 2. Anotações ProducesResponseType
- ✅ Define respostas possíveis
- ✅ Integração com Swagger
- ✅ Facilita o entendimento da API

### 3. Tratamento de Erros
- ✅ Exceptions específicas
- ✅ Mensagens claras
- ✅ Status HTTP adequados

### 4. Validações Implementadas
- ✅ Título obrigatório
- ✅ Tarefa deve existir
- ✅ ID não pode ser vazio
- ✅ **NÃO deletar tarefa concluída** ⭐

---

## 🚀 Quick Start (3 minutos)

```bash
# 1. Ir para a pasta do projeto
cd C:\Users\Lucas\source\repos\TarefasTeste

# 2. Executar
cd TarefasTeste.Api
dotnet run

# 3. Abrir no navegador
http://localhost:5000/swagger

# 4. Pronto! Teste os endpoints diretamente no Swagger UI
```

---

## 📞 Como Usar Esta Documentação

### Se quer...

📌 **Entender a API completamente:**
→ Leia `README.md`

🚀 **Iniciar a aplicação:**
→ Siga `STARTUP.md`

📝 **Ver exemplos de requisições:**
→ Veja `API_EXAMPLES.md`

✅ **Entender as validações:**
→ Consulte `VALIDATION_RULES.md`

🧪 **Testar rapidamente:**
→ Use o Swagger em `http://localhost:5000/swagger`

---

## ✅ Validação Principal - Resumo

### O Requisito
"Crie uma validação na controller que não permita excluir tarefa com status true que será status concluída"

### A Implementação
```csharp
if (tarefa.Status)  // Status true = concluído
	throw new InvalidOperationException(
		"Não é permitido excluir uma tarefa com status concluído (true).");
```

### Como Testar

**No Swagger:**
1. POST → Criar tarefa com `"status": true`
2. Copiar ID
3. DELETE → Usar o ID
4. Ver erro 400 com mensagem clara

**No Terminal:**
```bash
# Tarefa com status true
curl -X POST "http://localhost:5000/api/tarefa" \
  -H "Content-Type: application/json" \
  -d '{"titulo": "Concluída", "status": true}'

# Tentar deletar (vai retornar erro 400)
curl -X DELETE "http://localhost:5000/api/tarefa/ID_DA_TAREFA"
```

---

## 🎓 Conceitos Demonstrados

✅ **REST API** - 4 métodos (CRUD)
✅ **Validações** - Regras de negócio
✅ **Documentação** - Swagger/OpenAPI
✅ **Tratamento de Erros** - Try/Catch com status HTTP apropriados
✅ **Padrão de Camadas** - Domain, Service, Controller, Repository
✅ **Injeção de Dependência** - Configurada no Program.cs
✅ **HTTP Status Codes** - 200, 201, 204, 400, 404

---

## 📋 Checklist Final

Antes de considerar o projeto pronto:

- ✅ 4 métodos implementados
- ✅ Validação principal funcionando
- ✅ Documentação XML completa
- ✅ Swagger configurado e funcionando
- ✅ Projeto compila sem erros
- ✅ README criado
- ✅ Exemplos de requisições prontos
- ✅ Guia de startup criado
- ✅ Regras de validação documentadas

---

## 🎉 Status Final

**A API está completa e pronta para uso!**

```
✅ Compilação: Sucesso
✅ Documentação: Completa
✅ Validações: Implementadas
✅ Swagger: Funcionando
✅ Exemplos: Prontos
```

---

**Versão:** 1.0.0
**Data:** 2026-11-17
**Desenvolvedor:** Copilot
**Status:** 🟢 PRONTO PARA PRODUÇÃO

---

## 📚 Índice de Documentação

1. **README.md** - Documentação técnica completa
2. **STARTUP.md** - Instruções de execução
3. **API_EXAMPLES.md** - Exemplos de requisições
4. **VALIDATION_RULES.md** - Regras e validações
5. **DOCUMENTATION.md** - Este arquivo (índice geral)

👉 **Comece pelo README.md!**
