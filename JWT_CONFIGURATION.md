# 🔐 Autenticação JWT Bearer - Guia de Configuração

## Visão Geral

A API de Tarefas agora está protegida com **autenticação JWT Bearer** integrada com a API de Usuários. Todos os endpoints requerem um token JWT válido no cabeçalho `Authorization`.

## Fluxo de Autenticação

```
1. Cliente faz login na API de Usuários
   POST /api/usuario/autenticar
   {
	 "email": "usuario@example.com",
	 "senha": "SuaSenha123"
   }

   ↓

2. API de Usuários valida credenciais e retorna AccessToken
   {
	 "id": "...",
	 "nome": "...",
	 "email": "usuario@example.com",
	 "perfil": "...",
	 "dataHoraAcesso": "...",
	 "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
   }

   ↓

3. Cliente usa o AccessToken em todas as requisições para API de Tarefas
   GET /api/tarefa
   Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...

   ↓

4. API de Tarefas valida o token e processa a requisição
   - Valida assinatura com a mesma chave secreta
   - Verifica Issuer, Audience e Expiração
   - Extrai email do usuário do token (claim ClaimTypes.Name)
   - Filtra tarefas por usuário

   ↓

5. Resposta é retornada (ou erro 401 Unauthorized se inválido)
```

---

## Configuração de Variáveis de Ambiente

### 🔒 Chave Secreta JWT (CRÍTICO)

A chave secreta **NÃO** deve estar em arquivos versionados. Use uma das opções abaixo:

#### Opção 1: Variável de Ambiente (Recomendado)

**Windows:**
```bash
setx JWT_SECRET_KEY "sua-chave-base64-aqui"
```

**Linux/Mac:**
```bash
export JWT_SECRET_KEY="sua-chave-base64-aqui"
```

#### Opção 2: User Secrets (Desenvolvimento)

```bash
cd TarefasTeste.Api
dotnet user-secrets init
dotnet user-secrets set "JwtSettings:SecretKey" "sua-chave-base64-aqui"
```

#### Opção 3: appsettings.Development.json (APENAS DESENVOLVIMENTO)

```json
{
  "JwtSettings": {
	"SecretKey": "sua-chave-base64-aqui",
	"Issuer": "Usuarios.App.API",
	"Audience": "Usuarios.App.Clients",
	"ExpirationMinutes": 30
  }
}
```

⚠️ **NUNCA** use appsettings.json em produção com a chave secreta!

### Obter Chave Secreta da API de Usuários

A chave secreta deve ser **idêntica** à da API de Usuários. Consulte a documentação da API de Usuários ou o administrador do sistema.

**Exemplo de chave Base64:**
```
1a2b3c4d5e6f7g8h9i0j1k2l3m4n5o6p7q8r9s0t1u2v3w4x5y6z7a8b9c0d=
```

---

## Configurações no appsettings.json

**Arquivo:** `TarefasTeste.Api/appsettings.json`

```json
{
  "Logging": {
	"LogLevel": {
	  "Default": "Information",
	  "Microsoft.AspNetCore": "Warning"
	}
  },
  "AllowedHosts": "*",
  "JwtSettings": {
	"Issuer": "Usuarios.App.API",
	"Audience": "Usuarios.App.Clients",
	"ExpirationMinutes": 30
  }
}
```

**Notas:**
- `Issuer`: Deve ser idêntico ao da API de Usuários
- `Audience`: Deve ser idêntico ao da API de Usuários
- `ExpirationMinutes`: Tempo de validade do token (padrão: 30 minutos)
- `SecretKey`: **Nunca** coloque no appsettings.json! Use variável de ambiente ou User Secrets

---

## Endpoints Protegidos

Todos os endpoints agora requerem um token JWT válido:

| Método | Endpoint | Autenticação |
|--------|----------|--------------|
| `POST` | `/api/tarefa` | ✅ Requerida |
| `GET` | `/api/tarefa` | ✅ Requerida |
| `GET` | `/api/tarefa/{id}` | ✅ Requerida |
| `PUT` | `/api/tarefa/{id}` | ✅ Requerida |
| `DELETE` | `/api/tarefa/{id}` | ✅ Requerida |

---

## Como Fazer Requisições Autenticadas

### Com cURL

```bash
# Passo 1: Fazer login e obter o token
curl -X POST "http://localhost:5236/api/usuario/autenticar" \
  -H "Content-Type: application/json" \
  -d '{
	"email": "usuario@example.com",
	"senha": "SuaSenha123"
  }'

# Resposta:
# {
#   "id": "...",
#   "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
# }

# Passo 2: Usar o token em requisições
TOKEN="eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."

curl -X GET "http://localhost:5000/api/tarefa" \
  -H "Authorization: Bearer $TOKEN"
```

### Com Postman

1. **Faça login primeiro:**
   - Método: `POST`
   - URL: `http://localhost:5236/api/usuario/autenticar`
   - Body (JSON):
   ```json
   {
	 "email": "usuario@example.com",
	 "senha": "SuaSenha123"
   }
   ```

2. **Copie o AccessToken da resposta**

3. **Configure o header para todos os endpoints:**
   - Vá para a aba "Headers"
   - Adicione:
	 ```
	 Key: Authorization
	 Value: Bearer seu-token-aqui
	 ```

4. **Faça requisições autenticadas**

### Com Swagger UI

1. **Clique no botão "Authorize" no Swagger**
2. **Cole o token no formato:** `Bearer seu-token-aqui`
3. **Clique em Authorize**
4. **Faça as requisições** (o token será incluído automaticamente)

---

## Erros e Respostas

### 401 Unauthorized

```json
{
  "error": "Unauthorized",
  "message": "Token inválido, expirado ou ausente. Use o header: Authorization: Bearer <token>"
}
```

**Causas:**
- Token ausente
- Token expirado
- Token inválido (assinatura inválida)
- Issuer ou Audience não correspondem
- Token não está no formato Bearer

### 403 Forbidden

```json
{
  "error": "Forbidden",
  "message": "Você não possui permissão para acessar este recurso."
}
```

**Causas:**
- Usuário não tem permissão para acessar o recurso

### 404 Not Found

```json
{
  "message": "Tarefa não encontrada."
}
```

**Causas:**
- Tarefa não existe
- Tarefa pertence a outro usuário

---

## Validação do Token

A API valida os seguintes aspectos do token JWT:

| Aspecto | Validação | Origem |
|---------|-----------|--------|
| **Assinatura** | HMAC-SHA256 com SecretKey | ✅ Implementado |
| **Issuer** | Deve ser "Usuarios.App.API" | ✅ Implementado |
| **Audience** | Deve ser "Usuarios.App.Clients" | ✅ Implementado |
| **Expiração** | Token não deve estar expirado | ✅ Implementado |
| **Email do Usuário** | Extraído de ClaimTypes.Name | ✅ Implementado |

---

## Isolamento de Dados por Usuário

Cada usuário só pode acessar suas próprias tarefas:

```csharp
// A API filtra automaticamente por email do usuário
var usuarioEmail = User.FindFirst(ClaimTypes.Name)?.Value;
var minhasTarefas = tarefaService.GetAllTarefas(usuarioEmail);
// Retorna apenas tarefas onde UsuarioEmail == usuarioEmail
```

**Benefícios:**
- 🔒 Segurança: Um usuário não pode acessar tarefas de outro
- 🔑 Privacidade: Dados completamente isolados
- 📊 Escalabilidade: Suporta múltiplos usuários simultaneamente

---

## Exemplo Completo: Login e Listar Tarefas

### Com cURL

```bash
#!/bin/bash

# URLs
USUARIO_API="http://localhost:5236/api/usuario"
TAREFA_API="http://localhost:5000/api/tarefa"

# Passo 1: Fazer login
echo "1. Autenticando..."
RESPONSE=$(curl -s -X POST "$USUARIO_API/autenticar" \
  -H "Content-Type: application/json" \
  -d '{
	"email": "teste@example.com",
	"senha": "Senha123"
  }')

# Extrair token
TOKEN=$(echo $RESPONSE | grep -o '"accessToken":"[^"]*' | cut -d'"' -f4)

if [ -z "$TOKEN" ]; then
  echo "Erro ao fazer login!"
  echo $RESPONSE
  exit 1
fi

echo "✅ Login bem-sucedido!"
echo "Token: $TOKEN"

# Passo 2: Criar uma tarefa
echo -e "\n2. Criando tarefa..."
CREATE_RESPONSE=$(curl -s -X POST "$TAREFA_API" \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
	"titulo": "Minha tarefa com autenticação",
	"descricao": "Testando JWT Bearer",
	"status": false
  }')

echo $CREATE_RESPONSE | jq .

# Passo 3: Listar tarefas
echo -e "\n3. Listando tarefas..."
LIST_RESPONSE=$(curl -s -X GET "$TAREFA_API" \
  -H "Authorization: Bearer $TOKEN")

echo $LIST_RESPONSE | jq .

echo -e "\n✅ Autenticação JWT funcionando!"
```

---

## Troubleshooting

### "Token inválido ou expirado"

**Verificar:**
1. Token está no formato correto? `Authorization: Bearer <token>`
2. Token está expirado? (padrão: 30 minutos)
3. Chave secreta está correta? Deve ser idêntica à API de Usuários
4. Issuer e Audience estão corretos?

### "Email do usuário não encontrado no token"

**Verificar:**
1. API de Usuários está colocando o email na claim `ClaimTypes.Name`?
2. Token foi gerado pela API de Usuários autêntica?

### "Uma política de Controle de Aplicativo bloqueou este arquivo"

**Solução:**
Desbloquear o arquivo DLL no Windows:
```bash
# PowerShell como Administrator
Unblock-File -Path "C:\Users\Lucas\...\TarefasTeste.Domain.dll"
```

---

## Configuração para Produção

### Checklist de Segurança

- ✅ Chave JWT armazenada em variável de ambiente segura
- ✅ HTTPS obrigatório (use `Scheme` HTTPS)
- ✅ Token com `RequireExpirationTime = true`
- ✅ `ClockSkew = TimeSpan.Zero` para validação rigorosa
- ✅ Senhas do banco de dados em variáveis de ambiente
- ✅ Logging desabilitado em produção (apenas erros críticos)
- ✅ CORS configurado apenas para domínios permitidos

### Variáveis de Ambiente Necessárias

```bash
# Chave secreta JWT (CRÍTICO)
JWT_SECRET_KEY=sua-chave-base64-aqui

# Conexão ao banco (se usar variáveis)
DB_CONNECTION_STRING=Server=...;Database=...;...

# Configurações de email (se usar)
SMTP_USERNAME=seu-email@example.com
SMTP_PASSWORD=sua-senha
```

---

## Links Úteis

- [Microsoft JWT Bearer Documentation](https://docs.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.authentication.jwtbearer)
- [JWT.io - Decodificador de Token](https://jwt.io)
- [RFC 7519 - JWT](https://tools.ietf.org/html/rfc7519)

---

**Versão:** 1.0.0  
**Data:** 2026-11-17  
**Status:** ✅ Pronto para Uso
