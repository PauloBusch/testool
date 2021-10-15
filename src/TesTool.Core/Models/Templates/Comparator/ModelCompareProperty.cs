namespace TesTool.Core.Models.Templates.Comparator
{
    public class ModelCompareProperty
    {
        public ModelCompareProperty(string propertyName)
        {
            PropertyName = propertyName;
        }

        public string PropertyName { get; private set; }
    }
}
