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
            TranspileObjectCreationExpressions();
            TranspileModifiers();
            TranspileClassDeclarations();
            TranspileMethodDeclarations();
            TranspileVarIdentifiers();
            return _newCompilation;
        }

        private void TranspileObjectCreationExpressions()
        {
            var invocationExpressions = _newCompilation.DescendantNodesAndTokens().OfType<DInvocationExpressionSyntax>();

            foreach (var invocationExpression in invocationExpressions)
            {
                var type = invocationExpression.Expression.GetType();
                if (type == typeof(DMemberAccessExpression))
                {
                    var memberAccessExpression = invocationExpression.Expression as DMemberAccessExpression;
                    if (memberAccessExpression.Name.Identifier.ValueText != "New") continue;

                    var newKeyword = DSyntaxFactory.Token(DSyntaxFactory.TriviaList(), DSyntaxKind.NewKeyword, DSyntaxFactory.TriviaList(DSyntaxFactory.Space));
                    var qualifiedName = DSyntaxFactory.QualifiedName(memberAccessExpression);
                    var simplifiedName = qualifiedName.RemoveLastIdentifier();
                    var objectCreationExpression = DSyntaxFactory.ObjectCreationExpression(newKeyword,
                        simplifiedName, invocationExpression.ArgumentList.Clone<DArgumentListSyntax>());

                    var parent = invocationExpression.Parent as DEqualsValueClauseSyntax;
                    if (parent != null)
                        parent.Value = objectCreationExpression;
                    var parentX = invocationExpression.Parent as DMemberAccessExpression;
                    if (parentX != null)
                        parentX.Expression = objectCreationExpression;

                }
            }
        }

        private void TranspileModifiers()
        {
            var modifers = _newCompilation.DescendantNodesAndTokens().OfType<DSyntaxToken>()
                .Where(t => t.SyntaxKind == DSyntaxKind.PublicKeyword || t.SyntaxKind == DSyntaxKind.PrivateKeyword);

            foreach (var modifer in modifers)
            {
                var value = modifer.SyntaxKind == DSyntaxKind.PublicKeyword ? "public" : "private";
                var newModifer = DSyntaxFactory.Token(modifer.SyntaxKind, value)
                    .WithParent(modifer.Parent).WithLeadingTrivia(modifer.LeadingTrivia.Clone())
                    .WithTrailingTrivia(modifer.TrailingTrivia.Clone());
                var parent = modifer.Parent as DMethodDeclarationSyntax;
                parent.Modifiers.Replace(modifer, newModifer);
            }
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
                    .WithLeadingTrivia(declaration.Keyword.LeadingTrivia.Clone())
                    .WithTrailingTrivia(declaration.Keyword.TrailingTrivia.Clone()))
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