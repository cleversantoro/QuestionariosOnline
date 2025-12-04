namespace Questionarios.Domain.Entities;

public class ResponseItem
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid ResponseId { get; private set; }
    public Response Response { get; private set; } = default!;
    public Guid QuestionId { get; private set; }
    public Question Question { get; private set; } = default!;
    public Guid OptionId { get; private set; }
    public Option Option { get; private set; } = default!;

    private ResponseItem() { }

    public ResponseItem(Guid responseId, Guid questionId, Guid optionId)
    {
        ResponseId = responseId;
        QuestionId = questionId;
        OptionId = optionId;
    }
}
