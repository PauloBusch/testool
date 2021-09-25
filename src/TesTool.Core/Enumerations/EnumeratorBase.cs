using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace TesTool.Core.Enumerations
{
    public abstract class EnumeratorBase<TEnumerator, TModel>
            where TEnumerator : EnumeratorBase<TEnumerator, TModel>
            where TModel : class
    {
        private static IReadOnlyCollection<TModel> _collection;

        public static IReadOnlyCollection<TModel> GetAll()
        {
            if (_collection is not null) return _collection;

            _collection = typeof(TEnumerator)
                .GetFields(
                    BindingFlags.Public |
                    BindingFlags.Static |
                    BindingFlags.DeclaredOnly
                )
                .Select(f => f.GetValue(null))
                .Cast<TModel>()
                .ToList()
                .AsReadOnly();

            return _collection;
        }
    }
}
