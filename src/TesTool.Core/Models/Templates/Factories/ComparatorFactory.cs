namespace TesTool.Core.Models.Templates.Factories
{
    public class ComparatorFactory : FactoryBase<ComparatorFactoryMethod>
    {
        public ComparatorFactory(
            string name,
            string factoryNamespace,
            bool @static
        ) : base(name, factoryNamespace)
        {
            Static = @static;
        }

        public bool Static { get; private set; }
    }
}
