using System;
using System.Collections.Generic;
using System.Linq;

namespace DSharpCodeAnalysis.Syntax
{
    public static class DSyntaxFactory
    {
        public static Trivia Space => Whitespace(" ");
        public static Trivia LineFeed => EndOfLine("\n");

        public static DClassDeclarationSyntax ClassDeclaration(string identifier)
        {
            var identifierToken = Identifier(identifier);
            var newClass = new DClassDeclarationSyntax(identifierToken);
            identifierToken.Parent = newClass;
            return newClass;
        }

        public static DClassDeclarationSyntax ClassDeclaration(DSyntaxToken identifierToken)
        {
            var newClass = new DClassDeclarationSyntax(identifierToken);
            identifierToken.Parent = newClass;
            return newClass;
        }

        public static DMethodDeclarationSyntax MethodDeclaration(DTypeSyntax returnType, DSyntaxToken identifierToken)
        {
            return new DMethodDeclarationSyntax(returnType, identifierToken);
        }

        public static DSyntaxToken Identifier(string identifier)
        {
            var token = new DSyntaxToken(DSyntaxKind.IdentifierToken)
            {
                Value = identifier
            };
            return token;
        }

        public static DSyntaxToken Identifier(IEnumerable<Trivia> leading, string identifier, IEnumerable<Trivia> trailing)
        {
            var token = new DSyntaxToken(DSyntaxKind.IdentifierToken)
            {
                LeadingTrivia = leading,
                Value = identifier,
                TrailingTrivia = trailing
            };
            return token;
        }

        public static DSyntaxToken Token(DSyntaxKind syntaxKind)
        {
            var token = new DSyntaxToken(syntaxKind);
            return token;
        }

        public static DSyntaxToken Token(IEnumerable<Trivia> leading, DSyntaxKind syntaxKind, IEnumerable<Trivia> trailing)
        {
            return new DSyntaxToken(syntaxKind) { LeadingTrivia = leading, TrailingTrivia = trailing };
        }

        public static DPredefinedTypeSyntax PredefinedType(DSyntaxToken keyword)
        {
            return new DPredefinedTypeSyntax(keyword);
        }

        public static DBlockSyntax Block()
        {
            return new DBlockSyntax();
        }

        public static DBlockSyntax Block(DSyntaxList<DStatementSyntax> statements)
        {
            return new DBlockSyntax(statements);
        }

        public static Trivia SyntaxTrivia(DSyntaxKind syntaxKind, string triviaText)
        {
            return new Trivia(syntaxKind, triviaText);
        }

        public static DSyntaxList<T> SingletonList<T>() where T : DSyntaxNode
        {
            return new DSyntaxList<T>(new List<T>());
        }

        public static DSyntaxList<T> SingletonList<T>(T item) where T : DSyntaxNode
        {
            return new DSyntaxList<T>(new List<T> { item });
        }

        public static DSyntaxList<T> SingletonSeparatedList<T>(T item) where T : DSyntaxNode
        {
            return new DSyntaxList<T>(new List<T> { item });
        }

        public static string SyntaxString(DSyntaxKind syntaxKind)
        {
            return DSyntaxStrings.Get(syntaxKind);
        }

        public static DParameterListSyntax ParameterList()
        {
            return new DParameterListSyntax();
        }

        public static IEnumerable<Trivia> TriviaList()
        {
            return Enumerable.Empty<Trivia>();
        }

        public static IEnumerable<Trivia> TriviaList(Trivia trivia)
        {
            return new List<Trivia> { trivia };
        }

        public static IEnumerable<Trivia> TriviaList(params Trivia[] trivia)
        {
            return trivia;
        }

        public static Trivia Whitespace(string text)
        {
            return Trivia.Create(DSyntaxKind.WhitespaceTrivia, text);
        }

        public static Trivia EndOfLine(string text)
        {
            return Trivia.Create(DSyntaxKind.EndOfLineTrivia, text);
        }

        public static DLocalDeclarationStatementSyntax LocalDeclarationStatement(DVariableDeclarationSyntax variableSyntax)
        {
            return new DLocalDeclarationStatementSyntax(variableSyntax);
        }

        public static DVariableDeclarationSyntax VariableDeclaration(DTypeSyntax typeSyntax)
        {
            return new DVariableDeclarationSyntax(typeSyntax);
        }

        public static DIdentifierNameSyntax IdentifierName(DSyntaxToken identifierToken)
        {
            return new DIdentifierNameSyntax(identifierToken);
        }

        public static DVariableDeclaratorSyntax VariableDeclarator(DSyntaxToken identifierToken)
        {
            return new DVariableDeclaratorSyntax(identifierToken);
        }

        public static DEqualsValueClauseSyntax EqualsValueClause(DExpressionSyntax expressionSyntax)
        {
            return new DEqualsValueClauseSyntax(expressionSyntax);
        }

        public static DExpressionSyntax LiteralExpression(DSyntaxKind syntaxKind, DSyntaxToken syntaxToken)
        {
            return new DLiteralExpressionSyntax(syntaxKind, syntaxToken);
        }

        public static DSyntaxToken Literal(int value)
        {
            var token = new DSyntaxToken(DSyntaxKind.NumericLiteralToken, value);
            return token;
        }
    }
}