# Estrategia de Testes

## Abrangencia atual
- Unitarios: `tests/Questionarios.Tests.Unit` (entidades/servicos ainda superficiais).
- Integracao API: `tests/Questionarios.Tests.Integration` cobre `/api/results/{surveyId}/chart` com `WebApplicationFactory` + EF InMemory/seed.

## Lacunas e plano de acao
- API/servicos: adicionar casos para `survey` (CRUD, datas invalidas), `responses` (survey fechado ou fora da janela), `questions/options` (ordem, updates).
- Dominio: validar regras das entidades `Survey`, `Question`, `Option`, `User` (datas, texto obrigatorio, unicidade de email, password hash).
- Acesso a dados: testes de repositorio com `UseSqliteInMemory` para CRUD e cenarios tracked/detached.
- Front MVC: smoke tests com `WebApplicationFactory` para Web Publico (renderizacao de pesquisa) e Portal Admin (dashboard carrega) + testes de cliente HTTP para submit do form.
- Worker/fila: quando implementado, criar testes de consumo de fila e idempotencia.

## Execucao
- Comando: `dotnet test QuestionariosOnline.sln`
- Ambiente de CI: incluir passo de restauracao, build, test; opcionalmente publicar cobertura (`coverlet.collector` ja referenciado nos unitarios).

## Dados e seed
- Integracao usa banco InMemory com seed equivalente ao script SQL; para cenarios reais, considerar usar `Testcontainers` ou `docker-compose` do SQL Server.
