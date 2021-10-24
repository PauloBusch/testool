using TesTool.Core.Models.Templates.Comparator;
using TesTool.Core.Models.Templates.Controller;
using TesTool.Core.Models.Templates.Factories;
using TesTool.Core.Models.Templates.Faker;

namespace TesTool.Core.Interfaces.Services
{
    public interface ITemplateCodeInfraService
    {
        string BuildControllerTest(ControllerTest model);
        string BuildControllerTestMethod(ControllerTestMethod model);

        string BuildControllerTestMethodSectionArrage(ControllerTestMethodSectionArrage model);
        string BuildControllerTestMethodSectionAct(ControllerTestMethodSectionAct model);
        string BuildControllerTestMethodSectionAssertPost(ControllerTestMethodSectionAssert model);

        string BuildModelFaker(ModelFaker model);
        string BuildModelFakerFactory(ModelFakerFactory model);
        string BuildModelFakerFactoryMethod(ModelFakerFactoryMethod model);

        string BuildEntityFaker(EntityFaker model);
        string BuildEntityFakerFactory(EntityFakerFactory model);
        string BuildEntityFakerFactoryMethod(EntityFakerFactoryMethod model);

        string BuildCompareStatic(CompareStatic model);
        string BuildCompareDynamic(CompareDynamic model);

        string BuildComparatorFactory(ComparatorFactory model);
        string BuildComparatorFactoryMethod(ComparatorFactoryMethod model);

        string BuildAssertExtensions(string @namespace);
        string BuildHttpRequest(string @namespace);
    }
}
