using DSharpCodeAnalysis.Syntax;

namespace DSharpCodeAnalysis.Parser
{
    public static class DSharpScript
    {
        public static DCompilationUnitSyntax Create(string source)
        {
            var lexer = new DLexer(source);
            var parser = new DParser(lexer.Lex());
            var compilation = parser.ParseCompilationUnit();
            return compilation;
        }
    }
}