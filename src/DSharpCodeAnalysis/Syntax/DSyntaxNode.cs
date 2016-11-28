using System.Collections.Generic;
using System.Linq;

namespace DSharpCodeAnalysis.Syntax
{
    public class DSyntaxNode
    {
        protected IEnumerable<object> Children { get; set; } = Enumerable.Empty<object>();

        public DSyntaxKind SyntaxKind { get; set; }
        public DSyntaxNode Parent { get; set; }

        public IEnumerable<DSyntaxNode> ChildNodes()
        {
            foreach (var child in Children)
            {
                var nodeChild = child as DSyntaxNode;
                if (nodeChild == null) continue;

                yield return nodeChild;
            }
        }

        public IEnumerable<DSyntaxToken> ChildTokens()
        {
            foreach (var child in Children)
            {
                var tokenChild = child as DSyntaxToken;
                if (tokenChild == null) continue;

                yield return tokenChild;
            }
        }
    }

    public class DMemberDeclarationSyntax : DSyntaxNode
    {

    }

    public class DTypeSyntax : DSyntaxNode
    {

    }

    public class DPredefinedTypeSyntax : DTypeSyntax
    {
        public DSyntaxToken Keyword { get; }

        public DPredefinedTypeSyntax(DSyntaxToken keyword)
        {
            Keyword = keyword;
        }
    }

    public class DMethodDeclarationSyntax : DMemberDeclarationSyntax
    {
        //public IEnumerable<DMemberDeclarationSyntax> Members { get; set; }
        public DSyntaxToken Identifier { get; set; }
        public DTypeSyntax ReturnType { get; set; }

        public DMethodDeclarationSyntax(DTypeSyntax returnType, DSyntaxToken identifierToken)
        {
            ReturnType = returnType;
            Identifier = identifierToken;
            SyntaxKind = DSyntaxKind.MethodDeclaration;
        }

        public DMethodDeclarationSyntax WithModifiers()
        {
            return this;
        }
    }

    public class DClassDeclarationSyntax : DMemberDeclarationSyntax
    {
        //public IEnumerable<DMemberDeclarationSyntax> Members { get; set; }
        public DSyntaxToken Identifier { get; }

        public DClassDeclarationSyntax(DSyntaxToken identifierToken)
        {
            Identifier = identifierToken;
            SyntaxKind = DSyntaxKind.ClassDeclaration;
            Children = new List<object>
            {
                DSyntaxFactory.Token(DSyntaxKind.ClassKeyword),
                Identifier,
                DSyntaxFactory.Token(DSyntaxKind.OpenBraceToken),
                DSyntaxFactory.Token(DSyntaxKind.CloseBraceToken)
            };
        }

        public DClassDeclarationSyntax WithModifiers()
        {
            return this;
        }

        public DClassDeclarationSyntax WithMembers(IEnumerable<DMemberDeclarationSyntax> members)
        {
            var membersList = members as List<DMemberDeclarationSyntax> ?? members.ToList();
            var tempList = Children.ToList();
            var insertLocation = ChildTokens().Select(c => c.SyntaxKind).ToList().IndexOf(DSyntaxKind.OpenBraceToken);
            for (var i = 0; i < membersList.Count; i++)
            {
                membersList[i].Parent = this;
                //tempList.Insert()
            }
            //foreach (var member in membersList)
            //{
            //    member.Parent = this;
            //    tempList.Insert()
            //}
            Children = Children.Concat(membersList);
            return this;
        }
    }
}