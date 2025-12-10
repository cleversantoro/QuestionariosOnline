# Documentacao de Testes

Este arquivo lista os testes criados recentemente, seu objetivo e como executa-los.

## Testes unitarios
- `tests/Questionarios.Tests.Unit/Api/ResponseServiceTests.cs`: valida que `ResponseService.SubmitAsync` publica `ResponseMessage` na fila quando a pesquisa esta ativa (fakes de repositorio/clock/queue).
- `tests/Questionarios.Tests.Unit/PortalAdmin/QuestionariosApiClientTests.cs`: garante que `QuestionariosApiClient.LoginAsync` retorna `null` em 401 e chama `/api/users/login`.
- `tests/Questionarios.Tests.Unit/WebPublico/SurveyControllerTests.cs`: verifica que `SurveyController.Index` exibe mensagem de erro quando nao ha pesquisa configurada.

## Testes de integracao (API)
- `tests/Questionarios.Tests.Integration/Survey/SurveyControllerTests.cs`: GET `/api/survey/{id}` retorna detalhe com perguntas e opcoes do seed.
- `tests/Questionarios.Tests.Integration/Responses/ResponsesControllerTests.cs`: POST `/api/responses` aceita envio para survey ativa usando ids do seed.
- `tests/Questionarios.Tests.Integration/Results/ResultsControllerTests.cs`: GET `/api/results/{id}/chart` retorna estrutura de grafico valida.

## Infra de teste
- `tests/Questionarios.Tests.Integration/Infrastructure/CustomWebApplicationFactory.cs`: usa ambiente `IntegrationTest`, troca SQL Server por `UseInMemoryDatabase`, cria nome unico por execucao e aplica seed padrao (survey + perguntas/opcoes/respostas).
- `src/Questionarios.Api/Program.cs`: habilita provider InMemory quando `ASPNETCORE_ENVIRONMENT=IntegrationTest`.

## Como rodar
```
dotnet test QuestionariosOnline.sln
```
Executa unitarios e integracao com o seed InMemory.
