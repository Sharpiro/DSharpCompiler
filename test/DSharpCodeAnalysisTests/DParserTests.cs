using DSharpCodeAnalysis;
using DSharpCodeAnalysis.Exceptions;
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
            var source = "type Program { }";
            var dCompilationUnit = DSharpScript.Create(source);
            var cCompilationUnit = CSharpScript.Create(source).GetCompilation().SyntaxTrees.Single().GetCompilationUnitRoot();
            var cString = cCompilationUnit.ToString();
            var dString = dCompilationUnit.ToString();

            Assert.Equal(source, cString);
            Assert.Equal(source, dString);

        }

        [Fact]
        public void MultiLineClassParseTest()
        {
            var source =
@"type Program
{

}".Replace(Environment.NewLine, "\n");
            var dCompilationUnit = DSharpScript.Create(source);
            var cCompilationUnit = CSharpScript.Create(source).GetCompilation().SyntaxTrees.Single().GetCompilationUnitRoot();
            var cString = cCompilationUnit.ToString();
            var dString = dCompilationUnit.ToString();

            Assert.Equal(source, cString);
            Assert.Equal(source, dString);
        }

        [Fact]
        public void OneLineMethodTestParseTest()
        {
            var source = "func int Add(int x, int y);";
            var dCompilationUnit = DSharpScript.Create(source);
            var dString = dCompilationUnit.ToString();

            Assert.Equal(source, dString);
        }

        [Fact]
        public void InvocationExpressionParseTest()
        {
            var source = "System.Console.WriteLine(result);";
            var dCompilationUnit = DSharpScript.Create(source);
            var script = CSharpScript.Create(source);
            var cCompilationUnit = script.GetCompilation().SyntaxTrees.Single().GetCompilationUnitRoot();
            var cString = cCompilationUnit.ToString();
            var dString = dCompilationUnit.ToString();

            Assert.Equal(source, cString);
            Assert.Equal(source, dString);
        }

        [Fact]
        public void GlobalDeclarationParseTest()
        {
            var source = "let x = 2;";
            var dCompilationUnit = DSharpScript.Create(source);

            var script = CSharpScript.Create(source);
            var cCompilationUnit = script.GetCompilation().SyntaxTrees.Single().GetCompilationUnitRoot();
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
    let temp = 2;
    return x + y;
}
let result = Add(2, 3);
let temp = 3;".Replace(Environment.NewLine, "\n");
            var dCompilationUnit = DSharpScript.Create(source);
            var descendants = dCompilationUnit.DescendantNodesAndTokens();
            var dString = dCompilationUnit.ToString();
            Assert.Equal(source, dString);
        }

        [Fact]
        public void ModifiersParseTest()
        {
            var source =
@"external func int Add(int x, int y)
{
    let temp = 2;
    return x + y;
}".Replace(Environment.NewLine, "\n");

            var dCompilationUnit = DSharpScript.Create(source);
            var descendants = dCompilationUnit.DescendantNodesAndTokens();
            var dString = dCompilationUnit.ToString();
            Assert.Equal(source, dString);
        }

        [Fact]
        public void WrongTokenErrorTest()
        {
            var source =
@"external func int Add(int x, int y)
{
    let temp = 2;
    return x + y;
}+";

            var script = CSharpScript.Create(source);
            var cCompilationUnit = script.GetCompilation().SyntaxTrees.Single().GetCompilationUnitRoot();
            var cDescendants = cCompilationUnit.DescendantNodesAndTokens().ToList();
            var diagnostics = script.Compile();

            Assert.Throws<UnexpectedTokenException>(() => DSharpScript.Create(source));
        }
    }
}