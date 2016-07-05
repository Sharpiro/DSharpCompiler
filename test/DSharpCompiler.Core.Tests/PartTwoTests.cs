using System.Linq;
using Xunit;

namespace DSharpCompiler.Core.Tests
{
    public class PartTwoTests
    {
        [Fact]
        public void AddtionTest()
        {
            var code = "1+1+1";
            var lexer = new LexicalAnalyzer(code);
            var tokens = lexer.Analayze();
            var parser = new TokenParser(tokens.ToList());
            var rootNode = parser.Expression();
            var interpreter = new Interpreter(rootNode);
            var result = interpreter.Interpret();
            Assert.Equal(3, result);
        }

        [Fact]
        public void MultiplicationTest()
        {
            var code = "5*3";
            var lexer = new LexicalAnalyzer(code);
            var tokens = lexer.Analayze();
            var parser = new TokenParser(tokens.ToList());
            var rootNode = parser.Expression();
            var interpreter = new Interpreter(rootNode);
            var result = interpreter.Interpret();
            Assert.Equal(15, result);
        }

        [Fact]
        public void DivisionTest()
        {
            var code = "12/3";
            var lexer = new LexicalAnalyzer(code);
            var tokens = lexer.Analayze();
            var parser = new TokenParser(tokens.ToList());
            var rootNode = parser.Expression();
            var interpreter = new Interpreter(rootNode);
            var result = interpreter.Interpret();
            Assert.Equal(4, result);
        }

        [Fact]
        public void ContinuousOperationsTest()
        {
            var code = "9 - 5 + 3 + 11";
            var lexer = new LexicalAnalyzer(code);
            var tokens = lexer.Analayze();
            var parser = new TokenParser(tokens.ToList());
            var rootNode = parser.Expression();
            var interpreter = new Interpreter(rootNode);
            var result = interpreter.Interpret();
            Assert.Equal(18, result);
        }

        [Fact]
        public void MultiplicationAndDivisionTest()
        {
            var code = "7 * 4 / 2 * 3";
            var lexer = new LexicalAnalyzer(code);
            var tokens = lexer.Analayze();
            var parser = new TokenParser(tokens.ToList());
            var rootNode = parser.Expression();
            var interpreter = new Interpreter(rootNode);
            var result = interpreter.Interpret();
            Assert.Equal(42, result);
        }
    }
}