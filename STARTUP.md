# 🚀 Guia de Inicialização - API de Tarefas

## Pré-requisitos

- .NET 10.0 SDK instalado
- Visual Studio 2026 Community (opcional) ou VSCode
- SQL Server (ou LocalDB)

---

## Passo a Passo para Executar a API

### Opção 1: Usando o Terminal

#### 1. Abra o terminal na pasta do projeto

```bash
cd C:\Users\Lucas\source\repos\TarefasTeste
```

#### 2. Restaurar as dependências

```bash
dotnet restore
```

#### 3. Executar a aplicação

```bash
cd TarefasTeste.Api
dotnet run
```

#### Resultado esperado:

```
info: Microsoft.Hosting.Lifetime[14]
	  Now listening on: http://localhost:5000
info: Microsoft.Hosting.Lifetime[0]
	  Application started. Press Ctrl+C to exit
```

### Opção 2: Usando Visual Studio 2026

1. Abra a solução `TarefasTeste.slnx` no Visual Studio
2. Clique com botão direito em **TarefasTeste.Api** → Definir como Projeto de Inicialização
3. Pressione **F5** para executar com debug
4. A aplicação será aberta automaticamente no navegador

### Opção 3: Usando VSCode

1. Abra a pasta `TarefasTeste` no VSCode
2. Abra o terminal integrado (Ctrl + `)
3. Execute:
   ```bash
   cd TarefasTeste.Api
   dotnet run
   ```

---

## 🌐 Acessar a API

Após a aplicação iniciar, acesse:

### Swagger UI
```
http://localhost:5000/swagger
```

A interface do Swagger permite:
- ✅ Visualizar todos os endpoints
- ✅ Ver a documentação de cada método
- ✅ Testar os endpoints diretamente
- ✅ Ver exemplos de request/response

### OpenAPI JSON
```
http://localhost:5000/openapi/v1.json
```

Contém a especificação completa da API em formato JSON/YAML.

---

## 📝 Primeira Requisição

### No Swagger UI:

1. Navegue até `http://localhost:5000/swagger`
2. Clique em **POST /api/tarefa**
3. Clique em **"Try it out"**
4. Preencha o corpo:
   ```json
   {
	 "titulo": "Minha primeira tarefa",
	 "descricao": "Teste da API",
	 "status": false
   }
   ```
5. Clique em **"Execute"**
6. Veja o resultado 201 Created

### Pelo Terminal (cURL):

```bash
curl -X POST "http://localhost:5000/api/tarefa" \
  -H "Content-Type: application/json" \
  -d '{
	"titulo": "Minha primeira tarefa",
	"descricao": "Teste da API",
	"status": false
  }'
```

---

## 🔍 Testando Todos os Endpoints

### 1. Listar Tarefas
```bash
curl -X GET "http://localhost:5000/api/tarefa"
```

### 2. Obter Tarefa Específica
```bash
# Substitua o UUID por um ID válido
curl -X GET "http://localhost:5000/api/tarefa/550e8400-e29b-41d4-a716-446655440000"
```

### 3. Testar Validação (Excluir Tarefa Concluída)
```bash
# Primeiro, crie uma tarefa com status true
curl -X POST "http://localhost:5000/api/tarefa" \
  -H "Content-Type: application/json" \
  -d '{
	"titulo": "Tarefa concluída",
	"status": true
  }'

# Pegue o ID retornado e tente deletar
# Vai retornar erro 400
curl -X DELETE "http://localhost:5000/api/tarefa/ID_DA_TAREFA"
```

---

## 🛠️ Troubleshooting

### A porta 5000 já está em uso

Se a porta 5000 está ocupada, você pode especificar outra porta:

```bash
dotnet run --urls "http://localhost:5001"
```

### Erro de compilação

Certifique-se que todas as referências de projeto estão corretas:

```bash
dotnet build
```

Se ainda houver erro, limpe e reconstrua:

```bash
dotnet clean
dotnet build
```

### Banco de dados não conecta

Verifique se o SQL Server ou LocalDB está rodando.

---

## 📊 Estrutura de Pastas

```
TarefasTeste/
├── TarefasTeste.Api/              ← Projeto principal da API
│   ├── Controllers/
│   │   └── TarefaController.cs   ← Endpoints da API
│   ├── Program.cs                 ← Configuração e startup
│   ├── appsettings.json           ← Configurações
│   └── TarefasTeste.Api.csproj
├── TarefasTeste.Domain/           ← Lógica de negócio
│   ├── Entities/
│   ├── Dtos/
│   ├── Interfaces/
│   └── Service/
├── TarefasTeste.Infra.Data/       ← Acesso a dados
│   ├── Repositories/
│   └── Contexts/
├── README.md                       ← Documentação completa
├── API_EXAMPLES.md                 ← Exemplos de requisições
└── STARTUP.md                      ← Este arquivo
```

---

## 🔐 Configurações Importantes

### Variáveis de Ambiente

No arquivo `appsettings.Development.json`, você pode configurar:

```json
{
  "Logging": {
	"LogLevel": {
	  "Default": "Information",
	  "Microsoft.AspNetCore": "Warning"
	}
  },
  "ConnectionStrings": {
	"DefaultConnection": "Server=.;Database=TarefasDb;Trusted_Connection=true;"
  }
}
```

### Cors (Se Necessário)

Adicione ao `Program.cs`:

```csharp
builder.Services.AddCors(options =>
{
	options.AddPolicy("AllowAll",
		builder =>
		{
			builder.AllowAnyOrigin()
				   .AllowAnyMethod()
				   .AllowAnyHeader();
		});
});

// E na configuração do app:
app.UseCors("AllowAll");
```

---

## 📚 Documentação Adicional

- [README.md](./README.md) - Documentação completa da API
- [API_EXAMPLES.md](./API_EXAMPLES.md) - Exemplos de requisições
- [Microsoft .NET Documentation](https://docs.microsoft.com/dotnet/)
- [Swagger/OpenAPI](https://swagger.io/)

---

## ✅ Checklist de Verificação

Após iniciar a aplicação, verifique:

- [ ] API rodando em `http://localhost:5000`
- [ ] Swagger acessível em `http://localhost:5000/swagger`
- [ ] Conseguir criar uma tarefa (POST)
- [ ] Conseguir listar tarefas (GET)
- [ ] Conseguir obter tarefa por ID (GET /{id})
- [ ] Validação funciona: não conseguir deletar tarefa com status true
- [ ] Conseguir deletar tarefa com status false

---

## 🆘 Precisa de Ajuda?

Se encontrar problemas:

1. Verifique o console do Visual Studio/Terminal para mensagens de erro
2. Consulte a seção de Troubleshooting acima
3. Revise os logs em `appsettings.json`

---

**Versão:** 1.0.0  
**Última atualização:** 2026-11-17  
**Status:** ✅ Pronto para uso
