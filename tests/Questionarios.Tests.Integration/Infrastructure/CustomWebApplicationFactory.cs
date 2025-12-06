using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Questionarios.Domain.Entities;
using Questionarios.Infrastructure;

namespace Questionarios.Tests.Integration.Infrastructure;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.UseEnvironment("IntegrationTest");

        builder.ConfigureServices(services =>
        {
            services.RemoveAll(typeof(DbContextOptions<QuestionariosDbContext>));
            services.RemoveAll(typeof(QuestionariosDbContext));
            services.RemoveAll(typeof(Microsoft.Extensions.Options.IConfigureOptions<DbContextOptions<QuestionariosDbContext>>));
            services.RemoveAll(typeof(Microsoft.Extensions.Options.IOptions<DbContextOptions<QuestionariosDbContext>>));

            var dbName = $"QuestionariosTestDb-{Guid.NewGuid()}";
            services.AddDbContext<QuestionariosDbContext>(options =>
            {
                options.UseInMemoryDatabase(dbName);
            });

            var sp = services.BuildServiceProvider();

            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<QuestionariosDbContext>();
            db.Database.EnsureCreated();
            if (!db.Surveys.Any())
            {
                SeedDatabase(db);
            }
        });

        return base.CreateHost(builder);
    }

    private static void SeedDatabase(QuestionariosDbContext db)
    {
        var surveyId = Guid.Parse("11111111-1111-1111-1111-111111111111");
        var survey = new Survey(
            "Pesquisa Eleitoral 2026 - Intencao de voto para Prefeito",
            DateTime.UtcNow.AddDays(-1),
            DateTime.UtcNow.AddDays(7)
        );

        typeof(Survey).GetProperty(nameof(Survey.Id))!.SetValue(survey, surveyId);
        db.Surveys.Add(survey);

        var q1 = CreateQuestion(surveyId, "Se a eleicao para prefeito fosse hoje, em quem voce votaria?", 1);
        var q2 = CreateQuestion(surveyId, "Voce pretende comparecer para votar nas proximas eleicoes?", 2);

        db.Questions.AddRange(q1, q2);

        var o11 = CreateOption(q1.Id, "Candidato A", 1);
        var o12 = CreateOption(q1.Id, "Candidato B", 2);
        var o13 = CreateOption(q1.Id, "Branco/Nulo/Nao sabe", 3);

        var o21 = CreateOption(q2.Id, "Sim", 1);
        var o22 = CreateOption(q2.Id, "Nao", 2);
        var o23 = CreateOption(q2.Id, "Nao sabe", 3);

        db.Options.AddRange(o11, o12, o13, o21, o22, o23);

        var r1 = new Response(surveyId, DateTime.UtcNow.AddMinutes(-30));
        var r2 = new Response(surveyId, DateTime.UtcNow.AddMinutes(-10));

        db.Responses.AddRange(r1, r2);

        r1.AddItem(q1.Id, o11.Id);
        r1.AddItem(q2.Id, o21.Id);

        r2.AddItem(q1.Id, o12.Id);
        r2.AddItem(q2.Id, o23.Id);

        db.SaveChanges();
    }

    private static Question CreateQuestion(Guid surveyId, string text, int order) =>
        new(surveyId, text, order);

    private static Option CreateOption(Guid questionId, string text, int order) =>
        new(questionId, text, order);
}
