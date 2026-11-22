using POS.application.interfaces;
using POS.application.models;

namespace POS.infrastructure.Export;

using System.Globalization;
using System.Text;

public class CsvResultsExporter : IEvaluationExporter
{
    public bool Export(string path, List<ResultExportData> data)
    {
        if (string.IsNullOrWhiteSpace(path) || data == null)
            return false;

        try
        {
            using var writer = new StreamWriter(path, false, Encoding.UTF8);

            // заголовок
            writer.WriteLine("StudentId|StudentName|Result");

            // тело
            foreach (ResultExportData item in data)
            {
                writer.WriteLine(
                    $"{item.StudentId}|{item.StudentName}|{item.Result.ToString(CultureInfo.InvariantCulture)}"
                );
            }

            return true;
        }
        catch (IOException)
        {
            return false;
        }
        catch (UnauthorizedAccessException)
        {
            return false;
        }
    }
}