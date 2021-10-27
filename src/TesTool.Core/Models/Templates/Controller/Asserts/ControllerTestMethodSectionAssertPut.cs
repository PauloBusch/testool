using System.Collections.Generic;

namespace TesTool.Core.Models.Templates.Controller.Asserts
{
    public class ControllerTestMethodSectionAssertPut : ControllerTestMethodSectionAssertBase
    {
        public ControllerTestMethodSectionAssertPut(
            bool haveOutput,
            bool responseIsGeneric,
            string propertyData,
            string entityName,
            string requestModel,
            string comparatorModel,
            string comparatorEntity
        ) : base(haveOutput, responseIsGeneric, propertyData, entityName)
        {
            RequestModel = requestModel;
            ComparatorModel = comparatorModel;
            ComparatorEntity = comparatorEntity;
        }

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
