using System;
using System.Collections.Generic;
using System.Linq;
using DSharpCodeAnalysis.Models;

namespace DSharpCodeAnalysis.Syntax
{
    public class DSyntaxNode : IDSyntax
    {
        protected IEnumerable<IDSyntax> Children { get; set; } = Enumerable.Empty<IDSyntax>();

        public DSyntaxKind SyntaxKind { get; set; }
        public DSyntaxNode Parent { get; set; }
        public Span FullSpan { get; set; }
        public int Position { get; set; }

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

        public IEnumerable<IDSyntax> ChildNodesAndTokens()
        {
            return Children;
        }

        public IEnumerable<IDSyntax> DescendantNodesAndTokens()
        {
            foreach (var child in Children)
            {
                var nodeChild = child as DSyntaxNode;
                if (nodeChild != null)
                {
                    yield return child;
                    foreach (var item in nodeChild.DescendantNodesAndTokens().ToList())
                    {
                        yield return item;
                    }
                }
                else
                {
                    var tokenChild = child as DSyntaxToken;
                    yield return child;
                }
            }
            //return runningList;
        }

        public SyntaxHierarchyModel DescendantHierarchy()
        {
            var model = new SyntaxHierarchyModel
            {
                SyntaxKind = SyntaxKind.ToString(),
                SyntaxType = nameof(DSyntaxNode)
            };
            foreach (var child in Children)
            {
                IDSyntax syntaxChild;
                if ((syntaxChild = child as DSyntaxNode) != null)
                {
                    model.Children.Add(syntaxChild.DescendantHierarchy());
                }
                else if ((syntaxChild = child as DSyntaxToken) != null)
                {
                    model.Children.Add(new SyntaxHierarchyModel
                    {
                        SyntaxKind = syntaxChild.SyntaxKind.ToString(),
                        SyntaxType = nameof(DSyntaxToken),
                        Children = syntaxChild.DescendantHierarchy().Children
                    });
                }
                else if ((syntaxChild = child as Trivia) != null)
                {
                    model.Children.Add(new SyntaxHierarchyModel
                    {
                        SyntaxKind = syntaxChild.SyntaxKind.ToString(),
                        SyntaxType = nameof(DSyntaxToken)
                    });
                }
                else
                    throw new ArgumentException($"Invalid Syntax item for {child.ToString()}");
            }
            return model;
        }


        public override string ToString()
        {
            return string.Join(string.Empty, Children);
        }
    }

    public class DParameterListSyntax : DSyntaxNode
    {
        public DParameterListSyntax()
        {
            SyntaxKind = DSyntaxKind.ParameterList;
            Children = new List<IDSyntax>
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
            Children = new List<IDSyntax>
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
            Children = new List<IDSyntax>
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
            Children = new List<IDSyntax>
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
        private const int defaultSpanLength = 7;
        public DSyntaxToken Identifier { get; }

        public DClassDeclarationSyntax(DSyntaxToken identifierToken)
        {
            Identifier = identifierToken;
            SyntaxKind = DSyntaxKind.ClassDeclaration;
            FullSpan = new Span(Position, defaultSpanLength + identifierToken.FullSpan.Length);
            Children = new List<IDSyntax>
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