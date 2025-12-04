namespace Questionarios.WebPublico.Models;

public class ApiSettings
{
    public string BaseUrl { get; set; } = string.Empty;
    public Guid? DefaultSurveyId { get; set; }
}
