using System.Linq;
using Xunit;

namespace DSharpCompiler.Core.Tests
{
    public class PartEightTests
    {
        [Fact]
        public void UnaryTest()
        {
            var code = "2 + + - 2";
            var lexer = new LexicalAnalyzer(code);
            var tokens = lexer.Analayze();
            var parser = new TokenParser(tokens.ToList());
            var rootNode = parser.Expression();
            var interpreter = new Interpreter(rootNode);
            var result = interpreter.Interpret();
            Assert.Equal(0, result);
        }

        [Fact]
        public void NegativeUnaryTest()
        {
            var code = "5 - - - 2";
            var lexer = new LexicalAnalyzer(code);
            var tokens = lexer.Analayze();
            var parser = new TokenParser(tokens.ToList());
            var rootNode = parser.Expression();
            var interpreter = new Interpreter(rootNode);
            var result = interpreter.Interpret();
            Assert.Equal(3, result);
        }

        [Fact]
        public void UnaryAndPrecedenceTest()
        {
            var code = "5 - - - + - (3 + 4) - +2";
            var lexer = new LexicalAnalyzer(code);
            var tokens = lexer.Analayze();
            var parser = new TokenParser(tokens.ToList());
            var rootNode = parser.Expression();
            var interpreter = new Interpreter(rootNode);
            var result = interpreter.Interpret();
            Assert.Equal(10, result);
        }

        
    }
}