using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp;

namespace DSharpCodeAnalysis.Syntax
{
    public static class DSyntaxFactory
    {
        public static Trivia Space => WhiteSpace(" ");

        public static DClassDeclarationSyntax ClassDeclaration(string identifier)
        {
            var identifierToken = Identifier(identifier);
            var newClass = new DClassDeclarationSyntax(identifierToken);
            identifierToken.Parent = newClass;
            return newClass;
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
            var token = new DSyntaxToken(syntaxKind);
            return token;
        }

        public static DSyntaxToken Token(IEnumerable<Trivia> leading, DSyntaxKind classKeyword, IEnumerable<Trivia> trailing)
        {
            return new DSyntaxToken(classKeyword) { LeadingTrivia = leading, TrailingTrivia = trailing };
        }

        public static DPredefinedTypeSyntax PredefinedType(DSyntaxToken keyword)
        {
            return new DPredefinedTypeSyntax(keyword);
        }

        public static DBlockSyntax Block()
        {
            return new DBlockSyntax();
        }

        public static Trivia SyntaxTrivia(DSyntaxKind syntaxKind, string triviaText)
        {
            return new Trivia(syntaxKind, triviaText);
        }

        public static IEnumerable<T> SingletonList<T>(T item)
        {
            return Enumerable.Repeat(item, 1);
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

        public static IEnumerable<Trivia> TriviaList(Trivia space)
        {
            return new List<Trivia> { space };
        }

        public static Trivia WhiteSpace(string text)
        {
            return Trivia.Create(DSyntaxKind.WhitespaceTrivia, text);
        }
    }
}