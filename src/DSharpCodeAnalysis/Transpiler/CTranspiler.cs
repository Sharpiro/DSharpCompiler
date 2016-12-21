using DSharpCodeAnalysis.Syntax;
using System;
using System.Linq;

namespace DSharpCodeAnalysis.Transpiler
{
    public class CTranspiler
    {
        private readonly DCompilationUnitSyntax _compilation;

        public CTranspiler(DCompilationUnitSyntax compilation)
        {
            if (compilation == null) throw new ArgumentNullException(nameof(compilation));

            _compilation = compilation;
        }

        public DCompilationUnitSyntax Transpile()
        {
            TranspileMethodDeclarations();
            TranspileVarIdentifiers();
            return _compilation;
        }

        private void TranspileVarIdentifiers()
        {
            var varIdentifiers = _compilation.DescendantNodesAndTokens().OfType<DIdentifierNameSyntax>()
                .SelectMany(v => v.ChildTokens()).ToList();
            foreach (var variable in varIdentifiers)
            {
                if (variable.ValueText == "let")
                    variable.Value = "var";
            }
        }

        private void TranspileMethodDeclarations()
        {
            var methods = _compilation.DescendantNodesAndTokens().OfType<DMethodDeclarationSyntax>();
            foreach (var method in methods)
            {
                method.WithFunctionKeyword(null);
            }
        }
    }
}