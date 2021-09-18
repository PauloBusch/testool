using System;

namespace TesTool.Core.Attributes
{
    public class FlagAttribute : Attribute
    {
        public FlagAttribute(string name)
        {
            Name = name;
        }

        public string Name { get; private set; }
    }
}
