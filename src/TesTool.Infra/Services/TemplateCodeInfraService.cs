using System.Linq;
using TesTool.Core.Interfaces.Services;
using TesTool.Core.Models.Templates.Faker;
using TesTool.Infra.Templates;

namespace TesTool.Infra.Services
{
    public class TemplateCodeInfraService : ITemplateCodeInfraService
    {
        public string ProcessFaker(Bogus model)
        {
            var namespaces = model.Namespaces
                .Distinct()
                .Where(n => n != model.FakerNamespace)
                .OrderBy(n => n)
                .ToArray();

            var template = new FakerTemplate
            {
                Name = model.Name,
                Namespaces = namespaces,
                FakerNamespace = model.FakerNamespace,
                Properties = model.Properties.ToArray()
            };
            return template.TransformText();
        }
    }
}
