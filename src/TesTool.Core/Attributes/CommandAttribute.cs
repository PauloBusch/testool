using System;

namespace TesTool.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class CommandAttribute : Attribute
    {
        public CommandAttribute(string name, string alias = null)
        {
            Name = name;
            Alias = alias;
        }

        public string Name { get; private set; }
        public string Alias { get; private set; }
    }
}
