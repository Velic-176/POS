using POS.application.interfaces;
using POS.application.models;
using System.Globalization;
using Microsoft.VisualBasic.FileIO;

namespace POS.infrastructure.Import;

public class CsvEvaluationImporter : IEvaluationImporter
{
    public string Format { get; } = ".csv";

    public List<EvaluationImportData>? Import(string path)
    {
        if (!File.Exists(path))
            return null;

        var result = new List<EvaluationImportData>();

        try
        {
            using var parser = new TextFieldParser(path);

            parser.TextFieldType = FieldType.Delimited;
            parser.SetDelimiters(",");
            parser.HasFieldsEnclosedInQuotes = true;
            parser.TrimWhiteSpace = false;

            // Пропускаем заголовок
            if (parser.EndOfData)
                return result;

            parser.ReadFields();

            while (!parser.EndOfData)
            {
                string[]? fields = parser.ReadFields();

                if (fields == null || fields.Length != 6)
                    continue;

                if (!Guid.TryParse(fields[0], out Guid studentId))
                    continue;

                if (!float.TryParse(
                        fields[3],
                        NumberStyles.Float,
                        CultureInfo.InvariantCulture,
                        out float score))
                    continue;

                if (!DateTime.TryParseExact(
                        fields[4],
                        "yyyy-MM-dd",
                        CultureInfo.InvariantCulture,
                        DateTimeStyles.None,
                        out DateTime date))
                    continue;

                result.Add(new EvaluationImportData(
                    studentId,
                    fields[1],
                    Guid.TryParse(fields[2], out Guid criterionId)
                        ? criterionId
                        : Guid.Empty,
                    score,
                    date,
                    fields[5]
                ));
            }
        }
        catch (IOException)
        {
            return null;
        }
        catch (MalformedLineException)
        {
            return null;
        }

        return result;
    }
}