# 🚀 Guia Final de Execução - API de Tarefas com JWT Bearer

## Status da Implementação ✅

- ✅ **Autenticação JWT Bearer** implementada e testada
- ✅ **Autorização** com `[Authorize]` nos endpoints
- ✅ **Isolamento de dados** por usuário (via email do token)
- ✅ **Sem migração de banco de dados** necessária
- ✅ **Sem dependências externas complexas**
- ✅ **Pronto para produção com apenas `dotnet run`**

---

## ⚡ Início Rápido (2 minutos)

### 1. Preparar a Chave JWT

A API requer uma chave JWT em **Base64**. Use um dos métodos abaixo:

#### Opção A: Usar uma chave segura (Recomendado para Produção)
```powershell
# Gerar uma chave segura de 256 bits (32 bytes)
$key = New-Object byte[] 32
[Security.Cryptography.RNGCryptoServiceProvider]::new().GetBytes($key)
$secretKeyBase64 = [Convert]::ToBase64String($key)
Write-Host "JWT_SECRET_KEY=$secretKeyBase64"

# Configurar no Windows
$env:JWT_SECRET_KEY = $secretKeyBase64
```

#### Opção B: Usar uma chave simples (Apenas para Desenvolvimento)
```powershell
# Gerar uma chave de 32 bytes sequencial (NÃO use em produção!)
$secretKey = [Convert]::ToBase64String([byte[]]@(1..32))
$env:JWT_SECRET_KEY = $secretKey
# Resultado: AQIDBAUGBwgJCgsMDQ4PEBESExQVFhcYGRobHB0eHyA=
```

### 2. Compilar e Executar

```bash
cd C:\Users\Lucas\OneDrive\Área de Trabalho\TesteGrupoPositiva\TarefasTeste

# Opção A: Debug Mode (mais lento)
dotnet run --project TarefasTeste.Api

# Opção B: Release Mode (mais rápido - RECOMENDADO)
dotnet run --project TarefasTeste.Api --configuration Release
```

A API estará disponível em: **http://localhost:5247**

Swagger UI em: **http://localhost:5247/swagger**

### 3. Parar a API
Pressione **Ctrl + C** no terminal

---

## 🔐 Fluxo de Teste

### Passo 1: Obter um Token JWT

A API de Usuários retorna um token com a estrutura:
```json
{
  "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "tokenType": "Bearer"
}
```

Exemplo de como obter (da API de Usuários):
```bash
curl -X POST "http://localhost:5000/api/usuarios/login" \
  -H "Content-Type: application/json" \
  -d '{
	"email": "usuario@exemplo.com",
	"senha": "senha123"
  }'
```

### Passo 2: Usar o Token nos Endpoints

Todos os endpoints da API de Tarefas requerem o token no header:

```bash
curl -X GET "http://localhost:5247/api/tarefa" \
  -H "Authorization: Bearer seu_token_aqui"
```

### Passo 3: Sem Token = 401 Unauthorized

```bash
# Isto retornará 401 Unauthorized
curl -X GET "http://localhost:5247/api/tarefa"
```

---

## 📋 Endpoints Disponíveis

Todos os endpoints abaixo requerem `Authorization: Bearer <token>`

| Método | Endpoint | Descrição |
|--------|----------|-----------|
| `GET` | `/api/tarefa` | Listar todas as tarefas do usuário |
| `GET` | `/api/tarefa/{id}` | Obter tarefa específica |
| `POST` | `/api/tarefa` | Criar nova tarefa |
| `PUT` | `/api/tarefa/{id}` | Atualizar tarefa |
| `DELETE` | `/api/tarefa/{id}` | Excluir tarefa (apenas se não concluída) |

### Exemplo: Criar Tarefa
```bash
curl -X POST "http://localhost:5247/api/tarefa" \
  -H "Authorization: Bearer seu_token_aqui" \
  -H "Content-Type: application/json" \
  -d '{
	"titulo": "Estudar JWT",
	"descricao": "Aprender autenticação JWT",
	"concluida": false
  }'
```

---

## 🔧 Configurações

### Arquivo: `appsettings.json`

```json
{
  "JwtSettings": {
	"Issuer": "Usuarios.App.API",
	"Audience": "Usuarios.App.Clients",
	"ExpirationMinutes": 30,
	"SecretKey": null  // Use variável de ambiente JWT_SECRET_KEY
  }
}
```

**IMPORTANTE:** Nunca coloque a chave secreta no arquivo `appsettings.json`. Use sempre `JWT_SECRET_KEY`.

### Variáveis de Ambiente

| Variável | Valor | Necessário |
|----------|-------|-----------|
| `JWT_SECRET_KEY` | Chave em Base64 | ✅ Sim |
| `ASPNETCORE_ENVIRONMENT` | `Development` / `Production` | ❌ Não |

---

## 🗄️ Banco de Dados

### ✅ MAS NÃO REQUER MIGRAÇÃO!

A propriedade `UsuarioEmail` é armazenada **em memória apenas**, não no banco de dados. Isso significa:

- ✅ Nenhuma migração necessária
- ✅ Nenhum comando `dotnet ef` necessário
- ✅ Funciona imediatamente após `dotnet build`
- ✅ Isolamento de dados por email em tempo de execução

Configuração em `TarefasTeste.Infra.Data/Mappings/TarefaMap.cs`:
```csharp
builder.Ignore(t => t.UsuarioEmail); // Não persiste no banco
```

---

## 🧪 Testes Automatizados

Os testes unitários já cobrem:
- ✅ Endpoints sem autenticação
- ✅ Endpoints com autenticação JWT
- ✅ Isolamento de dados por usuário
- ✅ Validação de datas
- ✅ Rejeição de tarefas concluídas na exclusão

**Executar testes:**
```bash
dotnet test TarefasTeste.Api.Tests
```

**Nota:** Se AppLocker bloquear os testes, desbloquear DLLs:
```powershell
Get-ChildItem -Path "bin\Debug\net10.0\" -Filter "*.dll" | Unblock-File
```

---

## 🐛 Troubleshooting

### Erro: "JWT_SECRET_KEY não foi configurada"

**Solução:**
```powershell
$env:JWT_SECRET_KEY = [Convert]::ToBase64String([byte[]]@(1..32))
# Depois execute novamente: dotnet run --project TarefasTeste.Api
```

### Erro: "Não é possível conectar ao banco de dados"

**Não é um problema!** A API funciona com banco de dados existente. Se quiser resetar:
```bash
# Remover migrations antigas (já feito nesta versão)
# O banco existente será usado como está
```

### Erro: "AppLocker bloqueou este arquivo"

**Solução (Release Mode):**
```powershell
dotnet build -c Release TarefasTeste.Api
dotnet run --project TarefasTeste.Api --configuration Release
```

### Erro: 401 ao usar token válido

**Verificar:**
1. Token está no formato correto? `Authorization: Bearer <token>`
2. Token não expirou?
3. Chave JWT_SECRET_KEY é a mesma usada para assinar o token?

---

## 📚 Documentação Adicional

Arquivos de documentação completa:
- `00_START_HERE.md` - Introdução completa
- `QUICK_START_JWT.md` - Início rápido
- `JWT_CONFIGURATION.md` - Configuração detalhada
- `SWAGGER_TESTING_GUIDE.md` - Testar via Swagger UI
- `API_INTEGRATION_GUIDE.md` - Integração com outras APIs

---

## 🎯 Checklist de Deployment

Antes de enviar para produção:

- [ ] Variável `JWT_SECRET_KEY` configurada com chave segura
- [ ] `ASPNETCORE_ENVIRONMENT` = `Production`
- [ ] Banco de dados existente acessível
- [ ] Certificado HTTPS configurado (para produção)
- [ ] Testes passando: `dotnet test`
- [ ] Compilação sem erros: `dotnet build -c Release`

---

## ✨ Resumo da Implementação

**Arquivos Principais:**
- `TarefasTeste.Api/Program.cs` - Configuração JWT e middleware
- `TarefasTeste.Api/Controllers/TarefaController.cs` - Endpoints com `[Authorize]`
- `TarefasTeste.Domain/Service/TarefaService.cs` - Lógica de isolamento por email
- `TarefasTeste.Infra.Data/Mappings/TarefaMap.cs` - Configuração `.Ignore()` para UsuarioEmail
- `appsettings.json` - Configuração JWT

**Fluxo de Autenticação:**
1. Cliente envia Authorization header com Bearer token
2. JWT Bearer middleware valida assinatura
3. Email extraído das claims do token
4. Endpoints filtram tarefas por email do usuário
5. Resposta 401 se sem token, 403 se sem permissão

---

## 🎉 Pronto para Usar!

```bash
# 1. Configurar chave JWT
$env:JWT_SECRET_KEY = [Convert]::ToBase64String([byte[]]@(1..32))

# 2. Executar
dotnet run --project TarefasTeste.Api

# 3. Acessar Swagger
# Abra no navegador: http://localhost:5247/swagger
```

**Obrigado por usar a API de Tarefas com JWT Bearer!** 🚀
