namespace Questionarios.Application.DTOs;

public record AggregatedResultDto(
    Guid SurveyId,
    Guid QuestionId,
    Guid OptionId,
    int VotesByOption
);


//public record AggregatedResultDto(
//    Guid SurveyId,
//    IDictionary<Guid, int> VotesByOption
//);

// Para gráficos

public record ChartOptionDto(
    Guid OptionId,
    string Label,
    int Votes,
    double Percentage
);

public record ChartQuestionDto(
    Guid QuestionId,
    string QuestionText,
    IList<ChartOptionDto> Options
);

public record SurveyChartResultDto(
    Guid SurveyId,
    string Title,
    IList<ChartQuestionDto> Questions
);
