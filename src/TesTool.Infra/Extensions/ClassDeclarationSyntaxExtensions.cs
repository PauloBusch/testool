using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq;

namespace TesTool.Infra.Extensions
{
    public static class ClassDeclarationSyntaxExtensions
    {
        public static bool IsAbstract(this ClassDeclarationSyntax classDeclarationSyntax)
            => classDeclarationSyntax.Modifiers.Any(x => x.IsKind(SyntaxKind.AbstractKeyword));
    }
}
