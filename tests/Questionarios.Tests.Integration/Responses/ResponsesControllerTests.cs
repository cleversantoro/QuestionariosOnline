using FluentAssertions;
using Questionarios.Application.DTOs;
using Questionarios.Tests.Integration.Infrastructure;
using System.Net;
using System.Net.Http.Json;

namespace Questionarios.Tests.Integration.Responses;

public class ResponsesControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public ResponsesControllerTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Submit_DeveRetornarOk_ParaSurveyAtiva()
    {
        var client = _factory.CreateClient();
        var surveyId = Guid.Parse("11111111-1111-1111-1111-111111111111");

        // Obtem perguntas/opcoes do seed para montar uma resposta valida
        var surveyDetail = await client.GetFromJsonAsync<SurveyDetailDto>($"/api/survey/{surveyId}");
        surveyDetail.Should().NotBeNull();
        var firstQuestion = surveyDetail!.Questions.First();
        var firstOption = firstQuestion.Options.First();

        var payload = new ResponseCreateDto(
            surveyId,
            new List<ResponseItemDto>
            {
                new(firstQuestion.Id, firstOption.Id)
            });

        var response = await client.PostAsJsonAsync("/api/responses", payload);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var responseDto = await response.Content.ReadFromJsonAsync<ResponseDto>();
        responseDto.Should().NotBeNull();
        responseDto!.SurveyId.Should().Be(surveyId);
        responseDto.AnsweredAt.Should().BeAfter(DateTime.MinValue);
    }
}
