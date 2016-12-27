using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace DSharpCodeAnalysis.Syntax
{
    public static class DSyntaxFactory
    {
        public static DTrivia CarriageReturnLineFeed => EndOfLine("\r\n");
        public static DTrivia LineFeed => EndOfLine("\n");
        public static DTrivia Space => Whitespace(" ");
        public static DTrivia Tab => Whitespace("    ");

        public static DCompilationUnitSyntax CompilationUnit()
        {
            var newCompilation = new DCompilationUnitSyntax();
            return newCompilation;
        }

        public static DClassDeclarationSyntax ClassDeclaration(DSyntaxToken identifierToken)
        {
            var newClass = new DClassDeclarationSyntax(identifierToken);
            identifierToken.Parent = newClass;
            return newClass;
        }

        public static DClassDeclarationSyntax ClassDeclaration(string identifier) => ClassDeclaration(Identifier(identifier));

        public static DClassDeclarationSyntax ClassDeclaration(DSyntaxToken typeToken, DSyntaxToken identifierToken, DSyntaxToken openBrace, DSyntaxToken closeBrace)
        {
            var newClass = ClassDeclaration(identifierToken).WithKeyword(typeToken).WithOpenBraceToken(openBrace).WithCloseBraceToken(closeBrace);
            identifierToken.Parent = newClass;
            openBrace.Parent = newClass;
            closeBrace.Parent = newClass;
            typeToken.Parent = newClass;
            return newClass;
        }

        public static DMethodDeclarationSyntax MethodDeclaration(DTypeSyntax returnType, DSyntaxToken identifierToken)
        {
            return new DMethodDeclarationSyntax(returnType, identifierToken);
        }

        public static DSyntaxToken Identifier(string identifier)
        {
            var token = new DSyntaxToken(DSyntaxKind.IdentifierToken)
            {
                Value = identifier
            };
            return token;
        }

        public static DSyntaxToken Identifier(IEnumerable<DTrivia> leading, string identifier, IEnumerable<DTrivia> trailing)
        {
            return Identifier(identifier)
                .WithLeadingTrivia(ImmutableList.CreateRange(leading))
                .WithTrailingTrivia(ImmutableList.CreateRange(trailing));
        }

        public static DSyntaxToken Token(DSyntaxKind syntaxKind)
        {
            return new DSyntaxToken(syntaxKind);
        }

        public static DSyntaxToken Token(DSyntaxKind syntaxKind, object value)
        {
            return new DSyntaxToken(syntaxKind, value);
        }

        public static DSyntaxToken Token(IEnumerable<DTrivia> leading, DSyntaxKind syntaxKind, IEnumerable<DTrivia> trailing)
        {
            return Token(syntaxKind)
                .WithLeadingTrivia(ImmutableList.CreateRange(leading))
                .WithTrailingTrivia(ImmutableList.CreateRange(trailing));
        }

        public static DPredefinedTypeSyntax PredefinedType(DSyntaxToken keyword)
        {
            return new DPredefinedTypeSyntax(keyword);
        }

        public static DBlockSyntax Block()
        {
            return new DBlockSyntax();
        }

        public static DBlockSyntax Block(DSyntaxList<DStatementSyntax> statements)
        {
            return new DBlockSyntax(statements);
        }

        public static DBlockSyntax Block(params DStatementSyntax[] statements)
        {
            var list = List(statements);
            return new DBlockSyntax(list);
        }

        public static DTrivia SyntaxTrivia(DSyntaxKind syntaxKind, string triviaText)
        {
            return new DTrivia(syntaxKind, triviaText);
        }

        public static DSyntaxList<T> List<T>() where T : DSyntaxNode
        {
            return new DSyntaxList<T>(new List<T>());
        }

        public static DSyntaxTokenList TokenList(params DSyntaxToken[] tokens)
        {
            return new DSyntaxTokenList(tokens);
        }

        public static DSyntaxList<T> List<T>(IEnumerable<T> list) where T : DSyntaxNode
        {
            return new DSyntaxList<T>(list.ToList());
        }

        public static DSyntaxList<T> SingletonList<T>() where T : DSyntaxNode
        {
            return new DSyntaxList<T>(new List<T>());
        }

        public static DSeparatedSyntaxList<T> SeparatedList<T>() where T : DSyntaxNode
        {
            var nodes = Enumerable.Empty<T>();
            var seperators = Enumerable.Empty<DSyntaxToken>();
            return new DSeparatedSyntaxList<T>(nodes, seperators);
        }

        public static DSeparatedSyntaxList<T> SeparatedList<T>(IEnumerable<IDSyntax> nodesOrTokens) where T : DSyntaxNode
        {
            var nodes = nodesOrTokens.OfType<T>();
            var seperators = nodesOrTokens.OfType<DSyntaxToken>();
            return new DSeparatedSyntaxList<T>(nodes, seperators);
        }

        public static DSeparatedSyntaxList<T> SeparatedList<T>(IEnumerable<T> nodes, IEnumerable<DSyntaxToken> seperators) where T : DSyntaxNode
        {
            return new DSeparatedSyntaxList<T>(nodes, seperators);
        }

        public static DSyntaxList<T> SingletonList<T>(T item) where T : DSyntaxNode
        {
            if (item == null) throw new ArgumentNullException(nameof(item));

            return new DSyntaxList<T>(new List<T> { item });
        }

        public static DSeparatedSyntaxList<T> SingletonSeparatedList<T>(T item) where T : DSyntaxNode
        {
            return new DSeparatedSyntaxList<T>(new List<T> { item }, Enumerable.Empty<DSyntaxToken>());
        }

        public static string SyntaxString(DSyntaxKind syntaxKind)
        {
            var cache = new DSyntaxCache();
            return cache.Get(syntaxKind);
        }

        public static DParameterSyntax Parameter(DSyntaxToken identifier)
        {
            var parameter = new DParameterSyntax(identifier);
            identifier.Parent = parameter;
            return parameter;
        }

        public static DParameterListSyntax ParameterList()
        {
            return new DParameterListSyntax();
        }

        public static DParameterListSyntax ParameterList(DSeparatedSyntaxList<DParameterSyntax> parameters)
        {
            var parameterList = new DParameterListSyntax(parameters);
            foreach (var parameter in parameters.GetNodesAndSeperators())
            {
                parameter.Parent = parameterList;
            }
            return parameterList;
        }

        public static DArgumentListSyntax ArgumentList()
        {
            return new DArgumentListSyntax();
        }

        public static DArgumentListSyntax ArgumentList(DSeparatedSyntaxList<DArgumentSyntax> arguments)
        {
            var argumentList = new DArgumentListSyntax(arguments);
            arguments.SetParent(argumentList);
            return argumentList;
        }

        public static DSyntaxTriviaList TriviaList()
        {
            return TriviaList(Enumerable.Empty<DTrivia>());
        }

        public static DSyntaxTriviaList TriviaList(DTrivia trivia)
        {
            return TriviaList(new List<DTrivia> { trivia });
        }

        public static DSyntaxTriviaList TriviaList(params DTrivia[] trivia)
        {
            return TriviaList(trivia.AsEnumerable());
        }

        public static DSyntaxTriviaList TriviaList(IEnumerable<DTrivia> trivia)
        {
            return new DSyntaxTriviaList(trivia);
        }

        public static DTrivia Whitespace(string text, int position = 0)
        {
            return DTrivia.Create(DSyntaxKind.WhitespaceTrivia, text, position);
        }

        public static DTrivia EndOfLine(string text, int position = 0)
        {
            return DTrivia.Create(DSyntaxKind.EndOfLineTrivia, text, position);
        }

        public static DLocalDeclarationStatementSyntax LocalDeclarationStatement(DVariableDeclarationSyntax variableSyntax)
        {
            return new DLocalDeclarationStatementSyntax(variableSyntax);
        }

        public static DReturnStatementSyntax ReturnStatement(DExpressionSyntax expression)
        {
            return new DReturnStatementSyntax(expression);
        }

        public static DBinaryExpressionSyntax BinaryExpression(DSyntaxKind syntaxKind, DExpressionSyntax left, DExpressionSyntax right)
        {
            var binaryExpression = new DBinaryExpressionSyntax(syntaxKind, left, right); ;
            left.Parent = binaryExpression;
            right.Parent = binaryExpression;
            return binaryExpression;
        }

        public static DFieldDeclarationSytnax FieldDeclaration(DVariableDeclarationSyntax declaration)
        {
            return new DFieldDeclarationSytnax(declaration);
        }

        public static DVariableDeclarationSyntax VariableDeclaration(DTypeSyntax typeSyntax)
        {
            return new DVariableDeclarationSyntax(typeSyntax);
        }

        public static DIdentifierNameSyntax IdentifierName(DSyntaxToken identifierToken)
        {
            var identifierNameSyntax = new DIdentifierNameSyntax(identifierToken);
            identifierToken.Parent = identifierNameSyntax;
            return identifierNameSyntax;
        }

        public static DQualifiedNameSyntax QualifiedName(DNameSyntax left, DSyntaxToken seperator, DIdentifierNameSyntax right)
        {
            var qualifiedName = new DQualifiedNameSyntax(left, seperator, right);

            left.Parent = qualifiedName;
            seperator.Parent = qualifiedName;
            right.Parent = qualifiedName;

            return qualifiedName;
        }

        public static object QualifiedName(DMemberAccessExpression originalExpression)
        {
            var x = originalExpression.Expression as DMemberAccessExpression;
            if (x == null)
            {
                var temp = originalExpression.Expression as DIdentifierNameSyntax;
                return temp;
            }
            else
            {
                return QualifiedName(x);
            }
        }

        public static DIdentifierNameSyntax IdentifierName(string identifier)
        {
            var identifierToken = Identifier(identifier);
            return IdentifierName(identifierToken);
        }

        public static DVariableDeclaratorSyntax VariableDeclarator(DSyntaxToken identifierToken)
        {
            var variableDeclarator = new DVariableDeclaratorSyntax(identifierToken);
            identifierToken.Parent = variableDeclarator;
            return variableDeclarator;
        }

        public static DEqualsValueClauseSyntax EqualsValueClause(DExpressionSyntax expressionSyntax)
        {
            return new DEqualsValueClauseSyntax(expressionSyntax);
        }

        public static DLiteralExpressionSyntax LiteralExpression(DSyntaxKind syntaxKind, DSyntaxToken syntaxToken)
        {
            return new DLiteralExpressionSyntax(syntaxKind, syntaxToken);
        }

        public static DInvocationExpressionSyntax InvocationExpression(DExpressionSyntax expression)
        {
            var invocationExpression = new DInvocationExpressionSyntax(expression);
            expression.Parent = invocationExpression;
            return invocationExpression;
        }

        public static DMemberAccessExpression MemberAccessExpression(DSyntaxKind syntaxKind, DExpressionSyntax expression, DIdentifierNameSyntax name)
        {
            var invocationExpression = new DMemberAccessExpression(syntaxKind, expression, name);
            expression.Parent = invocationExpression;
            name.Parent = invocationExpression;
            return invocationExpression;
        }

        public static DSyntaxToken Literal(int value)
        {
            var token = new DSyntaxToken(DSyntaxKind.NumericLiteralToken, value);
            return token;
        }

        public static DSyntaxToken Literal(IEnumerable<DTrivia> leading, int value, IEnumerable<DTrivia> trailing)
        {
            var token = new DSyntaxToken(DSyntaxKind.NumericLiteralToken, value);
            token = token.WithLeadingTrivia(leading);
            token = token.WithTrailingTrivia(trailing);
            return token;
        }

        public static DArgumentSyntax Argument(DExpressionSyntax expression)
        {
            var argument = new DArgumentSyntax(expression);
            expression.Parent = argument;
            return argument;
        }

        public static DExpressionStatementSyntax ExpressionStatement(DExpressionSyntax expression)
        {
            var expressionStatementSyntax = new DExpressionStatementSyntax(expression);
            expression.Parent = expressionStatementSyntax;
            return expressionStatementSyntax;
        }

        public static DGlobalStatementSyntax GlobalStatement(DStatementSyntax dStatementSyntax)
        {
            var globalStatement = new DGlobalStatementSyntax(dStatementSyntax);
            dStatementSyntax.Parent = globalStatement;
            return globalStatement;
        }

        public static DObjectCreationExpressionSyntax ObjectCreationExpression(DSyntaxToken newKeyword, DTypeSyntax type, DArgumentListSyntax argumentList)
        {
            var newExpression = new DObjectCreationExpressionSyntax(newKeyword, type, argumentList);

            newExpression.NewKeyword.Parent = newExpression;
            newExpression.Type.Parent = newExpression;
            newExpression.ArgumentList.Parent = newExpression;

            return newExpression;
        }
    }
}