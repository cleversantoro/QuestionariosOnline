# Questionarios Online - Documentacao

## Visao geral
API .NET 9 para criar pesquisas, coletar respostas e expor resultados agregados, com dois fronts (Portal Admin e Web Publico) e suporte a fila/cache. Diagrama de arquitetura em `doc/arquitetura_questionarios.pdf` e diagramas de sequencia em `doc/mermaid-*.png`.

## Solucao e componentes
- API: ASP.NET Core Web API (`src/Questionarios.Api`), endpoints REST/JSON, Swagger, DI.
- Dominio/Aplicacao: entidades, DTOs e servicos (`src/Questionarios.Domain`, `src/Questionarios.Application`).
- Infra: EF Core + SQL Server (`QuestionariosDbContext`), repositorios, cache em memoria (stub Redis), fila (Console stub), logging basico (`src/Questionarios.Infrastructure`).
- Fronts:
  - Portal Admin (`src/Questionarios.PortalAdmin`, MVC): consome API para listar pesquisas e graficos.
  - Web Publico (`src/Questionarios.WebPublico`, MVC/Razor): exibe pesquisa e envia respostas.
- Worker (`src/Questionarios.Worker`): previsto para processar fila de respostas (stub atual).

## Fluxos-chave
- Coleta de respostas: Web Publico -> `POST /api/responses` -> (fila/worker planejados) -> EF Core/SQL.
- Resultados agregados: `GET /api/results/{surveyId}/chart` agrupa respostas por pergunta/opcao e retorna dados para graficos; Portal Admin consome.
- CRUD de pesquisas/perguntas/opcoes: endpoints `api/survey`, `api/questions`.

## Endpoints principais
- `GET /api/survey` / `GET /api/survey/{id}`: lista/detalhe com perguntas e opcoes.
- `POST /api/survey`, `PUT /api/survey/{id}`, `DELETE /api/survey/{id}`: gestao de pesquisas.
- `POST /api/questions`, `POST /api/questions/{questionId}/options`, `PUT/DELETE` correspondentes.
- `POST /api/responses`: envio de respostas.
- `GET /api/results/{surveyId}/chart`: dados agregados para graficos.

## Projetos e relacao front/back
- Portal Admin usa `QuestionariosApiClient` (HTTP/JSON) para consumir `survey` e `results/chart`.
- Web Publico usa `SurveyApiClient` para carregar pesquisa e postar respostas.
- DTOs compartilhados em `Questionarios.Application.DTOs` e view models correspondentes nos fronts.

## Banco de dados e seed
- Script SQL em `sql/init_questionarios.sql` (cria tabelas, seed de pesquisa/exemplos, dados agregados).
- Connection string padrao: `Server=localhost,1433;Database=QuestionariosDb;User ID=sa;Password=H56ut098;TrustServerCertificate=True;`.
- Suba o SQL Server via `docker-compose up -d sqlserver` e rode o script com `sqlcmd` ou SSMS.

## Como rodar (dev)
1) `dotnet restore`
2) API: `dotnet run --project src/Questionarios.Api` (Swagger em `/swagger`).
3) Portal Admin: `dotnet run --project src/Questionarios.PortalAdmin`
4) Web Publico: `dotnet run --project src/Questionarios.WebPublico`

## Testes
- Unitarios: `tests/Questionarios.Tests.Unit` (entidades/servicos a expandir).
- Integracao: `tests/Questionarios.Tests.Integration` cobre `/api/results/{id}/chart` com banco InMemory.
- Rode: `dotnet test QuestionariosOnline.sln`
- Ver estrategia detalhada em `doc/testing_strategy.md`.

## Observabilidade e resiliencia (planejado)
- Health checks para SQL/cache/fila.
- Logs estruturados (Serilog) + OpenTelemetry/AI.
- Policies de retry (Polly) para cache/fila.
- Migrations EF aplicadas no deploy.

## Referencias
- Diagramas: `doc/arquitetura_questionarios.pdf`, `doc/mermaid-diagram-*.png`.
- Notas adicionais: `doc/arquitetura_notas.md`.
