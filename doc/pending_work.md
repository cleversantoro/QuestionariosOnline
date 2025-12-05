# Pendências e próximos passos

Lista do que falta implementar para alinhar o código com a arquitetura planejada.

## Segurança
- Autenticação/autorização no Portal Admin e nos endpoints sensíveis (Identity/IdP + cookies/JWT e proteção de rotas).

## Infra de fila e cache
- Substituir `ConsoleQueueClient` por Service Bus/RabbitMQ.
- Trocar `InMemoryCacheService` por Redis, com health checks e configuração no deploy.

## Worker e agregação
- Implementar consumo da fila, criar `Response` a partir das mensagens e atualizar agregados/cache.
- Decidir estratégia de agregação: manter consulta on-the-fly ou pré-computar; remover `AggregatedResult` se não for usar.

## Observabilidade
- Configurar Serilog/OpenTelemetry: logs estruturados, traces e métricas.
- Adicionar health checks para API/Worker/DB/cache/fila.

## Testes e qualidade
- Ampliar cobertura: CRUD de API, respostas com survey fechado/fora da janela, domínio/serviços, repositórios EF, smoke de front MVC.
- Incluir execução no CI (`dotnet test QuestionariosOnline.sln`), opcionalmente com cobertura.

## Dados e deploy
- Garantir execução de migrations EF no deploy.
- Alinhar seed (script SQL vs. seeding via código) e documentar o procedimento.

## UX e proteção no Web Público
- Validar requisitos de limite de respostas/rate limit e melhorar feedback de erro no envio.
