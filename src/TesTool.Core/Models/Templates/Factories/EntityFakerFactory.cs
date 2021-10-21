using TesTool.Core.Models.Metadata;

namespace TesTool.Core.Models.Templates.Factories
{
    public class EntityFakerFactory : FactoryBase<EntityFakerFactoryMethod>
    {
        public string TestBase { get; set; }
        public string DbContext { get; set; }

        public EntityFakerFactory(
            string name, 
            string factoryNamespace,
            Class testBase,
            Class dbContext
        ) : base(name, factoryNamespace) 
        {
            TestBase = testBase.Name;
            DbContext = dbContext.Name;

            AddNamespace(testBase.Namespace);
            AddNamespace(dbContext.Namespace);
        }
    }
}
