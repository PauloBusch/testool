using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TesTool.Core.Enumerations;
using TesTool.Core.Interfaces.Services;
using TesTool.Core.Models.Metadata;
using TesTool.Infra.Extensions;

namespace TesTool.Infra.Services
{
    public class WebApiScanInfraService : ProjectScanInfraServiceBase, IWebApiScanInfraService
    {
        private readonly ISettingInfraService _settingInfraService;

        public WebApiScanInfraService(
            ILoggerInfraService loggerInfraService,
            ISettingInfraService settingInfraService
        ) : base(ProjectTypeEnumerator.WEB_API, loggerInfraService)
        {
            _settingInfraService = settingInfraService;
        }

        public async Task<Controller> GetControllerAsync(string className)
        {
            Controller controller = null;
            var classes = await GetClassesAsync();
            var controllerClass = classes.FirstOrDefault(c => c.Declaration.Identifier.Text == className);
            if (controllerClass is null) return controller;
            if (controllerClass.Declaration.IsAbstract()) return controller;

            var isController = controllerClass.TypeSymbol.ImplementsClass("ControllerBase", "Microsoft.AspNetCore.Mvc");
            if (!isController) return controller;

            var controllerName = controllerClass.Declaration.Identifier.Text.ToLower().Replace("controller", string.Empty);

            var routeAttribute = controllerClass.TypeSymbol.GetAttribute("RouteAttribute");
            var routeTemplate = routeAttribute?.ConstructorArguments.FirstOrDefault().Value?.ToString() ?? string.Empty;
            var route = routeTemplate.Replace("[controller]", controllerName);

            var types = controllerClass.TypeSymbol.GetStackTypes();
            var authorizeAttributeName = "AuthorizeAttribute";
            var authAttributeNames = new[] { authorizeAttributeName, "AllowAnonymousAttribute" };
            var authAttribute = types.SelectMany(t => t.GetAllAttributes())
                .FirstOrDefault(a => authAttributeNames.Contains(a.AttributeClass.Name));
            var authorizeController = authAttribute?.AttributeClass.Name == authorizeAttributeName;

            controller = new Controller(controllerClass.Declaration.Identifier.Text, route, authorizeController);
            FillEndpoints(controller, controllerClass.TypeSymbol);

            return controller;
        }

        public async Task<IEnumerable<Controller>> GetControllersAsync()
        {
            var controllers = new List<Controller>();
            var classes = await GetClassesAsync();
            foreach (var @class in classes)
            {
                if (@class.Declaration.IsAbstract()) continue;

                var isController = @class.TypeSymbol.ImplementsClass("ControllerBase", "Microsoft.AspNetCore.Mvc");
                if (!isController) continue;

                var controllerName = @class.Declaration.Identifier.Text.ToLower().Replace("controller", string.Empty);

                var routeAttribute = @class.TypeSymbol.GetAttribute("RouteAttribute");
                var routeTemplate = routeAttribute?.ConstructorArguments.FirstOrDefault().Value?.ToString() ?? string.Empty;
                var route = routeTemplate.Replace("[controller]", controllerName);

                var types = @class.TypeSymbol.GetStackTypes();
                var authorizeAttributeName = "AuthorizeAttribute";
                var authAttributeNames = new[] { authorizeAttributeName, "AllowAnonymousAttribute" };
                var authAttribute = types.SelectMany(t => t.GetAllAttributes())
                    .FirstOrDefault(a => authAttributeNames.Contains(a.AttributeClass.Name));
                var authorizeController = authAttribute?.AttributeClass.Name == authorizeAttributeName;

                var controller = new Controller(@class.Declaration.Identifier.Text, route, authorizeController);
                FillEndpoints(controller, @class.TypeSymbol);
                if (controller.Endpoints.Any()) controllers.Add(controller);
            }

            return controllers;
        }

        public async Task<TypeWrapper> GetModelAsync(string className)
        {
            var project = await GetProjectAsync();
            if (project is null) return default;
            
            var projects = new [] { project }.Concat(GetProjectReferences(project)).ToArray();
            var classes = await GetClassesAsync(projects);
            var @class = classes.FirstOrDefault(c => c.Declaration.Identifier.Text == className);
            if (@class is null) return default;

            return GetModelType(@class.TypeSymbol);
        }

        public async Task<bool> ModelExistAsync(string className)
        {
            return await GetModelAsync(className) is not null;
        }

        private void FillEndpoints(Controller controller, ITypeSymbol classSymbol)
        {
            var methodSymbols = classSymbol.GetMethods();
            foreach (var methodSymbol in methodSymbols)
            {
                var methodAttribute = methodSymbol.GetAttributes()
                    .FirstOrDefault(a => a.AttributeClass.Name.StartsWith("Http"));
                if (methodAttribute is null) continue;
                
                var methodType = HttpMethodEnumerator.GetAll()
                    .FirstOrDefault(m => methodAttribute.AttributeClass.Name.Contains(m.Name));
                if (methodType is null) continue;

                var route = methodAttribute?.ConstructorArguments.FirstOrDefault().Value?.ToString();

                var authorizeAttributeName = "AuthorizeAttribute";
                var allowAnonymousAttributeName = "AllowAnonymousAttribute";
                var authAttributeNames = new[] { authorizeAttributeName, allowAnonymousAttributeName };
                var authAttribute = methodSymbol.GetAttributes()
                    .FirstOrDefault(a => authAttributeNames.Contains(a.AttributeClass.Name));
                var authorizeEndpoint = controller.Authorize 
                    ? authAttribute?.AttributeClass.Name != allowAnonymousAttributeName
                    : authAttribute?.AttributeClass.Name == authorizeAttributeName;

                var endpoint = new Endpoint(route, methodSymbol.Name, authorizeEndpoint, methodType);
                FillInputs(endpoint, methodSymbol);
                FillOutput(endpoint, methodSymbol);
                controller.AddEndpoint(endpoint);
            }
        }

        private void FillInputs(Endpoint endpoint, IMethodSymbol methodSymbol)
        {
            foreach(var parameter in methodSymbol.Parameters)
            {
                var sourceAttribute = parameter.GetAttributes()
                    .FirstOrDefault(a => a.AttributeClass.Name.StartsWith("From"));
                var inputSource = sourceAttribute is null ? null : InputSourceEnumerator.GetAll()
                    .FirstOrDefault(m => sourceAttribute.AttributeClass.Name.Contains(m.Name));

                if (inputSource is null)
                {
                    var route = endpoint.Route ?? string.Empty;
                    inputSource = Regex.IsMatch(route, @$"{{{parameter.Name}:?.*}}")
                        ? InputSourceEnumerator.ROUTE : InputSourceEnumerator.BODY;
                }

                endpoint.AddInput(new Input(parameter.Name, GetModelType(parameter.Type), inputSource));
            }
        }

        private void FillOutput(Endpoint endpoint, IMethodSymbol methodSymbol)
        {
            var asyncResult = methodSymbol.ReturnType.BaseType.Name == typeof(Task).Name;
            var returnType = asyncResult
                ? methodSymbol.ReturnType.GetGeneritTypeArguments().First()
                : methodSymbol.ReturnType;

            endpoint.Output = GetModelType(returnType);
        }

        protected override string GetProjectPathFile()
            => _settingInfraService.GetStringAsync(SettingEnumerator.PROJECT_DIRECTORY).Result;
    }
}
