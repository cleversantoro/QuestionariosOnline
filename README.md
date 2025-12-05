# Questionarios Online

API em .NET 9 para criar pesquisas, coletar respostas e expor resultados agregados para graficos. O banco roda em SQL Server (imagem oficial) com script de seed em `sql/init_questionarios.sql`.

## Estrutura
- `src/Questionarios.Api`: Web API (Swagger, controllers de Survey, Responses e Results).
- `src/Questionarios.Application`: DTOs e servicos de aplicacao.
- `src/Questionarios.Domain`: Entidades e contratos de repositorio.
- `src/Questionarios.Infrastructure`: EF Core (DbContext, repositorios) e servicos auxiliares.
- `tests/Questionarios.Tests.Integration`: testes de integracao com WebApplicationFactory + banco InMemory.
- `tests/Questionarios.Tests.Unit`: esqueleto de testes unitarios.
- `sql/init_questionarios.sql`: cria DB, tabelas e dados de exemplo.

## Requisitos
- .NET SDK 9.0+
- Docker e Docker Compose (para o SQL Server local)
- `sqlcmd` ou SSMS para rodar o script de seed

## Subir o banco com Docker
1) Iniciar o container de SQL Server:
```
docker compose up -d sqlserver
```
   - Usuario: `sa` / Senha: `XXXXXXX` (definido no compose)
   - Porta exposta: 1433

2) Aplicar o script de schema + seed:
```
sqlcmd -S localhost,1433 -U sa -P XXXXXX -i sql/init_questionarios.sql
```

## Configuracao da API
- Connection string padrao em `src/Questionarios.Api/appsettings.json`:
  ```
  Server=localhost,1433;Database=QuestionariosDb;User ID=sa;Password=H56ut098;TrustServerCertificate=True;
  ```
- Variavel de ambiente equivalente: `ConnectionStrings__DefaultConnection`.
- Se rodar a API em container junto ao compose, use `Server=sqlserver,1433;...`.

## Executar a API localmente
```
dotnet restore
dotnet run --project src/Questionarios.Api
```
- Enderecos padrao: `http://localhost:5034` e `https://localhost:7145`.
- Swagger: `/swagger`.

## Endpoints principais
- `GET /api/survey/{id}`: detalhe da pesquisa com perguntas e opcoes.
- `POST /api/survey`: cria pesquisa.
- `POST /api/survey/{id}/close`: fecha pesquisa.
- `POST /api/responses`: envia respostas de um respondente.
- `GET /api/results/{surveyId}`: resultados agregados.
- `GET /api/results/{surveyId}/chart`: formato pronto para graficos (percentuais por opcao).
- `GET /api/users`: lista usuarios.
- `GET /api/users/{id}`: detalhe.
- `POST /api/users`: cria (nome, email, senha).
- `PUT /api/users/{id}`: atualiza dados e, opcionalmente, senha.
- `DELETE /api/users/{id}`: remove.
- `POST /api/users/login`: autentica (retorna usuario se ok, 401 se credencial invalida).

## Entidades de dominio
- `Survey`: raiz da pesquisa; tem `Title`, janela `StartAt/EndAt`, flag `IsClosed` e a colecao de `Questions`. Valida datas na criacao e sabe dizer se esta ativa (metodo `IsActive`) ou fechar (`Close`).
- `Question`: pergunta vinculada a um `Survey` (`SurveyId`), com `Text`, ordem opcional e lista de `Options`. Permite ajustar a ordem via `SetOrder`.
- `Option`: alternativa de uma `Question` (`QuestionId`), com texto e ordem opcional. Tambem tem `SetOrder`.
- `Response`: resposta de um respondente para um `Survey` (`SurveyId`) com `AnsweredAt` e colecao de `ResponseItem`. O metodo `AddItem` adiciona pares pergunta/opcao.
- `ResponseItem`: item de resposta granular, amarra `ResponseId`, `QuestionId` e `OptionId` (mais as navegacoes EF).
- `AggregatedResult`: estrutura para contagem agregada por `Survey/Question/Option` com `Votes` e o metodo `IncrementVote` para somar votos.

## Testes
- Testes de integracao usam banco InMemory com seed equivalente ao script SQL.
```
dotnet test QuestionariosOnline.sln
```

## Observacoes
- O seed cria uma pesquisa de exemplo com perguntas e opcoes fixas (GUIDs conhecidos) para facilitar debug e testes.
- O container `questionarios-sql` persiste dados em um volume Docker (`mssql_data`).
- Seed de usuarios para testes: `admin@questionarios.com` / `Admin@123` e `editor@questionarios.com` / `Editor@123`.
