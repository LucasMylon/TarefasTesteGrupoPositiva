# TarefasTeste

Aplicação de gerenciamento de tarefas composta por uma API ASP.NET Core, SQL Server, uma API de usuários separada (`Usuarios.App`) e o frontend Angular `TarefaWeb`.

## Requisitos

- .NET SDK 10 para esta API e seus testes.
- SQL Server 2022 local ou uma instância gerenciada.
- Docker Desktop, se for usar o SQL Server do `docker-compose.yml` incluído.
- Node.js e npm compatíveis com o Angular CLI 22 para o frontend.
- Repositórios `Usuarios.App` e `TarefaWeb`, que são separados desta solução.

## Inicialização local

### 1. Configurar o SQL Server

Na raiz deste repositório, inicie o container de desenvolvimento:

```powershell
docker compose up -d sqlserver
docker compose ps
```

O container publica o SQL Server em `localhost,1434`. O Compose é somente para desenvolvimento: suas credenciais são de demonstração e estão definidas no arquivo. Não use esse Compose, credenciais ou banco `master` em produção.

**Atenção:** no estado atual, `TarefasTeste.Infra.Data\Contexts\DataContext.cs` contém a connection string no código e aponta para o banco `master`. Antes de usar outro banco ou fazer deploy, altere o contexto para obter a connection string de configuração/segredo do ambiente e use um banco dedicado. Nunca publique credenciais.

### 2. Aplicar as migrações

Instale a ferramenta EF Core, se ainda não estiver disponível:

```powershell
dotnet tool install --global dotnet-ef --version 10.0.15
```

Confira as migrações e gere/aplique o script somente depois de confirmar o servidor e o banco de destino configurados em `DataContext.cs`:

```powershell
dotnet ef migrations list --project .\TarefasTeste.Infra.Data\TarefasTeste.Infra.Data.csproj --startup-project .\TarefasTeste.Infra.Data\TarefasTeste.Infra.Data.csproj
dotnet ef database update --project .\TarefasTeste.Infra.Data\TarefasTeste.Infra.Data.csproj --startup-project .\TarefasTeste.Infra.Data\TarefasTeste.Infra.Data.csproj
```

A migração de proprietário adiciona uma coluna opcional: tarefas antigas continuam sem proprietário e não são exibidas até serem atribuídas manualmente. Novas tarefas ficam associadas ao email do usuário autenticado.

### 3. Configurar a chave JWT

A API de tarefas e a API de usuários precisam usar **a mesma chave Base64 de pelo menos 256 bits**, o mesmo issuer e a mesma audience. No repositório `Usuarios.App`, siga `docs\GUIA-DE-USO.md` para configurar e iniciar a API de usuários. Para configurar a chave da API de tarefas localmente, salve a mesma chave já usada por `Usuarios.App` nos User Secrets; substitua o placeholder localmente e nunca o versione:

```powershell
dotnet user-secrets set "JwtSettings:SecretKey" "<MESMA_CHAVE_BASE64_DA_API_DE_USUARIOS>" --project .\TarefasTeste.Api\TarefasTeste.Api.csproj
```

O issuer e a audience da API de tarefas já estão definidos em `TarefasTeste.Api\appsettings.json` (`Usuarios.App.API` e `Usuarios.App.Clients`). Em produção, injete a chave por um cofre de segredos ou variável de ambiente `JWT_SECRET_KEY`.

### 4. Iniciar as APIs

Inicie a API de usuários em um terminal, usando o perfil HTTP do repositório `Usuarios.App` (porta `5236`):

```powershell
dotnet run --project <CAMINHO_DO_REPOSITORIO_USUARIOS>\Usuarios.App.API\Usuarios.App.API.csproj --launch-profile http
```

Em outro terminal, na raiz deste repositório, inicie a API de tarefas (porta `5247`):

```powershell
dotnet run --project .\TarefasTeste.Api\TarefasTeste.Api.csproj
```

No ambiente de desenvolvimento, o Swagger da API de tarefas fica em `http://localhost:5247/swagger`. As rotas da API exigem JWT Bearer; o Swagger é apenas uma interface de documentação e não encaminha automaticamente o token recebido pela API de usuários.

### 5. Iniciar o frontend

No repositório separado `TarefaWeb`, instale as dependências e inicie o servidor Angular:

```powershell
Set-Location <CAMINHO_DO_REPOSITORIO_TarefaWeb>
npm ci
npm.cmd start
```

Abra `http://localhost:4200`. O proxy de desenvolvimento já encaminha `/api/usuario/**` para `http://localhost:5236` e `/api/tarefa/**` para `http://localhost:5247`; veja `proxy.conf.json`. Faça login no próprio TarefaWeb e crie/lista tarefas na mesma aba. O token fica em memória, é enviado automaticamente à API de tarefas e é descartado ao sair, expirar ou atualizar a página.

## Build e testes

Na raiz desta solução:

```powershell
dotnet restore .\TarefasTeste.slnx
dotnet build .\TarefasTeste.slnx --no-restore
dotnet test .\TarefasTeste.Api.Tests\TarefasTeste.Api.Tests.csproj --no-restore
```

No repositório `TarefaWeb`:

```powershell
npm ci
npm.cmd test -- --watch=false
npm.cmd run build
```

## Publicação e deploy

### API de tarefas

**Não faça deploy com a configuração atual do banco:** antes, remova a connection string hardcoded de `DataContext.cs`, configure uma connection string segura para um banco dedicado e confirme que o ambiente de produção não usa o banco `master`. Rotacione também qualquer credencial que tenha sido usada em arquivos versionados.

Gere um script revisável das migrações, aplique-o ao banco de produção por um processo controlado (por exemplo, DBA/pipeline) e só então publique:

```powershell
New-Item -ItemType Directory -Force .\artifacts | Out-Null
dotnet ef migrations script --idempotent --project .\TarefasTeste.Infra.Data\TarefasTeste.Infra.Data.csproj --startup-project .\TarefasTeste.Infra.Data\TarefasTeste.Infra.Data.csproj --output .\artifacts\tarefas-migrations.sql
dotnet publish .\TarefasTeste.Api\TarefasTeste.Api.csproj --configuration Release --output .\publish\tarefas-api
```

O servidor de hospedagem da API precisa do .NET 10 Runtime, salvo se a publicação for configurada como self-contained.

No serviço de hospedagem da API, configure `ASPNETCORE_ENVIRONMENT=Production`, a connection string do banco por mecanismo seguro e `JWT_SECRET_KEY` com a mesma chave usada pela API de usuários. Configure HTTPS, restrinja acesso ao SQL Server e não exponha Swagger em produção (ele só é ativado em Development neste projeto).

### API de usuários e frontend

Publique a API de usuários seguindo as instruções do próprio repositório `Usuarios.App`; use HTTPS e mantenha issuer, audience e chave JWT compatíveis com a API de tarefas. Para o frontend, execute `npm ci` e `npm.cmd run build` no `TarefaWeb` e publique o conteúdo de `dist\tarefa-web\browser`. O proxy de `ng serve` é somente de desenvolvimento: em produção, configure o servidor web/reverse proxy para encaminhar `/api/usuario/**` e `/api/tarefa/**` às APIs correspondentes e configurar fallback de rotas Angular para `index.html`. Se frontend e APIs estiverem em origens diferentes, configure CORS apenas para as origens confiáveis e permita o cabeçalho `Authorization`.

## Endpoints da API

### Base URL
```
http://localhost:5247/api/tarefa
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

### 2. Obter as Tarefas do Usuário Autenticado (GET)

**Endpoint:** `GET /api/tarefa`

**Descrição:** Retorna somente as tarefas do proprietário identificado pelo token JWT.

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

### 4. Atualizar Tarefa (PUT)

**Endpoint:** `PUT /api/tarefa/{id}`

Envia todos os campos editáveis. Para concluir uma tarefa, envie `status: true`; para alterar o título ou a descrição sem mudar o status, envie o status atual.

```json
{
  "titulo": "Tarefa atualizada",
  "descricao": "Descrição atualizada",
  "status": true
}
```

Retorna `200 OK` com a tarefa atualizada e a data de criação original. Retorna `400 Bad Request` para ID inválido ou título vazio e `404 Not Found` se a tarefa não existir.

---

### 5. Excluir Tarefa (DELETE)

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

## Testar a API

A API está disponível em `http://localhost:5247` no perfil de desenvolvimento. O Swagger (`http://localhost:5247/swagger`) também é habilitado somente em Development. Todos os endpoints requerem o cabeçalho `Authorization: Bearer <token>` emitido pela API de usuários; para o fluxo normal, use o frontend TarefaWeb, que encaminha esse token após o login.

Títulos de tarefas são únicos na tabela. Cada usuário só pode consultar, atualizar e excluir as próprias tarefas. Tarefas antigas sem proprietário permanecem ocultas; tarefas concluídas não podem ser excluídas.


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

## Comportamento e limitações

- `dataHoraCriacao` é gerada pelo servidor em UTC e preservada nas atualizações. Tarefas existentes recebem a data UTC da aplicação da migração correspondente, pois a data original não foi registrada.
- O índice de título é único para toda a tabela, inclusive entre usuários.
- A autenticação JWT deve usar a mesma chave, issuer e audience nas duas APIs.
- O cadastro/login do TarefaWeb mantém o token somente em memória; após atualizar a página, faça login novamente.
- A connection string de SQL ainda está hardcoded em `DataContext.cs`; externalize-a antes de qualquer deploy.

**Versão da API:** 1.0.0
