using System.ComponentModel.DataAnnotations;
using Questionarios.Application.DTOs;

namespace Questionarios.WebPublico.Models;

public class QuestionAnswerInput
{
    public Guid QuestionId { get; set; }
    public string QuestionText { get; set; } = string.Empty;

    [Required(ErrorMessage = "Selecione uma opção.")]
    public Guid? SelectedOptionId { get; set; }
}

public class SurveyResponseViewModel
{
    public SurveyDetailDto? Survey { get; set; }

    public List<QuestionAnswerInput> Answers { get; set; } = [];

    public bool SendCopy { get; set; }

    [EmailAddress(ErrorMessage = "E-mail inválido.")]
    public string? Email { get; set; }

    public string? SuccessMessage { get; set; }
    public string? ErrorMessage { get; set; }
}
