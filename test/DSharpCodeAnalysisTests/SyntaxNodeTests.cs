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
        public void TestOne()
        {
            var @class = SyntaxFactory.ClassDeclaration("Test")
                .WithMembers(SyntaxFactory.SingletonList<MemberDeclarationSyntax>(SyntaxFactory.MethodDeclaration(SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.VoidKeyword)), SyntaxFactory.Identifier("Do"))));
            var method = @class.ChildNodes().OfType<MethodDeclarationSyntax>().FirstOrDefault();
            var childTokens = @class.ChildTokens();
            SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.VoidKeyword));
            var x = childTokens.First().LeadingTrivia.First();
            //var lexer = new Lexer();
            //var source = "using System;";
            //lexer.Lex(source);
            //SyntaxFactory.TokenList();
        }

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

            Assert.Equal(cChildNodes.Count, dChildNodes.Count);
            Assert.Equal(cChildTokens.Count, dChildTokens.Count);
            Assert.Equal(cChildren.Count, dChildren.Count);
            Assert.Equal(cDescendants.Count, dDescendants.Count);
            Assert.Equal(cSource, dSource);
            //Assert.Equal(cSpan.Length, dSpan.Length);
        }

        [Fact]
        public void ToJsonTest()
        {
            var dClass = DSyntaxFactory.ClassDeclaration("Test")
               .WithMembers(DSyntaxFactory.SingletonList(DSyntaxFactory.MethodDeclaration(
                   DSyntaxFactory.PredefinedType(DSyntaxFactory.Token(DSyntaxKind.VoidKeyword)), DSyntaxFactory.Identifier("Do"))
                   .WithBody(DSyntaxFactory.Block())));

            var dDescendants = dClass.DescendantNodesAndTokens().ToList();
            var dChildNodes = dClass.ChildNodes().ToList();
            var dChildTokens = dClass.ChildTokens().ToList();
            var dChildren = dClass.ChildNodesAndTokens().ToList();
            var dSource = dClass.ToString();

            var model = dClass.DescendantHierarchy();

            var json = JsonConvert.SerializeObject(model);
        }

        [Fact]
        public void SimpleSpanTest()
        {
            var cClass = SyntaxFactory.ClassDeclaration("Test");
            var dClass = DSyntaxFactory.ClassDeclaration("Test");

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

            
            //for (var i = 0; i < cDescendants.Count; i++)
            //{
            //    var cDesc = cDescendants[i];
            //    var dDesc = dDescendants[i];
            //    Assert.Equal(cDesc.FullSpan.Length, dDesc.FullSpan.Length);
            //}
        }
    }
}