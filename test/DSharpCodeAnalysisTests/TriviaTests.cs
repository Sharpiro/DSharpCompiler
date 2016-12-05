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
            var desiredSource =
@"class Test
{
    void Do()
    {
        var x = 2;
        System.Console.WriteLine(x);
    }
}".Replace(Environment.NewLine, "\n");

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
                                       DSyntaxFactory.LineFeed))),
                           DSyntaxFactory.ExpressionStatement(
                            DSyntaxFactory.InvocationExpression(
                                DSyntaxFactory.MemberAccessExpression(
                                    DSyntaxKind.SimpleMemberAccessExpression,
                                    DSyntaxFactory.MemberAccessExpression(
                                        DSyntaxKind.SimpleMemberAccessExpression,
                                        DSyntaxFactory.IdentifierName(
                                            DSyntaxFactory.Identifier(
                                                DSyntaxFactory.TriviaList(
                                                    DSyntaxFactory.Whitespace("        ")),
                                                "System",
                                                DSyntaxFactory.TriviaList())),
                                        DSyntaxFactory.IdentifierName("Console")),
                                    DSyntaxFactory.IdentifierName("WriteLine")))
                            .WithArgumentList(
                                DSyntaxFactory.ArgumentList(
                                    DSyntaxFactory.SingletonSeparatedList<DArgumentSyntax>(
                                        DSyntaxFactory.Argument(
                                            DSyntaxFactory.IdentifierName("x"))))))
                        .WithSemicolonToken(
                            DSyntaxFactory.Token(
                                DSyntaxFactory.TriviaList(),
                                DSyntaxKind.SemicolonToken,
                                DSyntaxFactory.TriviaList(
                                    DSyntaxFactory.LineFeed))))
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
            var dTypes = dDescendants.Select(d => d.GetType());
            var dGeneratedClassString = dGeneratedClass.ToString();
            var model = dGeneratedClass.DescendantHierarchy();
            var json = JsonConvert.SerializeObject(model);

            Assert.Equal(desiredSource, dGeneratedClassString);
        }

        [Fact]
        public void ReturnMethodTest()
        {
            var desiredSource =
@"void Add(int x, int y)
{
    return x + y;
}".Replace(Environment.NewLine, "\n");

            var dGeneratedClass =
                        DSyntaxFactory.MethodDeclaration(
                            DSyntaxFactory.PredefinedType(
                                DSyntaxFactory.Token(
                                    DSyntaxFactory.TriviaList(),
                                    DSyntaxKind.VoidKeyword,
                                    DSyntaxFactory.TriviaList(
                                        DSyntaxFactory.Space))),
                            DSyntaxFactory.Identifier("Add"))
                        .WithParameterList(
                            DSyntaxFactory.ParameterList(
                                DSyntaxFactory.SeparatedList<DParameterSyntax>(
                                    new IDSyntax[]{
                                        DSyntaxFactory.Parameter(
                                            DSyntaxFactory.Identifier("x"))
                                        .WithType(
                                            DSyntaxFactory.PredefinedType(
                                                DSyntaxFactory.Token(
                                                    DSyntaxFactory.TriviaList(),
                                                    DSyntaxKind.IntKeyword,
                                                    DSyntaxFactory.TriviaList(
                                                        DSyntaxFactory.Space)))),
                                        DSyntaxFactory.Token(
                                            DSyntaxFactory.TriviaList(),
                                            DSyntaxKind.CommaToken,
                                            DSyntaxFactory.TriviaList(
                                                DSyntaxFactory.Space)),
                                        DSyntaxFactory.Parameter(
                                            DSyntaxFactory.Identifier("y"))
                                        .WithType(
                                            DSyntaxFactory.PredefinedType(
                                                DSyntaxFactory.Token(
                                                    DSyntaxFactory.TriviaList(),
                                                    DSyntaxKind.IntKeyword,
                                                    DSyntaxFactory.TriviaList(
                                                        DSyntaxFactory.Space))))}))
                            .WithCloseParenToken(
                                DSyntaxFactory.Token(
                                    DSyntaxFactory.TriviaList(),
                                    DSyntaxKind.CloseParenToken,
                                    DSyntaxFactory.TriviaList(
                                        DSyntaxFactory.LineFeed))))
                        .WithBody(
                            DSyntaxFactory.Block(
                                DSyntaxFactory.SingletonList<DStatementSyntax>(
                                    DSyntaxFactory.ReturnStatement(
                                        DSyntaxFactory.BinaryExpression(
                                            DSyntaxKind.AddExpression,
                                            DSyntaxFactory.IdentifierName(
                                                DSyntaxFactory.Identifier(
                                                    DSyntaxFactory.TriviaList(),
                                                    "x",
                                                    DSyntaxFactory.TriviaList(
                                                        DSyntaxFactory.Space))),
                                            DSyntaxFactory.IdentifierName("y"))
                                        .WithOperatorToken(
                                            DSyntaxFactory.Token(
                                                DSyntaxFactory.TriviaList(),
                                                DSyntaxKind.PlusToken,
                                                DSyntaxFactory.TriviaList(
                                                    DSyntaxFactory.Space))))
                                    .WithReturnKeyword(
                                        DSyntaxFactory.Token(
                                            DSyntaxFactory.TriviaList(
                                                DSyntaxFactory.Whitespace("    ")),
                                            DSyntaxKind.ReturnKeyword,
                                            DSyntaxFactory.TriviaList(
                                                DSyntaxFactory.Space)))
                                    .WithSemicolonToken(
                                        DSyntaxFactory.Token(
                                            DSyntaxFactory.TriviaList(),
                                            DSyntaxKind.SemicolonToken,
                                            DSyntaxFactory.TriviaList(
                                                DSyntaxFactory.LineFeed)))))
                            .WithOpenBraceToken(
                                DSyntaxFactory.Token(
                                    DSyntaxFactory.TriviaList(),
                                    DSyntaxKind.OpenBraceToken,
                                    DSyntaxFactory.TriviaList(
                                        DSyntaxFactory.LineFeed))));

            var dDescendants = dGeneratedClass.DescendantNodesAndTokens().ToList();
            var dGeneratedClassString = dGeneratedClass.ToString();

            Assert.Equal(desiredSource, dGeneratedClassString);
        }
    }
}