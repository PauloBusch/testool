using System.Collections.Generic;

namespace TesTool.Core.Models.Templates.Controller.Asserts
{
    public class ControllerTestMethodSectionAssertPost : ControllerTestMethodSectionAssertBase
    {
        public ControllerTestMethodSectionAssertPost(
            bool requestHaveKey,
            bool responseHaveKey,
            bool haveOutput,
            bool responseIsGeneric,
            string propertyData,
            string entityName,
            string entityKey,
            string entityDbSet,
            string requestModel,
            string comparatorModel,
            string comparatorEntity
        ) : base(haveOutput, responseIsGeneric, propertyData, entityName)
        {
            RequestHaveKey = requestHaveKey;
            ResponseHaveKey = responseHaveKey;
            EntityKey = entityKey;
            EntityDbSet = entityDbSet;
            RequestModel = requestModel;
            ComparatorModel = comparatorModel;
            ComparatorEntity = comparatorEntity;
        }

        public bool RequestHaveKey { get; private set; }
        public bool ResponseHaveKey { get; private set; }
        public string EntityKey { get; private set; }
        public string EntityDbSet { get; private set; }
        public string RequestModel { get; private set; }
        public string ComparatorModel { get; private set; }
        public string ComparatorEntity { get; private set; }

        public override IEnumerable<string> GetComparators()
        {
            if (!string.IsNullOrWhiteSpace(ComparatorModel))
                yield return ComparatorModel;
            if (!string.IsNullOrWhiteSpace(ComparatorEntity))
                yield return ComparatorEntity;
        }
    }
}
