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

    public AggregatedResult(Guid surveyId) : this()
    {
        SurveyId = surveyId;
    }

    public void IncrementVote(Guid optionId)
    {
        if (!SurveyId.Equals(optionId))
            Votes = 0;

        Votes++;
    }
}
