using Questionarios.Domain.Entities;

namespace Questionarios.Application.DTOs;

public record SurveyCreateDto(string Title, DateTime StartAt, DateTime EndAt);
//public record SurveySummaryDto(Guid Id, string Title, DateTime StartAt, DateTime EndAt, bool IsClosed, ICollection<Question> Questions);
public record SurveySummaryDto(Guid Id, string Title, DateTime StartAt, DateTime EndAt, bool IsClosed);

// Para o front (detalhado)
public record OptionDetailDto(Guid Id, string Text, int? Order);
public record QuestionDetailDto(Guid Id, string Text, int? Order, IList<OptionDetailDto> Options);
public record SurveyDetailDto(Guid Id, string Title, DateTime StartAt, DateTime EndAt, bool IsClosed, IList<QuestionDetailDto> Questions
);
