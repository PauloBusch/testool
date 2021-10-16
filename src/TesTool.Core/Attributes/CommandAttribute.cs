using System;

namespace TesTool.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class CommandAttribute : AttributeBase
    {
        public CommandAttribute(string name, string alias = null)
        {
            Name = name;
            Alias = alias;
        }

        public int Order { get; set; }
        public string Name { get; private set; }
        public string Alias { get; private set; }

        public bool Equals(string value)
        {
            if (Name.Equals(value)) return true;
            if (!string.IsNullOrWhiteSpace(Alias)) return Alias.Equals(value);
            return false;
        }
    }
}
