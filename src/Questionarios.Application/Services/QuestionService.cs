using Questionarios.Application.DTOs;
using Questionarios.Domain.Abstractions;
using Questionarios.Domain.Entities;

namespace Questionarios.Application.Services;

public class QuestionService : IQuestionService
{
    private readonly ISurveyRepository _surveyRepository;

    public QuestionService(ISurveyRepository surveyRepository)
    {
        _surveyRepository = surveyRepository;
    }

    public async Task AddQuestionAsync(QuestionCreateDto dto, CancellationToken ct = default)
    {
        var survey = await _surveyRepository.GetByIdAsync(dto.SurveyId, ct)
                     ?? throw new InvalidOperationException("Survey not found");

        var question = new Question(dto.SurveyId, dto.Text);
        survey.Questions.Add(question);

        await _surveyRepository.UpdateAsync(survey, ct);
    }

    public Task AddOptionAsync(OptionCreateDto dto, CancellationToken ct = default)
    {
        throw new NotImplementedException("Implement IQuestionRepository and option add logic.");
    }
}
