namespace Questionarios.PortalAdmin.Models;

public class DashboardViewModel
{
    public string UserName { get; set; } = string.Empty;
    public Guid? SelectedSurveyId { get; set; }
    public List<SurveySummaryDto> Surveys { get; set; } = new();
    public SurveyChartResultDto? ChartData { get; set; }
    public string ApiBaseUrl { get; set; } = string.Empty;
    public List<HighlightCard> Highlights { get; set; } = new();
    public List<ActivityItem> Activities { get; set; } = new();
}

public record HighlightCard(string Title, string Value, string Trend, bool TrendIsPositive);

public record ActivityItem(string Title, string Description, string Status);

public class ProfileViewModel
{
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string Language { get; set; } = "pt";
}

public record SurveySummaryDto(Guid Id, string Title, DateTime StartAt, DateTime EndAt, bool IsClosed);

public record SurveyChartResultDto(Guid SurveyId, string Title, IList<ChartQuestionDto> Questions);

public record ChartQuestionDto(Guid QuestionId, string QuestionText, IList<ChartOptionDto> Options);

public record ChartOptionDto(Guid OptionId, string Label, int Votes, double Percentage);

public record UserInfo(Guid Id, string Name, string Email);

public record SurveyCreateInput(string Title, DateTime StartAt, DateTime EndAt);

public class SurveyListViewModel
{
    public List<SurveySummaryDto> Surveys { get; set; } = new();
}

public record QuestionCreateInput(Guid SurveyId, string Text, int? Order);
public record OptionDetailDto(Guid Id, string Text, int? Order);
public record QuestionDetailDto(Guid Id, string Text, int? Order, IList<OptionDetailDto> Options);
public record OptionCreateInput(Guid QuestionId, string Text, int? Order);

public class QuestionListViewModel
{
    public List<SurveySummaryDto> Surveys { get; set; } = new();
    public Guid? SelectedSurveyId { get; set; }
    public List<QuestionDetailDto> Questions { get; set; } = new();
}

public class OptionListViewModel
{
    public List<SurveySummaryDto> Surveys { get; set; } = new();
    public Guid? SelectedSurveyId { get; set; }
    public List<QuestionDetailDto> Questions { get; set; } = new();
    public Guid? SelectedQuestionId { get; set; }
    public List<OptionDetailDto> Options { get; set; } = new();
}
