using System;

namespace TesTool.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ParameterAttribute : Attribute {
        public bool IsDefault { get; set; }
    }
}
