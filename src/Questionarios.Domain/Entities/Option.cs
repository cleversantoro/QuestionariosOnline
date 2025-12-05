namespace Questionarios.Domain.Entities;

public class Option
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string Text { get; private set; } = default!;
    public Guid QuestionId { get; private set; }
    public Question Question { get; private set; } = default!;

    public int? Order { get; private set; }

    private Option() { }

    public Option(Guid questionId, string text, int? order = null)
    {
        if (string.IsNullOrWhiteSpace(text))
            throw new ArgumentException("Text is required.", nameof(text));

        QuestionId = questionId;
        Text = text;
        Order = order;
    }

    public void SetOrder(int order) => Order = order;

    public void Update(string text, int? order = null)
    {
        if (string.IsNullOrWhiteSpace(text))
            throw new ArgumentException("Text is required.", nameof(text));

        Text = text;
        Order = order;
    }
}
