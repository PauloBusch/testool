using System;

namespace TesTool.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ParameterAttribute : Attribute {
        public ParameterAttribute()
        {
            IsRequired = true;
        }

        public bool IsRequired { get; set; }
        public bool IsDefault { get; set; }
    }
}
