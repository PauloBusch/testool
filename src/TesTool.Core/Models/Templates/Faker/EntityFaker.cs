using TesTool.Core.Models.Metadata;

namespace TesTool.Core.Models.Templates.Faker
{
    public class EntityFaker : FakerBase<EntityProperty>
    {
        public EntityFaker(
            string name, 
            string fakerNamespace,
            Class entityFakerBase,
            Class dbContext
        ) : base(name, fakerNamespace) 
        { 
            DbContext = dbContext.Name;

            AddNamespace(dbContext.Namespace);
            AddNamespace(entityFakerBase.Namespace);
        }

        public string DbContext { get; private set; }
    }
}
