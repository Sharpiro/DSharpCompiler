using DSharpCodeAnalysis.Syntax;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace DSharpCodeAnalysisTests
{
    public class DSyntaxListTests
    {
        [Fact]

        public void GetNodesAndSeperatorsTest()
        {
            var nodes = new List<IDSyntax>
            {
                new DPredefinedTypeSyntax(DSyntaxFactory.Token(DSyntaxKind.IntKeyword)),
                DSyntaxFactory.Token(DSyntaxKind.CommaToken),
                new DPredefinedTypeSyntax(DSyntaxFactory.Token(DSyntaxKind.IntKeyword)),
                DSyntaxFactory.Token(DSyntaxKind.CommaToken),
                new DPredefinedTypeSyntax(DSyntaxFactory.Token(DSyntaxKind.IntKeyword)),
                DSyntaxFactory.Token(DSyntaxKind.CommaToken),
                new DPredefinedTypeSyntax(DSyntaxFactory.Token(DSyntaxKind.IntKeyword)),
                DSyntaxFactory.Token(DSyntaxKind.CommaToken),
                new DPredefinedTypeSyntax(DSyntaxFactory.Token(DSyntaxKind.IntKeyword)),
                DSyntaxFactory.Token(DSyntaxKind.CommaToken),
                new DPredefinedTypeSyntax(DSyntaxFactory.Token(DSyntaxKind.IntKeyword)),
            };
            var list = DSyntaxFactory.SeparatedList<DSyntaxNode>(nodes);

            var allItems = list.GetNodesAndSeperators().ToList();

            Assert.Equal(nodes.Count, allItems.Count);
            
        }
    }
}