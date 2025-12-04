namespace Questionarios.Application.DTOs;

public record ResponseItemDto(Guid QuestionId, Guid OptionId);

public record ResponseCreateDto(
    Guid SurveyId,
    IList<ResponseItemDto> Items
);
