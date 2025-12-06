# ADR 0004 - Site Publico (MVC)

- **Status**: Aceito  
- **Decisao**: Construir o site publico em ASP.NET Core MVC com Razor, consumindo a API via HttpClient (`SurveyApiClient`). O controller `SurveyController` carrega a pesquisa (por `DefaultSurveyId` configurado) e envia respostas com POST JSON para a API. Renderizacao server-side simples (forms) para maximizar compatibilidade.
- **Contexto**: Respondentes nao autenticados precisam acessar rapidamente a pesquisa via navegador comum. A equipe deseja minimizar JS custom e manter o deploy alinhado ao restante do stack .NET. Validacoes basicas (perguntas obrigatorias, e-mail opcional) podem ser feitas server-side.
- **Alternativas consideradas**:
  - SPA ou micro-frontend: descartado para reduzir complexidade e custo de manutencao; SSR atende o caso de uso.
  - Servir diretamente da API (endpoints Razor Pages na API): evitado para separar responsabilidades e permitir evolucao independente.
- **Consequencias**:
  - Baixo acoplamento: site apenas orquestra chamadas HTTP para a API; sem acesso direto a dados.
  - Necessidade de resiliência: falha na API hoje exibe mensagem generica; melhorias futuras podem incluir retry/backoff e pagina de manutencao.
  - Layout simples; se forem exigidas personalizacoes/branding dinamico, pode ser necessario camada de CMS/temas.
