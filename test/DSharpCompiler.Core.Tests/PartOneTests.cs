using DSharpCompiler.Core.Common;
using DSharpCompiler.Core.Pascal;
using Xunit;

namespace DSharpCompiler.Core.Tests
{
    public class PartOneTests
    {
        [Fact]
        public void BasicAdditionTest()
        {
            var code = "BEGIN a := 3 + 2; END.";
            var pascalTokens = new PascalTokens();
            var lexer = new LexicalAnalyzer(pascalTokens);
            var parser = new PascalParser();
            var visitor = new NodeVisitor();
            var interpreter = new Interpreter(lexer, parser, visitor);
            var dictionary = interpreter.Interpret(code);
            var result = dictionary.Get("a");
            Assert.Equal(5, result);
        }

        [Fact]
        public void MultiDigitTest()
        {
            var code = "BEGIN a := 12+3; END.";
            var pascalTokens = new PascalTokens();
            var lexer = new LexicalAnalyzer(pascalTokens);
            var parser = new PascalParser();
            var visitor = new NodeVisitor();
            var interpreter = new Interpreter(lexer, parser, visitor);
            var dictionary = interpreter.Interpret(code);
            var result = dictionary.Get("a");
            Assert.Equal(15, result);
        }

        [Fact]
        public void WhiteSpaceTest()
        {
            var code = "BEGIN a :=  12 + 3; END.";
            var pascalTokens = new PascalTokens();
            var lexer = new LexicalAnalyzer(pascalTokens);
            var parser = new PascalParser();
            var visitor = new NodeVisitor();
            var interpreter = new Interpreter(lexer, parser, visitor);
            var dictionary = interpreter.Interpret(code);
            var result = dictionary.Get("a");
            Assert.Equal(15, result);
        }

        [Fact]
        public void SubtractionTest()
        {
            var code = "BEGIN a := 7-5 END.";
            var pascalTokens = new PascalTokens();
            var lexer = new LexicalAnalyzer(pascalTokens);
            var parser = new PascalParser();
            var visitor = new NodeVisitor();
            var interpreter = new Interpreter(lexer, parser, visitor);
            var dictionary = interpreter.Interpret(code);
            var result = dictionary.Get("a");
            Assert.Equal(2, result);
        }
    }
}