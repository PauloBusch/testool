namespace TesTool.Core.Models.Templates.Controller.Asserts
{
    public class ControllerTestMethodSectionAssertGetList : ControllerTestMethodSectionAssertBase
    {
        public ControllerTestMethodSectionAssertGetList(
            bool haveOutput,
            bool responseIsGeneric,
            bool responseHaveKey,
            string propertyData,
            string entityName,
            string entityKey, 
            string comparatorEntity
        ) : base(
            haveOutput, responseIsGeneric,
            propertyData, entityName
        )
        {
            EntityKey = entityKey;
            ResponseHaveKey = responseHaveKey;
            ComparatorEntity = comparatorEntity;
        }

        public bool ResponseHaveKey { get; private set; }
        public string EntityKey { get; private set; }
        public string ComparatorEntity { get; private set; }
    }
}
