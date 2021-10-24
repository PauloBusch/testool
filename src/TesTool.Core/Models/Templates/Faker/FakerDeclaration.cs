using TesTool.Core.Extensions;

namespace TesTool.Core.Models.Templates.Faker
{
    public class FakerDeclaration
    {
        public FakerDeclaration(string name)
        {
            Name = name;
            Variable = name.ToLowerCaseFirst();
        }

        public string Name { get; private set; }
        public string Variable { get; private set; }
    }
}
