namespace POS.application.models;
public record EvaluationImportData(
    Guid StudentId,
    string StudentName,
    Guid CriterionId,
    float Score,
    DateTime Date,
    string Comment
);