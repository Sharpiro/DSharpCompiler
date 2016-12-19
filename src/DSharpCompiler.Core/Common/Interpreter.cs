using System.Collections.Generic;
using System.Linq;

namespace DSharpCompiler.Core.Common
{
    public class Interpreter
    {
        private readonly NodeVisitor _visitor;
        private readonly LexicalAnalyzer _lexer;
        private readonly ITokenParser _parser;

        public Interpreter(LexicalAnalyzer lexer, ITokenParser parser, NodeVisitor visitor)
        {
            _lexer = lexer;
            _parser = parser;
            _visitor = visitor;
        }

        public SymbolsTable Interpret(string source)
        {
            var tokens = _lexer.Analayze(source);
            var rootNode = _parser.Program(tokens.ToList());
            var dictionary = _visitor.VisitNodes(rootNode);
            return dictionary;
        }
    }
}