using DSharpCodeAnalysis.Models;
using DSharpCodeAnalysis.Parser;
using DSharpCodeAnalysis.Syntax;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Newtonsoft.Json;
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
        public void MultiLineClassTriviaTest()
        {
            const string desiredSource =
@"class Test
{

}";

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
        .WithCloseBraceToken(
            SyntaxFactory.Token(
                SyntaxFactory.TriviaList(
                    SyntaxFactory.LineFeed),
                SyntaxKind.CloseBraceToken,
                SyntaxFactory.TriviaList()));

            //var dGeneratedClass = DSyntaxFactory.ClassDeclaration("Test").WithKeyword(
            //    DSyntaxFactory.Token(DSyntaxFactory.TriviaList(), DSyntaxKind.ClassKeyword,
            //    DSyntaxFactory.TriviaList(DSyntaxFactory.Space)));


            var cDescendants = cGeneratedClass.DescendantNodesAndTokens().ToList();
            //var dDescendants = dGeneratedClass.DescendantNodesAndTokens().ToList();


            var cgeneratedClassString = cGeneratedClass.ToString();
            //var dgeneratedClassString = dGeneratedClass.ToString();

            Assert.Equal(desiredSource, cgeneratedClassString);
            //Assert.Equal(desiredSource, dgeneratedClassString);
        }
    }

    public class Test
    {

    }
}