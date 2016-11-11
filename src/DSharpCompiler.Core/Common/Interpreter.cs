using DSharpCompiler.Core.DSharp;
using DSharpCompiler.Core.Pascal;
using System.Linq;

namespace DSharpCompiler.Core.Common
{
    public class Interpreter
    {
        private readonly NodeVisitor _visitor;
        private readonly LexicalAnalyzer _lexer;
        private readonly ITokenParser _parser;

        public Interpreter(LexicalAnalyzer lexer, ITokenParser parser,
            NodeVisitor visitor)
        {
            _lexer = lexer;
            _parser = parser;
            _visitor = visitor;
        }

        public SymbolsTable Interpret(string source)
        {
            var tokens = _lexer.Analayze(source).ToList();
            var rootNode = _parser.Program(tokens);
            var dictionary = _visitor.VisitNodes(rootNode);
            return dictionary;
        }

        public static Interpreter GetDsharpInterpreter()
        {
            var tokenDefinitions = new DSharpTokenDefinitions();
            var lexer = new LexicalAnalyzer(tokenDefinitions);
            var typesTable = new TypesTable();
            var parser = new DSharpParser(typesTable);
            var visitor = new NodeVisitor(typesTable);
            var interpreter = new Interpreter(lexer, parser, visitor);
            return interpreter;
        }

        public static Interpreter GetPascalInterpreter()
        {
            var tokenDefinitions = new PascalTokens();
            var lexer = new LexicalAnalyzer(tokenDefinitions);
            var typesTable = new TypesTable();
            var parser = new PascalParser();
            var visitor = new NodeVisitor(typesTable);
            var interpreter = new Interpreter(lexer, parser, visitor);
            return interpreter;
        }
    }
}