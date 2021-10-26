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
            if (inputQuery is not null) inputModels.Add(inputBody.Name);

            return new ControllerTestMethodSectionArrage(entities, inputModels);
        }

        protected ControllerTestMethodSectionAct GetActSection(Endpoint endpoint, DbSet dbSet = default)
        {
            // TODO: map route
            var actSection = new ControllerTestMethodSectionAct(
                GetRouteResolved(endpoint, dbSet),
                _requestMethod.Name,
                GetReturnType(endpoint.Output),
                GetInputBodyClass(endpoint.Inputs)?.Name,
                GetInputQueryClass(endpoint.Inputs)?.Name
            );

            if (Regex.IsMatch(actSection.Route ?? string.Empty, "{{.*?}}")) actSection.MarkAsUnsafe();
            return actSection;
        }

        protected ControllerTestMethodSectionAssert GetAssertSection(Endpoint endpoint, DbSet dbSet)
        {
            var entityKey = GetEntityKey(dbSet.Entity);
            var requestModel = GetModelComparableEntity(endpoint.Inputs, dbSet.Entity);
            var responseModel = GetOutputModel(endpoint.Output);
            return new ControllerTestMethodSectionAssert(
                endpoint.Output is TypeBase type && type.Name != "Void",
                requestModel?.Properties.Any(p => p.Name == entityKey) ?? false,
                responseModel?.Properties.Any(p => p.Name == entityKey) ?? false,
                endpoint.Output is Class output && output.Generics.Any(),
                entityKey, dbSet.Property, GetPropertyData(endpoint.Output),
                dbSet.Entity.Name, requestModel?.Name,
                _compareService.GetComparatorNameOrDefault(requestModel?.Name, responseModel?.Name),
                _compareService.GetComparatorNameOrDefault(requestModel?.Name, dbSet.Entity.Name)
            );
        }

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

            if (output is TypeBase type)
                namespaces.Add(type.Namespace);
            return namespaces;
        }

        private string GetPropertyData(TypeWrapper wrapper)
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

        private Class GetOutputModel(TypeWrapper wrapper)
        {
            if (wrapper is Class @class)
            {
                if (!@class.Generics.Any()) return @class;

                var genericProperty = @class.Properties.FirstOrDefault(p => p.FromGeneric);
                if (genericProperty is null) return @class;

                return GetOutputModel(genericProperty.Type);
            }

            return default;
        }

        private Class GetModelComparableEntity(IEnumerable<Input> inputs, Class entity)
        {
            var inputBody = GetInputBodyClass(inputs);
            if (inputBody is not null && _compareService.IsComparableClasses(inputBody, entity)) return inputBody;

            var inputQuery = GetInputQueryClass(inputs);
            if (inputQuery is not null && _compareService.IsComparableClasses(inputQuery, entity)) return inputQuery;
            
            return default;
        }

        private string GetEntityKey(Class entity)
        {
            if (entity is null) return default;

            var expectedKey = "Id";
            var propertyKey = entity.Properties.FirstOrDefault(p => p.Name.Equals(expectedKey, StringComparison.OrdinalIgnoreCase));
            propertyKey ??= entity.Properties.FirstOrDefault(p => p.Name.EndsWith(expectedKey, StringComparison.OrdinalIgnoreCase));
            return propertyKey?.Name;
        }

        private string GetRouteResolved(Endpoint endpoint, DbSet dbSet = default)
        {
            if (string.IsNullOrWhiteSpace(endpoint.Route)) return default;

            var replacedRoute = endpoint.Route;
            var groups = Regex.Matches(replacedRoute, "{(.*?)}");
            foreach (Match match in groups.Reverse())
            {
                if (match.Groups.Count < 2) continue;
                var group = match.Groups[1];

                var param = group.Value;
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

                foreach (var input in inputs)
                {
                    var variable = $"{input.Name.ToLowerCaseFirst()}Request";
                    var modelProperty = input.Properties
                        .FirstOrDefault(p => p.Name.Equals(param, StringComparison.OrdinalIgnoreCase)); ;
                    if (modelProperty is not null)
                    {
                        replacedRoute = group.ReplaceValue(replacedRoute, $"{variable}.{modelProperty.Name}");
                        break;
                    }
                }

                replacedRoute = group.ReplaceValue(replacedRoute, $"{{{param}}}");
            }

            return replacedRoute;
        }
    }
}
