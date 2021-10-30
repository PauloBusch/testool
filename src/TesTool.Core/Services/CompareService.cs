using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using TesTool.Core.Interfaces.Services;
using TesTool.Core.Models.Metadata;
using TesTool.Core.Models.Templates.Comparator;

namespace TesTool.Core.Services
{
    public class CompareService : ICompareService
    {
        private readonly ISolutionInfraService _solutionService;
        private readonly ITestScanInfraService _testScanInfraService;

        public CompareService(
            ISolutionInfraService solutionService, 
            ITestScanInfraService testScanInfraService
        )
        {
            _solutionService = solutionService;
            _testScanInfraService = testScanInfraService;
        }

        public async Task<CompareDynamic> GetCompareDynamicAsync(Class source, Class target)
        {
            if (!IsComparableClasses(source, target)) throw new ValidationException("None properties to assert Equals.");

            var sourceClassName = source.Name;
            var targetClassName = target.Name;
            var templateModel = new CompareDynamic(GetNamespace(),
                GetComparatorName(sourceClassName, targetClassName),
                sourceClassName, targetClassName
            );
            templateModel.AddNamespace(source.Namespace);
            templateModel.AddNamespace(target.Namespace);
            return await Task.FromResult(templateModel);
        }

        public async Task<CompareStatic> GetCompareStaticAsync(Class source, Class target)
        {
            var propertiesToAssert = source.Properties.Where(p => target.Properties.Any(p1 => p1.Name == p.Name && p1.Type.Wrapper == p.Type.Wrapper));
            if (!propertiesToAssert.Any()) throw new ValidationException("None properties to assert Equals.");

            var sourceClassName = source.Name;
            var targetClassName = target.Name;
            var templateModel = new CompareStatic(GetNamespace(), 
                GetComparatorName(sourceClassName, targetClassName), 
                sourceClassName, targetClassName
            );
            templateModel.AddNamespace(source.Namespace);
            templateModel.AddNamespace(target.Namespace);

            foreach (var sourceProperty in propertiesToAssert)
            {
                var targetProperty = target.Properties.Single(p => p.Name == sourceProperty.Name && p.Type.Wrapper == sourceProperty.Type.Wrapper);
                if (sourceProperty.Type is Field || sourceProperty.Type is Enum)
                {
                    var propertyCompare = new CompareProperty(sourceProperty.Name);
                    templateModel.AddProperty(propertyCompare);
                }
                else if (sourceProperty.Type is Class sourceClass)
                {
                    var targetClass = targetProperty.Type as Class;
                    var comparatorClass = await GetComparatorClassAsync(sourceClass.Name, targetClass.Name);
                    if (comparatorClass is not null) templateModel.AddNamespace(comparatorClass.Namespace);
                    var comparatorClassName = comparatorClass?.Name ?? $"{sourceClass.Name}Equals{targetClass.Name}";

                    var comparerObject = new CompareObject(sourceProperty.Name, comparatorClassName, comparatorClass is null);
                    templateModel.AddComparer(comparerObject);
                }
            }

            return templateModel;
        }

        public string GetComparatorNameOrDefault(string sourceClassName, string targetClassName)
        {
            if (string.IsNullOrWhiteSpace(sourceClassName) || string.IsNullOrWhiteSpace(targetClassName)) return default;

            var comparator = GetComparatorClassAsync(sourceClassName, targetClassName).Result;
            if (comparator is null)
            {
                var classes = new [] { sourceClassName, targetClassName }
                    .OrderBy(c => c.Length)
                    .ThenBy(c => c);
                return GetComparatorName(classes.ElementAt(0), classes.ElementAt(1));
            }
            return comparator.Name;
        }

        public string GetComparatorName(string sourceClassName, string targetClassName)
        {
            return $"{sourceClassName}Equals{targetClassName}";
        }

        public async Task<Class> GetComparatorClassAsync(string sourceClassName, string targetClassName)
        {
            var comparatorClassName1 = $"{sourceClassName}Equals{targetClassName}";
            var comparatorClass1 = await _testScanInfraService.GetClassAsync(comparatorClassName1);
            if (comparatorClass1 is not null) return comparatorClass1;

            var comparatorClassName2 = $"{targetClassName}Equals{sourceClassName}";
            var comparatorClass2 = await _testScanInfraService.GetClassAsync(comparatorClassName2);
            if (comparatorClass2 is not null) return comparatorClass2;

            return default;
        }

        public string GetNamespace()
        {
            return _solutionService.GetTestNamespace("Assertions.Comparators");
        }

        public string GetDirectoryBase()
        {
            return $"{_testScanInfraService.GetDirectoryBase()}/Assertions/Comparators";
        }

        public bool IsComparableClasses(Class source, Class target)
        {
            if (source is null || target is null) return false;
            return source.Properties.Where(p => target.Properties.Any(p1 => p1.Name == p.Name && p1.Type.Wrapper == p.Type.Wrapper)).Any();
        }
    }
}
