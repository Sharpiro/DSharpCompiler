using System.Collections.Generic;
using System.Linq;

namespace DSharpCodeAnalysis.Syntax
{
    public static class DSyntaxFactory
    {
        public static DClassDeclarationSyntax ClassDeclaration(string identifier)
        {
            var identifierToken = Identifier(identifier);
            return new DClassDeclarationSyntax(identifierToken);
        }

        public static DMethodDeclarationSyntax MethodDeclaration(DTypeSyntax returnType, DSyntaxToken identifierToken)
        {
            var returnKeyword = Token(DSyntaxKind.VoidKeyword);
            var predefinedTYpe = PredefinedType(returnKeyword);

            return new DMethodDeclarationSyntax(predefinedTYpe, identifierToken);
        }

        public static DSyntaxToken Identifier(string identifier)
        {
            var token = new DSyntaxToken(DSyntaxKind.IdentifierToken)
            {
                ValueText = identifier
            };
            return token;
        }

        public static DSyntaxToken Token(DSyntaxKind syntaxKind)
        {
            return new DSyntaxToken(syntaxKind);
        }

        public static DPredefinedTypeSyntax PredefinedType(DSyntaxToken keyword)
        {
            return new DPredefinedTypeSyntax(keyword);
        }

        public static IEnumerable<T> SingletonList<T>(T item)
        {
            return Enumerable.Repeat(item, 1);
        }
    }
}