using DSharpCodeAnalysis;
using DSharpCodeAnalysis.Parser;
using DSharpCodeAnalysis.Syntax;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Scripting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
            var cString = cCompilationUnit.ToString();
            var dString = dCompilationUnit.ToString();

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
            var cString = cCompilationUnit.ToString();
            var dString = dCompilationUnit.ToString();

            Assert.Equal(source, cString);
            Assert.Equal(source, dString);
        }

        [Fact]
        public void OneLineMethodTestParseTest()
        {
            var source = "int Add(int x, int y);";
            var lexer = new DLexer(source);
            var lexedTokens = lexer.Lex();
            var parser = new DParser(lexedTokens);

            var cCompilationUnit = CSharpScript.Create(source).GetCompilation().SyntaxTrees.Single().GetCompilationUnitRoot();
            var dCompilationUnit = parser.ParseCompilationUnit();
            var cString = cCompilationUnit.ToString();
            var dString = dCompilationUnit.ToString();

            Assert.Equal(source, cString);
            Assert.Equal(source, dString);
        }

        [Fact]
        public void InvocationExpressionParseTest()
        {
            var source = "System.Console.WriteLine(result);";
            var lexer = new DLexer(source);
            var lexedTokens = lexer.Lex();
            var parser = new DParser(lexedTokens);
            var script = CSharpScript.Create(source);

            var cCompilationUnit = script.GetCompilation().SyntaxTrees.Single().GetCompilationUnitRoot();
            var dCompilationUnit = parser.ParseCompilationUnit();
            var cString = cCompilationUnit.ToString();
            var dString = dCompilationUnit.ToString();

            Assert.Equal(source, cString);
            Assert.Equal(source, dString);
        }

        [Fact]
        public void MethodBlockParseTest()
        {
            var source =
@"func int Add(int x, int y)
{
    var temp = 2;
    return x + y;
}
var result = Add(2, 3);
var temp = 3;".Replace(Environment.NewLine, "\n");
            var lexer = new DLexer(source);
            var lexedTokens = lexer.Lex();
            var parser = new DParser(lexedTokens);
            //var cScript = CSharpScript.Create<int>(source).WithDefaultOptions(); ;
            //var scriptState = cScript.RunAsync().Result;
            //var returnValue = scriptState.ReturnValue;

            //var cCompilationUnit = cScript.GetCompilation().SyntaxTrees.Single().GetCompilationUnitRoot();
            var dCompilationUnit = parser.ParseCompilationUnit();
            //var cString = cCompilationUnit.ToString();
            var dString = dCompilationUnit.ToString();
            //var dScript = CSharpScript.Create(dString).WithDefaultOptions();
            //var issues = dScript.Compile();

            //Assert.False(issues.Any(i => i.Severity == DiagnosticSeverity.Error));
            //Assert.Equal(source, cString);
            Assert.Equal(source, dString);
        }
    }
}