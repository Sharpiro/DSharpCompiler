using DSharpCodeAnalysis.Syntax;
using System.Collections.Immutable;

namespace DSharpCodeAnalysis.Parser
{
    public struct DTokenInfo
    {
        public DSyntaxKind SyntaxKind { get; set; }
        public string Text { get; set; }
    }

    public class DTriviaInfo
    {
        public string Text { get; set; }
        public DSyntaxKind SyntaxKind { get; set; }
    }
}