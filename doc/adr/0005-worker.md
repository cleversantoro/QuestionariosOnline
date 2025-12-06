# ADR 0005 - Worker de Processamento

- **Status**: Aceito  
- **Decisao**: Manter um Worker em .NET (BackgroundService) dedicado a consumir mensagens de respostas via `IQueueClient`, atualizar agregados no SQL Server (`IResultsRepository`/`ISurveyRepository`), e invalidar/atualizar cache (`ICacheService`). O worker roda separado da API para nao impactar latencia dos requests.
- **Contexto**: Envio de respostas pode gerar carga de escrita e agregacao; processar tudo na API aumenta tempo de resposta e risco de timeouts. Ja existe abstraçao de fila/cache no projeto e um esqueleto de worker, mas falta a implementaçao real do consumo.
- **Alternativas consideradas**:
  - Processar sincrono na API: descartado para nao degradar UX e escalabilidade.
  - Jobs agendados (Hangfire/Quartz) na propria API: manteria acoplamento e nao resolveria burst de trafego; preferiu-se processo isolado.
  - Serverless functions no lugar de worker: possivel no futuro, mas exigiria mudar deploy/infra; mantido worker simples para compose atual.
- **Consequencias**:
  - Requer infraestrutura de fila em produçao (Service Bus/RabbitMQ) e health checks correspondentes.
  - Precisamos de idempotencia no processamento de mensagens e politicas de retry/poison queue.
  - Operacional: mais um deployable para monitorar (logs/metrics) e garantir rollout alinhado com migrations.
  - Em dev, `ConsoleQueueClient` e InMemory cache permitem rodar sem dependencias externas; produçao deve trocar implementaçoes.
