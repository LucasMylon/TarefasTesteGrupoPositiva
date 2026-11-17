# 🔗 Guia de Integração: API de Usuários ↔ API de Tarefas

## Visão Geral da Arquitetura

```
┌──────────────────────────────────────────────────────────────┐
│                    Cliente (Web/Mobile)                       │
└───────────────────────┬──────────────────────────────────────┘
						│
		   ┌────────────┴────────────┐
		   │                         │
		   ▼                         ▼
	┌─────────────────────┐  ┌──────────────────┐
	│  API de Usuários    │  │  API de Tarefas  │
	│  (Usuarios.App)     │  │  (TarefasTeste)  │
	├─────────────────────┤  ├──────────────────┤
	│ POST /autenticar    │  │ GET /tarefa      │
	│ POST /registrar     │  │ POST /tarefa     │
	│ GET /perfil         │  │ PUT /tarefa/{id} │
	│                     │  │ DELETE /tarefa   │
	└─────────────────────┘  └──────────────────┘
			│                        │
			│                        │
			└────────────┬───────────┘
						 │
						 ▼
			┌──────────────────────────┐
			│   SQL Server Database    │
			│  (Usuarios + Tarefas)    │
			└──────────────────────────┘
```

## Fluxo de Autenticação e Autorização

```
1. Cliente faz LOGIN
   │
   ├─► POST /api/usuario/autenticar
   │   {
   │     "email": "usuario@example.com",
   │     "senha": "SuaSenha123"
   │   }
   │
   └─► Usuarios.App valida credenciais
	   ├─ ✅ Se válido:
	   │   ├─ Gera JWT Token
	   │   │  Claims: { email, role, exp, iss, aud }
	   │   └─ Retorna: { accessToken, ... }
	   │
	   └─ ❌ Se inválido:
		   └─ Retorna: 401 Unauthorized

2. Cliente usa TOKEN para acessar TAREFAS
   │
   ├─► GET /api/tarefa
   │   Headers: { Authorization: Bearer <token> }
   │
   └─► TarefasTeste.Api valida token
	   ├─ ✅ Se válido:
	   │   ├─ Extrai email do claim
	   │   ├─ Filtra tarefas por email
	   │   └─ Retorna: 200 OK com tarefas do usuário
	   │
	   └─ ❌ Se inválido:
		   └─ Retorna: 401 Unauthorized
```

---

## Configuração Necessária

### 1. API de Usuários (Usuarios.App)

**Status:** ✅ Já implementada

Deve emitir tokens JWT com:
```json
{
  "iss": "Usuarios.App.API",
  "aud": "Usuarios.App.Clients",
  "exp": 1234567890,
  "Name": "usuario@example.com",
  "Role": "user"
}
```

**Endpoints:**
- `POST /api/usuario/autenticar` - Login
- `POST /api/usuario/registrar` - Registro

### 2. API de Tarefas (TarefasTeste)

**Status:** ✅ Implementada com suporte a JWT

Valida tokens JWT com:
- Issuer: "Usuarios.App.API"
- Audience: "Usuarios.App.Clients"
- Chave de assinatura: `JWT_SECRET_KEY` (env var)

---

## Variáveis de Ambiente Necessárias

### API de Usuários (Usuarios.App)

```bash
# Chave secreta JWT (Base64)
JWT_SECRET_KEY=sua-chave-base64-aqui

# Conexão ao banco
DB_CONNECTION_STRING=Server=localhost;Database=...;...

# Issuer e Audience (em appsettings.json)
JwtSettings:Issuer=Usuarios.App.API
JwtSettings:Audience=Usuarios.App.Clients
JwtSettings:ExpirationMinutes=30
```

### API de Tarefas (TarefasTeste)

```bash
# Chave secreta JWT (DEVE SER IDÊNTICA à API de Usuários)
JWT_SECRET_KEY=sua-chave-base64-aqui

# Conexão ao banco (pode ser o mesmo banco ou banco separado)
DB_CONNECTION_STRING=Server=localhost;Database=...;...

# Issuer e Audience (em appsettings.json - devem corresponder)
JwtSettings:Issuer=Usuarios.App.API
JwtSettings:Audience=Usuarios.App.Clients
JwtSettings:ExpirationMinutes=30
```

**⚠️ CRÍTICO:** As chaves de ambiente devem ser idênticas entre as duas APIs!

---

## Teste de Integração: Passo a Passo

### Pré-requisitos

1. ✅ Ambas as APIs compilam sem erros
2. ✅ Ambas as APIs podem iniciar sem erros
3. ✅ Variável `JWT_SECRET_KEY` está configurada
4. ✅ Issuer/Audience correspondem

### Teste 1: Login e Obter Token

**Terminal 1: Iniciar API de Usuários**
```bash
cd C:\Users\Lucas\OneDrive\Área de Trabalho\TesteGrupoPositiva\Usuarios.App
dotnet run
# Listening on http://localhost:5236
```

**Terminal 2: Fazer Login**
```bash
curl -X POST "http://localhost:5236/api/usuario/autenticar" \
  -H "Content-Type: application/json" \
  -d '{
	"email": "usuario@example.com",
	"senha": "SuaSenha123"
  }'

# Resposta esperada:
# {
#   "id": "...",
#   "nome": "Usuário",
#   "email": "usuario@example.com",
#   "perfil": "user",
#   "dataHoraAcesso": "2026-11-17T...",
#   "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
# }
```

**Copie o valor do `accessToken`** ← Você vai usar isso a seguir

### Teste 2: Usar Token na API de Tarefas

**Terminal 3: Iniciar API de Tarefas**
```bash
cd C:\Users\Lucas\OneDrive\Área de Trabalho\TesteGrupoPositiva\TarefasTeste
dotnet run --project TarefasTeste.Api
# Listening on http://localhost:5000
```

**Terminal 4: Testar com Token**
```bash
# Substituir SEU_TOKEN_AQUI pelo accessToken do Teste 1
TOKEN="eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."

# Listar tarefas
curl -X GET "http://localhost:5000/api/tarefa" \
  -H "Authorization: Bearer $TOKEN"

# Resposta esperada (200 OK): [] ou lista de tarefas

# Criar tarefa
curl -X POST "http://localhost:5000/api/tarefa" \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
	"titulo": "Minha tarefa",
	"descricao": "Descrição",
	"status": false
  }'

# Resposta esperada (201 Created): { id, titulo, ... }
```

### Teste 3: Validar Isolamento de Dados

**Terminal 4: Criar Usuário 2 e Obter Token 2**
```bash
# Usar um email diferente
curl -X POST "http://localhost:5236/api/usuario/autenticar" \
  -H "Content-Type: application/json" \
  -d '{
	"email": "outro@example.com",
	"senha": "OutraSenha123"
  }'

# Copie o novo accessToken
TOKEN2="eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
```

**Terminal 4: Testar Isolamento**
```bash
# Usuário 1 (original)
TOKEN1="..."

# Usuário 2 (novo)
TOKEN2="..."

# Usuário 1 lista suas tarefas
curl -X GET "http://localhost:5000/api/tarefa" \
  -H "Authorization: Bearer $TOKEN1"
# Resposta: Tarefas de usuario@example.com

# Usuário 2 lista suas tarefas
curl -X GET "http://localhost:5000/api/tarefa" \
  -H "Authorization: Bearer $TOKEN2"
# Resposta: Tarefas de outro@example.com (vazio ou diferentes tarefas)

# Usuário 2 tenta acessar tarefa de Usuário 1
TASK_ID="<id-de-uma-tarefa-do-usuario-1>"
curl -X GET "http://localhost:5000/api/tarefa/$TASK_ID" \
  -H "Authorization: Bearer $TOKEN2"
# Resposta esperada: 404 Not Found ✅ (Isolamento funcionando!)
```

### Teste 4: Validar Rejeição Sem Token

```bash
# Sem token
curl -X GET "http://localhost:5000/api/tarefa"

# Resposta esperada (401 Unauthorized):
# {
#   "error": "Unauthorized",
#   "message": "Token inválido, expirado ou ausente. Use o header: Authorization: Bearer <token>"
# }
```

### Teste 5: Validar Rejeição com Token Inválido

```bash
# Token inválido
curl -X GET "http://localhost:5000/api/tarefa" \
  -H "Authorization: Bearer token-invalido-123"

# Resposta esperada (401 Unauthorized)
```

---

## Troubleshooting da Integração

### Erro: "JWT_SECRET_KEY não foi configurada"

**Causa:** Variável de ambiente não definida

**Solução:**
```bash
# Windows (PowerShell como Admin)
[Environment]::SetEnvironmentVariable("JWT_SECRET_KEY", "sua-chave-base64", "User")

# Depois reinicie a aplicação
```

### Erro: "Invalid issuer" ou "Invalid audience"

**Causa:** Configurações em appsettings.json não correspondem

**Solução:** Verifique ambos os arquivos `appsettings.json`:
```json
// Deve ser idêntico nas duas APIs
{
  "JwtSettings": {
	"Issuer": "Usuarios.App.API",
	"Audience": "Usuarios.App.Clients",
	"ExpirationMinutes": 30
  }
}
```

### Erro: "Invalid signature"

**Causa:** Chaves secretas não correspondem

**Solução:**
1. Ambas as APIs devem usar o mesmo valor de `JWT_SECRET_KEY`
2. Verifique que é Base64 válido
3. Não pode haver espaços em branco

### Erro: "Email do usuário não encontrado no token"

**Causa:** Token foi gerado por outro sistema ou não contém email

**Solução:** Verifique na API de Usuários se ela coloca o email na claim `ClaimTypes.Name`

### Erro: 404 Not Found ao tentar acessar tarefa de outro usuário

✅ **Esperado!** Isso significa que o isolamento de dados está funcionando corretamente!

---

## Verificação Pós-Integração

### Checklist

- ✅ API de Usuários emite token JWT
- ✅ Issuer e Audience correspondem
- ✅ JWT_SECRET_KEY é idêntica
- ✅ API de Tarefas valida token
- ✅ Tarefas são filtradas por usuário (email)
- ✅ Sem token retorna 401
- ✅ Com token inválido retorna 401
- ✅ Isolamento de dados funciona (usuário A não vê tarefas de B)
- ✅ Endpoints com [Authorize] protegem os dados
- ✅ Resposta 403 para usuários sem permissão (quando aplicável)

### Teste de Produção

Antes de deployar para produção:

1. ✅ Chave JWT armazenada em variável de ambiente segura
2. ✅ HTTPS obrigatório
3. ✅ Token com `RequireExpirationTime = true`
4. ✅ ClockSkew = TimeSpan.Zero
5. ✅ CORS configurado apenas para domínios permitidos
6. ✅ Logging seguro (sem expor tokens)
7. ✅ Tratamento de erros não expõe detalhes internos

---

## Fluxo Completo de Produção

```
Cliente
  │
  ├─ Login na API de Usuários
  │  POST /api/usuario/autenticar
  │  └─ Recebe JWT (válido por 30 min)
  │
  ├─ Armazena JWT localmente (localStorage, cookies)
  │
  ├─ Requisições para API de Tarefas
  │  GET /api/tarefa
  │  Headers: { Authorization: Bearer <JWT> }
  │  │
  │  └─ API valida:
  │     ├─ Assinatura (SymmetricKey)
  │     ├─ Issuer (Usuarios.App.API)
  │     ├─ Audience (Usuarios.App.Clients)
  │     ├─ Expiração
  │     └─ Extrai email do claim
  │  │
  │  └─ Filtra tarefas por email do usuário
  │     └─ Retorna apenas tarefas do usuário
  │
  └─ Quando expirado
	 └─ Cliente faz novo login
		└─ Obtém novo JWT
		   └─ Continua fazendo requisições
```

---

## Dependências e Limitações

### Dependências Atuais

- ✅ **API de Usuários emite email como claim**
  - Necessário para identificar o usuário
  - Realizado como `ClaimTypes.Name`

- ⚠️ **Sem ID do usuário no token**
  - Atualmente usa email como identificador
  - Email é único e funciona bem
  - Considerado suficiente para fase 1

### Possíveis Melhorias Futuras

1. **Adicionar UserId na claim do token**
   - Maior performance (não precisa buscar por email)
   - Mais seguro (email pode mudar)
   - Requer alteração em Usuarios.App

2. **Adicionar refresh token**
   - Permite sessões mais longas
   - Melhora UX (não desconecta tão rápido)
   - Requer alteração em Usuarios.App

3. **Integrar roles/permissions mais complexas**
   - Atualmente valida apenas presença de [Authorize]
   - Poderia usar [Authorize(Roles = "admin")]
   - Requer mapeamento de roles

---

**Versão:** 1.0.0  
**Data:** 2026-11-17  
**Status:** ✅ Pronto para Integração  
**Próximo Passo:** Executar testes de integração acima
