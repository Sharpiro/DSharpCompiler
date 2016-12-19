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
    public abstract class DSyntaxNode : IDSyntax
    {
        protected abstract List<IDSyntax> Children { get; }
        public DSyntaxKind SyntaxKind { get; set; }
        public DSyntaxNode Parent { get; set; }
        public Span Span => new Span(Position, Width);
        public Span FullSpan => new Span(Position, FullWidth);
        public int Position { get; set; }
        public int Width
        {
            get
            {
                var @this = this;
                var nodeSum = ChildNodesAndTokens().Sum(c => c.Span.Length);
                var firstToken = ChildTokens().FirstOrDefault();
                var lastToken = ChildTokens().LastOrDefault();
                var leadingTriviaLength = firstToken?.LeadingTrivia.Sum(t => t.FullSpan.Length) ?? 0;
                var trailingTriviaLength = lastToken?.TrailingTrivia.Sum(t => t.FullSpan.Length) ?? 0;
                return leadingTriviaLength == 0 && trailingTriviaLength == 0 ? FullWidth : nodeSum + leadingTriviaLength + trailingTriviaLength;
            }
        }
        public int FullWidth => ChildNodesAndTokens().Sum(c => c.FullSpan.Length);



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
                Span = Span,
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
                        Span = syntaxChild.Span,
                        FullSpan = syntaxChild.FullSpan
                    });
                }
                else if ((syntaxChild = child as DTrivia) != null)
                {
                    model.Children.Add(new SyntaxHierarchyModel
                    {
                        SyntaxKind = syntaxChild.SyntaxKind.ToString(),
                        SyntaxType = nameof(DSyntaxToken),
                        Span = syntaxChild.Span,
                        FullSpan = syntaxChild.FullSpan
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

    public class DArgumentSyntax : DSyntaxNode
    {
        public DExpressionSyntax Expression { get; set; }
        protected override List<IDSyntax> Children => new List<IDSyntax> { Expression };


        public DArgumentSyntax(DExpressionSyntax expression)
        {
            if (expression == null) throw new ArgumentNullException(nameof(expression));

            Expression = expression;
            SyntaxKind = DSyntaxKind.Argument;
        }
    }

    public class DParameterSyntax : DSyntaxNode
    {
        public DTypeSyntax Type { get; set; }
        public DSyntaxToken Identifier { get; set; }
        protected override List<IDSyntax> Children => new List<IDSyntax> { Type, Identifier };

        public DParameterSyntax(DSyntaxToken identifier)
        {
            Identifier = identifier;
            SyntaxKind = DSyntaxKind.Parameter;
        }

        public DParameterSyntax WithType(DTypeSyntax type)
        {
            var newParameter = new DParameterSyntax(Identifier) { Type = type };
            Identifier.Parent = newParameter;
            type.Parent = newParameter;
            newParameter.Parent = Parent;
            return newParameter;
        }
    }

    public class DParameterListSyntax : DSyntaxNode
    {
        public DSyntaxToken OpenParenToken { get; set; }
        public DSyntaxToken CloseParenToken { get; set; }
        public DSeparatedSyntaxList<DParameterSyntax> Parameters { get; set; }
        protected override List<IDSyntax> Children
        {
            get
            {
                var list = new List<IDSyntax> { OpenParenToken };
                list.AddRange(Parameters.GetNodesAndSeperators());
                list.Add(CloseParenToken);
                return list;
            }
        }

        public DParameterListSyntax()
        {
            SyntaxKind = DSyntaxKind.ParameterList;
            OpenParenToken = new DSyntaxToken(DSyntaxKind.OpenParenToken) { Parent = this };
            CloseParenToken = new DSyntaxToken(DSyntaxKind.CloseParenToken) { Parent = this };
            Parameters = DSyntaxFactory.SeparatedList<DParameterSyntax>();
        }

        public DParameterListSyntax(DSeparatedSyntaxList<DParameterSyntax> parameters)
        {
            SyntaxKind = DSyntaxKind.ParameterList;
            OpenParenToken = new DSyntaxToken(DSyntaxKind.OpenParenToken) { Parent = this };
            CloseParenToken = new DSyntaxToken(DSyntaxKind.CloseParenToken) { Parent = this };
            Parameters = parameters;
        }

        public DParameterListSyntax WithCloseParenToken(DSyntaxToken closeParentToken)
        {
            var newParameterList = Clone();
            closeParentToken.Parent = newParameterList;
            newParameterList.CloseParenToken = closeParentToken;
            return newParameterList;
        }

        public DParameterListSyntax WithOpenParenToken(DSyntaxToken openParenToken)
        {
            var newParameterList = Clone();
            openParenToken.Parent = newParameterList;
            newParameterList.OpenParenToken = openParenToken;
            return newParameterList;
        }

        private DParameterListSyntax Clone()
        {
            var newParameterList = DSyntaxFactory.ParameterList(Parameters);
            OpenParenToken.Parent = newParameterList;
            CloseParenToken.Parent = newParameterList;
            Parameters.SetParent(newParameterList);
            newParameterList.OpenParenToken = OpenParenToken;
            newParameterList.CloseParenToken = CloseParenToken;
            newParameterList.Parent = Parent;
            return newParameterList;
        }
    }

    public class DArgumentListSyntax : DSyntaxNode
    {
        public DSeparatedSyntaxList<DArgumentSyntax> Arguments { get; set; }
        public DSyntaxToken OpenParenToken { get; set; }
        public DSyntaxToken CloseParenToken { get; set; }
        protected override List<IDSyntax> Children
        {
            get
            {
                var list = new List<IDSyntax> { OpenParenToken };
                list.AddRange(Arguments.GetNodesAndSeperators());
                list.Add(CloseParenToken);
                return list;
            }
        }

        public DArgumentListSyntax(DSeparatedSyntaxList<DArgumentSyntax> arguments = default(DSeparatedSyntaxList<DArgumentSyntax>))
        {
            Arguments = arguments ?? DSyntaxFactory.SeparatedList<DArgumentSyntax>();
            OpenParenToken = new DSyntaxToken(DSyntaxKind.OpenParenToken) { Parent = this };
            CloseParenToken = new DSyntaxToken(DSyntaxKind.CloseParenToken) { Parent = this };
            SyntaxKind = DSyntaxKind.ArgumentList;
        }

        public DArgumentListSyntax WithOpenParenToken(DSyntaxToken openParenToken)
        {
            var newArgumentList = Clone();
            newArgumentList.OpenParenToken = openParenToken;
            openParenToken.Parent = newArgumentList;
            return newArgumentList;
        }

        public DArgumentListSyntax WithCloseParenToken(DSyntaxToken closeParenToken)
        {
            var newArgumentList = Clone();
            newArgumentList.CloseParenToken = closeParenToken;
            closeParenToken.Parent = newArgumentList;
            return newArgumentList;
        }

        private DArgumentListSyntax Clone()
        {
            var newArgumentList = new DArgumentListSyntax
            {
                Arguments = Arguments,
                OpenParenToken = OpenParenToken,
                CloseParenToken = CloseParenToken
            };

            Arguments.SetParent(newArgumentList);
            OpenParenToken.Parent = newArgumentList;
            CloseParenToken.Parent = newArgumentList;
            return newArgumentList;
        }
    }

    public abstract class DMemberDeclarationSyntax : DSyntaxNode
    {
        protected override abstract List<IDSyntax> Children { get; }
    }

    public class DGlobalStatementSyntax : DMemberDeclarationSyntax
    {
        public DStatementSyntax Statement { get; }
        protected override List<IDSyntax> Children => new List<IDSyntax> { Statement };

        public DGlobalStatementSyntax(DStatementSyntax statement)
        {
            if (statement == null) throw new ArgumentNullException(nameof(statement));

            Statement = statement;
            SyntaxKind = DSyntaxKind.GlobalStatement;
        }
    }

    public abstract class DTypeSyntax : DExpressionSyntax
    {
        protected override abstract List<IDSyntax> Children { get; }
    }

    public class DPredefinedTypeSyntax : DTypeSyntax
    {
        public DSyntaxToken Keyword { get; }
        protected override List<IDSyntax> Children => new List<IDSyntax> { Keyword };
        //public override int Width => ChildNodesAndTokens().Sum(i => i.Span.Length);

        public DPredefinedTypeSyntax(DSyntaxToken keyword)
        {
            //todo: update parent in factory
            keyword.Parent = this;
            Keyword = keyword;
            SyntaxKind = DSyntaxKind.PredefinedType;
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
        public DSeparatedSyntaxList<DVariableDeclaratorSyntax> Variables { get; set; }
        protected override List<IDSyntax> Children
        {
            get
            {
                var list = new List<IDSyntax> { Type };
                list.AddRange(Variables.GetNodesAndSeperators());
                return list;
            }
        }

        public DVariableDeclarationSyntax(DTypeSyntax typeSyntax)
        {
            typeSyntax.Parent = this;
            Type = typeSyntax;
            SyntaxKind = DSyntaxKind.VariableDeclaration;

        }

        public DVariableDeclarationSyntax(DTypeSyntax typeSyntax, DSeparatedSyntaxList<DVariableDeclaratorSyntax> variables)
            : this(typeSyntax)
        {
            Variables = variables;
        }

        public DVariableDeclarationSyntax WithVariables(DSeparatedSyntaxList<DVariableDeclaratorSyntax> variables)
        {
            var newVariableDeclaration = new DVariableDeclarationSyntax(Type, variables);
            Type.Parent = newVariableDeclaration;
            variables.SetParent(newVariableDeclaration);
            return newVariableDeclaration;
        }
    }

    public abstract class DStatementSyntax : DSyntaxNode
    {
        protected override abstract List<IDSyntax> Children { get; }
    }

    public class DExpressionStatementSyntax : DStatementSyntax
    {
        public DExpressionSyntax Expression { get; }
        public DSyntaxToken SemicolonToken { get; set; }
        protected override List<IDSyntax> Children => new List<IDSyntax> { Expression, SemicolonToken };

        public DExpressionStatementSyntax(DExpressionSyntax expression)
        {
            if (expression == null) throw new ArgumentNullException(nameof(expression));

            Expression = expression;
            SemicolonToken = new DSyntaxToken(DSyntaxKind.SemicolonToken) { Parent = this };
            SyntaxKind = DSyntaxKind.ExpressionStatement;
        }

        public DExpressionStatementSyntax WithSemicolonToken(DSyntaxToken token)
        {
            var newExpressionStatement = new DExpressionStatementSyntax(Expression);
            Expression.Parent = newExpressionStatement;
            token.Parent = newExpressionStatement;
            newExpressionStatement.SemicolonToken = token;
            return newExpressionStatement;
        }
    }

    public abstract class DExpressionSyntax : DSyntaxNode
    {
        protected override abstract List<IDSyntax> Children { get; }
    }

    public class DBinaryExpressionSyntax : DExpressionSyntax
    {
        public DExpressionSyntax Left { get; set; }
        public DSyntaxToken OperatorToken { get; set; }
        public DExpressionSyntax Right { get; set; }
        protected override List<IDSyntax> Children => new List<IDSyntax> { Left, OperatorToken, Right };
        //public override int Width => ChildNodesAndTokens().Sum(i => i.FullSpan.Length);

        public DBinaryExpressionSyntax(DSyntaxKind syntaxKind, DExpressionSyntax left, DExpressionSyntax right)
        {
            SyntaxKind = syntaxKind;
            Left = left;
            Right = right;
        }

        public DBinaryExpressionSyntax WithOperatorToken(DSyntaxToken token)
        {
            var newBinaryExpression = new DBinaryExpressionSyntax(SyntaxKind, Left, Right)
            {
                OperatorToken = token
            };
            token.Parent = newBinaryExpression;
            Left.Parent = newBinaryExpression;
            Right.Parent = newBinaryExpression;
            newBinaryExpression.Parent = Parent;
            return newBinaryExpression;
        }
    }

    public class DReturnStatementSyntax : DStatementSyntax
    {
        public DExpressionSyntax Expression { get; set; }
        public DSyntaxToken ReturnKeyword { get; set; }
        public DSyntaxToken SemicolonToken { get; set; }
        protected override List<IDSyntax> Children => new List<IDSyntax> { ReturnKeyword, Expression, SemicolonToken };
        //public override Span Span
        //{
        //    get
        //    {
        //        var firstToken = ChildTokens().FirstOrDefault();
        //        var leadingTriviaLength = firstToken?.LeadingTrivia.Sum(t => t.FullSpan.Length) ?? 0;
        //        return new Span(Position + leadingTriviaLength, Width - leadingTriviaLength);
        //    }
        //}

        public DReturnStatementSyntax(DExpressionSyntax expression)
        {
            Expression = expression;
            ReturnKeyword = new DSyntaxToken(DSyntaxKind.ReturnKeyword) { Parent = this };
            SemicolonToken = new DSyntaxToken(DSyntaxKind.SemicolonToken) { Parent = this };
            SyntaxKind = DSyntaxKind.ReturnStatement;
        }

        public DReturnStatementSyntax WithReturnKeyword(DSyntaxToken returnKeyword)
        {
            var newReturnStatement = new DReturnStatementSyntax(Expression)
            {
                ReturnKeyword = returnKeyword,
                SemicolonToken = SemicolonToken
            };
            Expression.Parent = newReturnStatement;
            returnKeyword.Parent = newReturnStatement;
            SemicolonToken.Parent = newReturnStatement;
            newReturnStatement.Parent = Parent;
            return newReturnStatement;
        }

        public DReturnStatementSyntax WithSemicolonToken(DSyntaxToken semicolan)
        {
            var newReturnStatement = new DReturnStatementSyntax(Expression)
            {
                ReturnKeyword = ReturnKeyword,
                SemicolonToken = semicolan
            };
            Expression.Parent = newReturnStatement;
            semicolan.Parent = newReturnStatement;
            ReturnKeyword.Parent = newReturnStatement;
            newReturnStatement.Parent = Parent;
            return newReturnStatement;
        }
    }

    public class DMemberAccessExpression : DExpressionSyntax
    {
        public DExpressionSyntax Expression { get; }
        public DIdentifierNameSyntax Name { get; set; }
        public DSyntaxToken OperatorToken { get; set; }
        protected override List<IDSyntax> Children => new List<IDSyntax> { Expression, OperatorToken, Name };


        public DMemberAccessExpression(DSyntaxKind syntaxKind, DExpressionSyntax expression, DIdentifierNameSyntax name)
        {
            SyntaxKind = syntaxKind;
            Expression = expression;
            Name = name;
            OperatorToken = new DSyntaxToken(DSyntaxKind.DotToken) { Parent = this };
        }

        public DMemberAccessExpression WithOperator(DSyntaxToken operatorToken)
        {
            var newExpression = Clone();
            newExpression.OperatorToken = operatorToken;
            operatorToken.Parent = newExpression;
            return newExpression;
        }

        private DMemberAccessExpression Clone()
        {
            var newExpression = new DMemberAccessExpression(SyntaxKind, Expression, Name);
            newExpression.OperatorToken = OperatorToken;

            Expression.Parent = newExpression;
            Name.Parent = newExpression;
            OperatorToken.Parent = newExpression;
            newExpression.Parent = Parent;

            return newExpression;
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

    public class DInvocationExpressionSyntax : DExpressionSyntax
    {
        public DArgumentListSyntax ArgumentList { get; set; }
        public DExpressionSyntax Expression { get; }
        protected override List<IDSyntax> Children => new List<IDSyntax> { Expression, ArgumentList };

        public DInvocationExpressionSyntax(DExpressionSyntax expression)
        {
            ArgumentList = DSyntaxFactory.ArgumentList();
            Expression = expression;
            SyntaxKind = DSyntaxKind.InvocationExpression;
        }

        public DInvocationExpressionSyntax WithArgumentList(DArgumentListSyntax argumentList)
        {
            var newInvocationExpression = new DInvocationExpressionSyntax(Expression);
            Expression.Parent = newInvocationExpression;
            argumentList.Parent = newInvocationExpression;
            newInvocationExpression.ArgumentList = argumentList;
            return newInvocationExpression;
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
        //public override int Width => ChildNodesAndTokens().Sum(i => i.Span.Length);

        public DIdentifierNameSyntax(DSyntaxToken identifierToken)
        {
            Identifier = identifierToken;
            SyntaxKind = DSyntaxKind.IdentifierName;
        }
    }

    public class DBlockSyntax : DStatementSyntax
    {
        public DSyntaxToken OpenBraceToken { get; set; }
        public DSyntaxToken CloseBraceToken { get; set; }
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

        public DBlockSyntax WithOpenBraceToken(DSyntaxToken openBraceToken)
        {
            var newBlock = Clone();

            newBlock.OpenBraceToken = openBraceToken;
            if (openBraceToken != null)
                openBraceToken.Parent = newBlock;

            return newBlock;
        }

        public DBlockSyntax WithCloseBraceToken(DSyntaxToken closeBraceToken)
        {
            var newBlock = Clone();

            newBlock.CloseBraceToken = closeBraceToken;
            if (closeBraceToken != null)
                closeBraceToken.Parent = newBlock;

            return newBlock;
        }

        public DBlockSyntax WithStatements(DSyntaxList<DStatementSyntax> statements)
        {
            var newBlock = Clone();

            statements.SetParent(newBlock);

            newBlock.Statements = statements;

            return newBlock;
        }

        private DBlockSyntax Clone()
        {
            var newBlock = new DBlockSyntax(Statements);

            Statements.SetParent(newBlock);
            OpenBraceToken.Parent = newBlock;
            CloseBraceToken.Parent = newBlock;
            newBlock.Parent = Parent;

            newBlock.OpenBraceToken = OpenBraceToken;
            newBlock.CloseBraceToken = CloseBraceToken;

            return newBlock;
        }
    }

    public class DMethodDeclarationSyntax : DMemberDeclarationSyntax
    {
        public DSyntaxTokenList Modifiers { get; set; }
        public DTypeSyntax ReturnType { get; set; }
        public DSyntaxToken Identifier { get; set; }
        public DParameterListSyntax ParameterList { get; set; }
        public DBlockSyntax Body { get; set; }
        public DSyntaxToken SemicolonToken { get; set; }
        protected override List<IDSyntax> Children => new List<IDSyntax>(Modifiers) { ReturnType, Identifier, ParameterList, Body, SemicolonToken }.Where(i => i != null).ToList();

        public DMethodDeclarationSyntax(DTypeSyntax returnType, DSyntaxToken identifierToken)
        {
            returnType.Parent = this;
            ReturnType = returnType;
            identifierToken.Parent = this;
            Identifier = identifierToken;
            ParameterList = new DParameterListSyntax { Parent = this };
            Modifiers = new DSyntaxTokenList();
            SyntaxKind = DSyntaxKind.MethodDeclaration;
        }

        public DMethodDeclarationSyntax WithModifiers(DSyntaxTokenList modifiers)
        {
            var newMethodDeclaration = Clone();
            modifiers.SetParent(newMethodDeclaration);
            newMethodDeclaration.Modifiers = modifiers;
            return newMethodDeclaration;
        }

        public DMethodDeclarationSyntax WithBody(DBlockSyntax body)
        {
            if (body == null) return this;

            var newMethodDeclaration = Clone();
            newMethodDeclaration.Body = body;
            body.Parent = newMethodDeclaration;
            return newMethodDeclaration;
        }

        public DMethodDeclarationSyntax WithSemicolonToken(DSyntaxToken semicolonToken)
        {
            var newMethodDeclaration = Clone();
            newMethodDeclaration.SemicolonToken = semicolonToken;
            if (semicolonToken != null)
                semicolonToken.Parent = newMethodDeclaration;
            return newMethodDeclaration;
        }

        public DMethodDeclarationSyntax WithParameterList(DParameterListSyntax parameterList)
        {
            var newMethodDeclaration = Clone();
            parameterList.Parent = newMethodDeclaration;
            newMethodDeclaration.ParameterList = parameterList;
            return newMethodDeclaration;
        }

        private DMethodDeclarationSyntax Clone()
        {
            var newMethodDeclaration = new DMethodDeclarationSyntax(ReturnType, Identifier);

            Modifiers.SetParent(newMethodDeclaration);
            ReturnType.Parent = newMethodDeclaration;
            Identifier.Parent = newMethodDeclaration;
            ParameterList.Parent = newMethodDeclaration;
            if (Body != null)
                Body.Parent = newMethodDeclaration;

            newMethodDeclaration.Modifiers = Modifiers;
            newMethodDeclaration.ParameterList = ParameterList;
            newMethodDeclaration.Body = Body;
            newMethodDeclaration.Parent = Parent;

            return newMethodDeclaration;
        }
    }

    public class DCompilationUnitSyntax : DSyntaxNode
    {
        public DSyntaxList<DMemberDeclarationSyntax> Members { get; set; } = DSyntaxFactory.List<DMemberDeclarationSyntax>();
        public DSyntaxToken EndOfFileToken { get; set; }
        protected override List<IDSyntax> Children => new List<IDSyntax>(Members) { EndOfFileToken };

        public DCompilationUnitSyntax()
        {
            EndOfFileToken = new DSyntaxToken(DSyntaxKind.EndOfFileToken)
            {
                Parent = this
            };
            SyntaxKind = DSyntaxKind.CompilationUnit;
        }

        public DCompilationUnitSyntax WithMembers(DSyntaxList<DMemberDeclarationSyntax> members)
        {
            var newCompilation = Clone();
            members.SetParent(newCompilation);
            newCompilation.Members = members;
            return newCompilation;
        }

        private DCompilationUnitSyntax Clone()
        {
            var newCompilation = new DCompilationUnitSyntax();
            Members.SetParent(newCompilation);
            newCompilation.Members = Members;
            return newCompilation;
        }
    }

    public class DFieldDeclarationSytnax : DMemberDeclarationSyntax
    {
        public DVariableDeclarationSyntax Declaration { get; set; }
        public DSyntaxToken SemicolonToken { get; set; }
        protected override List<IDSyntax> Children => new List<IDSyntax> { Declaration, SemicolonToken };

        public DFieldDeclarationSytnax(DVariableDeclarationSyntax declaration)
        {
            Declaration = declaration;
            SemicolonToken = new DSyntaxToken(DSyntaxKind.SemicolonToken) { Parent = this };
            SyntaxKind = DSyntaxKind.FieldDeclaration;
        }

        public DFieldDeclarationSytnax WithSemicolonToken(DSyntaxToken token)
        {
            var newField = Clone();
            token.Parent = newField;
            newField.SemicolonToken = token;
            return newField;
        }

        private DFieldDeclarationSytnax Clone()
        {
            var newFieldSyntax = new DFieldDeclarationSytnax(Declaration);

            Declaration.Parent = newFieldSyntax;
            SemicolonToken.Parent = newFieldSyntax;

            newFieldSyntax.SemicolonToken = SemicolonToken;

            return newFieldSyntax;
        }
    }

    public abstract class DTypeDeclarationSyntax : DMemberDeclarationSyntax
    {

    }

    public sealed class DClassDeclarationSyntax : DTypeDeclarationSyntax
    {
        public DSyntaxToken Keyword { get; set; }
        public DSyntaxToken Identifier { get; }
        public DSyntaxToken OpenBraceToken { get; private set; }
        public DSyntaxList<DMemberDeclarationSyntax> Members { get; set; }
        public DSyntaxToken CloseBraceToken { get; private set; }
        protected override List<IDSyntax> Children
        {
            get
            {
                var list = new List<IDSyntax> { Keyword, Identifier, OpenBraceToken, CloseBraceToken };
                list.InsertRange(3, Members.OfType<IDSyntax>());
                return list;
            }
        }

        public DClassDeclarationSyntax(DSyntaxToken identifierToken)
        {
            Identifier = identifierToken;
            Keyword = new DSyntaxToken(DSyntaxKind.ClassKeyword) { Parent = this };
            OpenBraceToken = new DSyntaxToken(DSyntaxKind.OpenBraceToken) { Parent = this };
            CloseBraceToken = new DSyntaxToken(DSyntaxKind.CloseBraceToken) { Parent = this };
            SyntaxKind = DSyntaxKind.ClassDeclaration;
            Members = DSyntaxFactory.List<DMemberDeclarationSyntax>();
        }

        public DClassDeclarationSyntax WithModifiers()
        {
            throw new NotImplementedException();
        }

        public DClassDeclarationSyntax WithMembers(IEnumerable<DMemberDeclarationSyntax> members)
        {
            var newClassDeclaration = Clone();
            var memberSyntaxList = new DSyntaxList<DMemberDeclarationSyntax>(members);
            memberSyntaxList.SetParent(newClassDeclaration);
            newClassDeclaration.Members = memberSyntaxList;

            return newClassDeclaration;
        }

        public DClassDeclarationSyntax WithKeyword(DSyntaxToken dSyntaxToken)
        {
            var newClassDeclaration = Clone();
            dSyntaxToken.Parent = newClassDeclaration;
            newClassDeclaration.Keyword = dSyntaxToken;
            return newClassDeclaration;
        }

        public DClassDeclarationSyntax WithOpenBraceToken(DSyntaxToken dSyntaxToken)
        {
            var newClassDeclaration = Clone();
            dSyntaxToken.Parent = newClassDeclaration;
            newClassDeclaration.OpenBraceToken = dSyntaxToken;
            return newClassDeclaration;
        }

        public DClassDeclarationSyntax WithCloseBraceToken(DSyntaxToken dSyntaxToken)
        {
            var newClassDeclaration = Clone();
            dSyntaxToken.Parent = newClassDeclaration;
            newClassDeclaration.CloseBraceToken = dSyntaxToken;
            return newClassDeclaration;
        }

        private DClassDeclarationSyntax Clone()
        {
            var newClassDeclaration = new DClassDeclarationSyntax(Identifier);

            Members.SetParent(newClassDeclaration);
            Identifier.Parent = newClassDeclaration;
            Keyword.Parent = newClassDeclaration;
            OpenBraceToken.Parent = newClassDeclaration;
            CloseBraceToken.Parent = newClassDeclaration;

            newClassDeclaration.Keyword = Keyword;
            newClassDeclaration.OpenBraceToken = OpenBraceToken;
            newClassDeclaration.CloseBraceToken = CloseBraceToken;
            newClassDeclaration.Members = Members;

            return newClassDeclaration;
        }
    }
}