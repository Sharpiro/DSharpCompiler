using DSharpCodeAnalysis.Parser;
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
            var source = "var x = System.StringBuilder.New();";

            var cCompilation = CSharpScript.Create(source).GetCompilation().SyntaxTrees.Single().GetCompilationUnitRoot();
            var dCompilation = DSharpScript.Create(source);

            var cDescendants = cCompilation.DescendantNodes().ToList();
            var dDescendants = dCompilation.DescendantNodesAndTokens().ToList();

            var transpiler = new CTranspiler(dCompilation);
            var transCompilation = transpiler.Transpile();
        }
    }
}
