using POS.application.interfaces;

namespace POS.infrastructure.Import;

public class EvaluationImporterFactory : IEvaluationImporterFactory
{
    private readonly Dictionary<string, IEvaluationImporter> _importers;

    public EvaluationImporterFactory(IEnumerable<IEvaluationImporter> importers)
    {
        _importers = importers.ToDictionary(
            importer => importer.Format,
            importer => importer,
            StringComparer.OrdinalIgnoreCase
        );
    }

    public IEvaluationImporter? Create(string path)
    {
        string extension = Path.GetExtension(path);

        _importers.TryGetValue(extension, out var importer);

        return importer;
    }
}