using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Questionarios.Domain.Entities;
using Questionarios.Infrastructure;
using System;
using System.Linq;

namespace Questionarios.Tests.Integration.Infrastructure;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove o DbContext original (SQL Server)
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<QuestionariosDbContext>));

            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            // Adiciona DbContext InMemory
            services.AddDbContext<QuestionariosDbContext>(options =>
            {
                options.UseInMemoryDatabase("QuestionariosTestDb");
            });

            // Constrói o provider e faz SEED
            var sp = services.BuildServiceProvider();

            using var scope = sp.CreateScope();
            var scopedServices = scope.ServiceProvider;
            var db = scopedServices.GetRequiredService<QuestionariosDbContext>();

            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();

            SeedDatabase(db);
        });

        return base.CreateHost(builder);
    }

    private static void SeedDatabase(QuestionariosDbContext db)
    {
        // Mesmo cenário que o script SQL (simplificado)

        var surveyId = Guid.Parse("11111111-1111-1111-1111-111111111111");
        var survey = new Survey(
            "Pesquisa Eleitoral 2026 - Intenção de voto para Prefeito",
            DateTime.UtcNow.AddDays(-1),
            DateTime.UtcNow.AddDays(7)
        );

        // Força o Id igual ao script:
        typeof(Survey)
            .GetProperty(nameof(Survey.Id))!
            .SetValue(survey, surveyId);

        db.Surveys.Add(survey);

        // Perguntas
        var q1 = CreateQuestion(surveyId, "Se a eleição para prefeito fosse hoje, em quem você votaria?", 1);
        var q2 = CreateQuestion(surveyId, "Você pretende comparecer para votar nas próximas eleições?", 2);

        db.Questions.AddRange(q1, q2);

        // Opções Q1
        var o11 = CreateOption(q1.Id, "Candidato A", 1);
        var o12 = CreateOption(q1.Id, "Candidato B", 2);
        var o13 = CreateOption(q1.Id, "Branco/Nulo/Não sabe", 3);

        // Opções Q2
        var o21 = CreateOption(q2.Id, "Sim", 1);
        var o22 = CreateOption(q2.Id, "Não", 2);
        var o23 = CreateOption(q2.Id, "Não sabe", 3);

        db.Options.AddRange(o11, o12, o13, o21, o22, o23);

        // Algumas respostas pra não ficar vazio
        var r1 = new Response(surveyId, DateTime.UtcNow.AddMinutes(-30));
        var r2 = new Response(surveyId, DateTime.UtcNow.AddMinutes(-10));

        db.Responses.AddRange(r1, r2);

        r1.AddItem(q1.Id, o11.Id);
        r1.AddItem(q2.Id, o21.Id);

        r2.AddItem(q1.Id, o12.Id);
        r2.AddItem(q2.Id, o23.Id);

        db.SaveChanges();
    }

    private static Question CreateQuestion(Guid surveyId, string text, int order)
    {
        var q = new Question(surveyId, text, order);
        return q;
    }

    private static Option CreateOption(Guid questionId, string text, int order)
    {
        var o = new Option(questionId, text, order);
        return o;
    }
}
