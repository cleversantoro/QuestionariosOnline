using FluentAssertions;
using Questionarios.Application.DTOs;
using Questionarios.Tests.Integration.Infrastructure;
using System.Net;
using System.Net.Http.Json;

namespace Questionarios.Tests.Integration.Surveys;

public class SurveyControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public SurveyControllerTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Get_DeveRetornarSurveyDetalhado_ComPerguntasEOpcoes()
    {
        var client = _factory.CreateClient();
        var surveyId = Guid.Parse("11111111-1111-1111-1111-111111111111");

        var response = await client.GetAsync($"/api/survey/{surveyId}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var survey = await response.Content.ReadFromJsonAsync<SurveyDetailDto>();
        survey.Should().NotBeNull();
        survey!.Id.Should().Be(surveyId);
        survey.Questions.Should().NotBeEmpty();
        foreach (var q in survey.Questions)
        {
            q.Options.Should().NotBeEmpty();
        }
    }
}
