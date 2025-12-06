# ADR 0006 - Estrategia de Testes (Unitarios e Integracao)

- **Status**: Aceito  
- **Decisao**: Manter combinacao de testes unitarios (isolados com fakes/stubs) e testes de integracao da API usando `WebApplicationFactory` com EF Core InMemory. Configurar o ambiente `IntegrationTest` para trocar SQL Server por banco em memoria e semear dados padrao.
- **Contexto**: Precisamos garantir comportamento basico da API, Portal Admin e Web Publico sem depender de infra externa. O seed ja existe e facilita cenarios repetiveis. Os testes unitarios atuais cobrem logica de fila/callback no client e validacao de fluxo no controller MVC publico; os de integracao verificam endpoints principais com dados coerentes.
- **Alternativas consideradas**:
  - Manter apenas unitarios: nao valida pipeline HTTP nem configuracao de DI/EF.
  - Usar banco real (SQL Server docker) nos testes: mais fiel, mas mais lento e frágil em CI; optou-se por InMemory para rapidez.
  - Mockar HttpClient nos testes de integracao: invalidaria objetivo de exercitar controllers e roteamento; preferido in-memory host.
- **Consequencias**:
  - Execucao rapida e deterministica (InMemory) com seed conhecido; nao cobre SQL especifico/migracoes — testes end-to-end com banco real podem ser adicionados depois.
  - `Program` passou a aceitar ambiente `IntegrationTest` e referencia `Microsoft.EntityFrameworkCore.InMemory`.
  - `CustomWebApplicationFactory` gerencia DbContext InMemory com nome unico e seed para evitar conflitos entre execucoes.
