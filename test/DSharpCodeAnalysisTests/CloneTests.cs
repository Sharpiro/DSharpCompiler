using DSharpCodeAnalysis.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Xunit;

namespace DSharpCodeAnalysisTests
{
    public class CloneTests
    {
        [Fact]
        public void SyntaxTokenCloneTest()
        {
            var originalToken = DSyntaxFactory.Token(DSyntaxKind.ClassKeyword);
            originalToken = originalToken.WithTrailingTrivia(DSyntaxFactory.TriviaList(DSyntaxFactory.Space));

            var cloneToken = originalToken.Clone();

            var properties = cloneToken.GetType().GetTypeInfo().GetProperties();

            foreach (var property in properties)
            {
                var propertyname = property.Name;

                var originalValue = property.GetValue(originalToken);
                var cloneValue = property.GetValue(cloneToken);

                var originalhash = originalValue.GetHashCode();
                var cloneHash = cloneValue.GetHashCode();

                var x = originalValue.Equals(cloneValue);
                var y = originalValue == cloneValue;
                //var z = (DSyntaxKind)originalValue == (DSytnaxKind)cloneValue;
            }
        }
    }
}
