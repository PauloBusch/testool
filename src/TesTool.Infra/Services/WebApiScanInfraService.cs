using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
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
        ) : base(loggerInfraService)
        {
            _settingInfraService = settingInfraService;
        }

        public async Task<IEnumerable<Controller>> GetControllersAsync()
        {
            var controllers = new List<Controller>();
            await ForEachClassesAsync((@class, root, model) => {
                if (@class.IsAbstract()) return;

                var classSymbol = model.GetDeclaredSymbol(@class) as ITypeSymbol;
                var isController = classSymbol.ImplementsClass("ControllerBase", "Microsoft.AspNetCore.Mvc");
                if (!isController) return;

                var controllerName = @class.Identifier.Text.ToLower().Replace("controller", string.Empty);

                var routeAttribute = classSymbol.GetAttribute("RouteAttribute");
                var routeTemplate = routeAttribute?.ConstructorArguments.FirstOrDefault().Value?.ToString() ?? string.Empty;
                var route = routeTemplate.Replace("[controller]", controllerName);

                var types = classSymbol.GetStackTypes();
                var authorizeAttributeName = "AuthorizeAttribute";
                var authAttributeNames = new[] { authorizeAttributeName, "AllowAnonymousAttribute" };
                var authAttribute = types.SelectMany(t => t.GetAllAttributes())
                    .FirstOrDefault(a => authAttributeNames.Contains(a.AttributeClass.Name));
                var authorizeController = authAttribute?.AttributeClass.Name == authorizeAttributeName;

                var controller = new Controller(@class.Identifier.Text, route, authorizeController);
                FillEndpoints(controller, root, model);
                if (controller.Endpoints.Any()) controllers.Add(controller);
            });

            return controllers;
        }

        public async Task<TypeBase> GetModelAsync(string name)
        {
            var controllers = await GetControllersAsync();
            foreach(var controller in controllers)
            {
                foreach(var endpoint in controller.Endpoints)
                {
                    foreach (var input in endpoint.Inputs)
                    {
                        var inputModel = GetModelRecursive(input.Type, name);
                        if (inputModel is not null) return inputModel;
                    }

                    var outputModel = GetModelRecursive(endpoint.Output, name);
                    if (outputModel is not null) return outputModel;
                }
            }

            return default;
        }

        private TypeBase GetModelRecursive(TypeBase typeBase, string name)
        {
            if (typeBase is null) return default;

            if (typeBase.Namespace.EndsWith(name)) return typeBase;
            
            if (typeBase is Core.Models.Metadata.Array array)
                return GetModelRecursive(array.Type, name);

            if (typeBase is Dto dto) { 
                foreach(var property in dto.Properties)
                {
                    var model = GetModelRecursive(property.Type, name);
                    if (model is not null) return model;
                }
            }

            return default;
        }

        private void FillEndpoints(Controller controller, SyntaxNode root, SemanticModel model)
        {
            var methods = root.DescendantNodes().OfType<MethodDeclarationSyntax>();
            foreach (var method in methods)
            {
                var methodSymbol = model.GetDeclaredSymbol(method);
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

                var endpoint = new Endpoint(route, authorizeEndpoint, methodType);
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

        protected override string GetProjectPath()
            => _settingInfraService.GetStringAsync(SettingEnumerator.PROJECT_DIRECTORY.Key).Result;
    }
}
