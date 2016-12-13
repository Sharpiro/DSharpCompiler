using DSharpCodeAnalysis.Parser;
using Microsoft.CodeAnalysis.CSharp;
using System;
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
            parser.ParseCompilationUnit();
        }
    }
}