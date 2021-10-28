using System.Collections.Generic;

namespace TesTool.Core.Models.Templates.Controller.Asserts
{
    public class ControllerTestMethodSectionAssertGetOne : ControllerTestMethodSectionAssertBase
    {
        public ControllerTestMethodSectionAssertGetOne(
            bool haveOutput,
            bool responseIsGeneric, 
            string propertyData, 
            string entityName,
            string comparatorEntity
        ) : base(
            haveOutput, responseIsGeneric, 
            propertyData, entityName
        )
        { 
            ComparatorEntity = comparatorEntity;
        }

        public string ComparatorEntity { get; private set; }

        public override IEnumerable<string> GetComparators()
        {
            if (!string.IsNullOrWhiteSpace(ComparatorEntity))
                yield return ComparatorEntity;
        }
    }
}
