﻿namespace TesTool.Core.Models.Templates.Comparator
{
    public class CompareObject
    {
        public CompareObject(
            string propertyName, 
            string comparerName,
            bool @unsafe
        )
        {
            PropertyName = propertyName;
            ComparerClassName = comparerName;
            Unsafe = @unsafe;
        }

        public bool Unsafe { get; private set; }
        public string PropertyName { get; private set; }
        public string ComparerClassName { get; private set; }
    }
}
