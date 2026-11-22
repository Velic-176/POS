using POS.application.interfaces;
using POS.application.models;
using System.Text.Json;

namespace POS.infrastructure.Import;

public class JsonEvaluationImporter : IEvaluationImporter
{
    public string Format { get; } = ".json";

    public List<EvaluationImportData>? Import(string path)
    {
        if (!File.Exists(path))
            return null;

        var result = new List<EvaluationImportData>();

        try
        {
            string json = File.ReadAllText(path);

            using JsonDocument document = JsonDocument.Parse(json);

            // Корневой элемент должен быть массивом
            if (document.RootElement.ValueKind != JsonValueKind.Array)
                return null;

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            foreach (JsonElement element in document.RootElement.EnumerateArray())
            {
                // Каждая запись должна быть объектом
                if (element.ValueKind != JsonValueKind.Object)
                    continue;

                EvaluationImportData? item;

                try
                {
                    item = element.Deserialize<EvaluationImportData>(options);
                }
                catch (JsonException)
                {
                    continue;
                }

                if (item == null ||
                    item.StudentId == Guid.Empty ||
                    item.CriterionId == Guid.Empty ||
                    item.StudentName == null ||
                    item.Comment == null ||
                    item.Date == default)
                    continue;

                result.Add(item);
            }
        }
        catch (IOException)
        {
            return null;
        }
        catch (JsonException)
        {
            return null;
        }

        return result;
    }
}