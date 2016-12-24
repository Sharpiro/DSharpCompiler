using DSharpCodeAnalysis.Syntax;
using System;
using System.Linq;

namespace DSharpCodeAnalysis.Transpiler
{
    public class CTranspiler
    {
        private readonly DCompilationUnitSyntax _originalCompilation;
        private readonly DCompilationUnitSyntax _newCompilation;

        public CTranspiler(DCompilationUnitSyntax compilation)
        {
            if (compilation == null) throw new ArgumentNullException(nameof(compilation));

            _originalCompilation = compilation;
            _newCompilation = compilation.Clone<DCompilationUnitSyntax>();
        }

        public DCompilationUnitSyntax Transpile()
        {
            TranspileClassDeclarations();
            TranspileMethodDeclarations();
            TranspileVarIdentifiers();
            return _newCompilation;
        }

        private void TranspileClassDeclarations()
        {
            var declarations = _newCompilation.DescendantNodesAndTokens()
                .OfType<DClassDeclarationSyntax>().ToList();

            foreach (var declaration in declarations)
            {
                if (declaration.Keyword.ValueText != "type") continue;

                var parent = declaration.Parent as IMemberHolder;

                var newDeclaration = declaration.Clone<DClassDeclarationSyntax>()
                    .WithKeyword(DSyntaxFactory.Token(DSyntaxKind.ClassKeyword, "class")
                    .WithLeadingTrivia(declaration.Keyword.LeadingTrivia)
                    .WithTrailingTrivia(declaration.Keyword.TrailingTrivia))
                    .WithParent<DClassDeclarationSyntax>(declaration.Parent);

                parent.Members.Replace(declaration, newDeclaration);
            }
        }

        private void TranspileVarIdentifiers()
        {
            var varIdentifiers = _newCompilation.DescendantNodesAndTokens().OfType<DIdentifierNameSyntax>()
                .SelectMany(v => v.ChildTokens()).ToList();
            foreach (var oldVariable in varIdentifiers)
            {
                var parent = oldVariable.Parent as DIdentifierNameSyntax;
                if (oldVariable.ValueText != "let") continue;
                var newVariable = oldVariable.Clone();
                newVariable.Value = "var";
                parent.Identifier = newVariable;
            }
        }

        private void TranspileMethodDeclarations()
        {
            var methods = _newCompilation.DescendantNodesAndTokens().OfType<DMethodDeclarationSyntax>();
            foreach (var oldMethod in methods)
            {
                var parent = oldMethod.Parent as IMemberHolder;
                var newMethod = oldMethod.WithFunctionKeyword(null);
                parent.Members.Replace(oldMethod, newMethod);
            }
        }
    }
}