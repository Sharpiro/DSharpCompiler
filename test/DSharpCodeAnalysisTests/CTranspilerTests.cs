using DSharpCodeAnalysis.Parser;
using DSharpCodeAnalysis.Transpiler;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
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

            var transpiledString = transCompilation.ToString();
            var dString = compilation.ToString();


            Assert.Equal(source, dString);
            Assert.Equal(transpiledSource, transpiledString);
        }

        [Fact]
        public void TranspileFuncTest()
        {
            var source = 
@"func int add(int x, int y)
{
    return x + y;
}".Replace(Environment.NewLine, "\n");
            var transpiledSource =
@"int add(int x, int y)
{
    return x + y;
}".Replace(Environment.NewLine, "\n");
            var lexer = new DLexer(source);
            var lexedTokens = lexer.Lex();
            var parser = new DParser(lexedTokens);
            var compilation = parser.ParseCompilationUnit();
            var transpiler = new CTranspiler(compilation);
            var transCompilation = transpiler.Transpile();

            var transpiledString = transCompilation.ToString();
            var dString = compilation.ToString();

            Assert.Equal(source, dString);
            Assert.Equal(transpiledSource, transpiledString);
        }

        [Fact]
        public void TranspileSimpleProgramTest()
        {
            var source =
@"func int Add(int x, int y)
{
    let temp = 2;
    return x + y;
}
let result = Add(2, 3);
let temp = 3;".Replace(Environment.NewLine, "\n");
            var transpiledSource =
@"int Add(int x, int y)
{
    var temp = 2;
    return x + y;
}
var result = Add(2, 3);
var temp = 3;".Replace(Environment.NewLine, "\n");
            var lexer = new DLexer(source);
            var lexedTokens = lexer.Lex();
            var parser = new DParser(lexedTokens);
            var compilation = parser.ParseCompilationUnit();
            var xString = compilation.ToString();
            var transpiler = new CTranspiler(compilation);
            var transCompilation = transpiler.Transpile();

            var transpiledString = transCompilation.ToString();
            var dString = compilation.ToString();

            Assert.Equal(source, dString);
            Assert.Equal(transpiledSource, transpiledString);
        }

        [Fact]
        public void ClassTest()
        {
            var source =
@"type Adder
{
    func int Add(int x, int y)
    {
        let temp = 2;
        return x + y;
    }
}
let test = new System.Exception();
let adder = new Adder();
let result = adder.Add(2, 3);".Replace(Environment.NewLine, "\n");
            var lexer = new DLexer(source);
            var lexedTokens = lexer.Lex();
            var parser = new DParser(lexedTokens);
            var compilation = parser.ParseCompilationUnit();
            var xString = compilation.ToString();
            var transpiler = new CTranspiler(compilation);
            var transCompilation = transpiler.Transpile();

            var transpiledString = transCompilation.ToString();
            var dString = compilation.ToString();
            Assert.Equal(source, dString);
        }
    }
}