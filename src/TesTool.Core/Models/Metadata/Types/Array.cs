using System.Collections.Generic;

namespace TesTool.Core.Models.Metadata
{
    public class Array : TypeBase
    {
        private readonly List<TypeWrapper> _generics;

        public Array(string name, string @namespace, TypeWrapper type) 
            : base(nameof(Array), name, @namespace) 
        { 
            Type = type;
            _generics = new List<TypeWrapper>();
        }

        public TypeWrapper Type { get; private set; }
        public IReadOnlyCollection<TypeWrapper> Generics => _generics.AsReadOnly();

        public void AddGeneric(TypeWrapper generic) => _generics.Add(generic);
    }
}
