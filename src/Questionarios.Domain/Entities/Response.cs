namespace Questionarios.Domain.Entities;

public class Response
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid SurveyId { get; private set; }
    public DateTime AnsweredAt { get; private set; }

    public ICollection<ResponseItem> Items { get; private set; } = new List<ResponseItem>();

    private Response() { }

    public Response(Guid surveyId, DateTime answeredAt)
    {
        SurveyId = surveyId;
        AnsweredAt = answeredAt;
    }

    public void AddItem(Guid questionId, Guid optionId)
    {
        Items.Add(new ResponseItem(Id, questionId, optionId));
    }
}
