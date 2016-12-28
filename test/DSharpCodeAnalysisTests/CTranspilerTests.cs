using DSharpCodeAnalysis.Parser;
using DSharpCodeAnalysis.Syntax;
using DSharpCodeAnalysis.Transpiler;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Linq;
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
            var compilation = DSharpScript.Create(source);
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
            var compilation = DSharpScript.Create(source);
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
            var compilation = DSharpScript.Create(source);
            var xString = compilation.ToString();
            var transpiler = new CTranspiler(compilation);
            var transCompilation = transpiler.Transpile();

            var transpiledString = transCompilation.ToString();
            var dString = compilation.ToString();

            Assert.Equal(source, dString);
            Assert.Equal(transpiledSource, transpiledString);
        }

        [Fact]
        public void NewTest()
        {
            const string cSource = "var x = new System.StringBuilder();";
            const string dSource = "let x = System.StringBuilder.New();";

            var cCompilation = CSharpScript.Create(cSource).GetCompilation().SyntaxTrees.Single().GetCompilationUnitRoot();
            var dCompilation = DSharpScript.Create(dSource);

            var cDescendants = cCompilation.DescendantNodes().ToList();
            var dDescendants = dCompilation.DescendantNodesAndTokens().ToList();

            var transpiler = new CTranspiler(dCompilation);
            var transCompilation = transpiler.Transpile();

            var transpiledString = transCompilation.ToString();
            var dString = dCompilation.ToString();
            Assert.Equal(dSource, dString);
            Assert.Equal(cSource, transpiledString);
        }

        [Fact]
        public void RemoveIdentifierTest()
        {
            const string dSource = "let x = System.Whatever.StringBuilder.New();";
            const string w = "System.Whatever.StringBuilder.New";
            const string x = "System.Whatever.StringBuilder";
            const string y = "System.Whatever";
            const string z = "System";

            var dCompilation = DSharpScript.Create(dSource);
            var dDescendants = dCompilation.DescendantNodesAndTokens().ToList();
            var invocation = dDescendants.OfType<DInvocationExpressionSyntax>().Single();
            var topMemberAccess = (DMemberAccessExpression)invocation.Expression;

            DQualifiedNameSyntax qualifiedName;
            qualifiedName = DSyntaxFactory.QualifiedName(topMemberAccess);
            Assert.Equal(w, qualifiedName.ToString());

            qualifiedName = (DQualifiedNameSyntax)qualifiedName.RemoveLastIdentifier();
            Assert.Equal(x, qualifiedName.ToString());

            qualifiedName = (DQualifiedNameSyntax)qualifiedName.RemoveLastIdentifier();
            Assert.Equal(y, qualifiedName.ToString());

            var identifierName = qualifiedName.RemoveLastIdentifier();
            Assert.Equal(z, identifierName.ToString());
        }

        [Fact]
        public void FluentObjectionCreationTest()
        {
            const string cSource = "var x = new Exception().ToString();";
            const string dSource = "let x = Exception.New().ToString();";

            var cCompilation = CSharpScript.Create(cSource);
            var dCompilation = DSharpScript.Create(dSource);

            var cDescendants = cCompilation.GetCompilation().SyntaxTrees.Single().GetCompilationUnitRoot().DescendantNodesAndTokens().ToList();
            var dDescendants = dCompilation.DescendantNodesAndTokens().ToList();

            var transpiler = new CTranspiler(dCompilation);
            var transCompilation = transpiler.Transpile();
            var transString = transCompilation.ToString();

            Assert.Equal(dSource, dCompilation.ToString());
            Assert.Equal(cSource, transString);
        }

        [Fact]
        public void ClassTest()
        {
            var source =
@"type Adder
{
    external func int Add(int x, int y)
    {
        let temp = 2;
        return x + y;
    }
}
let test = System.Exception.New();
let adder = Adder.New();
let result = adder.Add(2, 3);
let xxx = Adder.New().Add(1, 1);".Replace(Environment.NewLine, "\n");
            var transpiledSource =
@"class Adder
{
    public int Add(int x, int y)
    {
        var temp = 2;
        return x + y;
    }
}
var test = new System.Exception();
var adder = new Adder();
var result = adder.Add(2, 3);
var xxx = new Adder().Add(1, 1);".Replace(Environment.NewLine, "\n");
            var compilation = DSharpScript.Create(source);
            var dDescendants = compilation.DescendantNodesAndTokens().ToList();
            var xString = compilation.ToString();
            var transpiler = new CTranspiler(compilation);
            var transCompilation = transpiler.Transpile();

            var transpiledString = transCompilation.ToString();
            var dString = compilation.ToString();
            Assert.Equal(source, dString);
            Assert.Equal(transpiledSource, transpiledString);
        }
    }
}