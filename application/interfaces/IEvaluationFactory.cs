namespace POS.application.interfaces;

public interface IEvaluationFactory
{
    IEvaluationImporter? Create(string path);
}

