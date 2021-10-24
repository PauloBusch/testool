namespace TesTool.Core.Models.Templates.Controller
{
    public class ControllerTestMethodSectionAssert
    {
        public ControllerTestMethodSectionAssert(
            bool haveOutput, 
            bool requestHaveKey, 
            bool responseHaveKey, 
            bool responseIsGeneric,
            string entityKey, 
            string entityDbSet, 
            string propertyData, 
            string entityName, 
            string requestModel,
            string comparatorModel,
            string comparatorEntity
        )
        {
            HaveOutput = haveOutput;
            RequestHaveKey = requestHaveKey;
            ResponseHaveKey = responseHaveKey;
            ResponseIsGeneric = responseIsGeneric;
            EntityKey = entityKey;
            EntityDbSet = entityDbSet;
            PropertyData = propertyData;
            EntityName = entityName;
            RequestModel = requestModel;
            ComparatorModel = comparatorModel;
            ComparatorEntity = comparatorEntity;
        }

        public bool HaveOutput { get; private set; }
        public bool RequestHaveKey { get; private set; }
        public bool ResponseHaveKey { get; private set; }
        public bool ResponseIsGeneric { get; private set; }
        public string EntityKey { get; private set; }
        public string EntityDbSet { get; private set; }
        public string PropertyData { get; private set; }
        public string EntityName { get; private set; }
        public string RequestModel { get; private set; }
        public string ComparatorModel { get; private set; }
        public string ComparatorEntity { get; private set; }
    }
}
