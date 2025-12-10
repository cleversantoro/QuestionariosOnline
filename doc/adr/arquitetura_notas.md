# Complementos para a rubrica

Este arquivo documenta as escolhas de componentes, protocolo de comunicacao, testes e relacoes front/back para cobrir os pontos pendentes da rubrica.

## Componentes .NET e justificativas
- API ASP.NET Core Web API: expone endpoints REST em JSON para CRUD de pesquisas, perguntas, opcoes, respostas e resultados. Escolhida pela integracao nativa com DI, filtros, Swagger e facilidade de hospedar em conteiner.
- Front-end Portal Admin (ASP.NET Core MVC) e Web Publico (ASP.NET Core MVC/Razor): reutilizam a pilha .NET, facilitam o deploy conjunto e compartilham modelos/DTOs. Portal Admin consome `/api/results/{surveyId}/chart` para graficos; Web Publico consome `/api/survey/{id}` e envia respostas para `/api/responses`.
- Worker .NET: destinado a consumir fila (Service Bus/RabbitMQ) e processar respostas de forma assicrona. Hoje o worker e stub, mas o desenho cobre o papel dele na arquitetura.
- Acesso a dados com EF Core + SQL Server: mapeamento code-first em `QuestionariosDbContext`, repositorios para `Survey`, `Question`, `Option`, `Response`, `User` e consulta agregada para resultados. Escolha pela produtividade, tracking e suporte a SQL Server/Azure SQL.
- Cache (InMemory/Redis): no codigo atual ha `InMemoryCacheService`; o diagrama considera Redis para producao, reduzindo latencia em consultas de resultados agregados.
- Fila de mensagens: o diagrama preve Service Bus/RabbitMQ; no codigo atual existe `ConsoleQueueClient` como stub. O objetivo e desacoplar captura de respostas do processamento pesado/agragacao.
- Logs/monitoracao: previstos via Serilog/Application Insights (conforme diagrama). No codigo atual a API usa logging basico do ASP.NET; configurar sinks e dashboards e o proximo passo.

## Garantia de funcionamento e resiliencia (planejado)
- Health checks na API (SQL Server, cache, fila) e no Worker.
- Retries/policies (Polly) para chamadas externas (cache/fila) e resiliencia de acesso a dados.
- Observabilidade: request logging estruturado, traces de SQL (EF Core logging), metricas de requisicoes e filas. Integracao sugerida: Serilog + OpenTelemetry + Application Insights.
- Migrations: aplicar migrations EF no startup ou pipeline de deploy para manter schema alinhado.

## Protocolo de comunicacao front/back
- HTTP/HTTPS + JSON. Principais endpoints consumidos:
  - `GET /api/survey` (lista) e `GET /api/survey/{id}` (detalhe com perguntas/opcoes).
  - `POST /api/survey` e `PUT /api/survey/{id}` (cadastro/edicao), `DELETE /api/survey/{id}`.
  - `POST /api/questions` e `POST /api/questions/{questionId}/options` (cadastro de perguntas/opcoes).
  - `POST /api/responses` (envio de respostas).
  - `GET /api/results/{surveyId}/chart` (dados agregados para graficos).
- Payloads de resposta seguem os DTOs em `Questionarios.Application.DTOs` e espelhados nos view models dos clientes MVC.

## Relacao front/back
- Web Publico: carrega pesquisa (`GET /api/survey/{id}`), renderiza perguntas e posta respostas (`POST /api/responses`). Usa JavaScript/Chart.js somente no Portal Admin.
- Portal Admin: lista pesquisas (`GET /api/survey`), mostra graficos (`GET /api/results/{id}/chart`) e pode criar/remover pesquisas, perguntas e opcoes via demais endpoints.
- Clientes tipados: `Questionarios.PortalAdmin.Services.QuestionariosApiClient` e `Questionarios.WebPublico.Services.SurveyApiClient` (REST + JSON).

## Requisitos → componentes (resumo)
- Criar pesquisas/perguntas/opcoes: API ASP.NET Core + EF Core + SQL Server; UI via Portal Admin MVC.
- Coletar respostas: Web Publico MVC (form) → API `/api/responses` → fila/worker (planejado) → EF Core/SQL Server.
- Expor resultados agregados: API `/api/results/{id}/chart` + consulta agrupada em `ResultsRepository`; cache para acelerar leitura; Portal Admin exibe graficos.
- Autenticacao/seguranca (Portal Admin): prevista no diagrama via Identity/IdP; no codigo atual ainda nao implementada, manter prox passos.

## Testes (estrategia e cobertura)
- API/servicos: testes de integracao existentes em `tests/Questionarios.Tests.Integration` cobrem `/api/results/{id}/chart` com banco InMemory. Expandir para `/api/survey`, `/api/responses`, `/api/questions` incluindo cenarios de erro e datas de survey inativo.
- Dominio/aplicacao: adicionar testes unitarios para validacao de entidades (`Survey`, `Question`, `Option`, `Response`) e servicos (`SurveyService`, `QuestionService`, `UserService`), cobrindo regras de data e unicidade de email.
- Acesso a dados: testes de repositorio com `UseInMemoryDatabase` ou `Sqlite InMemory` para garantir CRUD e tracking/disconnected updates.
- Front-end MVC: smoke tests de renderizacao com `WebApplicationFactory` para Home/Survey, e testes de cliente HTTP para submissao do formulario publico.
- Automatizar no pipeline: `dotnet test QuestionariosOnline.sln` e, se possivel, adicionar lint/format.

## Proximos passos recomendados
- Implementar autenticacao/autorizacao conforme diagrama (Identity + cookies/JWT) protegendo Portal Admin e endpoints sensiveis.
- Substituir stubs de fila/cache por implementacoes reais (Service Bus/RabbitMQ e Redis) com health checks.
- Completar configuracao de logs/metricas (Serilog + OpenTelemetry + sink escolhido).
- Publicar diagramas e este arquivo no repositório de docs e referenciar no README principal.
