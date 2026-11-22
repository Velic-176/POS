namespace POS.application.models;

public record ResultExportData(
    Guid StudentId,
    string StudentName,
    float Result
);