using Microsoft.CodeAnalysis;
using System;
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

        public static IEnumerable<IMethodSymbol> GetMethods(this ITypeSymbol typeSymbol)
        {
            return typeSymbol.GetStackTypes().Reverse()
                    .SelectMany(t => t.GetMembers())
                    .OfType<IMethodSymbol>()
                    .Where(x => x.DeclaredAccessibility == Accessibility.Public)
                    .Where(m => m.MethodKind == MethodKind.Ordinary)
                    .ToList();
        }

        public static IEnumerable<ITypeSymbol> GetStackTypes(this ITypeSymbol typeSymbol)
        {
            var current = typeSymbol;
            while (current != null)
            {
                if (current.Name != nameof(Object)) yield return current;
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

        public static string GetName(this ITypeSymbol type)
        {
            var display = type.GetDisplayString();
            if (display.Contains("<")) display = display.Split("<").First();
            return display.Split(".").Last();
        }

        public static string GetNamespace(this ITypeSymbol type)
        {
            var display = type.GetDisplayString();
            if (display.Contains("<")) display = display.Split("<").First();
            var displayParts = display.Split(".");
            var name = displayParts.Last();
            return string.Join(".", displayParts.Where(p => p != name));
        }

        public static string GetDisplayString(this ITypeSymbol type)
        {
            if (type.IsNullable()) return type.NullableOf().GetDisplayString();

            var name = type.ToDisplayString();
            if (_fullNamesMaping.ContainsKey(name)) return _fullNamesMaping[name];

            return name;
        }

        private static readonly Dictionary<string, string> _fullNamesMaping = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            {"string",     typeof(string).ToString()},
            {"long",       typeof(long).ToString()},
            {"int",        typeof(Int32).ToString()},
            {"short",      typeof(short).ToString()},
            {"ulong",      typeof(ulong).ToString()},
            {"uint",       typeof(uint).ToString()},
            {"ushort",     typeof(ushort).ToString()},
            {"byte",       typeof(byte).ToString()},
            {"double",     typeof(double).ToString()},
            {"float",      typeof(float).ToString()},
            {"decimal",    typeof(decimal).ToString()},
            {"bool",       typeof(bool).ToString()},
        };
    }
}
