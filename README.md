# Questionarios Online

Plataforma em .NET 9 para criar pesquisas, coletar respostas e expor resultados agregados. Repo traz API, Portal Admin, Site Publico, Worker e docs de arquitetura (C4 + ADRs).

## Visao rapida
- **API**: ASP.NET Core Web API (Swagger), serviços de aplicacao e repositorios EF Core sobre SQL Server.
- **Portal Admin**: MVC/Razor com HttpClient para a API e sessao basica.
- **Web Publico**: MVC/Razor consumindo a API para exibir/enviar respostas.
- **Worker**: BackgroundService (esqueleto) para processar fila de respostas.
- **Docs**: C4 em `doc/c4.md` e ADRs em `doc/adr`.

## Estrutura do repo
- `src/Questionarios.Api`: Web API.
- `src/Questionarios.PortalAdmin`: portal interno.
- `src/Questionarios.WebPublico`: site para respondentes.
- `src/Questionarios.Worker`: worker de fila (atualizar agregados).
- `src/Questionarios.Application`: DTOs e serviços de aplicacao.
- `src/Questionarios.Domain`: entidades e contratos de repositorio.
- `src/Questionarios.Infrastructure`: EF Core, DbContext, cache/fila stubs.
- `sql/init_questionarios.sql`: schema + seed de exemplo.
- `tests/Questionarios.Tests.Integration`: integracao com WebApplicationFactory + banco InMemory.
- `tests/Questionarios.Tests.Unit`: esqueleto inicial.
- `doc/`: C4 (`c4.md`, `.mmd`), ADRs (`doc/adr`), notas (`arquitetura_notas.md`, `pending_work.md`, `testing_strategy.md`).

## Requisitos
- .NET SDK 9.0+
- Docker e Docker Compose (SQL Server)
- `sqlcmd` ou SSMS para aplicar seed

## Subir banco e seed
1) Container SQL Server:
```
docker compose up -d sqlserver
```
   - Usuario: `sa` / Senha: definida no compose (ver `docker-compose.yml`)
2) Aplicar schema+seed:
```
sqlcmd -S localhost,1433 -U sa -P <senha> -i sql/init_questionarios.sql
```
3) Para usar o site de Admin
```
user : admin@questionarios.com
senha : Admin@123
```
## Rodando local
Restaurar dependencias uma vez:
```
dotnet restore
```

- **API**  
```
dotnet run --project src/Questionarios.Api
```
Swagger: `http://localhost:5034/swagger`

- **Portal Admin** (usa API):  
```
dotnet run --project src/Questionarios.PortalAdmin
```
Configurar `ApiOptions:BaseUrl` em `appsettings.Development.json` para apontar para a API.

- **Web Publico** (usa API):  
```
dotnet run --project src/Questionarios.WebPublico
```
Configurar `ApiSettings:BaseUrl` e `DefaultSurveyId`.

- **Worker** (fila/aggregacao, esqueleto):  
```
dotnet run --project src/Questionarios.Worker
```

## Configuracao
- Connection string padrao (`DefaultConnection`) em `src/Questionarios.Api/appsettings.json`. Variavel equivalente: `ConnectionStrings__DefaultConnection`.
- Em container junto ao compose, use `Server=sqlserver,1433;...`.
- Cache e fila usam stubs (`InMemoryCacheService`, `ConsoleQueueClient`) e podem ser trocados por Redis/Service Bus implementando `ICacheService`/`IQueueClient`.

## Endpoints principais (API)
- `GET /api/survey/{id}`: detalhe com perguntas/opcoes.
- `POST /api/survey`: cria pesquisa.
- `POST /api/survey/{id}/close`: fecha pesquisa.
- `POST /api/responses`: envia respostas de um respondente (publica na fila).
- `GET /api/results/{surveyId}`: agregados.
- `GET /api/results/{surveyId}/chart`: pronto para graficos.
- `GET/POST/PUT/DELETE /api/users` + `POST /api/users/login`.

Credenciais seed: `admin@questionarios.com / Admin@123` e `editor@questionarios.com / Editor@123`.

## Testes
```
dotnet test QuestionariosOnline.sln
```
Estrategia e gaps documentados em `doc/testing_strategy.md`.

## Arquitetura e decisoes
- Diagramas C4 (contexto, container e componentes) em `doc/c4.md` e fontes Mermaid em `doc/*.mmd`.
- ADRs em `doc/adr` (`0001-geral`, `0002-api`, `0003-portal-admin`, `0004-web-publico`).
- Notas e backlog tecnico em `doc/arquitetura_notas.md` e `doc/pending_work.md`.

## Pendencias destacadas
- Implementar consumo real de fila no worker e trocas de stubs (Redis/Service Bus).
- Endurecer autenticacao/autorizar no Portal Admin e API.
- Garantir execucao de migrations no deploy e health checks de dependencias.
