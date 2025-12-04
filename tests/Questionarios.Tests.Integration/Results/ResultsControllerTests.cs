using FluentAssertions;
using Questionarios.Application.DTOs;
using Questionarios.Tests.Integration.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;

namespace Questionarios.Tests.Integration.Results;

public class ResultsControllerTests
    : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public ResultsControllerTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task GetChart_DeveRetornar200_ComEstruturaDeGrafico()
    {
        // Arrange
        var client = _factory.CreateClient();

        var surveyId = "11111111-1111-1111-1111-111111111111";

        // Act
        var response = await client.GetAsync($"/api/results/{surveyId}/chart");

        // Assert status
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var chart = await response.Content.ReadFromJsonAsync<SurveyChartResultDto>();
        chart.Should().NotBeNull();
        chart!.SurveyId.ToString().Should().Be(surveyId);
        chart.Title.Should().NotBeNullOrWhiteSpace();
        chart.Questions.Should().NotBeEmpty();

        // Checa estrutura da primeira pergunta
        var firstQuestion = chart.Questions.First();
        firstQuestion.QuestionId.Should().NotBeEmpty();
        firstQuestion.QuestionText.Should().NotBeNullOrWhiteSpace();
        firstQuestion.Options.Should().NotBeEmpty();

        // Checa que porcentagens batem ~100% (tolerância)
        foreach (var q in chart.Questions)
        {
            var totalPercent = q.Options.Sum(o => o.Percentage);
            totalPercent.Should().BeInRange(99.0, 101.0);
        }
    }
}
