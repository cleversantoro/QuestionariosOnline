# ADR 0003 - Portal Admin (MVC)

- **Status**: Aceito  
- **Decisao**: Implementar o Portal Admin em ASP.NET Core MVC (Razor), com session para manter o usuario autenticado e um HttpClient tipado (`QuestionariosApiClient`) para consumir a API via JSON. Controllers separados para login, dashboard, surveys, perguntas e opcoes; Views Razor renderizam os ViewModels retornados da API.
- **Contexto**: Analistas/admins precisam CRUD de pesquisas e visualizacao de charts sem instalar app nativo. A equipe domina MVC/Razor e deseja reusar DTOs e modelos proximos aos da API. O front deve permanecer fino, delegando regras e persistencia para a API.
- **Alternativas consideradas**:
  - SPA (React/Angular/Vue): descartado para evitar stack adicional e manter rapidez de entrega; MVC server-side suficiente para o escopo.
  - Acesso direto ao banco: descartado para preservar unica porta de entrada via API e manter regras centralizadas.
  - Autenticacao complexa (IdP/OIDC) neste momento: adiado; optou-se por sessao simples com login via endpoint `/api/users/login`.
- **Consequencias**:
  - Ciclo de entrega mais rapido, aproveitando layouts Razor e validação server-side.
  - Dependencia forte da disponibilidade da API; erros de rede devem ser tratados (hoje com alertas/logs basicos).
  - Sessao precisa de configuracao (cookie essential, idle timeout); futuras camadas de autorizacao e CSRF devem ser avaliadas para producao.
  - Caso o portal precise de interatividade avancada, pode ser necessario evoluir para SPA ou adicionar componentes HTMX/JS progressivos.
