# 🚀 Guia de Início Rápido - JWT Bearer Auth

## ⚡ 5 Minutos para Começar

### Passo 1: Configure a Chave Secreta (1 min)

**Windows (PowerShell como Admin):**
```powershell
[Environment]::SetEnvironmentVariable("JWT_SECRET_KEY", "dGVzdC1jaGF2ZS1zZWNyZXRhLWJhc2U2NC1sb25nbyO4=", "User")
```

**Linux/Mac:**
```bash
export JWT_SECRET_KEY="dGVzdC1jaGF2ZS1zZWNyZXRhLWJhc2U2NC1sb25nbyO4="
```

### Passo 2: Compile (1 min)

```bash
cd C:\Users\Lucas\OneDrive\Área de Trabalho\TesteGrupoPositiva\TarefasTeste
dotnet build
```

Deve retornar: ✅ **Compilação bem-sucedida**

### Passo 3: Execute (1 min)

```bash
dotnet run --project TarefasTeste.Api
```

Deve exibir:
```
Building...
info: Microsoft.Hosting.Lifetime[14]
	  Now listening on: http://localhost:5000
```

### Passo 4: Abra o Swagger (1 min)

Navegador: **http://localhost:5000/swagger**

### Passo 5: Teste Sem Token (1 min)

1. Clique em qualquer endpoint (ex: `GET /api/tarefa`)
2. Clique em "Try it out"
3. Clique em "Execute"
4. **Resultado esperado: 401 Unauthorized** ✅

---

## 🔐 Próximo Passo: Obter Token

Para usar a API, você precisa de um token JWT da **API de Usuários**.

### Opção A: Se API de Usuários está rodando

```bash
# Em outro terminal
curl -X POST "http://localhost:5236/api/usuario/autenticar" \
  -H "Content-Type: application/json" \
  -d '{
	"email": "usuario@example.com",
	"senha": "SuaSenha123"
  }'

# Resposta terá um campo "accessToken"
# Copie este valor
```

### Opção B: Usar token de teste

Você pode gerar um token de teste com a chave secreta que configurou.

---

## 🧪 Testar com Token

1. **No Swagger**, clique no botão **"Authorize"** (cadeado)
2. **Cole no campo:**
   ```
   Bearer seu-token-aqui
   ```
3. **Clique em "Authorize"**
4. **Clique em "Close"**
5. **Teste um endpoint:**
   - Clique em `POST /api/tarefa`
   - Clique em "Try it out"
   - Preencha:
	 ```json
	 {
	   "titulo": "Minha tarefa",
	   "descricao": "Teste",
	   "status": false
	 }
	 ```
   - Clique em "Execute"
   - **Resultado esperado: 201 Created** ✅

---

## 📊 Verificação Rápida

```
✅ Variável JWT_SECRET_KEY configurada?
✅ Projeto compila sem erros?
✅ API rodando em http://localhost:5000?
✅ Swagger acessível em http://localhost:5000/swagger?
✅ Endpoints retornam 401 sem token?
✅ Endpoints funcionam com token?
```

---

## 🆘 Se Algo Falhar

| Erro | Solução |
|------|---------|
| "JWT_SECRET_KEY não foi configurada" | Reinicie o terminal/IDE após configurar env var |
| Compilação falha | Rode `dotnet clean` depois `dotnet build` |
| Porta 5000 em uso | Mate o processo anterior ou use outra porta |
| 401 com token | Verificar se token é válido e não expirou |

---

## 📚 Próximos Passos

Leia a documentação apropriada:

- **Para testes:** [SWAGGER_TESTING_GUIDE.md](SWAGGER_TESTING_GUIDE.md)
- **Para configuração:** [JWT_CONFIGURATION.md](JWT_CONFIGURATION.md)
- **Para integração:** [API_INTEGRATION_GUIDE.md](API_INTEGRATION_GUIDE.md)
- **Para histórico:** [IMPLEMENTATION_SUMMARY.md](IMPLEMENTATION_SUMMARY.md)

---

## 🎯 Checklist de Status

- ✅ Compilação
- ✅ Variáveis de ambiente
- ⏳ Migração do banco (Pendente - veja IMPLEMENTATION_SUMMARY.md)
- ⏳ Integração com API de Usuários (Pendente - veja API_INTEGRATION_GUIDE.md)
- ⏳ Testes automatizados (Pendente - veja IMPLEMENTATION_CHECKLIST.md)

---

**🎉 Pronto! Sua API de Tarefas com autenticação JWT está rodando!**

Próximo: Leia [SWAGGER_TESTING_GUIDE.md](SWAGGER_TESTING_GUIDE.md) para testes completos.
