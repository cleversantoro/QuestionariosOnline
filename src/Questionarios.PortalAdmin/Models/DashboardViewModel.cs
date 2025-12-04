namespace Questionarios.PortalAdmin.Models;

public class DashboardViewModel
{
    public string UserName { get; set; } = string.Empty;
    public string SelectedQuestionario { get; set; } = string.Empty;
    public List<string> Questionarios { get; set; } = new();
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
