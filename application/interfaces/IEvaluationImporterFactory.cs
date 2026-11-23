namespace POS.application.interfaces;

public interface IEvaluationImporterFactory
{
    IEvaluationImporter? Create(string path);
}

