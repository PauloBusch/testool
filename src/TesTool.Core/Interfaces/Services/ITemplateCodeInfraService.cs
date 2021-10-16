using TesTool.Core.Models.Templates.Comparator;
using TesTool.Core.Models.Templates.Factory;
using TesTool.Core.Models.Templates.Faker;

namespace TesTool.Core.Interfaces.Services
{
    public interface ITemplateCodeInfraService
    {
        string ProcessFaker(Bogus model);
        string ProcessFakerFactory(ModelFactory model);
        string ProcessFakerFactoryMethod(ModelFactoryMethod model);
        string ProcessComparerStatic(CompareStatic model);
        string ProcessComparerDynamic(CompareDynamic model);
        string ProcessComparerFactory(ComparatorFactory model);
        string ProcessComparerFactoryMethod(ComparatorFactoryMethod model);
        string ProcessAssertExtensions(string @namespace);
        string ProcessHttpRequest(string @namespace);
    }
}
