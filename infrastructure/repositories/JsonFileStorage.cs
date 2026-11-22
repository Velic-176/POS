using System.Text.Json;

namespace POS.infrastructure.repositories;

internal static class JsonFileStorage
{
    private static readonly JsonSerializerOptions Options = new()
    {
        WriteIndented = true
    };

    public static T? Read<T>(string path)
    {
        try
        {
            if (!File.Exists(path))
                return default;

            string json = File.ReadAllText(path);
            return JsonSerializer.Deserialize<T>(json, Options);
        }
        catch
        {
            return default;
        }
    }

    public static bool Write<T>(string path, T data)
    {
        try
        {
            string? directory = Path.GetDirectoryName(path);

            if (!string.IsNullOrEmpty(directory))
                Directory.CreateDirectory(directory);

            string json = JsonSerializer.Serialize(data, Options);
            File.WriteAllText(path, json);
            return true;
        }
        catch
        {
            return false;
        }
    }
}