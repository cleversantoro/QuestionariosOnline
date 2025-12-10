namespace Questionarios.Domain.Entities;

public class AggregatedResult
{
    public Guid SurveyId { get; private set; }
    public Guid QuestionId { get; private set; }
    public Guid OptionId { get; private set; }
    public int Votes { get; private set; }

    private AggregatedResult()
    {
        Votes = 0;
    }

    public AggregatedResult(Guid surveyId, Guid questionId, Guid optionId) : this()
    {
        SurveyId = surveyId;
        QuestionId = questionId;
        OptionId = optionId;
    }

    public void IncrementVote() => Votes++;
}
