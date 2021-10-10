namespace TesTool.Core.Models.Configuration
{
    public class Convention
    {
        public Convention(
            string bogusExpression, 
            string propertyMatch = default,
            string typeMatch = default
        )
        {
            BogusExpression = bogusExpression;
            PropertyMatch = propertyMatch;
            TypeMatch = typeMatch;
        }

        public string BogusExpression { get; private set; }
        public string PropertyMatch { get; private set; }
        public string TypeMatch { get; private set; }
    }
}
