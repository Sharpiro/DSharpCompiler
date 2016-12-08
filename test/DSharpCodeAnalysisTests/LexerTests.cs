using DSharpCodeAnalysis.Parser;
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
        public void SimpleLexTest()
        {
            var source = "class Test{}";
            var lexer = new DLexer(source);
            lexer.Lex();
        }
    }
}
