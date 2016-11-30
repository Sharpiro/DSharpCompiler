using System;
using System.Collections.Generic;
using System.Linq;
using DSharpCodeAnalysis.Models;

namespace DSharpCodeAnalysis.Syntax
{
    public class DSyntaxNode : IDSyntax
    {
        protected IList<IDSyntax> Children { get; set; } = Enumerable.Empty<IDSyntax>().ToList();

        public DSyntaxKind SyntaxKind { get; set; }
        public DSyntaxNode Parent { get; set; }
        public Span FullSpan => new Span(Position, Width);
        public int Position { get; set; }
        public int Width => ChildNodesAndTokens().Sum(c => c.Width);

        public IEnumerable<DSyntaxNode> ChildNodes()
        {
            var descendants = DescendantNodesAndTokens().OfType<DSyntaxNode>().ToList();
            foreach (var child in Children.OfType<DSyntaxNode>())
            {
                if (!descendants.Contains(child)) continue;
                yield return child;
            }
        }

        public IEnumerable<DSyntaxToken> ChildTokens()
        {
            return Children.OfType<DSyntaxToken>();
            //var descendants = DescendantNodesAndTokens().OfType<DSyntaxToken>().ToList();
            //foreach (var child in Children.OfType<DSyntaxToken>())
            //{
            //    if (!descendants.Contains(child)) continue;
            //    yield return child;
            //}
        }

        public IEnumerable<IDSyntax> ChildNodesAndTokens()
        {
            var descendants = DescendantNodesAndTokens().ToList();
            foreach (var child in Children)
            {
                if (!descendants.Contains(child)) continue;
                yield return child;
            }
        }

        public IEnumerable<IDSyntax> DescendantNodesAndTokens()
        {
            var @this = this;
            var position = Position;
            foreach (var child in Children)
            {
                var nodeChild = child as DSyntaxNode;
                if (nodeChild != null)
                {
                    nodeChild.Position = position;
                    yield return nodeChild;
                    foreach (var item in nodeChild.DescendantNodesAndTokens().ToList())
                    {
                        var isToken = (item as DSyntaxToken) != null;
                        if (isToken)
                            position += item.FullSpan.Length;
                        yield return item;
                    }
                }
                else
                {
                    child.Position = position;
                    position += child.FullSpan.Length;
                    yield return child;
                }
            }
        }

        public SyntaxHierarchyModel DescendantHierarchy()
        {
            var model = new SyntaxHierarchyModel
            {
                SyntaxKind = SyntaxKind.ToString(),
                SyntaxType = nameof(DSyntaxNode),
                FullSpan = FullSpan
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
                        Children = syntaxChild.DescendantHierarchy().Children,
                        FullSpan = syntaxChild.FullSpan
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
                new DSyntaxToken(DSyntaxKind.OpenParenToken) {Parent = this },
                new DSyntaxToken(DSyntaxKind.CloseParenToken) {Parent = this },
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
            keyword.Parent = this;
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
                new DSyntaxToken(DSyntaxKind.OpenBraceToken) {Parent = this },
                new DSyntaxToken(DSyntaxKind.CloseBraceToken) {Parent = this },
            };
        }
    }

    public class DMethodDeclarationSyntax : DMemberDeclarationSyntax
    {
        public DSyntaxToken Identifier { get; set; }
        public DTypeSyntax ReturnType { get; set; }

        public DMethodDeclarationSyntax(DTypeSyntax returnType, DSyntaxToken identifierToken)
        {
            returnType.Parent = this;
            ReturnType = returnType;
            identifierToken.Parent = this;
            Identifier = identifierToken;
            SyntaxKind = DSyntaxKind.MethodDeclaration;
            Children = new List<IDSyntax>
            {
                returnType,
                Identifier,
                new DParameterListSyntax {Parent = this }
            };
        }

        public DMethodDeclarationSyntax WithModifiers()
        {
            throw new NotImplementedException();
            return this;
        }
        public DMethodDeclarationSyntax WithBody(DBlockSyntax syntax)
        {
            syntax.Parent = this;
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
        public DSyntaxToken Keyword { get; set; }

        public DClassDeclarationSyntax(DSyntaxToken identifierToken)
        {
            Identifier = identifierToken;
            Keyword = new DSyntaxToken(DSyntaxKind.ClassKeyword) { Parent = this };
            SyntaxKind = DSyntaxKind.ClassDeclaration;
            Children = new List<IDSyntax>
            {
                Keyword,
                Identifier,
                new DSyntaxToken(DSyntaxKind.OpenBraceToken) {Parent = this },
                new DSyntaxToken(DSyntaxKind.CloseBraceToken) {Parent = this },
            };
        }

        public DClassDeclarationSyntax WithModifiers()
        {
            throw new NotImplementedException();
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

        public DClassDeclarationSyntax WithKeyword(DSyntaxToken dSyntaxToken)
        {
            Keyword = dSyntaxToken;
            Children[0] = Keyword;
            return this;
        }
    }
}