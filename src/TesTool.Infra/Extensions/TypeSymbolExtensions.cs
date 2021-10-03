using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Linq;

namespace TesTool.Infra.Extensions
{
    public static class TypeSymbolExtensions
    {
        public static bool ImplementsClass(
            this ITypeSymbol typeSymbol, 
            string className,
            string namespaceName = default
        )
        {
            if (typeSymbol.Name == className)
            {
                if (string.IsNullOrWhiteSpace(namespaceName)) return true;
                if (typeSymbol.ContainingNamespace.ToString() == namespaceName) return true;
            }

            if (typeSymbol.BaseType is not null) 
                return typeSymbol.BaseType.ImplementsClass(className, namespaceName);

            return false;
        }

        public static IEnumerable<ITypeSymbol> GetTypes(this ITypeSymbol typeSymbol)
        {
            var current = typeSymbol;
            while (current != null)
            {
                yield return current;
                current = current.BaseType;
            }
        }

        public static AttributeData GetAttribute(
            this ITypeSymbol typeSymbol,
            string attributeName
        )
        {
            var attribute = typeSymbol.GetAttributes()
                .FirstOrDefault(a => a.AttributeClass.Name == attributeName);
            if (attribute is not null) return attribute;

            if (typeSymbol.BaseType is not null)
                return typeSymbol.BaseType.GetAttribute(attributeName);

            return default;
        }

        public static IEnumerable<ITypeSymbol> GetGeneritTypeArguments(this ITypeSymbol typeSymbol)
        {
            return (typeSymbol as INamedTypeSymbol)?.TypeArguments;
        }
    }
}
