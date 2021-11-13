using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using TesTool.Core.Interfaces.Services;
using TesTool.Core.Models.Configuration;

namespace TesTool.Infra.Services
{
    public class SettingsInfraService : ISettingInfraService
    {
        private readonly string _filePath;
        private readonly IProjectInfraManager _projectExplorer;
        
        private static IEnumerable<Setting> _cacheSetting;

        public SettingsInfraService(IProjectInfraManager projectExplorer)
        {
            _filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.json");
            _projectExplorer = projectExplorer;
            _cacheSetting = LoadSettings();
        }

        public string ConventionPathFile 
        { 
            get => Setting.ConventionPathFile; 
            set {
                var setting = Setting;
                setting.ConventionPathFile = value;
                SaveSetting(setting);
            }
        }

        public string ProjectWebApiDirectory 
        {
            get => Setting.ProjectWebApiDirectory;
            set
            {
                var setting = Setting;
                setting.ProjectWebApiDirectory = value;
                SaveSetting(setting);
            }
        }

        public string ProjectIntegrationTestDirectory
        {
            get => Setting.ProjectIntegrationTestDirectory;
            set
            {
                var setting = Setting;
                setting.ProjectIntegrationTestDirectory = value;
                SaveSetting(setting);
            }
        }

        public string DbContextName
        {
            get => Setting.DbContextName;
            set
            {
                var setting = Setting;
                setting.DbContextName = value;
                SaveSetting(setting);
            }
        }

        public string FixtureName
        {
            get => Setting.FixtureName;
            set
            {
                var setting = Setting;
                setting.FixtureName = value;
                SaveSetting(setting);
            }
        }

        public void CreateTemporarySetting()
        {
            _temporary = new Setting();
        }

        private static Setting _temporary;
        public Setting Setting 
        {
            get {
                if (_temporary is not null) return _temporary;
                var currentProjectFile = _projectExplorer.GetCurrentProject();
                if (string.IsNullOrWhiteSpace(currentProjectFile))
                    throw new ValidationException(
                        "Não foi possível determinar o projeto de trabalho.\n" +
                        "Por favor acesse um diretório que faça parte de um projeto."
                    );
                var isTestProject = _projectExplorer.IsTestProjectFile(currentProjectFile);
                var setting = _cacheSetting.SingleOrDefault(c =>
                    currentProjectFile.Equals(isTestProject ? c.ProjectIntegrationTestDirectory : c.ProjectWebApiDirectory, StringComparison.InvariantCultureIgnoreCase)
                );
                return setting ?? new Setting { ProjectIntegrationTestDirectory = isTestProject ? currentProjectFile : default };
            }
        }

        private IEnumerable<Setting> LoadSettings()
        {
            if (!File.Exists(_filePath)) return Array.Empty<Setting>();
            var json = File.ReadAllText(_filePath);
            return JsonConvert.DeserializeObject<IEnumerable<Setting>>(json);
        }

        private void SaveSetting(Setting setting)
        {
            if (setting is null) return;
            if (!_cacheSetting.Any(s => s.ProjectWebApiDirectory == setting.ProjectWebApiDirectory))
                _cacheSetting = _cacheSetting.Concat(new [] { setting });
            SaveSettings(_cacheSetting);
        }

        private void SaveSettings(IEnumerable<Setting> settings)
        {
            if (settings is null || !settings.Any()) return;
            var jsonSerializerSettings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
            var json = JsonConvert.SerializeObject(settings, Formatting.Indented, jsonSerializerSettings);
            File.WriteAllText(_filePath, json);
        }
    }
}
