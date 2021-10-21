namespace TesTool.Core.Models.Templates.Factories
{
    public class EntityFakerFactoryMethod
    {
        public EntityFakerFactoryMethod(string name, string faker)
        {
            Name = name;
            Faker = faker;
        }

        public string Name { get; private set; }
        public string Faker { get; private set; }
    }
}
