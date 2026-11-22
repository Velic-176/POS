namespace POS.application.models;

public record EvaluationInfo(
    Guid? CriterionId = null,
    string CriterionName = "",
    float? Score = null,
    float MaxScore = 0,
    float Weight = 0,
    DateTime? Date = null,
    string Comment = ""
);