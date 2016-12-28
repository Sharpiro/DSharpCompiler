using DSharpCodeAnalysis.Parser;
using Microsoft.CodeAnalysis.CSharp;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace DSharpCodeAnalysisTests
{
    public class LexerTests
    {
        [Fact]
        public void SimpleClassLexTest()
        {
            var source = 
@"type Test
{

}".Replace(Environment.NewLine, "\n");
            var lexer = new DLexer(source);
            lexer.Lex();
            
        }

        [Fact]
        public void SimpleMethodLexTest()
        {
            var source ="int Add(int x, int y)";
            var lexer = new DLexer(source);
            lexer.Lex();

        }

        [Fact]
        public void SimpleProgramLexTest()
        {
            //SyntaxFactory.Literal
            var source =
@"func int Add(int x, int y)
{
    return x + y;
}
let result = Add(2, 3);".Replace(Environment.NewLine, "\n");
            var lexer = new DLexer(source);
            lexer.Lex();
        }
    }
}