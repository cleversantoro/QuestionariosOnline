namespace Questionarios.Application.DTOs;

public record QuestionCreateDto(Guid SurveyId, string Text, int? Order = null);
public record OptionCreateDto(Guid QuestionId, string Text, int? Order = null);
