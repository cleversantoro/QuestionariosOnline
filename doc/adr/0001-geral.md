# ADR 0001 - Arquitetura Geral do Questionarios

- **Status**: Aceito  
- **Decisao**: Adotar arquitetura em camadas com quatro containers principais (.NET 9): API, Portal Admin (MVC), Web Publico (MVC) e Worker. Persistencia central em SQL Server via EF Core; cache via interface `ICacheService` (InMemory/Redis) e fila via `IQueueClient` (stub/Service Bus ou RabbitMQ). Documentacao de arquitetura usando C4 em Mermaid.
- **Contexto**: O produto precisa coletar respostas de pesquisas e exibir resultados agregados para analistas. O time domina .NET/SQL Server e ja possui seeds e compose para esse stack. Ha necessidade de diferenciar front publico (sem login) e portal interno (CRUD).
- **Alternativas consideradas**:
  - Monolito unico MVC: descartado para nao acoplar front publico e admin, e para permitir exposicao da API a outros canais.
  - Persistencia NoSQL: descartado; modelo relacional com joins simples e integridade se alinha melhor a surveys/opcoes/respostas.
  - Abordagem puramente sincrona (sem fila/cache): aceita para MVP, mas mantida extensao para fila/cache para suportar volume.
- **Consequencias**:
  - Clareza de responsabilidades por deployable; facilita escalonar API separada dos fronts.
  - Dependencia forte em SQL Server/EF Core; migracoes precisam ser executadas no deploy.
  - Cache/fila sao abstracoes: InMemory/Console viabilizam dev, mas e necessario planejar operacionais (Redis/Service Bus) em producao.
  - Worker ainda e esqueleto; o backlog inclui implementar consumo real da fila e atualizar agregados/resultados.
