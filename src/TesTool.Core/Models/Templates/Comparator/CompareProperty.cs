namespace TesTool.Core.Models.Templates.Comparator
{
    public class CompareProperty
    {
        public CompareProperty(string propertyName)
        {
            PropertyName = propertyName;
        }

        public string PropertyName { get; private set; }
    }
}
