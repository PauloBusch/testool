using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq;
using System.Threading.Tasks;
using TesTool.Core.Interfaces.Services;

namespace TesTool.Infra.Services
{
    public class TestCodeInfraService : TestScanInfraService, ITestCodeInfraService
    {
        public TestCodeInfraService(
            ILoggerInfraService loggerInfraService, 
            IEnvironmentInfraService environmentInfraService
        ) : base(loggerInfraService, environmentInfraService) { }
        
        public async Task<string> MergeClassCodeAsync(string className, string sourceCode)
        {
            var classes = await GetClassesAsync();
            var storedContext = classes.FirstOrDefault(c => c.Declaration.Identifier.Text == className);
            if (storedContext is null) return default;
            var storedClass = storedContext.Declaration;

            var compilationUnitGenerated = SyntaxFactory.ParseCompilationUnit(sourceCode);
            var sourceClass = compilationUnitGenerated.DescendantNodes()
                .OfType<ClassDeclarationSyntax>()
                .Single(c => c.Identifier.Text == className);
            var sourceUsings = compilationUnitGenerated.Usings;
            var sourceMethods = sourceClass.Members.OfType<MethodDeclarationSyntax>().ToList();

            var compilationUnitStored = storedContext.Root as CompilationUnitSyntax;
            var storedUsings = compilationUnitStored.Usings;
            var storedMethods = storedClass.Members.OfType<MethodDeclarationSyntax>().ToList();

            var usingsToAppend = sourceUsings.Where(u => !storedUsings.Any(s => s.Name.ToString() == u.Name.ToString())).ToList();
            var methodsToAppend = sourceMethods.Where(s => !storedMethods.Any(m => m.Identifier.Text == s.Identifier.Text)).ToList();

            var updatedClass = null as ClassDeclarationSyntax;
            foreach (var method in methodsToAppend) updatedClass = (updatedClass ?? storedClass).AddMembers(method);
            if (updatedClass is not null) compilationUnitStored = compilationUnitStored.ReplaceNode(storedClass, updatedClass);

            foreach (var @using in usingsToAppend) compilationUnitStored = compilationUnitStored.AddUsings(@using);

            return compilationUnitStored.ToFullString();
        }
    }
}
