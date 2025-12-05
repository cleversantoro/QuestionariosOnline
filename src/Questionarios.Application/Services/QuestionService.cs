using Questionarios.Application.DTOs;
using Questionarios.Domain.Abstractions;
using Questionarios.Domain.Entities;

namespace Questionarios.Application.Services;

public class QuestionService : IQuestionService
{
    private readonly ISurveyRepository _surveyRepository;
    private readonly IQuestionRepository _questionRepository;

    public QuestionService(ISurveyRepository surveyRepository, IQuestionRepository questionRepository)
    {
        _surveyRepository = surveyRepository;
        _questionRepository = questionRepository;
    }

    public async Task AddQuestionAsync(QuestionCreateDto dto, CancellationToken ct = default)
    {
        var survey = await _surveyRepository.GetByIdAsync(dto.SurveyId, ct)
                     ?? throw new InvalidOperationException("Survey not found");

        var question = new Question(dto.SurveyId, dto.Text, dto.Order);
        await _questionRepository.AddAsync(question, ct);
    }

    public async Task AddOptionAsync(OptionCreateDto dto, CancellationToken ct = default)
    {
        var question = await _questionRepository.GetByIdAsync(dto.QuestionId, ct)
                       ?? throw new InvalidOperationException("Question not found");

        var option = new Option(dto.QuestionId, dto.Text, dto.Order);
        await _questionRepository.AddOptionAsync(option, ct);
    }

    public async Task<QuestionDetailDto> GetAsync(Guid id, CancellationToken ct = default)
    {
        var question = await _questionRepository.GetByIdAsync(id, ct)
                       ?? throw new InvalidOperationException("Question not found");

        return MapQuestion(question);
    }

    public async Task<IReadOnlyList<QuestionDetailDto>> GetBySurveyAsync(Guid surveyId, CancellationToken ct = default)
    {
        var questions = await _questionRepository.GetBySurveyIdAsync(surveyId, ct);
        return questions.Select(MapQuestion).ToList();
    }

    public async Task UpdateQuestionAsync(Guid id, string text, int? order, CancellationToken ct = default)
    {
        var question = await _questionRepository.GetByIdAsync(id, ct)
                       ?? throw new InvalidOperationException("Question not found");

        question.Update(text, order);
        await _questionRepository.UpdateAsync(question, ct);
    }

    public async Task DeleteQuestionAsync(Guid id, CancellationToken ct = default)
    {
        var question = await _questionRepository.GetByIdAsync(id, ct)
                       ?? throw new InvalidOperationException("Question not found");

        await _questionRepository.DeleteAsync(question, ct);
    }

    public async Task UpdateOptionAsync(Guid id, string text, int? order, CancellationToken ct = default)
    {
        var option = await _questionRepository.GetOptionByIdAsync(id, ct)
                     ?? throw new InvalidOperationException("Option not found");

        option.Update(text, order);
        await _questionRepository.UpdateOptionAsync(option, ct);
    }

    public async Task DeleteOptionAsync(Guid id, CancellationToken ct = default)
    {
        var option = await _questionRepository.GetOptionByIdAsync(id, ct)
                     ?? throw new InvalidOperationException("Option not found");

        await _questionRepository.DeleteOptionAsync(option, ct);
    }

    private static QuestionDetailDto MapQuestion(Question question) =>
        new(
            question.Id,
            question.Text,
            question.Order,
            question.Options
                .OrderBy(o => o.Order ?? int.MaxValue)
                .Select(o => new OptionDetailDto(o.Id, o.Text, o.Order))
                .ToList()
        );
}
