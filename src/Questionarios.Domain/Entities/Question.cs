namespace Questionarios.Domain.Entities;

public class Question
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string Text { get; private set; } = default!;
    public Guid SurveyId { get; private set; }
    public Survey Survey { get; private set; } = default!;
    public int? Order { get; private set; }
    public ICollection<Option> Options { get; private set; } = new List<Option>();

    private Question() { }

    public Question(Guid surveyId, string text, int? order = null)
    {
        if (string.IsNullOrWhiteSpace(text))
            throw new ArgumentException("Text is required.", nameof(text));

        SurveyId = surveyId;
        Text = text;
        Order = order;
    }

    public void SetOrder(int order) => Order = order;
}

