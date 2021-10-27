using System;
using System.Collections.Generic;

namespace TesTool.Core.Models.Templates.Controller.Asserts
{
    public abstract class ControllerTestMethodSectionAssertBase
    {
        protected ControllerTestMethodSectionAssertBase(
            bool haveOutput,
            bool responseIsGeneric,
            string propertyData,
            string entityName
        )
        {
            HaveOutput = haveOutput;
            ResponseIsGeneric = responseIsGeneric;
            PropertyData = propertyData;
            EntityName = entityName;
        }

        public bool HaveOutput { get; private set; }
        public bool ResponseIsGeneric { get; private set; }
        public string PropertyData { get; private set; }
        public string EntityName { get; private set; }

        public virtual IEnumerable<string> GetComparators() => Array.Empty<string>();
    }
}
