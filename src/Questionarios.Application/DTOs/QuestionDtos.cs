namespace Questionarios.Application.DTOs;

public record QuestionCreateDto(Guid SurveyId, string Text);
public record OptionCreateDto(Guid QuestionId, string Text);
