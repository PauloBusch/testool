using System;

namespace TesTool.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class FlagAttribute : Attribute
    {
        public FlagAttribute(string name)
        {
            Name = name;
        }

        public string Name { get; private set; }
    }
}
