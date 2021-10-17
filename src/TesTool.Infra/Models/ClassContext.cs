using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace TesTool.Infra.Models
{
    public class ClassContext
    {
        public ClassContext(
            ClassDeclarationSyntax declaration, 
            ITypeSymbol typeSymbol,
            SyntaxNode root,
            string filePath
        )
        {
            TypeSymbol = typeSymbol;
            Declaration = declaration;
            FilePath = filePath;
            Root = root;
        }

        public string FilePath { get; private set; }
        public ITypeSymbol TypeSymbol { get; private set; }
        public ClassDeclarationSyntax Declaration { get; private set; }
        public SyntaxNode Root { get; private set; }
    }
}
