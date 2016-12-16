using DSharpCodeAnalysis.Exceptions;
using DSharpCodeAnalysis.Syntax;
using System;
using System.Collections.Generic;

namespace DSharpCodeAnalysis.Parser
{
    public class DParser
    {
        private List<DSyntaxToken> _lexedTokens;
        private int _tokenOffset;

        private DSyntaxToken currentToken
        {
            get
            {
                return _lexedTokens[_tokenOffset];
            }
        }

        public DParser(List<DSyntaxToken> lexedTokens)
        {
            if (lexedTokens.IsNullOrEmpty()) throw new ArgumentNullException(nameof(lexedTokens));

            _lexedTokens = lexedTokens;
        }

        public DCompilationUnitSyntax ParseCompilationUnit()
        {
            var compilation = DSyntaxFactory.CompilationUnit();

            var members = new List<DMemberDeclarationSyntax>();
            for (var i = 0; i < 1; i++)
            {
                members.Add(ParseMemberDeclarationOrStatement());
            }
            compilation = compilation.WithMembers(DSyntaxFactory.List(members));

            if (_tokenOffset != _lexedTokens.Count) throw new TokenException("Compilation finished with un-parsed tokens");

            return compilation;
        }

        private DMemberDeclarationSyntax ParseMemberDeclarationOrStatement()
        {
            if (currentToken.SyntaxKind == DSyntaxKind.ClassKeyword)
                return ParseTypeDeclaration();
            var returnType = ParseType();
            var identifier = ParseIdentifier();
            var memberDeclaration = ParseMethodDeclaration(returnType, identifier);

            return memberDeclaration;
        }

        private DTypeSyntax ParseType()
        {
            var typeToken = EatToken();
            return DSyntaxFactory.PredefinedType(typeToken);
        }

        private DSyntaxToken ParseIdentifier()
        {
            var typeToken = EatToken(DSyntaxKind.IdentifierToken);
            return typeToken;
        }

        private DMethodDeclarationSyntax ParseMethodDeclaration(DTypeSyntax returnType, DSyntaxToken identifier)
        {
            var parameterList = ParseParenthesizedParameterList();
            DBlockSyntax body;
            DSyntaxToken semicolonToken;

            ParseBlockAndExpressionBodiesWithSemicolon(out body, out semicolonToken);
            var method = DSyntaxFactory.MethodDeclaration(returnType, identifier)
                .WithParameterList(parameterList).WithBody(body).WithSemicolonToken(semicolonToken);
            return method;
        }

        public void ParseBlockAndExpressionBodiesWithSemicolon(out DBlockSyntax blockBody, out DSyntaxToken semicolonToken)
        {
            blockBody = null;
            semicolonToken = null;

            if (currentToken.SyntaxKind == DSyntaxKind.SemicolonToken)
                semicolonToken = EatToken(DSyntaxKind.SemicolonToken);

            else if (currentToken.SyntaxKind == DSyntaxKind.OpenBraceToken)
                blockBody = ParseBlock();
        }

        public DBlockSyntax ParseBlock()
        {
            var openBrace = EatToken(DSyntaxKind.OpenBraceToken);
            var statements = ParseStatements();
            var closeBrace = EatToken(DSyntaxKind.CloseBraceToken);

            var block = DSyntaxFactory.Block().WithOpenBraceToken(openBrace)
                .WithStatements(statements).WithCloseBraceToken(closeBrace);

            return block;
        }

        public DSyntaxList<DStatementSyntax> ParseStatements()
        {
            return DSyntaxFactory.List<DStatementSyntax>();
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
            var identifier = ParseIdentifier();
            var parameter = DSyntaxFactory.Parameter(identifier).WithType(type);
            return parameter;
        }

        private DMemberDeclarationSyntax ParseTypeDeclaration() => ParseClassOrStructOrInterfaceDeclaration();

        private DTypeDeclarationSyntax ParseClassOrStructOrInterfaceDeclaration()
        {
            var typeKeyword = EatToken();
            var identifier = ParseIdentifierToken();
            var openBrace = EatToken(DSyntaxKind.OpenBraceToken);
            var closeBrace = EatToken(DSyntaxKind.CloseBraceToken);

            switch (typeKeyword.SyntaxKind)
            {
                case DSyntaxKind.ClassKeyword:
                    return DSyntaxFactory.ClassDeclaration(typeKeyword, identifier, openBrace, closeBrace);
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
            return _lexedTokens[n];
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
                throw new UnexpectedTokenException(syntaxKind.ToString(), cToken.SyntaxKind.ToString());

            _tokenOffset++;
            return cToken;
        }
    }
}