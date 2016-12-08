using DSharpCodeAnalysis.Syntax;

namespace DSharpCodeAnalysis.Parser
{
    public struct DTokenInfo
    {
        public DSyntaxKind SyntaxKind { get; set; }
        public string Text { get; set; }
    }
}