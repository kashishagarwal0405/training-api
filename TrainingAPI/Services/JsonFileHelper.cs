using System.Text.Json;

namespace TrainingAPI.Services
{
    public static class JsonFileHelper
    {
        public static async Task<List<T>> ReadListAsync<T>(string filePath)
        {
            if (!File.Exists(filePath))
                return new List<T>();
            var json = await File.ReadAllTextAsync(filePath);
            return JsonSerializer.Deserialize<List<T>>(json) ?? new List<T>();
        }

        public static async Task WriteListAsync<T>(string filePath, List<T> data)
        {
            var json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(filePath, json);
        }
    }
} 