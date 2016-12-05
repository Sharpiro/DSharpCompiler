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
@"var x = 2;
var y = 3;
".Replace(Environment.NewLine, "\n");

            var script = CSharpScript.Create(desiredSource);
            var cRoot = script.GetCompilation().SyntaxTrees.Single().GetRoot();

            //dsharp
            var dRoot =
                DSyntaxFactory.CompilationUnit()
                    .WithMembers(
                        DSyntaxFactory.List<DMemberDeclarationSyntax>(
                            new DMemberDeclarationSyntax[]{
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
                                                    "y",
                                                    DSyntaxFactory.TriviaList(
                                                        DSyntaxFactory.Space)))
                                            .WithInitializer(
                                                DSyntaxFactory.EqualsValueClause(
                                                    DSyntaxFactory.LiteralExpression(
                                                        DSyntaxKind.NumericLiteralExpression,
                                                        DSyntaxFactory.Literal(3)))
                                                .WithEqualsToken(
                                                    DSyntaxFactory.Token(
                                                        DSyntaxFactory.TriviaList(),
                                                        DSyntaxKind.EqualsToken,
                                                        DSyntaxFactory.TriviaList(
                                                            DSyntaxFactory.Space)))))))}));

            var cDescendants = cRoot.DescendantNodesAndTokens().ToList();
            var dDescendants = dRoot.DescendantNodesAndTokens().ToList();
            var x = cDescendants.LastOrDefault().ToFullString();
            for (var i = 0; i < cDescendants.Count; i++)
            {
                Assert.True(cDescendants[i].RawKind == (int)dDescendants[i].SyntaxKind);
            }
        }
    }
}