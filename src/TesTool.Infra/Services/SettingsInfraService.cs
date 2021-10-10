using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using TesTool.Core.Interfaces.Services;

namespace TesTool.Infra.Services
{
    public class SettingsInfraService : ISettingInfraService
    {
        private readonly string _filePath = "config.json";

        public SettingsInfraService()
        {
            _filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.json");
        }

        public async Task<string> GetStringAsync(string key)
        {
            if (!File.Exists(_filePath)) return default;
            var json = await File.ReadAllTextAsync(_filePath);
            var dictionary = JsonSerializer.Deserialize<Dictionary<string, string>>(json);
            if (dictionary.ContainsKey(key)) return dictionary[key];
            return default;
        }

        public async Task SetStringAsync(string key, string value)
        {
            var dictionary = new Dictionary<string, string>();
            if (File.Exists(_filePath))
            {
                var json = await File.ReadAllTextAsync(_filePath);
                dictionary = JsonSerializer.Deserialize<Dictionary<string, string>>(json);
            }
            dictionary[key] = value;
            await File.WriteAllTextAsync(_filePath, JsonSerializer.Serialize(dictionary));
        }
    }
}
