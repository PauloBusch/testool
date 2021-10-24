namespace TesTool.Core.Models.Metadata.Types
{
    public class DbSet : TypeWrapper
    {
        public DbSet(
            Class entity,
            string property
        ) : base(nameof(DbSet))
        {
            Entity = entity;
            Property = property;
        }

        public string Property { get; private set; }
        public Class Entity { get; private set; }
    }
}
