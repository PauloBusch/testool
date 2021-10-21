using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using TesTool.Core.Interfaces.Services;
using TesTool.Core.Models.Configuration;

namespace TesTool.Infra.Services
{
    public class SettingsInfraService : ISettingInfraService
    {
        private readonly string _filePath = "config.json";

        public SettingsInfraService()
        {
            _filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.json");
        }

        public async Task<string> GetStringAsync(Setting setting)
        {
            if (!File.Exists(_filePath)) return default;
            var json = await File.ReadAllTextAsync(_filePath);
            var dictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
            if (dictionary.ContainsKey(setting.Key)) return dictionary[setting.Key];
            return default;
        }

        public async Task SetStringAsync(Setting setting, string value)
        {
            var dictionary = new Dictionary<string, string>();
            if (File.Exists(_filePath))
            {
                var json = await File.ReadAllTextAsync(_filePath);
                dictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
            }
            dictionary[setting.Key] = value;
            await File.WriteAllTextAsync(_filePath, JsonConvert.SerializeObject(dictionary, Formatting.Indented));
        }
    }
}
