﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;
using TesTool.Core.Interfaces.Services;

namespace TesTool.Infra.Services
{
    public class ProjectInfraManager : IProjectInfraManager
    {
        private readonly IEnvironmentInfraService _environmentInfraService;

        public ProjectInfraManager(IEnvironmentInfraService environmentInfraService)
        {
            _environmentInfraService = environmentInfraService;
        }

        private readonly static Dictionary<string, string> _cacheProjects = new();
        public string GetCurrentProject(Func<string, bool> filter = null)
        {
            var applicationBasePath = _environmentInfraService.GetWorkingDirectory();
            if (_cacheProjects.ContainsKey(applicationBasePath)) return _cacheProjects[applicationBasePath];

            var directoryInfo = new DirectoryInfo(applicationBasePath);
            do
            {
                var projectDirectoryInfo = new DirectoryInfo(directoryInfo.FullName);
                if (projectDirectoryInfo.Exists)
                {
                    var projectFiles = Directory.GetFiles(projectDirectoryInfo.FullName, "*.csproj");
                    foreach (var projectPathFile in projectFiles)
                    {
                        if (filter is null || filter(projectPathFile))
                        {
                            _cacheProjects[applicationBasePath] = projectPathFile;
                            return projectPathFile;
                        }
                    }
                }

                directoryInfo = directoryInfo.Parent;
            }
            while (directoryInfo.Parent != null);

            return default;
        }

        public string GetProjectVersion(string projectPathFile)
        {
            var csprojXml = File.ReadAllText(projectPathFile);
            return XDocument.Parse(csprojXml).XPathSelectElement("//Version").Value;
        }

        public IEnumerable<string> GetProjectPackages(string projectPathFile)
        {
            var csprojXml = File.ReadAllText(projectPathFile);
            return XDocument.Parse(csprojXml).XPathSelectElements("//PackageReference")
                .Select(pr => pr.Attribute("Include").Value)
                .ToArray();
        }

        public bool IsTestProjectFile(string projectPathFile)
        {
            if (!File.Exists(projectPathFile)) return false;
            var packages = GetProjectPackages(projectPathFile);
            return packages.Any(p => p == "Microsoft.NET.Test.Sdk");
        }

        public void AddFileCopyToOutput(string projectPathFile, string fileName)
        {
            var csprojXml = File.ReadAllText(projectPathFile);
            var xDocument = XDocument.Parse(csprojXml);
            var project = xDocument.Root;

            var itemGroup = new XElement("ItemGroup");
            var none = new XElement("None");
            none.SetAttributeValue("Update", fileName);
            none.Add(new XElement("CopyToOutputDirectory", "PreserveNewest"));
            itemGroup.Add(none);
            project.Add(itemGroup);
            xDocument.Save(projectPathFile);
        }
    }
}
