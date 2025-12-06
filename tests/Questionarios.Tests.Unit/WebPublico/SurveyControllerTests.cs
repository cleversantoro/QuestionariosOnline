using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Questionarios.Application.DTOs;
using Questionarios.WebPublico.Controllers;
using Questionarios.WebPublico.Models;
using Questionarios.WebPublico.Services;

namespace Questionarios.Tests.Unit.WebPublico;

public class SurveyControllerTests
{
    [Fact]
    public async Task Index_WhenNoDefaultSurvey_ReturnsErrorMessage()
    {
        var apiClient = new FakeSurveyApiClient(null);
        var options = Options.Create(new ApiSettings { DefaultSurveyId = null, BaseUrl = "http://localhost" });
        var controller = new SurveyController(apiClient, options)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            }
        };

        var result = await controller.Index(null, CancellationToken.None);

        var view = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<SurveyResponseViewModel>(view.Model);
        Assert.Equal("Nenhuma pesquisa configurada.", model.ErrorMessage);
    }

    private sealed class FakeSurveyApiClient : ISurveyApiClient
    {
        private readonly SurveyDetailDto? _survey;

        public FakeSurveyApiClient(SurveyDetailDto? survey) => _survey = survey;

        public Task<SurveyDetailDto?> GetSurveyAsync(Guid surveyId, CancellationToken ct = default) =>
            Task.FromResult(_survey);

        public Task<bool> SubmitResponseAsync(Guid surveyId, IList<ResponseItemDto> items, CancellationToken ct = default) =>
            Task.FromResult(true);
    }
}
