using POS.application.models;

namespace POS.application.interfaces;

public interface IEvaluationImporter
{
    string Format { get; }
    List<EvaluationImportData>? Import(string path);
}