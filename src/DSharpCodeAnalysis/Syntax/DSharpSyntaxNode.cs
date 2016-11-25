using System;
using System.Collections;
using System.Collections.Generic;

namespace DSharpCodeAnalysis.Syntax
{
    public class DSharpSyntaxNode
    {
        public DSharpSyntaxKind SyntaxKind { get; set; }
        public DSharpSyntaxNode Parent { get; set; }
        public IEnumerable<DSharpSyntaxNode> Children { get; set; }
    }

    public class MemberDeclarationSyntax : DSharpSyntaxNode
    {

    }

    public class DSharpClassDeclarationSyntax : MemberDeclarationSyntax
    {
        public void WithModifiers()
        {

        }
    }

    public class SyntaxTokenList : IEnumerable<SyntaxToken>
    {
        public IEnumerator<SyntaxToken> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }

    public class SyntaxToken
    {

    }
}