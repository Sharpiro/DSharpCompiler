using Microsoft.CodeAnalysis.CSharp;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace DSharpCodeAnalysis.Parser
{
    public class Lexer
    {
        public void Lex(string source)
        {
            var x = UsingDirective(IdentifierName("System"));
            var z = x.Kind();
            var y = SyntaxKind.UsingDirective;
        }
    }
}