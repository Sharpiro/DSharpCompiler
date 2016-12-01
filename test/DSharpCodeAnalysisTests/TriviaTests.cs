using DSharpCodeAnalysis.Models;
using DSharpCodeAnalysis.Parser;
using DSharpCodeAnalysis.Syntax;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using Xunit;

namespace DSharpCodeAnalysisTests
{
    public class TriviaTests
    {
        [Fact]
        public void OneLineClassTriviaTest()
        {
            const string desiredSource = "class Test{}";

            var cGeneratedClass = SyntaxFactory.ClassDeclaration("Test").WithKeyword(
                SyntaxFactory.Token(SyntaxFactory.TriviaList(), SyntaxKind.ClassKeyword,
                SyntaxFactory.TriviaList(SyntaxFactory.Space)));

            var dGeneratedClass = DSyntaxFactory.ClassDeclaration("Test").WithKeyword(
                DSyntaxFactory.Token(DSyntaxFactory.TriviaList(), DSyntaxKind.ClassKeyword,
                DSyntaxFactory.TriviaList(DSyntaxFactory.Space)));


            var cDescendants = cGeneratedClass.DescendantNodesAndTokens().ToList();
            var dDescendants = dGeneratedClass.DescendantNodesAndTokens().ToList();


            var cgeneratedClassString = cGeneratedClass.ToString();
            var dgeneratedClassString = dGeneratedClass.ToString();

            Assert.Equal(desiredSource, cgeneratedClassString);
            Assert.Equal(desiredSource, dgeneratedClassString);
        }

        [Fact]
        public void ClassMethodTriviaTest()
        {
            //const string desiredSource = "class Test\n{\n    void Do()\n    {\n\n    }\n}";
            var desiredSource =
@"class Test
{
	void Do()
	{
		var x = 2;
	}
}".Replace(Environment.NewLine, "\n");

            var cGeneratedClass = SyntaxFactory.ClassDeclaration(
            SyntaxFactory.Identifier(
                SyntaxFactory.TriviaList(),
                "Test",
                SyntaxFactory.TriviaList(
                    SyntaxFactory.LineFeed)))
        .WithKeyword(
            SyntaxFactory.Token(
                SyntaxFactory.TriviaList(),
                SyntaxKind.ClassKeyword,
                SyntaxFactory.TriviaList(
                    SyntaxFactory.Space)))
        .WithOpenBraceToken(
            SyntaxFactory.Token(
                SyntaxFactory.TriviaList(),
                SyntaxKind.OpenBraceToken,
                SyntaxFactory.TriviaList(
                    SyntaxFactory.LineFeed)))
        .WithMembers(
            SyntaxFactory.SingletonList<MemberDeclarationSyntax>(
                SyntaxFactory.MethodDeclaration(
                    SyntaxFactory.PredefinedType(
                        SyntaxFactory.Token(
                            SyntaxFactory.TriviaList(
                                SyntaxFactory.Whitespace("    ")),
                            SyntaxKind.VoidKeyword,
                            SyntaxFactory.TriviaList(
                                SyntaxFactory.Space))),
                    SyntaxFactory.Identifier("Do"))
                .WithParameterList(
                    SyntaxFactory.ParameterList()
                    .WithCloseParenToken(
                        SyntaxFactory.Token(
                            SyntaxFactory.TriviaList(),
                            SyntaxKind.CloseParenToken,
                            SyntaxFactory.TriviaList(
                                SyntaxFactory.LineFeed))))
                .WithBody(
                    SyntaxFactory.Block(
                        SyntaxFactory.SingletonList<StatementSyntax>(
                            SyntaxFactory.LocalDeclarationStatement(
                                SyntaxFactory.VariableDeclaration(
                                    SyntaxFactory.IdentifierName(
                                        SyntaxFactory.Identifier(
                                            SyntaxFactory.TriviaList(
                                                SyntaxFactory.Whitespace("        ")),
                                            "var",
                                            SyntaxFactory.TriviaList(
                                                SyntaxFactory.Space))))
                                .WithVariables(
                                    SyntaxFactory.SingletonSeparatedList<VariableDeclaratorSyntax>(
                                        SyntaxFactory.VariableDeclarator(
                                            SyntaxFactory.Identifier(
                                                SyntaxFactory.TriviaList(),
                                                "x",
                                                SyntaxFactory.TriviaList(
                                                    SyntaxFactory.Space)))
                                        .WithInitializer(
                                            SyntaxFactory.EqualsValueClause(
                                                SyntaxFactory.LiteralExpression(
                                                    SyntaxKind.NumericLiteralExpression,
                                                    SyntaxFactory.Literal(2)))
                                            .WithEqualsToken(
                                                SyntaxFactory.Token(
                                                    SyntaxFactory.TriviaList(),
                                                    SyntaxKind.EqualsToken,
                                                    SyntaxFactory.TriviaList(
                                                        SyntaxFactory.Space)))))))
                            .WithSemicolonToken(
                                SyntaxFactory.Token(
                                    SyntaxFactory.TriviaList(),
                                    SyntaxKind.SemicolonToken,
                                    SyntaxFactory.TriviaList(
                                        SyntaxFactory.LineFeed)))))
                    .WithOpenBraceToken(
                        SyntaxFactory.Token(
                            SyntaxFactory.TriviaList(
                                SyntaxFactory.Whitespace("    ")),
                            SyntaxKind.OpenBraceToken,
                            SyntaxFactory.TriviaList(
                                SyntaxFactory.LineFeed)))
                    .WithCloseBraceToken(
                        SyntaxFactory.Token(
                            SyntaxFactory.TriviaList(
                                SyntaxFactory.Whitespace("    ")),
                            SyntaxKind.CloseBraceToken,
                            SyntaxFactory.TriviaList(
                                SyntaxFactory.LineFeed))))));

            var dGeneratedClass = DSyntaxFactory.ClassDeclaration(
           DSyntaxFactory.Identifier(
               DSyntaxFactory.TriviaList(),
               "Test",
               DSyntaxFactory.TriviaList(
                   DSyntaxFactory.LineFeed)))
       .WithKeyword(
           DSyntaxFactory.Token(
               DSyntaxFactory.TriviaList(),
               DSyntaxKind.ClassKeyword,
               DSyntaxFactory.TriviaList(
                   DSyntaxFactory.Space)))
       .WithOpenBraceToken(
           DSyntaxFactory.Token(
               DSyntaxFactory.TriviaList(),
               DSyntaxKind.OpenBraceToken,
               DSyntaxFactory.TriviaList(
                   DSyntaxFactory.LineFeed)))
       .WithMembers(
           DSyntaxFactory.SingletonList<DMemberDeclarationSyntax>(
               DSyntaxFactory.MethodDeclaration(
                   DSyntaxFactory.PredefinedType(
                       DSyntaxFactory.Token(
                           DSyntaxFactory.TriviaList(
                               DSyntaxFactory.Whitespace("    ")),
                           DSyntaxKind.VoidKeyword,
                           DSyntaxFactory.TriviaList(
                               DSyntaxFactory.Space))),
                   DSyntaxFactory.Identifier("Do"))
               .WithParameterList(
                   DSyntaxFactory.ParameterList()
                   .WithCloseParenToken(
                       DSyntaxFactory.Token(
                           DSyntaxFactory.TriviaList(),
                           DSyntaxKind.CloseParenToken,
                           DSyntaxFactory.TriviaList(
                               DSyntaxFactory.LineFeed))))
               .WithBody(
                   DSyntaxFactory.Block(
                       DSyntaxFactory.SingletonList<DStatementSyntax>(
                           DSyntaxFactory.LocalDeclarationStatement(
                               DSyntaxFactory.VariableDeclaration(
                                   DSyntaxFactory.IdentifierName(
                                       DSyntaxFactory.Identifier(
                                           DSyntaxFactory.TriviaList(
                                               DSyntaxFactory.Whitespace("        ")),
                                           "var",
                                           DSyntaxFactory.TriviaList(
                                               DSyntaxFactory.Space))))
                               .WithVariables(
                                   DSyntaxFactory.SingletonSeparatedList<DVariableDeclaratorSyntax>(
                                       DSyntaxFactory.VariableDeclarator(
                                           DSyntaxFactory.Identifier(
                                               DSyntaxFactory.TriviaList(),
                                               "x",
                                               DSyntaxFactory.TriviaList(
                                                   DSyntaxFactory.Space)))
                                       .WithInitializer(
                                           DSyntaxFactory.EqualsValueClause(
                                               DSyntaxFactory.LiteralExpression(
                                                   DSyntaxKind.NumericLiteralExpression,
                                                   DSyntaxFactory.Literal(2)))
                                           .WithEqualsToken(
                                               DSyntaxFactory.Token(
                                                   DSyntaxFactory.TriviaList(),
                                                   DSyntaxKind.EqualsToken,
                                                   DSyntaxFactory.TriviaList(
                                                       DSyntaxFactory.Space)))))))
                           .WithSemicolonToken(
                               DSyntaxFactory.Token(
                                   DSyntaxFactory.TriviaList(),
                                   DSyntaxKind.SemicolonToken,
                                   DSyntaxFactory.TriviaList(
                                       DSyntaxFactory.LineFeed)))))
                   .WithOpenBraceToken(
                       DSyntaxFactory.Token(
                           DSyntaxFactory.TriviaList(
                               DSyntaxFactory.Whitespace("    ")),
                           DSyntaxKind.OpenBraceToken,
                           DSyntaxFactory.TriviaList(
                               DSyntaxFactory.LineFeed)))
                   .WithCloseBraceToken(
                       DSyntaxFactory.Token(
                           DSyntaxFactory.TriviaList(
                               DSyntaxFactory.Whitespace("    ")),
                           DSyntaxKind.CloseBraceToken,
                           DSyntaxFactory.TriviaList(
                               DSyntaxFactory.LineFeed))))));

            var dDescendants = dGeneratedClass.DescendantNodesAndTokens().ToList();
            var dgeneratedClassString = dGeneratedClass.ToString();

            Assert.Equal(desiredSource, dgeneratedClassString);
        }
    }

    class Test
    {
        void Do()
        {
            Console.WriteLine("Hi");
        }
    }
}