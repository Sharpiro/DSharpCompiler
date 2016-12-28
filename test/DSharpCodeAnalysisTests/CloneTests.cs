using DSharpCodeAnalysis.Parser;
using DSharpCodeAnalysis.Syntax;
using DSharpCodeAnalysis.Transpiler;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
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
            var compilation = DSyntaxFactory.CompilationUnit();

            var originalToken = DSyntaxFactory.Token(DSyntaxKind.ClassKeyword)
                .WithTrailingTrivia(DSyntaxFactory.TriviaList(DSyntaxFactory.Space))
                .WithParent(compilation);


            var cloneToken = originalToken.Clone();

            var properties = cloneToken.GetType().GetTypeInfo().GetProperties();

            foreach (var property in properties)
            {
                var propertyname = property.Name;

                var originalValue = property.GetValue(originalToken);
                var cloneValue = property.GetValue(cloneToken);

                var originalhash = originalValue?.GetHashCode();
                var cloneHash = cloneValue?.GetHashCode();


                bool successfullyCloned;
                var isValueType = property.PropertyType.GetTypeInfo().IsValueType;
                var isStringType = property.PropertyType == typeof(string);
                var isLaterStringType = (originalValue as string) != null;

                if (isValueType || isStringType || isLaterStringType)
                    successfullyCloned = originalValue.Equals(cloneValue);
                else
                    successfullyCloned = originalValue != cloneValue || (originalValue == null && cloneValue == null);

                Assert.True(successfullyCloned);
            }
        }

        [Fact]
        public void SyntaxNodeCloneTest()
        {
            var source = "type Test { }";

            var compilation = DSharpScript.Create(source);
            var originalNode = compilation.DescendantNodesAndTokens().OfType<DClassDeclarationSyntax>().Single();
            var cloneNode = originalNode.Clone<DClassDeclarationSyntax>();

            var properties = typeof(DClassDeclarationSyntax).GetTypeInfo().GetProperties();

            foreach (var property in properties)
            {
                var propertyname = property.Name;

                var originalValue = property.GetValue(originalNode);
                var cloneValue = property.GetValue(cloneNode);

                var originalhash = originalValue?.GetHashCode();
                var cloneHash = cloneValue?.GetHashCode();

                bool successfullyCloned;
                var isValueType = property.PropertyType.GetTypeInfo().IsValueType;
                var isStringType = property.PropertyType == typeof(string);
                var isLaterStringType = (originalValue as string) != null;

                if (isValueType || isStringType || isLaterStringType)
                    successfullyCloned = originalValue.Equals(cloneValue);
                else
                    successfullyCloned = originalValue != cloneValue || (originalValue == null && cloneValue == null);

                Assert.True(successfullyCloned);
            }

            var originalDescendants = originalNode.DescendantNodesAndTokens().ToList();
            var cloneDescendants = cloneNode.DescendantNodesAndTokens().ToList();

            Assert.Equal(originalDescendants.Count, cloneDescendants.Count);

            for (var i = 0; i < originalDescendants.Count; i++)
            {
                var original = originalDescendants[i];
                var clone = cloneDescendants[i];

                var x = original as DSyntaxToken;
                var y = clone as DSyntaxToken;
                if (x != null && y != null)
                {
                    var originalTrivia = x.AllTrivia.ToList();
                    var cloneTrivia = y.AllTrivia.ToList();
                    Assert.Equal(originalTrivia.Count, cloneTrivia.Count);

                    for (var j = 0; j < originalTrivia.Count; j++)
                    {
                        var oTrivia = originalTrivia[j];
                        var cTrivia = cloneTrivia[j];
                        Assert.False(oTrivia == cTrivia);
                    }
                }

                Assert.False(original == clone);
            }
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
            var clone = compilation.Clone();

            var originalDecendants = compilation.DescendantNodesAndTokens().ToList();
            var cloneDescendants = clone.DescendantNodesAndTokens().ToList();

            for (var i = 0; i < originalDecendants.Count; i++)
            {
                var x = originalDecendants[i];
                var y = cloneDescendants[i];
                Assert.False(x == y);
            }


            var dString = compilation.ToString();
            Assert.Equal(source, dString);
        }
    }
}