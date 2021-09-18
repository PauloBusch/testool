using System;

namespace TesTool.Core.Attributes
{
    public class ParameterAttribute : Attribute
    {
        public ParameterAttribute(string name)
        {
            Name = name;
        }

        public string Name { get; private set; }
    }
}
