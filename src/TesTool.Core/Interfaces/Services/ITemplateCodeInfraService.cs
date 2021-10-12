using TesTool.Core.Models.Templates.Factory;
using TesTool.Core.Models.Templates.Faker;

namespace TesTool.Core.Interfaces.Services
{
    public interface ITemplateCodeInfraService
    {
        string ProcessFaker(Bogus model);
        string ProcessFakerFactory(ModelFactory model);
        string ProcessFakerFactoryMethod(ModelFactoryMethod model);
    }
}
