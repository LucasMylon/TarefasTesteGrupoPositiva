# 📚 Guia de Uso do Swagger para Testar a API de Tarefas

## Acessar o Swagger UI

1. **Inicie a aplicação:**
   ```bash
   dotnet run --project TarefasTeste.Api
   ```

2. **Abra no navegador:**
   ```
   http://localhost:5000/swagger/ui
   ```
   ou
   ```
   http://localhost:5000/swagger
   ```

---

## Passo 1: Obter um Token JWT

A primeira coisa que você precisa fazer é obter um token JWT da API de Usuários.

### No Swagger da API de Usuários

1. Acesse: `http://localhost:5236/swagger` (ou a porta configurada)
2. Encontre o endpoint `POST /api/usuario/autenticar`
3. Clique em **"Try it out"**
4. Preencha com um usuário válido:
   ```json
   {
	 "email": "usuario@example.com",
	 "senha": "SuaSenha123"
   }
   ```
5. Clique em **"Execute"**
6. **Copie o valor do campo `accessToken`** da resposta

---

## Passo 2: Configurar Autenticação no Swagger da API de Tarefas

Agora você precisa informar ao Swagger qual é o seu token de autenticação.

### Com a Versão Atualizada

Se a configuração de segurança estiver ativada no Swagger:

1. Clique no botão **"Authorize"** (cadeado) no topo direito do Swagger
2. No campo **"Value"**, cole:
   ```
   Bearer seu-token-aqui
   ```
   Exemplo:
   ```
   Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJlbWFpbCI6InVzdWFyaW9AZXhhbXBsZS5jb20iLCJpYXQiOjE2OTQ5MzIwMDB9.XXX...
   ```
3. Clique em **"Authorize"**
4. Clique em **"Close"**

Agora todos os seus endpoints terão este token automaticamente incluído!

### Sem Configuração de Segurança (Versão Manual)

Se a configuração de segurança não estiver no Swagger, você precisa adicionar manualmente o header em cada requisição:

1. Clique em **"Try it out"** em qualquer endpoint
2. Vá até a seção **"Headers"** (se visível)
3. Adicione um header:
   ```
   Key: Authorization
   Value: Bearer seu-token-aqui
   ```

---

## Passo 3: Testar os Endpoints

### 1. Listar Todas as Tarefas do Usuário

**Endpoint:** `GET /api/tarefa`

1. Clique em **"Try it out"**
2. Clique em **"Execute"**
3. Veja a resposta com todas as suas tarefas

**Resposta esperada (200 OK):**
```json
[
  {
	"id": "550e8400-e29b-41d4-a716-446655440000",
	"titulo": "Minha tarefa",
	"descricao": "Descrição da tarefa",
	"status": false,
	"dataHoraCriacao": "2026-11-17T14:30:00Z",
	"usuarioEmail": "usuario@example.com"
  }
]
```

**Se receber 401 Unauthorized:**
- Verifique se o token foi incluído corretamente
- Verifique se o token não expirou (padrão: 30 minutos)
- Obtenha um novo token

---

### 2. Criar uma Nova Tarefa

**Endpoint:** `POST /api/tarefa`

1. Clique em **"Try it out"**
2. Preencha o corpo (body) com:
   ```json
   {
	 "titulo": "Minha nova tarefa",
	 "descricao": "Esta é uma descrição detalhada",
	 "status": false
   }
   ```
3. Clique em **"Execute"**

**Resposta esperada (201 Created):**
```json
{
  "id": "660e8400-e29b-41d4-a716-446655440001",
  "titulo": "Minha nova tarefa",
  "descricao": "Esta é uma descrição detalhada",
  "status": false,
  "dataHoraCriacao": "2026-11-17T15:00:00Z",
  "usuarioEmail": "usuario@example.com"
}
```

---

### 3. Obter uma Tarefa Específica

**Endpoint:** `GET /api/tarefa/{id}`

1. Clique em **"Try it out"**
2. Cole o ID de uma tarefa no campo `id`
   ```
   660e8400-e29b-41d4-a716-446655440001
   ```
3. Clique em **"Execute"**

**Resposta esperada (200 OK):**
```json
{
  "id": "660e8400-e29b-41d4-a716-446655440001",
  "titulo": "Minha nova tarefa",
  "descricao": "Esta é uma descrição detalhada",
  "status": false,
  "dataHoraCriacao": "2026-11-17T15:00:00Z",
  "usuarioEmail": "usuario@example.com"
}
```

**Se receber 404 Not Found:**
- O ID pode estar incorreto
- Ou a tarefa pode pertencer a outro usuário

---

### 4. Atualizar uma Tarefa

**Endpoint:** `PUT /api/tarefa/{id}`

1. Clique em **"Try it out"**
2. Cole o ID da tarefa no campo `id`
3. Preencha o corpo com:
   ```json
   {
	 "titulo": "Tarefa atualizada",
	 "descricao": "Nova descrição",
	 "status": true
   }
   ```
4. Clique em **"Execute"**

**Resposta esperada (200 OK):**
```json
{
  "id": "660e8400-e29b-41d4-a716-446655440001",
  "titulo": "Tarefa atualizada",
  "descricao": "Nova descrição",
  "status": true,
  "dataHoraCriacao": "2026-11-17T15:00:00Z",
  "usuarioEmail": "usuario@example.com"
}
```

---

### 5. Excluir uma Tarefa

**Endpoint:** `DELETE /api/tarefa/{id}`

**⚠️ IMPORTANTE:** Você não pode excluir tarefas com status `true` (concluído).

1. Clique em **"Try it out"**
2. Cole o ID da tarefa no campo `id`
3. Clique em **"Execute"**

**Resposta esperada (204 No Content):**
- Sem corpo na resposta
- Status: 204

**Se receber 400 Bad Request:**
```json
{
  "message": "Não é permitido excluir tarefas com status concluído."
}
```

Neste caso, você precisa:
1. Primeiro atualizar a tarefa para status `false`
2. Depois excluir

---

## Troubleshooting

### Erro: "Token inválido, expirado ou ausente"

**Solução:**
1. Verifique se o token foi incluído no header `Authorization`
2. Verifique se está no formato: `Bearer <token>`
3. Obtenha um novo token (pode ter expirado)
4. Reautentique no Swagger

### Erro: "Tarefa não encontrada"

**Possíveis causas:**
1. O ID está incorreto
2. A tarefa foi excluída
3. A tarefa pertence a outro usuário (isolamento de dados)

### Erro: "Não é permitido excluir tarefas com status concluído"

**Solução:**
1. Primeiro, atualize a tarefa para status `false`
2. Depois, delete-a

### Erro: "Email do usuário não encontrado no token"

**Possíveis causas:**
1. O token não foi gerado pela API de Usuários
2. A claim de email não está no token

Obtenha um novo token da API de Usuários autêntica.

---

## Fluxo Completo de Teste

Aqui está um exemplo completo de como testar a API:

### 1. Login
```bash
# Terminal
curl -X POST "http://localhost:5236/api/usuario/autenticar" \
  -H "Content-Type: application/json" \
  -d '{
	"email": "usuario@example.com",
	"senha": "SuaSenha123"
  }'

# Copie o accessToken da resposta
```

### 2. Usar o Swagger
1. Abra http://localhost:5000/swagger
2. Clique em "Authorize"
3. Cole: `Bearer seu-token-aqui`
4. Clique "Authorize"

### 3. Testar Endpoints

```
✅ GET /api/tarefa - Listar tarefas
✅ POST /api/tarefa - Criar tarefa
✅ GET /api/tarefa/{id} - Ver tarefa
✅ PUT /api/tarefa/{id} - Atualizar tarefa
✅ DELETE /api/tarefa/{id} - Deletar tarefa (apenas se status = false)
```

---

## Testando Sem Autenticação (Deve Falhar)

Para verificar se a autenticação está funcionando corretamente:

1. No Swagger, clique em "Authorize" novamente
2. Limpe o campo (remova o token)
3. Clique em "Logout"
4. Tente acessar qualquer endpoint
5. Deve retornar **401 Unauthorized**

Teste com cada um:
- GET /api/tarefa → 401
- POST /api/tarefa → 401
- GET /api/tarefa/{id} → 401
- PUT /api/tarefa/{id} → 401
- DELETE /api/tarefa/{id} → 401

✅ Se todos retornarem 401, a autenticação está funcionando corretamente!

---

## Testando Isolamento de Dados

Para verificar se as tarefas estão isoladas por usuário:

### Cenário 1: Dois Usuários

1. **Usuário A:**
   - Faça login e obtenha token A
   - Crie uma tarefa
   - Veja `GET /api/tarefa` (deve listar apenas suas tarefas)

2. **Usuário B:**
   - Faça logout do Swagger
   - Faça login da Usuário B e obtenha token B
   - Reautentique no Swagger com token B
   - Veja `GET /api/tarefa` (deve listar apenas suas tarefas, não de A)

3. **Teste de Acesso Cruzado:**
   - Copie um ID de tarefa do Usuário A
   - Tente acessar com token B: `GET /api/tarefa/{id-de-A}`
   - Deve retornar **404 Not Found** ✅

---

## Dicas e Boas Práticas

✅ **Sempre teste sem token primeiro** - Deve retornar 401
✅ **Sempre teste com token expirado** - Deve retornar 401
✅ **Sempre teste com token inválido** - Deve retornar 401
✅ **Sempre teste o isolamento de dados** - Um usuário não deve ver tarefas de outro
✅ **Sempre teste as validações** - Não pode deletar tarefa concluída

---

**Versão:** 1.0.0  
**Data:** 2026-11-17  
**Status:** ✅ Pronto para Uso
