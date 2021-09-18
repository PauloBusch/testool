using System;

namespace TesTool.Core.Attributes
{
    public class CommandAttribute : Attribute
    {
        public CommandAttribute(string name)
        {
            Name = name;
        }

        public string Name { get; private set; }
    }
}
