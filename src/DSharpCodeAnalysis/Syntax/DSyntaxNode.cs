using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DSharpCodeAnalysis.Models;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Diagnostics;

namespace DSharpCodeAnalysis.Syntax
{
    [DebuggerDisplay("{GetDebuggerDisplay(), nq}")]
    public class DSyntaxNode : IDSyntax
    {
        protected virtual List<IDSyntax> Children { get; set; } = Enumerable.Empty<IDSyntax>().ToList();

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

        private string GetDebuggerDisplay()
        {
            return GetType().Name + " " + SyntaxKind.ToString() + " " + ToString();
        }
    }

    public class DParameterListSyntax : DSyntaxNode
    {
        public DSyntaxToken OpenParenToken { get; set; }
        public DSyntaxToken CloseParenToken { get; set; }
        public DParameterListSyntax()
        {
            SyntaxKind = DSyntaxKind.ParameterList;
            OpenParenToken = new DSyntaxToken(DSyntaxKind.OpenParenToken) { Parent = this };
            CloseParenToken = new DSyntaxToken(DSyntaxKind.CloseParenToken) { Parent = this };
            Children = new List<IDSyntax>
            {
                OpenParenToken,
                CloseParenToken,
            };
        }

        public DParameterListSyntax WithCloseParenToken(DSyntaxToken dSyntaxToken)
        {
            CloseParenToken.WithLeadingTrivia(dSyntaxToken.LeadingTrivia);
            CloseParenToken.WithTrailingTrivia(dSyntaxToken.TrailingTrivia);
            return this;
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

    public class DEqualsValueClauseSyntax : DSyntaxNode
    {
        public DSyntaxToken EqualsToken { get; set; }
        public DExpressionSyntax Value { get; set; }
        protected override List<IDSyntax> Children => new List<IDSyntax> { EqualsToken, Value };

        public DEqualsValueClauseSyntax(DExpressionSyntax expressionSyntax)
        {
            expressionSyntax.Parent = this;
            Value = expressionSyntax;
            SyntaxKind = DSyntaxKind.EqualsValueClause;
        }

        public DEqualsValueClauseSyntax WithEqualsToken(DSyntaxToken dSyntaxToken)
        {
            dSyntaxToken.Parent = this;
            var newClause = new DEqualsValueClauseSyntax(Value)
            {
                EqualsToken = dSyntaxToken,
                Parent = Parent
            };
            return newClause;
        }
    }

    public class DVariableDeclaratorSyntax : DSyntaxNode
    {
        public DSyntaxToken Identifier { get; set; }
        public DEqualsValueClauseSyntax Initializer { get; set; }
        protected override List<IDSyntax> Children => new List<IDSyntax> { Identifier, Initializer };

        public DVariableDeclaratorSyntax(DSyntaxToken identifierToken)
        {
            Identifier = identifierToken;
            SyntaxKind = DSyntaxKind.VariableDeclarator;
        }

        public DVariableDeclaratorSyntax WithInitializer(DEqualsValueClauseSyntax equalsSyntax)
        {
            equalsSyntax.Parent = this;
            var newVariableDeclarator = new DVariableDeclaratorSyntax(Identifier)
            {
                Parent = Parent,
                Initializer = equalsSyntax
            };
            return newVariableDeclarator;
        }
    }

    public class DVariableDeclarationSyntax : DSyntaxNode
    {
        public DTypeSyntax Type { get; set; }
        public DSyntaxList<DVariableDeclaratorSyntax> Variables { get; set; }
        protected override List<IDSyntax> Children
        {
            get
            {
                var list = new List<IDSyntax> { Type };
                list.AddRange(Variables);
                return list;
            }
        }

        public DVariableDeclarationSyntax(DTypeSyntax typeSyntax)
        {
            typeSyntax.Parent = this;
            Type = typeSyntax;
            SyntaxKind = DSyntaxKind.VariableDeclaration;

        }

        public DVariableDeclarationSyntax(DTypeSyntax typeSyntax, DSyntaxList<DVariableDeclaratorSyntax> variables)
            : this(typeSyntax)
        {
            Variables = variables;
        }

        public DVariableDeclarationSyntax WithVariables(DSyntaxList<DVariableDeclaratorSyntax> variables)
        {
            return new DVariableDeclarationSyntax(Type, variables);
        }
    }

    public class DStatementSyntax : DSyntaxNode
    {

    }

    public class DExpressionSyntax : DSyntaxNode
    {

    }

    public class DSyntaxList<T> : DSyntaxNode, IEnumerable<T> where T : DSyntaxNode
    {
        public DSyntaxList(IList<T> list)
        {
            Children = list.OfType<IDSyntax>().ToList();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return Children.OfType<T>().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public class DLiteralExpressionSyntax : DExpressionSyntax
    {
        public DSyntaxToken Token { get; set; }
        protected override List<IDSyntax> Children => new List<IDSyntax> { Token };

        public DLiteralExpressionSyntax(DSyntaxKind syntaxKind, DSyntaxToken token)
        {
            SyntaxKind = syntaxKind;
            token.Parent = this;
            Token = token;
        }
    }

    public class DLocalDeclarationStatementSyntax : DStatementSyntax
    {
        public DVariableDeclarationSyntax Declaration { get; set; }
        public DSyntaxToken SemicolonToken { get; set; }
        protected override List<IDSyntax> Children => new List<IDSyntax> { Declaration, SemicolonToken };

        public DLocalDeclarationStatementSyntax(DVariableDeclarationSyntax declaration)
        {
            declaration.Parent = this;
            Declaration = declaration;
            SyntaxKind = DSyntaxKind.LocalDeclarationStatement;
        }

        public DStatementSyntax WithSemicolonToken(DSyntaxToken dSyntaxToken)
        {
            dSyntaxToken.Parent = this;
            var newLocalStatement = new DLocalDeclarationStatementSyntax(Declaration)
            {
                Parent = Parent,
                SemicolonToken = dSyntaxToken
            };
            return newLocalStatement;
        }
    }

    public class DIdentifierNameSyntax : DTypeSyntax
    {
        public DSyntaxToken Identifier { get; set; }
        protected override List<IDSyntax> Children => new List<IDSyntax> { Identifier };

        public DIdentifierNameSyntax(DSyntaxToken identifierToken)
        {
            identifierToken.Parent = this;
            Identifier = identifierToken;
            SyntaxKind = DSyntaxKind.IdentifierName;
        }
    }

    public class DBlockSyntax : DStatementSyntax
    {
        public DSyntaxToken OpenBraceToken { get; }
        public DSyntaxToken CloseBraceToken { get; }
        public DSyntaxList<DStatementSyntax> Statements { get; set; } = DSyntaxFactory.SingletonList<DStatementSyntax>();
        protected override List<IDSyntax> Children
        {
            get
            {
                var newList = new List<IDSyntax> { OpenBraceToken };
                newList.AddRange(Statements);
                newList.Add(CloseBraceToken);
                return newList;
            }
        }

        public DBlockSyntax()
        {
            SyntaxKind = DSyntaxKind.Block;
            OpenBraceToken = new DSyntaxToken(DSyntaxKind.OpenBraceToken) { Parent = this };
            CloseBraceToken = new DSyntaxToken(DSyntaxKind.CloseBraceToken) { Parent = this };
        }

        public DBlockSyntax(DSyntaxList<DStatementSyntax> statements)
        {
            SyntaxKind = DSyntaxKind.Block;
            OpenBraceToken = new DSyntaxToken(DSyntaxKind.OpenBraceToken) { Parent = this };
            CloseBraceToken = new DSyntaxToken(DSyntaxKind.CloseBraceToken) { Parent = this };
            foreach (var statement in statements)
            {
                statement.Parent = this;
            }
            Statements = statements;
        }

        public DBlockSyntax WithOpenBraceToken(DSyntaxToken dSyntaxToken)
        {
            OpenBraceToken.WithLeadingTrivia(dSyntaxToken.LeadingTrivia);
            OpenBraceToken.WithTrailingTrivia(dSyntaxToken.TrailingTrivia);
            return this;
        }

        public DBlockSyntax WithCloseBraceToken(DSyntaxToken dSyntaxToken)
        {
            CloseBraceToken.WithLeadingTrivia(dSyntaxToken.LeadingTrivia);
            CloseBraceToken.WithTrailingTrivia(dSyntaxToken.TrailingTrivia);
            return this;
        }

        public DBlockSyntax WithStatements(DSyntaxList<DStatementSyntax> statements)
        {
            foreach (var item in statements)
            {
                item.Parent = this;
            }
            var newBlock = DSyntaxFactory.Block(statements);
            newBlock.Parent = Parent;
            return newBlock;
        }
    }

    public class DMethodDeclarationSyntax : DMemberDeclarationSyntax
    {
        public DSyntaxToken Identifier { get; set; }
        public DTypeSyntax ReturnType { get; set; }
        public DParameterListSyntax ParameterList { get; set; }

        public DMethodDeclarationSyntax(DTypeSyntax returnType, DSyntaxToken identifierToken)
        {
            returnType.Parent = this;
            ReturnType = returnType;
            identifierToken.Parent = this;
            Identifier = identifierToken;
            SyntaxKind = DSyntaxKind.MethodDeclaration;
            ParameterList = new DParameterListSyntax { Parent = this };
            Children = new List<IDSyntax>
            {
                returnType,
                Identifier,
                ParameterList
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

        public DMethodDeclarationSyntax WithParameterList(DParameterListSyntax parameterList)
        {
            parameterList.Parent = this;
            Children[2] = parameterList;
            ParameterList = parameterList;
            return this;
        }
    }

    public class DClassDeclarationSyntax : DMemberDeclarationSyntax
    {
        private const int defaultSpanLength = 7;
        public DSyntaxToken Identifier { get; }
        public DSyntaxToken Keyword { get; set; }
        public DSyntaxToken OpenBraceToken { get; }
        public DSyntaxToken SemicolonToken { get; }
        public DSyntaxToken CloseBraceToken { get; }


        public DClassDeclarationSyntax(DSyntaxToken identifierToken)
        {
            Keyword = new DSyntaxToken(DSyntaxKind.ClassKeyword) { Parent = this };
            Identifier = identifierToken;
            SyntaxKind = DSyntaxKind.ClassDeclaration;
            OpenBraceToken = new DSyntaxToken(DSyntaxKind.OpenBraceToken) { Parent = this };
            CloseBraceToken = new DSyntaxToken(DSyntaxKind.CloseBraceToken) { Parent = this };
            Children = new List<IDSyntax>
            {
                Keyword,
                Identifier,
                OpenBraceToken,
                CloseBraceToken
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
            Keyword.WithLeadingTrivia(dSyntaxToken.LeadingTrivia);
            Keyword.WithTrailingTrivia(dSyntaxToken.TrailingTrivia);
            return this;
        }

        public DClassDeclarationSyntax WithOpenBraceToken(DSyntaxToken dSyntaxToken)
        {
            OpenBraceToken.WithLeadingTrivia(dSyntaxToken.LeadingTrivia);
            OpenBraceToken.WithTrailingTrivia(dSyntaxToken.TrailingTrivia);
            return this;
        }

        public DClassDeclarationSyntax WithCloseBraceToken(DSyntaxToken dSyntaxToken)
        {
            CloseBraceToken.WithLeadingTrivia(dSyntaxToken.LeadingTrivia);
            CloseBraceToken.WithTrailingTrivia(dSyntaxToken.TrailingTrivia);
            return this;
        }
    }
}