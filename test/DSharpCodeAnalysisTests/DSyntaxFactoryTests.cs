using DSharpCodeAnalysis.Parser;
using DSharpCodeAnalysis.Syntax;
using DSharpCodeAnalysis.Transpiler;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace DSharpCodeAnalysisTests
{
    public class DSyntaxFactoryTests
    {
        [Fact]
        public void QualifiedNameTest()
        {
            const string source = "var x = System.Whatevers.StringBuilder.New();";

            var cCompilation = CSharpScript.Create(source).GetCompilation().SyntaxTrees.Single().GetCompilationUnitRoot();
            var dCompilation = DSharpScript.Create(source);

            var cDescendants = cCompilation.DescendantNodes().ToList();
            var dDescendants = dCompilation.DescendantNodesAndTokens().ToList();

            var memberAccessExpression = dDescendants.OfType<DMemberAccessExpression>().First();

            var qualifiedName = DSyntaxFactory.QualifiedName(memberAccessExpression);
            var qualifiedNameString = qualifiedName.ToString();

            var dCompilationString = dCompilation.ToString();

            Assert.Equal(source, dCompilationString);
        }
    }
}
