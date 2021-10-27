namespace TesTool.Core.Models.Templates.Controller.Asserts
{
    public class ControllerTestMethodSectionAssertDelete : ControllerTestMethodSectionAssertBase
    {
        public ControllerTestMethodSectionAssertDelete(
            bool haveOutput,
            bool responseIsGeneric,
            string propertyData,
            string entityName,
            string entityKey, 
            string entityDbSet
        ) : base(haveOutput, responseIsGeneric, propertyData, entityName)
        {
            EntityKey = entityKey;
            EntityDbSet = entityDbSet;
        }

        public string EntityKey { get; private set; }
        public string EntityDbSet { get; private set; }
    }
}
