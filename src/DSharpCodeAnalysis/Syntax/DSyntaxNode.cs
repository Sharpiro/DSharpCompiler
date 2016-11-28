using System;
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

        public IEnumerable<object> ChildNodesAndTokens()
        {
            return Children;
        }

        public IEnumerable<object> DescendantNodesAndTokens()
        {
            var runningList = new List<object>();
            foreach (var child in Children)
            {
                var nodeChild = child as DSyntaxNode;
                if (nodeChild != null)
                {
                    runningList.Add(nodeChild);
                    runningList = runningList.Concat(nodeChild.DescendantNodesAndTokens()).ToList();
                }
                else
                {
                    var tokenChild = child as DSyntaxToken;
                    runningList.Add(tokenChild);
                    runningList = runningList.Concat(tokenChild.AllTrivia).ToList();
                }
            }
            return runningList;
        }

        public override string ToString()
        {
            return string.Join(String.Empty, Children);
        }
    }

    public class DParameterListSyntax : DSyntaxNode
    {
        public DParameterListSyntax()
        {
            SyntaxKind = DSyntaxKind.ParameterList;
            Children = new List<object>
            {
                DSyntaxFactory.Token(DSyntaxKind.OpenParenToken),
                DSyntaxFactory.Token(DSyntaxKind.CloseParenToken)
            };
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
            SyntaxKind = DSyntaxKind.PredefinedType;
            Children = new List<object>
            {
                keyword
            };
        }
    }

    public class DStatementSyntax : DSyntaxNode
    {

    }

    public class DBlockSyntax : DStatementSyntax
    {
        public DBlockSyntax()
        {
            SyntaxKind = DSyntaxKind.Block;
            Children = new List<object>
            {
                DSyntaxFactory.Token(DSyntaxKind.OpenBraceToken),
                DSyntaxFactory.Token(DSyntaxKind.CloseBraceToken)
            };
        }
    }

    public class DMethodDeclarationSyntax : DMemberDeclarationSyntax
    {
        public DSyntaxToken Identifier { get; set; }
        public DTypeSyntax ReturnType { get; set; }

        public DMethodDeclarationSyntax(DTypeSyntax returnType, DSyntaxToken identifierToken)
        {
            ReturnType = returnType;
            Identifier = identifierToken;
            SyntaxKind = DSyntaxKind.MethodDeclaration;
            Children = new List<object>
            {
                returnType,
                Identifier,
                DSyntaxFactory.ParameterList()
            };
        }

        public DMethodDeclarationSyntax WithModifiers()
        {
            throw new NotImplementedException();
            return this;
        }
        public DMethodDeclarationSyntax WithBody(DBlockSyntax syntax)
        {
            var childrenList = Children.ToList();
            childrenList.Insert(childrenList.Count, syntax);
            Children = childrenList;
            return this;
        }
    }

    public class DClassDeclarationSyntax : DMemberDeclarationSyntax
    {
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
            var modifiedChildren = Children.ToList();
            var insertLocation = ChildTokens().Select(c => c.SyntaxKind).ToList().IndexOf(DSyntaxKind.OpenBraceToken) + 1;
            for (var i = 0; i < membersList.Count; i++)
            {
                var member = membersList[i];
                member.Parent = this;
                modifiedChildren.Insert(insertLocation + i, member);
            }
            Children = modifiedChildren;
            return this;
        }
    }
}