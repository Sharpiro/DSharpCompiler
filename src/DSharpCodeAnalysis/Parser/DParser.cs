﻿using DSharpCodeAnalysis.Exceptions;
using DSharpCodeAnalysis.Syntax;
using System;
using System.Collections.Generic;

namespace DSharpCodeAnalysis.Parser
{
    public class DParser
    {
        private List<DSyntaxToken> _lexedTokens;
        private int _tokenOffset;
        private readonly string _source;

        private DSyntaxToken currentToken
        {
            get
            {
                return _lexedTokens[_tokenOffset];
            }
        }

        public DParser(string source, List<DSyntaxToken> lexedTokens)
        {
            if (string.IsNullOrEmpty(source)) throw new ArgumentNullException(nameof(source));
            if (lexedTokens.IsNullOrEmpty()) throw new ArgumentNullException(nameof(lexedTokens));

            _source = source;
            _lexedTokens = lexedTokens;
        }

        public DCompilationUnitSyntax ParseCompilationUnit()
        {
            var compilation = DSyntaxFactory.CompilationUnit();

            var members = ParseMembers();
            compilation = compilation.WithMembers(members);

            if (_tokenOffset != _lexedTokens.Count) throw new TokenException("Compilation finished with un-parsed tokens");

            return compilation;
        }

        private DSyntaxList<DMemberDeclarationSyntax> ParseMembers()
        {
            var members = new List<DMemberDeclarationSyntax>();
            while (_tokenOffset < _lexedTokens.Count &&
                currentToken.SyntaxKind != DSyntaxKind.CloseBraceToken)
            {
                members.Add(ParseMemberDeclarationOrStatement());
            }

            return DSyntaxFactory.List(members);
        }

        private DMemberDeclarationSyntax ParseMemberDeclarationOrStatement()
        {
            if (currentToken.SyntaxKind == DSyntaxKind.ClassKeyword)
                return ParseTypeDeclaration();
            var statement = ParseStatementNoDeclaration();
            if (statement != null) return DSyntaxFactory.GlobalStatement(statement);

            if (IsFieldDeclaration())
                return ParseNormalFieldDeclaration();
            return ParseMethodDeclaration();
        }

        private DFieldDeclarationSytnax ParseNormalFieldDeclaration()
        {
            var returnType = ParseType();
            var variable = ParseVariableDeclarator();
            var semicolonToken = EatToken(DSyntaxKind.SemicolonToken);
            var field = DSyntaxFactory.FieldDeclaration(DSyntaxFactory.VariableDeclaration(returnType)
                .WithVariables(DSyntaxFactory.SingletonSeparatedList(variable))).WithSemicolonToken(semicolonToken);
            return field;
        }

        private DStatementSyntax ParseLocalDeclarationStatement()
        {
            var type = ParseType();
            var variable = ParseVariableDeclarator();
            var declaration = DSyntaxFactory.VariableDeclaration(type).WithVariables(DSyntaxFactory.SingletonSeparatedList(variable));
            var semicolon = EatToken(DSyntaxKind.SemicolonToken);
            var statement = DSyntaxFactory.LocalDeclarationStatement(declaration).WithSemicolonToken(semicolon);
            return statement;
        }

        private DVariableDeclaratorSyntax ParseVariableDeclarator()
        {
            var identifier = ParseIdentifierToken();
            switch (currentToken.SyntaxKind)
            {
                case DSyntaxKind.EqualsToken:
                    var equalsToken = EatToken(DSyntaxKind.EqualsToken);
                    var expression = ParseExpression();
                    var equalsValueClause = DSyntaxFactory.EqualsValueClause(expression).WithEqualsToken(equalsToken);
                    return DSyntaxFactory.VariableDeclarator(identifier).WithInitializer(equalsValueClause);
                default:
                    throw new ArgumentException($"Invalid switch in 'ParseVariableDeclarator': {currentToken.SyntaxKind}");
            }
        }

        private bool IsFieldDeclaration()
        {
            if (currentToken.SyntaxKind != DSyntaxKind.IdentifierToken)
                return false;

            var nextToken = PeekToken();
            switch (nextToken.SyntaxKind)
            {
                case DSyntaxKind.OpenParenToken:
                    return false;
                default: return true;
            }
        }

        private DTypeSyntax ParseType()
        {
            if (DSyntaxCache.IsPredefinedType(currentToken.SyntaxKind))
                return DSyntaxFactory.PredefinedType(EatToken());
            return ParseQualifiedName();
        }

        private DNameSyntax ParseQualifiedName()
        {
            DNameSyntax name = ParseIdentifierName();

            top:
            switch (currentToken.SyntaxKind)
            {
                case DSyntaxKind.DotToken:
                    var seperator = EatToken(DSyntaxKind.DotToken);
                    name = ParseQualifiedNameRight(name, seperator);
                    goto top;
            }
            return name;
        }

        private DQualifiedNameSyntax ParseQualifiedNameRight(DNameSyntax left, DSyntaxToken seperator)
        {
            var right = ParseIdentifierName();

            return DSyntaxFactory.QualifiedName(left, seperator, right);
        }

        private DIdentifierNameSyntax ParseIdentifierName()
        {
            var typeToken = EatToken(DSyntaxKind.IdentifierToken);
            return DSyntaxFactory.IdentifierName(typeToken);
        }

        private DMethodDeclarationSyntax ParseMethodDeclaration()
        {
            var modifiers = ParseModifiers();
            var functionKeyword = EatToken(DSyntaxKind.FunctionKeyword);
            var returnType = ParseType();
            var identifier = ParseIdentifierToken();
            var parameterList = ParseParenthesizedParameterList();
            DBlockSyntax body;
            DSyntaxToken semicolonToken;

            ParseBlockAndExpressionBodiesWithSemicolon(out body, out semicolonToken);

            var method = DSyntaxFactory.MethodDeclaration(returnType, identifier)
                .WithParameterList(parameterList).WithBody(body).WithSemicolonToken(semicolonToken)
                .WithFunctionKeyword(functionKeyword).WithModifiers(modifiers);
            return method;
        }

        private DSyntaxTokenList ParseModifiers()
        {
            var modifiers = new List<DSyntaxToken>();

            top:
            switch (currentToken.SyntaxKind)
            {
                case DSyntaxKind.PublicKeyword:
                case DSyntaxKind.PrivateKeyword:
                    modifiers.Add(EatToken());
                    goto top;
                default: break;
            }

            return DSyntaxFactory.TokenList(modifiers.ToArray());
        }

        private void ParseBlockAndExpressionBodiesWithSemicolon(out DBlockSyntax blockBody, out DSyntaxToken semicolonToken)
        {
            blockBody = null;
            semicolonToken = null;

            if (currentToken.SyntaxKind == DSyntaxKind.SemicolonToken)
                semicolonToken = EatToken(DSyntaxKind.SemicolonToken);

            else if (currentToken.SyntaxKind == DSyntaxKind.OpenBraceToken)
                blockBody = ParseBlock();
        }

        private DBlockSyntax ParseBlock()
        {
            var openBrace = EatToken(DSyntaxKind.OpenBraceToken);
            var statements = ParseStatements();
            var closeBrace = EatToken(DSyntaxKind.CloseBraceToken);

            var block = DSyntaxFactory.Block().WithOpenBraceToken(openBrace)
                .WithStatements(statements).WithCloseBraceToken(closeBrace);

            return block;
        }

        private DSyntaxList<DStatementSyntax> ParseStatements()
        {
            var statements = DSyntaxFactory.SingletonList<DStatementSyntax>();
            while (currentToken.SyntaxKind != DSyntaxKind.CloseBraceToken)
            {
                var statement = ParseStatement();
                statements.Add(statement);
            }
            return statements;
        }

        private DStatementSyntax ParseStatement()
        {
            var statement = ParseStatementNoDeclaration();
            if (statement != null) return statement;

            statement = ParseLocalDeclarationStatement();
            return statement;
        }

        private DStatementSyntax ParseStatementNoDeclaration()
        {
            switch (currentToken.SyntaxKind)
            {
                case DSyntaxKind.IdentifierToken:
                    return IsPossibleLocalDeclarationStatement() ? null : ParseExpressionStatement();
                case DSyntaxKind.ReturnKeyword:
                    return ParseReturnStatement();
                default: return null;
            }
        }

        private DExpressionStatementSyntax ParseExpressionStatement()
        {
            var expression = ParseExpression();
            var semicolon = EatToken(DSyntaxKind.SemicolonToken);
            return DSyntaxFactory.ExpressionStatement(expression).WithSemicolonToken(semicolon);
        }

        private bool IsPossibleLocalDeclarationStatement()
        {
            var current = currentToken;
            var next = PeekToken();
            return IsPossibleTypedIdentifierStart(current, next);
        }

        private static bool IsPossibleTypedIdentifierStart(DSyntaxToken current, DSyntaxToken next)
        {
            if (current.SyntaxKind != DSyntaxKind.IdentifierToken) return false;

            if (next.SyntaxKind != DSyntaxKind.IdentifierToken) return false;

            return true;
        }

        private DStatementSyntax ParseReturnStatement()
        {
            var returnToken = EatToken(DSyntaxKind.ReturnKeyword);
            var expression = ParseExpression();
            var semicolonToken = EatToken(DSyntaxKind.SemicolonToken);
            return DSyntaxFactory.ReturnStatement(expression).WithReturnKeyword(returnToken)
                .WithSemicolonToken(semicolonToken);
        }

        private DExpressionSyntax ParseExpression()
        {
            var leftOperand = ParseTerm();
            DSyntaxKind opKind = GetBinaryExpression(currentToken.SyntaxKind);
            if (opKind != DSyntaxKind.Null)
            {
                var operatorToken = EatToken();
                leftOperand = DSyntaxFactory.BinaryExpression(opKind, leftOperand, ParseExpression())
                    .WithOperatorToken(operatorToken);
            }

            return leftOperand;
        }

        private DSyntaxKind GetBinaryExpression(DSyntaxKind token)
        {
            switch (token)
            {
                case DSyntaxKind.PlusToken:
                    return DSyntaxKind.AddExpression;
                default:
                    return DSyntaxKind.Null;
            }
        }

        private DExpressionSyntax ParseTerm()
        {
            DExpressionSyntax expression = null;
            switch (currentToken.SyntaxKind)
            {
                case DSyntaxKind.IdentifierToken:
                    expression = ParseAliasQualifiedName();
                    break;
                case DSyntaxKind.NumericLiteralToken:
                    expression = DSyntaxFactory.LiteralExpression(DSyntaxCache.GetLiteralExpression(currentToken.SyntaxKind), EatToken());
                    break;
                case DSyntaxKind.NewKeyword:
                    expression = ParseNewExpression();
                    break;
                case DSyntaxKind.OpenParenToken:
                    expression = ParseParenthesizedExpression();
                    break;
                default: throw new ArgumentException("Invalid switch in ParseTerm()");
            }
            return ParsePostFixExpression(expression);
        }

        private DExpressionSyntax ParseParenthesizedExpression()
        {
            var openParenToken = EatToken(DSyntaxKind.OpenParenToken);
            var expression = ParseTerm();
            var closeParenToken = EatToken(DSyntaxKind.CloseParenToken);

            return DSyntaxFactory.ParenthesizedExpression(openParenToken, expression, closeParenToken);
        }

        private DObjectCreationExpressionSyntax ParseNewExpression()
        {
            var newToken = EatToken(DSyntaxKind.NewKeyword);
            var type = ParseType();
            var argumentList = ParseParenthesizedArgumentList();
            return DSyntaxFactory.ObjectCreationExpression(newToken, type, argumentList);
        }

        private DExpressionSyntax ParsePostFixExpression(DExpressionSyntax expression)
        {
            top:
            switch (currentToken.SyntaxKind)
            {
                case DSyntaxKind.OpenParenToken:
                    var arguments = ParseParenthesizedArgumentList();
                    expression = DSyntaxFactory.InvocationExpression(expression).WithArgumentList(arguments);
                    goto top;
                case DSyntaxKind.DotToken:
                    var dotToken = EatToken(DSyntaxKind.DotToken);
                    expression = DSyntaxFactory.MemberAccessExpression(DSyntaxKind.SimpleMemberAccessExpression, expression, ParseIdentifierName())
                        .WithOperator(dotToken);
                    goto top;
                default: return expression;
            }
        }

        private DArgumentListSyntax ParseParenthesizedArgumentList()
        {
            var openToken = EatToken(DSyntaxKind.OpenParenToken);
            var arguments = DSyntaxFactory.SeparatedList<DArgumentSyntax>();
            if (currentToken.SyntaxKind != DSyntaxKind.CloseParenToken)
            {
                arguments.Add(ParseArgumentExpression());
            }
            while (currentToken.SyntaxKind != DSyntaxKind.CloseParenToken)
            {
                arguments.AddSeperator(EatToken(DSyntaxKind.CommaToken));
                arguments.Add(ParseArgumentExpression());
            }
            var closeParenToken = EatToken(DSyntaxKind.CloseParenToken);

            return DSyntaxFactory.ArgumentList(arguments)
                .WithOpenParenToken(openToken).WithCloseParenToken(closeParenToken);
        }

        private DArgumentSyntax ParseArgumentExpression()
        {
            var expression = ParseTerm();
            return DSyntaxFactory.Argument(expression);
        }

        private DTypeSyntax ParseAliasQualifiedName()
        {
            var nameSyntax = ParseIdentifierName();

            return nameSyntax;
        }

        private DParameterListSyntax ParseParenthesizedParameterList()
        {
            var openParenToken = EatToken(DSyntaxKind.OpenParenToken);
            var parameters = DSyntaxFactory.SeparatedList<DParameterSyntax>();
            var hasAnyParameters = PeekToken().SyntaxKind != DSyntaxKind.CloseParenToken;
            if (hasAnyParameters)
            {
                var parameter = ParseParameter();
                parameters.Add(parameter);
            }
            while (hasAnyParameters)
            {
                if (currentToken.SyntaxKind == DSyntaxKind.CloseParenToken)
                    break;
                else if (currentToken.SyntaxKind == DSyntaxKind.CommaToken)
                {
                    var commaToken = EatToken(DSyntaxKind.CommaToken);
                    parameters.AddSeperator(commaToken);
                    var parameter = ParseParameter();
                    parameters.Add(parameter);
                }
            }

            var closeParemToken = EatToken(DSyntaxKind.CloseParenToken);
            var parameterList = DSyntaxFactory.ParameterList(parameters)
                .WithOpenParenToken(openParenToken).WithCloseParenToken(closeParemToken);
            return parameterList;
        }

        private DParameterSyntax ParseParameter()
        {
            var type = ParseType();
            var identifier = ParseIdentifierToken();
            var parameter = DSyntaxFactory.Parameter(identifier).WithType(type);
            return parameter;
        }

        private DMemberDeclarationSyntax ParseTypeDeclaration() => ParseClassOrStructOrInterfaceDeclaration();

        private DTypeDeclarationSyntax ParseClassOrStructOrInterfaceDeclaration()
        {
            var typeKeyword = EatToken();
            var identifier = ParseIdentifierToken();
            var openBrace = EatToken(DSyntaxKind.OpenBraceToken);
            var members = ParseMembers();
            var closeBrace = EatToken(DSyntaxKind.CloseBraceToken);

            switch (typeKeyword.SyntaxKind)
            {
                case DSyntaxKind.ClassKeyword:
                    return DSyntaxFactory.ClassDeclaration(typeKeyword, identifier, openBrace, closeBrace)
                        .WithMembers(members);
                default: throw new ArgumentOutOfRangeException(nameof(typeKeyword.SyntaxKind));
            }
        }

        private DSyntaxToken ParseIdentifierToken()
        {
            var identifier = EatToken(DSyntaxKind.IdentifierToken);

            return identifier;
        }

        private DSyntaxToken PeekToken(int n = 1)
        {
            return _lexedTokens[_tokenOffset + n];
        }

        private DSyntaxToken EatToken()
        {
            var cToken = currentToken;
            _tokenOffset++;
            return cToken;
        }

        private DSyntaxToken EatToken(DSyntaxKind syntaxKind)
        {
            var cToken = currentToken;
            if (cToken.SyntaxKind != syntaxKind)
            {
                var locationInfo = ParserFunctions.GetLocationFromLength(_source, cToken.Position);
                throw new UnexpectedTokenException(syntaxKind.ToString(), cToken.SyntaxKind.ToString(),
                    locationInfo.Item1, locationInfo.Item2, cToken.Width);
            }

            _tokenOffset++;
            return cToken;
        }
    }
}