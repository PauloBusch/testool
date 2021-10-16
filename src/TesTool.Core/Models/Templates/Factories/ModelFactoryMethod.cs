namespace TesTool.Core.Models.Templates.Factory
{
    public class ModelFactoryMethod
    {
        public ModelFactoryMethod(string name, string faker)
        {
            Name = name;
            Faker = faker;
        }

        public string Name { get; private set; }
        public string Faker { get; private set; }
    }
}
