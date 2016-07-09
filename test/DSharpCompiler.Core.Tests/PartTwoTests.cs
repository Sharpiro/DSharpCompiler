using DSharpCompiler.Core.Common;
using DSharpCompiler.Core.Pascal;
using System.Linq;
using Xunit;

namespace DSharpCompiler.Core.Tests
{
    public class PartTwoTests
    {
        [Fact]
        public void AddtionTest()
        {
            var code = "BEGIN a := 1+1+1 END.";
            var pascalTokens = new PascalTokens();
            var lexer = new LexicalAnalyzer(pascalTokens);
            var parser = new PascalParser();
            var visitor = new NodeVisitor();
            var interpreter = new Interpreter(lexer, parser, visitor);
            var dictionary = interpreter.Interpret(code);
            var result = dictionary.FirstOrDefault().Value;
            Assert.Equal(3, result);
        }

        [Fact]
        public void MultiplicationTest()
        {
            var code = "BEGIN a := 5*3 END.";
            var pascalTokens = new PascalTokens();
            var lexer = new LexicalAnalyzer(pascalTokens);
            var parser = new PascalParser();
            var visitor = new NodeVisitor();
            var interpreter = new Interpreter(lexer, parser, visitor);
            var dictionary = interpreter.Interpret(code);
            var result = dictionary.FirstOrDefault().Value;
            Assert.Equal(15, result);
        }

        [Fact]
        public void DivisionTest()
        {
            var code = "BEGIN a := 12/3 END.";
            var pascalTokens = new PascalTokens();
            var lexer = new LexicalAnalyzer(pascalTokens);
            var parser = new PascalParser();
            var visitor = new NodeVisitor();
            var interpreter = new Interpreter(lexer, parser, visitor);
            var dictionary = interpreter.Interpret(code);
            var result = dictionary.FirstOrDefault().Value;
            Assert.Equal(4, result);
        }

        [Fact]
        public void ContinuousOperationsTest()
        {
            var code = "BEGIN a := 9 - 5 + 3 + 11 END.";
            var pascalTokens = new PascalTokens();
            var lexer = new LexicalAnalyzer(pascalTokens);
            var parser = new PascalParser();
            var visitor = new NodeVisitor();
            var interpreter = new Interpreter(lexer, parser, visitor);
            var dictionary = interpreter.Interpret(code);
            var result = dictionary.FirstOrDefault().Value;
            Assert.Equal(18, result);
        }

        [Fact]
        public void MultiplicationAndDivisionTest()
        {
            var code = "BEGIN a := 7 * 4 / 2 * 3 END.";
            var pascalTokens = new PascalTokens();
            var lexer = new LexicalAnalyzer(pascalTokens);
            var parser = new PascalParser();
            var visitor = new NodeVisitor();
            var interpreter = new Interpreter(lexer, parser, visitor);
            var dictionary = interpreter.Interpret(code);
            var result = dictionary.FirstOrDefault().Value;
            Assert.Equal(42, result);
        }
    }
}