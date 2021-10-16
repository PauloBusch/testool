using TesTool.Core.Models.Templates.Comparator;
using TesTool.Core.Models.Templates.Factory;
using TesTool.Core.Models.Templates.Faker;

namespace TesTool.Core.Interfaces.Services
{
    public interface ITemplateCodeInfraService
    {
        string BuildModel(Bogus model);
        string BuildModelFactory(ModelFactory model);
        string BuildModelFactoryMethod(ModelFactoryMethod model);
        string BuildCompareStatic(CompareStatic model);
        string BuildCompareDynamic(CompareDynamic model);
        string BuildComparatorFactory(ComparatorFactory model);
        string BuildComparatorFactoryMethod(ComparatorFactoryMethod model);
        string BuildAssertExtensions(string @namespace);
        string BuildHttpRequest(string @namespace);
    }
}
