using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using TesTool.Core.Interfaces.Services;

namespace TesTool.Infra.Services
{
    public class SettingsService : ISettingsService
    {
        private static string FILE_PATH = "config.json";

        public async Task<string> GetStringAsync(string key)
        {
            if (!File.Exists(FILE_PATH)) return default;
            var json = await File.ReadAllTextAsync(FILE_PATH);
            var dictionary = JsonSerializer.Deserialize<Dictionary<string, string>>(json);
            if (dictionary.ContainsKey(key)) return dictionary[key];
            return default;
        }

        public async Task SetStringAsync(string key, string value)
        {
            var dictionary = null as Dictionary<string, string>;
            if (File.Exists(FILE_PATH))
            {
                var json = await File.ReadAllTextAsync(FILE_PATH);
                dictionary = JsonSerializer.Deserialize<Dictionary<string, string>>(json);
            }
            dictionary[key] = value;
            await File.WriteAllTextAsync(FILE_PATH, JsonSerializer.Serialize(dictionary));
        }
    }
}
