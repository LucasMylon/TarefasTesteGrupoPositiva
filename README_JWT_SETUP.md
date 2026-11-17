# 🎯 API de Tarefas com JWT Bearer - Setup Completo

## 📋 Visão Geral

A **API de Tarefas (TarefasTeste)** agora possui autenticação **JWT Bearer** integrada com a **API de Usuários (Usuarios.App)**. 

Todos os endpoints requerem um token JWT válido e garantem isolamento de dados por usuário.

---

## 🚀 Setup Rápido (5 minutos)

### 1. Configurar Variável de Ambiente

**Windows (PowerShell como Admin):**
```powershell
[Environment]::SetEnvironmentVariable("JWT_SECRET_KEY", "sua-chave-base64-aqui", "User")
```

**Linux/Mac:**
```bash
export JWT_SECRET_KEY="sua-chave-base64-aqui"
```

### 2. Compilar

```bash
cd C:\Users\Lucas\OneDrive\Área de Trabalho\TesteGrupoPositiva\TarefasTeste
dotnet build
```

### 3. Executar Migrações do Banco (Pendente)

```bash
dotnet ef database update --project TarefasTeste.Infra.Data --startup-project TarefasTeste.Api
```

**⚠️ Nota:** Se receber erro de AppLocker, execute a migração manualmente ou consulte JWT_CONFIGURATION.md

### 4. Iniciar a API

```bash
dotnet run --project TarefasTeste.Api
```

Será escutado em: **http://localhost:5000**

### 5. Testar

Abra no navegador: **http://localhost:5000/swagger**

---

## 🔐 Configuração de Segurança

### Obter Chave Secreta

A chave deve ser **idêntica** à da API de Usuários. 

**Gerar uma chave Base64 (C#):**
```csharp
using System;
using System.Text;

var key = "sua-chave-secreta-minimo-24-caracteres";
var encoded = Convert.ToBase64String(Encoding.UTF8.GetBytes(key));
Console.WriteLine(encoded);
```

### Configuração Necessária

**Arquivo:** `TarefasTeste.Api/appsettings.json`
```json
{
  "JwtSettings": {
	"Issuer": "Usuarios.App.API",
	"Audience": "Usuarios.App.Clients",
	"ExpirationMinutes": 30
  }
}
```

**Variável de Ambiente:**
```
JWT_SECRET_KEY=sua-chave-base64-aqui
```

---

## 🔄 Fluxo Completo de Autenticação

```
1. Cliente faz LOGIN na API de Usuários
   POST /api/usuario/autenticar

   ↓

2. Recebe JWT (válido por 30 minutos)
   {
	 "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
   }

   ↓

3. Cliente usa o token em requisições para API de Tarefas
   GET /api/tarefa
   Headers: { Authorization: Bearer <token> }

   ↓

4. API valida o token e retorna dados do usuário
   {
	 "id": "...",
	 "titulo": "Minha tarefa",
	 "descricao": "...",
	 "status": false,
	 "usuarioEmail": "usuario@example.com"
   }
```

---

## 📡 Endpoints Disponíveis

Todos requerem token JWT no header `Authorization: Bearer <token>`

| Método | Endpoint | Descrição |
|--------|----------|-----------|
| `GET` | `/api/tarefa` | Listar tarefas do usuário |
| `POST` | `/api/tarefa` | Criar nova tarefa |
| `GET` | `/api/tarefa/{id}` | Obter tarefa específica |
| `PUT` | `/api/tarefa/{id}` | Atualizar tarefa |
| `DELETE` | `/api/tarefa/{id}` | Deletar tarefa ⚠️ |

⚠️ **Restrição Delete:** Não é permitido deletar tarefas com status `true` (concluído)

---

## 🧪 Testar com Curl

```bash
# 1. Fazer login na API de Usuários
TOKEN=$(curl -s -X POST "http://localhost:5236/api/usuario/autenticar" \
  -H "Content-Type: application/json" \
  -d '{
	"email": "usuario@example.com",
	"senha": "SuaSenha123"
  }' | jq -r '.accessToken')

# 2. Listar tarefas
curl -X GET "http://localhost:5000/api/tarefa" \
  -H "Authorization: Bearer $TOKEN"

# 3. Criar tarefa
curl -X POST "http://localhost:5000/api/tarefa" \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
	"titulo": "Minha tarefa",
	"descricao": "Descrição",
	"status": false
  }'
```

---

## 📊 Arquitetura

```
┌─────────────────────────────────────────────────────────┐
│                      Cliente                             │
└──────────────────────┬──────────────────────────────────┘
					   │
		┌──────────────┴───────────────┐
		│                              │
		▼                              ▼
   ┌─────────────┐           ┌──────────────────┐
   │ Users API   │           │ Tasks API        │
   │ (Auth)      │◄──────────│ (Protected)      │
   └─────────────┘ Valida    └──────────────────┘
		│          Token           │
		│                          │
		└──────────────┬───────────┘
					   │
					   ▼
			  ┌────────────────────┐
			  │  SQL Server DB     │
			  │ (Usuários + Tarefas│
			  └────────────────────┘
```

---

## 📝 Documentação Completa

Consulte os arquivos de documentação:

| Arquivo | Descrição |
|---------|-----------|
| **JWT_CONFIGURATION.md** | Configuração JWT, variáveis de ambiente, troubleshooting |
| **SWAGGER_TESTING_GUIDE.md** | Guia prático de testes via Swagger UI |
| **API_INTEGRATION_GUIDE.md** | Integração entre APIs, fluxo de autenticação |
| **IMPLEMENTATION_SUMMARY.md** | Resumo de todas as alterações realizadas |

---

## ✅ Validação Pós-Setup

### Checklist

- ✅ Projeto compila sem erros
- ✅ Variável de ambiente JWT_SECRET_KEY configurada
- ✅ Issuer/Audience em appsettings.json
- ⏳ Migração de banco de dados aplicada
- ⏳ API de Usuários rodando
- ⏳ API de Tarefas rodando
- ⏳ Token obtido com sucesso
- ⏳ Endpoints retornam 200 OK com token válido
- ⏳ Endpoints retornam 401 sem token
- ⏳ Isolamento de dados funcionando (usuário A não vê tarefas de B)

---

## 🔧 Troubleshooting

### "JWT_SECRET_KEY não foi configurada"

**Solução:** Configure a variável de ambiente (veja seção "Configurar Variável de Ambiente")

### "Token inválido ou expirado"

**Causas possíveis:**
1. Token expirou (padrão: 30 minutos)
2. Chave secreta não corresponde
3. Issuer/Audience não correspondem

**Solução:** Obtenha um novo token

### "Email do usuário não encontrado no token"

**Causa:** Token foi gerado por outro sistema

**Solução:** Obtenha token da API de Usuários autêntica

### "Tarefa não encontrada" (404)

**Causa:** Tarefa pertence a outro usuário (isolamento de dados)

**Esperado!** Significa que a segurança está funcionando.

---

## 🚀 Deployment para Produção

Antes de deployar, verifique:

- ✅ JWT_SECRET_KEY em variável de ambiente segura
- ✅ HTTPS obrigatório
- ✅ Token com expiração configurada
- ✅ CORS restrito a domínios permitidos
- ✅ Logging não expõe tokens
- ✅ Banco de dados migrado
- ✅ Testes passando
- ✅ Chave de assinatura segura (não em appsettings.json)

---

## 📞 Contato e Suporte

Para problemas com:
- **Configuração JWT:** Veja `JWT_CONFIGURATION.md`
- **Testes via Swagger:** Veja `SWAGGER_TESTING_GUIDE.md`
- **Integração de APIs:** Veja `API_INTEGRATION_GUIDE.md`
- **Histórico de Alterações:** Veja `IMPLEMENTATION_SUMMARY.md`

---

## 📚 Referências Úteis

- [Microsoft JWT Bearer Documentation](https://docs.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.authentication.jwtbearer)
- [JWT.io - Decodificador de Token](https://jwt.io)
- [RFC 7519 - JSON Web Token (JWT)](https://tools.ietf.org/html/rfc7519)

---

**Versão:** 1.0.0  
**Data:** 2026-11-17  
**Status:** ✅ Pronto para Uso

---

## 🎯 Próximos Passos

1. ✅ Ler este README
2. ✅ Configurar `JWT_SECRET_KEY`
3. ✅ Compilar o projeto
4. ⏳ Aplicar migração do banco
5. ⏳ Iniciar API de Usuários
6. ⏳ Iniciar API de Tarefas
7. ⏳ Testar via Swagger UI
8. ⏳ Validar integração

Consulte `SWAGGER_TESTING_GUIDE.md` para testes detalhados.
