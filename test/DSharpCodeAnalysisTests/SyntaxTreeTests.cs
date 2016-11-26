using DSharpCodeAnalysis.Parser;
using Microsoft.CodeAnalysis.CSharp;
using Xunit;

namespace DSharpCodeAnalysisTests
{
    public class SyntaxTreeTests
    {
        [Fact]
        public void TestOne()
        {
            var x = SyntaxFactory.ClassDeclaration("");
            var lexer = new Lexer();
            var source = "using System;";
            lexer.Lex(source);
            SyntaxFactory.TokenList();
        }
    }
}