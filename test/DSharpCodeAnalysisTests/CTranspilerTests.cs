using DSharpCodeAnalysis.Parser;
using DSharpCodeAnalysis.Transpiler;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace DSharpCodeAnalysisTests
{
    public class CTranspilerTests
    {
        [Fact]
        public void TranspileLetTest()
        {
            const string source = "let x = 2;";
            const string transpiledSource = "var x = 2;";
            var lexer = new DLexer(source);
            var lexedTokens = lexer.Lex();
            var parser = new DParser(lexedTokens);
            var compilation = parser.ParseCompilationUnit();
            var transpiler = new CTranspiler(compilation);
            var transCompilation = transpiler.Transpile();

            var dString = compilation.ToString();
            var transpiledString = transCompilation.ToString();

            Assert.Equal(source, dString);
            Assert.Equal(transpiledSource, transpiledString);
        }
    }
}