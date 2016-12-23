
using DSharpCodeAnalysis.Syntax;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq;
using Xunit;

namespace DSharpCodeAnalysisTests
{
    public class SyntaxNodeTests
    {
        [Fact]
        public void ParentNotNullTest()
        {
            var root = DSyntaxFactory.ClassDeclaration("Test")
              .WithMembers(DSyntaxFactory.SingletonList(DSyntaxFactory.MethodDeclaration(
                  DSyntaxFactory.PredefinedType(DSyntaxFactory.Token(DSyntaxKind.VoidKeyword)), DSyntaxFactory.Identifier("Do"))
                  .WithBody(DSyntaxFactory.Block())));

            var dDescendants = root.DescendantNodesAndTokens().ToList();

            for (var i = 0; i < dDescendants.Count; i++)
            {
                dynamic desc = dDescendants[i];
                Assert.NotNull(desc.Parent);
            }
        }
    }
}