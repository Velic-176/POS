using POS.application.models;

namespace POS.application.interfaces;

public interface IEvaluationExporter
{
    bool Export(string path, List<ResultExportData> data);
}