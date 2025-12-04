namespace Questionarios.Application.Abstractions;

public record ResponseMessage(
    Guid SurveyId,
    IList<(Guid QuestionId, Guid OptionId)> Items
);

public interface IQueueClient
{
    Task PublishResponseAsync(ResponseMessage message, CancellationToken ct = default);
}
