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
var x = Add(1, 2);
".Replace(Environment.NewLine, "\n");

            var cClass = CSharpSyntaxTree.ParseText(desiredSource).GetRoot();
            
            var x = cClass.DescendantNodesAndTokens();
            var y = 2;
            //var cClass =
            //            SyntaxFactory.MethodDeclaration(
            //    SyntaxFactory.PredefinedType(
            //        SyntaxFactory.Token(
            //            SyntaxFactory.TriviaList(),
            //            SyntaxKind.IntKeyword,
            //            SyntaxFactory.TriviaList(
            //                SyntaxFactory.Space))),
            //    SyntaxFactory.Identifier("Add"))
            //.WithParameterList(
            //    SyntaxFactory.ParameterList(
            //        SyntaxFactory.SeparatedList<ParameterSyntax>(
            //            new SyntaxNodeOrToken[]{
            //                SyntaxFactory.Parameter(
            //                    SyntaxFactory.Identifier("x"))
            //                .WithType(
            //                    SyntaxFactory.PredefinedType(
            //                        SyntaxFactory.Token(
            //                            SyntaxFactory.TriviaList(),
            //                            SyntaxKind.IntKeyword,
            //                            SyntaxFactory.TriviaList(
            //                                SyntaxFactory.Space)))),
            //                SyntaxFactory.Token(
            //                    SyntaxFactory.TriviaList(),
            //                    SyntaxKind.CommaToken,
            //                    SyntaxFactory.TriviaList(
            //                        SyntaxFactory.Space)),
            //                SyntaxFactory.Parameter(
            //                    SyntaxFactory.Identifier("y"))
            //                .WithType(
            //                    SyntaxFactory.PredefinedType(
            //                        SyntaxFactory.Token(
            //                            SyntaxFactory.TriviaList(),
            //                            SyntaxKind.IntKeyword,
            //                            SyntaxFactory.TriviaList(
            //                                SyntaxFactory.Space))))}))
            //    .WithCloseParenToken(
            //        SyntaxFactory.Token(
            //            SyntaxFactory.TriviaList(),
            //            SyntaxKind.CloseParenToken,
            //            SyntaxFactory.TriviaList(
            //                SyntaxFactory.LineFeed))))
            //.WithBody(
            //    SyntaxFactory.Block(
            //        SyntaxFactory.SingletonList<StatementSyntax>(
            //            SyntaxFactory.ReturnStatement(
            //                SyntaxFactory.BinaryExpression(
            //                    SyntaxKind.AddExpression,
            //                    SyntaxFactory.IdentifierName(
            //                        SyntaxFactory.Identifier(
            //                            SyntaxFactory.TriviaList(),
            //                            "x",
            //                            SyntaxFactory.TriviaList(
            //                                SyntaxFactory.Space))),
            //                    SyntaxFactory.IdentifierName("y"))
            //                .WithOperatorToken(
            //                    SyntaxFactory.Token(
            //                        SyntaxFactory.TriviaList(),
            //                        SyntaxKind.PlusToken,
            //                        SyntaxFactory.TriviaList(
            //                            SyntaxFactory.Space))))
            //            .WithReturnKeyword(
            //                SyntaxFactory.Token(
            //                    SyntaxFactory.TriviaList(
            //                        SyntaxFactory.Whitespace("    ")),
            //                    SyntaxKind.ReturnKeyword,
            //                    SyntaxFactory.TriviaList(
            //                        SyntaxFactory.Space)))
            //            .WithSemicolonToken(
            //                SyntaxFactory.Token(
            //                    SyntaxFactory.TriviaList(),
            //                    SyntaxKind.SemicolonToken,
            //                    SyntaxFactory.TriviaList(
            //                        SyntaxFactory.LineFeed)))))
            //    .WithOpenBraceToken(
            //        SyntaxFactory.Token(
            //            SyntaxFactory.TriviaList(),
            //            SyntaxKind.OpenBraceToken,
            //            SyntaxFactory.TriviaList(
            //                SyntaxFactory.LineFeed)))
            //    .WithCloseBraceToken(
            //        SyntaxFactory.Token(
            //            SyntaxFactory.TriviaList(),
            //            SyntaxKind.CloseBraceToken,
            //            SyntaxFactory.TriviaList(
            //                SyntaxFactory.LineFeed)))),
            //SyntaxFactory.FieldDeclaration(
            //    SyntaxFactory.VariableDeclaration(
            //        SyntaxFactory.IdentifierName(
            //            SyntaxFactory.Identifier(
            //                SyntaxFactory.TriviaList(),
            //                "var",
            //                SyntaxFactory.TriviaList(
            //                    SyntaxFactory.Space))))
            //    .WithVariables(
            //        SyntaxFactory.SingletonSeparatedList<VariableDeclaratorSyntax>(
            //            SyntaxFactory.VariableDeclarator(
            //                SyntaxFactory.Identifier(
            //                    SyntaxFactory.TriviaList(),
            //                    "x",
            //                    SyntaxFactory.TriviaList(
            //                        SyntaxFactory.Space)))
            //            .WithInitializer(
            //                SyntaxFactory.EqualsValueClause(
            //                    SyntaxFactory.InvocationExpression(
            //                        SyntaxFactory.IdentifierName("Add"))
            //                    .WithArgumentList(
            //                        SyntaxFactory.ArgumentList(
            //                            SyntaxFactory.SeparatedList<ArgumentSyntax>(
            //                                new SyntaxNodeOrToken[]{
            //                                    SyntaxFactory.Argument(
            //                                        SyntaxFactory.LiteralExpression(
            //                                            SyntaxKind.NumericLiteralExpression,
            //                                            SyntaxFactory.Literal(1))),
            //                                    SyntaxFactory.Token(
            //                                        SyntaxFactory.TriviaList(),
            //                                        SyntaxKind.CommaToken,
            //                                        SyntaxFactory.TriviaList(
            //                                            SyntaxFactory.Space)),
            //                                    SyntaxFactory.Argument(
            //                                        SyntaxFactory.LiteralExpression(
            //                                            SyntaxKind.NumericLiteralExpression,
            //                                            SyntaxFactory.Literal(2)))}))))
            //                .WithEqualsToken(
            //                    SyntaxFactory.Token(
            //                        SyntaxFactory.TriviaList(),
            //                        SyntaxKind.EqualsToken,
            //                        SyntaxFactory.TriviaList(
            //                            SyntaxFactory.Space)))))));

            //dsharp
            var dClass =
                        DSyntaxFactory.MethodDeclaration(
                            DSyntaxFactory.PredefinedType(
                                DSyntaxFactory.Token(
                                    DSyntaxFactory.TriviaList(),
                                    DSyntaxKind.VoidKeyword,
                                    DSyntaxFactory.TriviaList(
                                        DSyntaxFactory.Space))),
                            DSyntaxFactory.Identifier("Main"))
                        .WithModifiers(
                            DSyntaxFactory.TokenList(
                                DSyntaxFactory.Token(
                                    DSyntaxFactory.TriviaList(),
                                    DSyntaxKind.StaticKeyword,
                                    DSyntaxFactory.TriviaList(
                                        DSyntaxFactory.Space))))
                        .WithParameterList(
                            DSyntaxFactory.ParameterList()
                            .WithCloseParenToken(
                                DSyntaxFactory.Token(
                                    DSyntaxFactory.TriviaList(),
                                    DSyntaxKind.CloseParenToken,
                                    DSyntaxFactory.TriviaList(
                                        DSyntaxFactory.LineFeed))))
                        .WithBody(
                            DSyntaxFactory.Block()
                            .WithOpenBraceToken(
                                DSyntaxFactory.Token(
                                    DSyntaxFactory.TriviaList(),
                                    DSyntaxKind.OpenBraceToken,
                                    DSyntaxFactory.TriviaList(
                                        DSyntaxFactory.LineFeed)))
                            .WithCloseBraceToken(
                                DSyntaxFactory.Token(
                                    DSyntaxFactory.TriviaList(
                                        new[]{
                                            DSyntaxFactory.Tab,
                                            DSyntaxFactory.LineFeed}),
                                    DSyntaxKind.CloseBraceToken,
                                    DSyntaxFactory.TriviaList())));

            var cDesc = cClass.DescendantNodesAndTokens().ToList();
            var dDesc = dClass.DescendantNodesAndTokens().ToList();

            Assert.Equal(cDesc.Count, dDesc.Count);
            for (var i = 0; i < cDesc.Count; i++)
            {
                Assert.Equal(cDesc[i].RawKind, (int)dDesc[i].SyntaxKind);
            }
        }
    }
}