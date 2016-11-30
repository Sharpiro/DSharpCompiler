using DSharpCodeAnalysis.Models;
using DSharpCodeAnalysis.Parser;
using DSharpCodeAnalysis.Syntax;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Newtonsoft.Json;
using System.Linq;
using Xunit;

namespace DSharpCodeAnalysisTests
{
    public class SyntaxNodeTests
    {
        [Fact]
        public void GetChildNodesTest()
        {
            var cClass = SyntaxFactory.ClassDeclaration("Test")
               .WithMembers(SyntaxFactory.SingletonList<MemberDeclarationSyntax>(SyntaxFactory.MethodDeclaration(
                   SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.VoidKeyword)), SyntaxFactory.Identifier("Do"))
                   .WithBody(SyntaxFactory.Block())));

            var dClass = DSyntaxFactory.ClassDeclaration("Test")
               .WithMembers(DSyntaxFactory.SingletonList(DSyntaxFactory.MethodDeclaration(
                   DSyntaxFactory.PredefinedType(DSyntaxFactory.Token(DSyntaxKind.VoidKeyword)), DSyntaxFactory.Identifier("Do"))
                   .WithBody(DSyntaxFactory.Block())));

            var cDescendants = cClass.DescendantNodesAndTokens().ToList();
            var cChildNodes = cClass.ChildNodes().ToList();
            var cChildTokens = cClass.ChildTokens().ToList();
            var cChildren = cClass.ChildNodesAndTokens().ToList();
            var cSource = cClass.ToString();
            var cSpan = cClass.FullSpan;

            var dDescendants = dClass.DescendantNodesAndTokens().ToList();
            var dChildNodes = dClass.ChildNodes().ToList();
            var dChildTokens = dClass.ChildTokens().ToList();
            var dChildren = dClass.ChildNodesAndTokens().ToList();
            var dSource = dClass.ToString();
            var dSpan = dClass.FullSpan;

            Assert.Equal(cDescendants.Count, dDescendants.Count);
            Assert.Equal(cChildNodes.Count, dChildNodes.Count);
            Assert.Equal(cChildTokens.Count, dChildTokens.Count);
            Assert.Equal(cChildren.Count, dChildren.Count);
            Assert.Equal(cSource, dSource);
            Assert.Equal(cSpan.Length, dSpan.Length);
        }



        [Fact]
        public void ToJsonTest()
        {
            var dClass = DSyntaxFactory.ClassDeclaration("Test")
               .WithMembers(DSyntaxFactory.SingletonList(DSyntaxFactory.MethodDeclaration(
                   DSyntaxFactory.PredefinedType(DSyntaxFactory.Token(DSyntaxKind.VoidKeyword)), DSyntaxFactory.Identifier("Do"))
                   .WithBody(DSyntaxFactory.Block())));

            var model = dClass.DescendantHierarchy();

            var json = JsonConvert.SerializeObject(model);
        }

        [Fact]
        public void ClassSpanTest()
        {
            var cClass = SyntaxFactory.ClassDeclaration("Test")
               .WithMembers(SyntaxFactory.SingletonList<MemberDeclarationSyntax>(SyntaxFactory.MethodDeclaration(
                   SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.VoidKeyword)), SyntaxFactory.Identifier("Do"))
                   .WithBody(SyntaxFactory.Block())));

            var dClass = DSyntaxFactory.ClassDeclaration("Test")
               .WithMembers(DSyntaxFactory.SingletonList(DSyntaxFactory.MethodDeclaration(
                   DSyntaxFactory.PredefinedType(DSyntaxFactory.Token(DSyntaxKind.VoidKeyword)), DSyntaxFactory.Identifier("Do"))
                   .WithBody(DSyntaxFactory.Block())));

            var cDescendants = cClass.DescendantNodesAndTokens().ToList();
            var dDescendants = dClass.DescendantNodesAndTokens();
            var enumerator = dDescendants.GetEnumerator();


            for (var i = 0; i < cDescendants.Count; i++)
            {
                enumerator.MoveNext();
                var cDesc = cDescendants[i];
                var dDesc = enumerator.Current;
                Assert.Equal(cDesc.FullSpan.Length, dDesc.FullSpan.Length);
                Assert.Equal(cDesc.FullSpan.Start, dDesc.FullSpan.Start);
                Assert.Equal(cDesc.FullSpan.End, dDesc.FullSpan.End);
            }
        }

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