using DSharpCodeAnalysis.Models;
using DSharpCodeAnalysis.Parser;
using DSharpCodeAnalysis.Syntax;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using Xunit;

namespace DSharpCodeAnalysisTests
{
    public class SimpleProgramTests
    {
        [Fact]
        public void SimpleProgramTest()
        {
            var desiredSource =
@"int Add(int x, int y)
{
    return x + y;
}
var result = Add(2, 3);
System.Console.WriteLine(result);".Replace(Environment.NewLine, "\n");

            var script = CSharpScript.Create(desiredSource);
            var csRoot = script.GetCompilation().SyntaxTrees.Single().GetRoot();

            var dRoot =
                DSyntaxFactory.CompilationUnit()
.WithMembers(
    DSyntaxFactory.List<DMemberDeclarationSyntax>(
        new DMemberDeclarationSyntax[]{
                        DSyntaxFactory.MethodDeclaration(
                            DSyntaxFactory.PredefinedType(
                                DSyntaxFactory.Token(
                                    DSyntaxFactory.TriviaList(),
                                    DSyntaxKind.IntKeyword,
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
                                        DSyntaxFactory.LineFeed)))
                            .WithCloseBraceToken(
                                DSyntaxFactory.Token(
                                    DSyntaxFactory.TriviaList(),
                                    DSyntaxKind.CloseBraceToken,
                                    DSyntaxFactory.TriviaList(
                                        DSyntaxFactory.LineFeed)))),
                        DSyntaxFactory.FieldDeclaration(
                            DSyntaxFactory.VariableDeclaration(
                                DSyntaxFactory.IdentifierName(
                                    DSyntaxFactory.Identifier(
                                        DSyntaxFactory.TriviaList(),
                                        "var",
                                        DSyntaxFactory.TriviaList(
                                            DSyntaxFactory.Space))))
                            .WithVariables(
                                DSyntaxFactory.SingletonSeparatedList<DVariableDeclaratorSyntax>(
                                    DSyntaxFactory.VariableDeclarator(
                                        DSyntaxFactory.Identifier(
                                            DSyntaxFactory.TriviaList(),
                                            "result",
                                            DSyntaxFactory.TriviaList(
                                                DSyntaxFactory.Space)))
                                    .WithInitializer(
                                        DSyntaxFactory.EqualsValueClause(
                                            DSyntaxFactory.InvocationExpression(
                                                DSyntaxFactory.IdentifierName("Add"))
                                            .WithArgumentList(
                                                DSyntaxFactory.ArgumentList(
                                                    DSyntaxFactory.SeparatedList<DArgumentSyntax>(
                                                        new IDSyntax[]{
                                                            DSyntaxFactory.Argument(
                                                                DSyntaxFactory.LiteralExpression(
                                                                    DSyntaxKind.NumericLiteralExpression,
                                                                    DSyntaxFactory.Literal(2))),
                                                            DSyntaxFactory.Token(
                                                                DSyntaxFactory.TriviaList(),
                                                                DSyntaxKind.CommaToken,
                                                                DSyntaxFactory.TriviaList(
                                                                    DSyntaxFactory.Space)),
                                                            DSyntaxFactory.Argument(
                                                                DSyntaxFactory.LiteralExpression(
                                                                    DSyntaxKind.NumericLiteralExpression,
                                                                    DSyntaxFactory.Literal(3)))}))))
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
                         DSyntaxFactory.GlobalStatement(
                DSyntaxFactory.ExpressionStatement(
                DSyntaxFactory.InvocationExpression(
                                    DSyntaxFactory.MemberAccessExpression(
                                        DSyntaxKind.SimpleMemberAccessExpression,
                                        DSyntaxFactory.MemberAccessExpression(
                                            DSyntaxKind.SimpleMemberAccessExpression,
                                            DSyntaxFactory.IdentifierName("System"),
                                            DSyntaxFactory.IdentifierName("Console")),
                                        DSyntaxFactory.IdentifierName("WriteLine")))
                                .WithArgumentList(
                                    DSyntaxFactory.ArgumentList(
                                        DSyntaxFactory.SingletonSeparatedList<DArgumentSyntax>(
                                            DSyntaxFactory.Argument(
                                                DSyntaxFactory.IdentifierName("result")))))))}));
            
            var csDescendants = csRoot.DescendantNodesAndTokens().ToList();
            var dDescendants = dRoot.DescendantNodesAndTokens().ToList();

            var csNodes = csRoot.ChildNodes().ToList();
            var dNodes = dRoot.ChildNodes().ToList();

            var csTokens = csRoot.ChildTokens().ToList();
            var dTokens = dRoot.ChildTokens().ToList();

            var csString = csRoot.ToString();
            var dString = dRoot.ToString();

            Assert.Equal(csString, csString);
            Assert.Equal(csString, dString);
            for (var i = 0; i < csDescendants.Count; i++)
            {
                var cDesc = csDescendants[i];
                var dDesc = dDescendants[i];

                Assert.True(cDesc.RawKind == (int)dDesc.SyntaxKind);
                if (i == 0) continue;
                Assert.NotNull(dDesc.Parent);
            }
        }
    }
}