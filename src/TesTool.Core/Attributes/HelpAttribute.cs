using System;

namespace TesTool.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class HelpAttribute : Attribute
    {
        public HelpAttribute(string description)
        {
            Description = description;
        }

        public string Description { get; private set; }
    }
}
