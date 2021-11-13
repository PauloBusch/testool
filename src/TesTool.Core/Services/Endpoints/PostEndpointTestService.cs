using System;
using System.Linq;
using TesTool.Core.Enumerations;
using TesTool.Core.Interfaces.Services;
using TesTool.Core.Interfaces.Services.Endpoints;
using TesTool.Core.Models.Metadata;
using TesTool.Core.Models.Metadata.Types;
using TesTool.Core.Models.Templates.Controller;
using TesTool.Core.Models.Templates.Controller.Asserts;

namespace TesTool.Core.Services.Endpoints
{
    public class PostEndpointTestService : EndpointTestServiceBase, IPostEndpointTestService
    {
        public PostEndpointTestService(ICompareService compareService)
            : base(RequestMethodEnumerator.POST, compareService) { }

        public ControllerTestMethod GetControllerTestMethod(Controller controller, Endpoint endpoint, DbSet dbSet)
        {
            var testMethod = new ControllerTestMethod(
                GetMethodName(endpoint), endpoint.Method,
                GetArrageSection(endpoint),
                GetActSection(controller, endpoint),
                GetAssertSection(endpoint, dbSet)
            );
            var requiredNamespaces = GetRequitedNamespaces(testMethod, endpoint);
            testMethod.AddRequiredNamespaces(requiredNamespaces);
            return testMethod;
        }

        protected override ControllerTestMethodSectionAssertBase GetAssertSection(Endpoint endpoint, DbSet dbSet)
        {
            var entityKey = GetEntityKey(dbSet?.Entity);
            var requestModel = GetModelComparableEntity(endpoint.Inputs, dbSet?.Entity);
            var responseModel = GetResponseModel(endpoint.Output);
            var outputModel = GetOutputModel(endpoint.Output) as Class;
            return new ControllerTestMethodSectionAssertPost(
                requestModel?.Properties.Any(p => p.Name == entityKey) ?? false,
                outputModel?.Properties.Any(p => p.Name == entityKey) ?? false,
                responseModel is TypeBase type && type.Name != "Void",
                responseModel is Class output && output.Generics.Any(),
                GetPropertyData(endpoint.Output), dbSet?.Entity.Name, 
                entityKey, dbSet?.Property, requestModel?.Name,
                _compareService.IsComparableClasses(requestModel, outputModel) 
                    ? _compareService.GetComparatorNameOrDefault(requestModel?.Name, outputModel?.Name) : default,
                _compareService.IsComparableClasses(requestModel, dbSet?.Entity) 
                    ? _compareService.GetComparatorNameOrDefault(requestModel?.Name, dbSet?.Entity.Name) : default
            );
        }

        private string GetMethodName(Endpoint endpoint)
        {
            var withSpecificName = !endpoint.Name.Contains("post", StringComparison.OrdinalIgnoreCase);
            if (withSpecificName)
            {
                var action = endpoint.Name.Replace("Async", string.Empty, StringComparison.OrdinalIgnoreCase);
                return TestMethodEnumerator.GENERIC.Name.Replace("{ACTION}", action, StringComparison.OrdinalIgnoreCase);
            }

            return TestMethodEnumerator.CREATE.Name;
        }
    }
}
