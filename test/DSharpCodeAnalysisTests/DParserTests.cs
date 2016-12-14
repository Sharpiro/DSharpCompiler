using DSharpCodeAnalysis.Parser;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Newtonsoft.Json;
using System;
using System.Linq;
using Xunit;

namespace DSharpCodeAnalysisTests
{
    public class DParserTests
    {
        [Fact]
        public void OneLineClassParseTest()
        {
            var source = "class Program { }";
            var lexer = new DLexer(source);
            var lexedTokens = lexer.Lex();
            var parser = new DParser(lexedTokens);
            var cCompilationUnit = CSharpScript.Create(source).GetCompilation().SyntaxTrees.Single().GetCompilationUnitRoot();
            var dCompilationUnit = parser.ParseCompilationUnit();

            var cDescendantNodesAndTokens = cCompilationUnit.DescendantNodesAndTokens().ToList();
            var dDescendantNodesAndTokens = dCompilationUnit.DescendantNodesAndTokens().ToList();
            var childNodes = dCompilationUnit.ChildNodes().ToList();
            var childTokens = dCompilationUnit.ChildTokens().ToList();
            var hierarchy = dCompilationUnit.DescendantHierarchy();
            var hierarchyJson = JsonConvert.SerializeObject(hierarchy);
            var cString = cCompilationUnit.ToString();
            var dString = dCompilationUnit.ToString();

            for (var i = 0; i < cDescendantNodesAndTokens.Count; i++)
            {
                var cToken = cDescendantNodesAndTokens[i];
                var dToken = dDescendantNodesAndTokens[i];
                Assert.Equal(cToken.Span.Start, dToken.Span.Start);
                Assert.Equal(cToken.Span.End, dToken.Span.End);
                Assert.Equal(cToken.Span.Length, dToken.Span.Length);

                Assert.Equal(cToken.FullSpan.Start, dToken.FullSpan.Start);
                Assert.Equal(cToken.FullSpan.End, dToken.FullSpan.End);
                Assert.Equal(cToken.FullSpan.Length, dToken.FullSpan.Length);
            }

            Assert.Equal(cCompilationUnit.Span.Start, dCompilationUnit.FullSpan.Start);
            Assert.Equal(cCompilationUnit.Span.End, dCompilationUnit.FullSpan.End);
            Assert.Equal(cCompilationUnit.Span.Length, dCompilationUnit.FullSpan.Length);

            Assert.Equal(source, cString);
            Assert.Equal(source, dString);

        }

        [Fact]
        public void MultiLineClassParseTest()
        {
            var source =
@"class Program
{

}".Replace(Environment.NewLine, "\n");
            var lexer = new DLexer(source);
            var lexedTokens = lexer.Lex();
            var parser = new DParser(lexedTokens);
            var cCompilationUnit = CSharpScript.Create(source).GetCompilation().SyntaxTrees.Single().GetCompilationUnitRoot();
            var dCompilationUnit = parser.ParseCompilationUnit();

            var cDescendantNodesAndTokens = cCompilationUnit.DescendantNodesAndTokens().ToList();
            var dDescendantNodesAndTokens = dCompilationUnit.DescendantNodesAndTokens().ToList();
            var childNodes = dCompilationUnit.ChildNodes().ToList();
            var childTokens = dCompilationUnit.ChildTokens().ToList();
            var hierarchy = dCompilationUnit.DescendantHierarchy();
            var hierarchyJson = JsonConvert.SerializeObject(hierarchy);
            var cString = cCompilationUnit.ToString();
            var dString = dCompilationUnit.ToString();

            for (var i = 0; i < cDescendantNodesAndTokens.Count; i++)
            {
                var cToken = cDescendantNodesAndTokens[i];
                var cTokenTrivia = cToken.GetLeadingTrivia().Concat(cToken.GetTrailingTrivia()).ToList();
                var dToken = dDescendantNodesAndTokens[i];
                Assert.Equal(cToken.Span.Start, dToken.Span.Start);
                Assert.Equal(cToken.Span.End, dToken.Span.End);
                Assert.Equal(cToken.Span.Length, dToken.Span.Length);

                Assert.Equal(cToken.FullSpan.Start, dToken.FullSpan.Start);
                Assert.Equal(cToken.FullSpan.End, dToken.FullSpan.End);
                Assert.Equal(cToken.FullSpan.Length, dToken.FullSpan.Length);
            }

            Assert.Equal(cCompilationUnit.Span.Start, dCompilationUnit.FullSpan.Start);
            Assert.Equal(cCompilationUnit.Span.End, dCompilationUnit.FullSpan.End);
            Assert.Equal(cCompilationUnit.Span.Length, dCompilationUnit.FullSpan.Length);

            Assert.Equal(source, cString);
            Assert.Equal(source, dString);
        }

        [Fact]
        public void OneLineMethodTestParseTest()
        {
            var source = "int Add(int x, int y)";
            var lexer = new DLexer(source);
            var lexedTokens = lexer.Lex();
            var parser = new DParser(lexedTokens);
            var cCompilationUnit = CSharpScript.Create(source).GetCompilation().SyntaxTrees.Single().GetCompilationUnitRoot();
            var dCompilationUnit = parser.ParseCompilationUnit();

            var cDescendantNodesAndTokens = cCompilationUnit.DescendantNodesAndTokens().ToList();
            var cChildNodes= cCompilationUnit.ChildNodes().ToList();
            var dDescendantNodesAndTokens = dCompilationUnit.DescendantNodesAndTokens().ToList();
            var childNodes = dCompilationUnit.ChildNodes().ToList();
            var childTokens = dCompilationUnit.ChildTokens().ToList();
            var hierarchy = dCompilationUnit.DescendantHierarchy();
            var hierarchyJson = JsonConvert.SerializeObject(hierarchy);
            var cString = cCompilationUnit.ToString();
            var dString = dCompilationUnit.ToString();

            for (var i = 0; i < cDescendantNodesAndTokens.Count; i++)
            {
                var cToken = cDescendantNodesAndTokens[i];
                var cTokenTrivia = cToken.GetLeadingTrivia().Concat(cToken.GetTrailingTrivia()).ToList();
                var dToken = dDescendantNodesAndTokens[i];
                Assert.Equal(cToken.Span.Start, dToken.Span.Start);
                Assert.Equal(cToken.Span.End, dToken.Span.End);
                Assert.Equal(cToken.Span.Length, dToken.Span.Length);

                Assert.Equal(cToken.FullSpan.Start, dToken.FullSpan.Start);
                Assert.Equal(cToken.FullSpan.End, dToken.FullSpan.End);
                Assert.Equal(cToken.FullSpan.Length, dToken.FullSpan.Length);
            }

            Assert.Equal(cCompilationUnit.Span.Start, dCompilationUnit.FullSpan.Start);
            Assert.Equal(cCompilationUnit.Span.End, dCompilationUnit.FullSpan.End);
            Assert.Equal(cCompilationUnit.Span.Length, dCompilationUnit.FullSpan.Length);

            Assert.Equal(source, cString);
            Assert.Equal(source, dString);
        }
    }
}