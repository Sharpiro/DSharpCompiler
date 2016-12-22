using DSharpCodeAnalysis.Syntax;
using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reflection;
using Xunit;

namespace DSharpCodeAnalysisTests
{
    public class CloneTests
    {
        [Fact]
        public void TriviaCloneTest()
        {
            var trivia = DSyntaxFactory.Space;
            var clone = trivia.Clone();

            trivia.FullText = "abc";

            Assert.Equal("abc", trivia.FullText);
            Assert.Equal(" ", clone.FullText);
        }

        [Fact]
        public void SyntaxTokenCloneTest()
        {
            var originalToken = DSyntaxFactory.Token(DSyntaxKind.ClassKeyword)
                .WithTrailingTrivia(DSyntaxFactory.TriviaList(DSyntaxFactory.Space));

            var cloneToken = originalToken.Clone();

            var properties = cloneToken.GetType().GetTypeInfo().GetProperties();

            foreach (var property in properties)
            {
                var propertyname = property.Name;

                var originalValue = property.GetValue(originalToken);
                var cloneValue = property.GetValue(cloneToken);

                var originalhash = originalValue?.GetHashCode();
                var cloneHash = cloneValue?.GetHashCode();


                bool? isEqual;
                var isValueType = property.PropertyType.GetTypeInfo().IsValueType;
                if (isValueType)
                    isEqual = originalValue?.Equals(cloneValue);
                else
                    isEqual = originalValue == cloneValue;

                if (isEqual != null)
                    Assert.True(isEqual);
                else
                {
                    Assert.Null(originalValue);
                    Assert.Null(cloneValue);
                }
            }
        }

        [Fact]
        public void ImmutableTest()
        {
            var originalToken = DSyntaxFactory.Token(DSyntaxKind.ClassKeyword)
                .WithTrailingTrivia(DSyntaxFactory.TriviaList(DSyntaxFactory.Space));

            var otherToken = DSyntaxFactory.Token(DSyntaxKind.ClassKeyword)
                .WithTrailingTrivia(DSyntaxFactory.TriviaList(DSyntaxFactory.Space));

            var list = new List<DSyntaxToken> { originalToken };
            var immutableList = ImmutableList.CreateRange(list);

            list[0].Value = "abc123xxx";

            Assert.Equal("abc123xxx", list[0].ValueText);
            Assert.NotEqual("abc123xxx", immutableList[0].ValueText);

        }
    }
}
