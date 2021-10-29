using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TesTool.Core.Enumerations;
using TesTool.Core.Extensions;
using TesTool.Core.Interfaces.Services;
using TesTool.Core.Models.Enumerators;
using TesTool.Core.Models.Metadata;
using TesTool.Core.Models.Metadata.Types;
using TesTool.Core.Models.Templates.Controller;
using TesTool.Core.Models.Templates.Controller.Asserts;

namespace TesTool.Core.Services.Endpoints
{
    public abstract class EndpointTestServiceBase
    {
        protected readonly RequestMethod _requestMethod;
        protected readonly ICompareService _compareService;

        protected EndpointTestServiceBase(
            RequestMethod requestMethod,
            ICompareService compareService
        )
        {
            _requestMethod = requestMethod;
            _compareService = compareService;
        }

        protected ControllerTestMethodSectionArrage GetArrageSection(Endpoint endpoint, DbSet dbSet = default)
        {
            var entities = new List<string>();
            if (dbSet is not null) entities.Add(dbSet.Entity.Name);

            var inputModels = new List<string>();
            var inputBody = GetInputBodyClass(endpoint.Inputs);
            var inputQuery = GetInputQueryClass(endpoint.Inputs);
            if (inputBody is not null) inputModels.Add(inputBody.Name);
            if (inputQuery is not null) inputModels.Add(inputQuery.Name);

            return new ControllerTestMethodSectionArrage(entities, inputModels);
        }

        protected ControllerTestMethodSectionAct GetActSection(Controller controller, Endpoint endpoint, DbSet dbSet = default)
        {
            var actSection = new ControllerTestMethodSectionAct(
                GetRouteResolved(controller, endpoint, dbSet),
                _requestMethod.Name,
                GetReturnType(endpoint.Output),
                GetInputBodyClass(endpoint.Inputs)?.Name,
                GetInputQueryClass(endpoint.Inputs)?.Name
            );

            var requireBody = new [] { RequestMethodEnumerator.POST, RequestMethodEnumerator.PUT };
            if (requireBody.Contains(_requestMethod) && GetInputBodyClass(endpoint.Inputs) is null)
                actSection.MarkAsUnsafe();
            if (Regex.IsMatch(actSection.Route ?? string.Empty, "{{.*?}}")) 
                actSection.MarkAsUnsafe();
            return actSection;
        }

        protected abstract ControllerTestMethodSectionAssertBase GetAssertSection(Endpoint endpoint, DbSet dbSet);

        private Class GetInputBodyClass(IEnumerable<Input> inputs) => GetInputClass(inputs, InputSourceEnumerator.BODY);
        private Class GetInputQueryClass(IEnumerable<Input> inputs) => GetInputClass(inputs, InputSourceEnumerator.QUERY);

        private Class GetInputClass(IEnumerable<Input> inputs, InputSource inputSource)
        {
            return inputs.FirstOrDefault(i => i.Source == inputSource)?.Type as Class;
        }

        private string GetReturnType(TypeWrapper wrapper)
        {
            if (wrapper is null) return default;
            if (wrapper is Class @class)
            {
                if (@class.Generics.Any())
                {
                    var generics = @class.Generics.Select(g => GetReturnType(g));
                    return $"{@class.Name}<{string.Join(", ", generics)}>";
                }

                return @class.Name;
            }
            if (wrapper is Models.Metadata.Nullable nullable)
                return $"{GetReturnType(nullable.Type)}?";
            if (wrapper is Models.Metadata.Array array)
            {
                if (array.Generics.Any())
                {
                    var generics = array.Generics.Select(g => GetReturnType(g));
                    return $"{array.Name}<{string.Join(", ", generics)}>";
                }
                return $"{GetReturnType(array.Type)}[]";
            }
            if (wrapper is Field field) { 
                if (field.SystemType == "System.Void") return default;
                if (field.SystemType == "System.Int32") return "int";    
            }
            if (wrapper is TypeBase type) return type.Name;
            return default;
        }

        protected IEnumerable<string> GetRequitedNamespaces(ControllerTestMethod method, Endpoint endpoint)
        {
            var namespaces = new List<string>();
            namespaces.AddRange(GetActNamespaces(method.Act));
            namespaces.AddRange(GetOutputNamespaces(endpoint.Output));
            return namespaces;
        }

        private IEnumerable<string> GetActNamespaces(ControllerTestMethodSectionAct actSection)
        {
            var namespaces = new List<string>();
            if (actSection is null) return namespaces;
            if (!string.IsNullOrWhiteSpace(actSection.Route)) namespaces.Add("System");

            return namespaces;
        }

        private IEnumerable<string> GetOutputNamespaces(TypeWrapper output)
        {
            var namespaces = new List<string>();
            if (output is null) return namespaces;
            if (output is Class @class)
            {
                namespaces.Add(@class.Namespace);
                if (@class.Generics.Any())
                {
                    foreach (var generic in @class.Generics)
                        namespaces.AddRange(GetOutputNamespaces(generic));
                }
            }

            if (output is Models.Metadata.Nullable nullable)
                namespaces.AddRange(GetOutputNamespaces(nullable.Type));

            if (output is Models.Metadata.Array array)
                namespaces.AddRange(GetOutputNamespaces(array.Type));

            if (output is TypeBase type)
                namespaces.Add(type.Namespace);
            return namespaces;
        }

        protected string GetPropertyData(TypeWrapper wrapper)
        {
            if (wrapper is Class @class)
            {
                if (!@class.Generics.Any()) return default;
                
                var genericProperty = @class.Properties.FirstOrDefault(p => p.FromGeneric);
                if (genericProperty is null) return default;

                var paths = new List<string> { genericProperty.Name };
                var childProperties = GetPropertyData(genericProperty.Type);
                if (childProperties is not null) paths.Add(childProperties);
                return string.Join(".", paths);
            }

            return default;
        }

        protected TypeBase GetOutputModel(TypeWrapper wrapper)
        {
            if (wrapper is Class @class)
            {
                if (!@class.Generics.Any()) return @class;

                var genericProperty = @class.Properties.FirstOrDefault(p => p.FromGeneric);
                if (genericProperty is null) return @class;

                return GetOutputModel(genericProperty.Type);
            }

            if (wrapper is Models.Metadata.Array array)
                return array;

            return default;
        }

        protected Class GetModelComparableEntity(IEnumerable<Input> inputs, Class entity)
        {
            var inputBody = GetInputBodyClass(inputs);
            if (inputBody is not null && _compareService.IsComparableClasses(inputBody, entity)) return inputBody;

            var inputQuery = GetInputQueryClass(inputs);
            if (inputQuery is not null && _compareService.IsComparableClasses(inputQuery, entity)) return inputQuery;
            
            return default;
        }

        protected string GetEntityKey(Class entity)
        {
            if (entity is null) return default;

            var expectedKey = "Id";
            var propertyKey = entity.Properties.FirstOrDefault(p => p.Name.Equals(expectedKey, StringComparison.OrdinalIgnoreCase));
            propertyKey ??= entity.Properties.FirstOrDefault(p => p.Name.EndsWith(expectedKey, StringComparison.OrdinalIgnoreCase));
            return propertyKey?.Name;
        }

        private string GetRouteResolved(Controller controller, Endpoint endpoint, DbSet dbSet = default)
        {
            var baseRoute = GetBaseRoute(controller.Route);
            var endpointRoute = $"{baseRoute}/{(endpoint.Route ?? string.Empty).Trim('/')}".Trim('/');
            if (string.IsNullOrWhiteSpace(endpointRoute)) return default;

            var replacedRoute = endpointRoute;
            var groups = Regex.Matches(replacedRoute, "{(.*?)}");
            foreach (Match match in groups.Reverse())
            {
                if (match.Groups.Count < 2) continue;
                var group = match.Groups[1];

                var param = group.Value.Contains(":") 
                    ? group.Value.Substring(0, group.Value.IndexOf(":")) : group.Value;
                if (dbSet is not null)
                {
                    var variable = dbSet.Entity.Name.ToLowerCaseFirst();
                    var entityProperty = dbSet.Entity.Properties
                        .FirstOrDefault(p => p.Name.Equals(param, StringComparison.OrdinalIgnoreCase));
                    if (entityProperty is not null)
                    {
                        replacedRoute = group.ReplaceValue(replacedRoute, $"{variable}.{entityProperty.Name}");
                        continue;
                    }
                }

                var inputs = new [] {
                    GetInputBodyClass(endpoint.Inputs),
                    GetInputQueryClass(endpoint.Inputs)
                }.Where(i => i is not null);

                bool resolved = false;
                foreach (var input in inputs)
                {
                    var variable = $"{input.Name.ToLowerCaseFirst()}Request";
                    var modelProperty = input.Properties
                        .FirstOrDefault(p => p.Name.Equals(param, StringComparison.OrdinalIgnoreCase));
                    if (modelProperty is not null)
                    {
                        replacedRoute = group.ReplaceValue(replacedRoute, $"{variable}.{modelProperty.Name}");
                        resolved = true;
                        break;
                    }
                }

                if (!resolved) replacedRoute = group.ReplaceValue(replacedRoute, $"{{{param}}}");
            }

            return replacedRoute;
        }

        private string GetBaseRoute(string route)
        {
            var match = Regex.Match(route, "{(.*?)}");
            if (!match.Success || match.Groups.Count < 2) return string.Empty;

            return route.Substring(match.Index).Trim('/');
        }
    }
}
