using DSharpCodeAnalysis.Syntax;
using System;
using System.Collections.Generic;

namespace DSharpCodeAnalysis.Parser
{
    public class DParser
    {
        private List<DSyntaxToken> _lexedTokens;
        private DSyntaxToken _currentToken;
        private int _tokenOffset;

        private DSyntaxToken CurrentToken
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
            compilation.WithMembers(DSyntaxFactory.List(members));

            return compilation;
        }

        private DMemberDeclarationSyntax ParseMemberDeclarationOrStatement()
        {
            switch (CurrentToken.SyntaxKind)
            {
                case DSyntaxKind.ClassKeyword:
                    break;
                default: break;
            }

            throw new NotImplementedException();
        }

        private DMemberDeclarationSyntax ParseTypeDeclaration()
        {


            throw new NotImplementedException();
        }

        private void ParseModifiers()
        {

        }

        private DSyntaxToken PeekToken(int n = 1)
        {
            return _lexedTokens[n];
        }
    }
}