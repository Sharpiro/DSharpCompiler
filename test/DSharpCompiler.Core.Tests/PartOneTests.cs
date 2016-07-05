using System.Linq;
using Xunit;

namespace DSharpCompiler.Core.Tests
{
    public class PartOneTests
    {
        [Fact]
        public void BasicAdditionTest()
        {
            var code = "3+2";
            var lexer = new LexicalAnalyzer(code);
            var tokens = lexer.Analayze();
            var parser = new TokenParser(tokens.ToList());
            var rootNode = parser.Expression();
            var interpreter = new Interpreter(rootNode);
            var result = interpreter.Interpret();
            Assert.Equal(5, result);
        }

        [Fact]
        public void MultiDigitTest()
        {
            var code = "12+3";
            var lexer = new LexicalAnalyzer(code);
            var tokens = lexer.Analayze();
            var parser = new TokenParser(tokens.ToList());
            var rootNode = parser.Expression();
            var interpreter = new Interpreter(rootNode);
            var result = interpreter.Interpret();
            Assert.Equal(15, result);
        }

        [Fact]
        public void WhiteSpaceTest()
        {
            var code = " 12 + 3";
            var lexer = new LexicalAnalyzer(code);
            var tokens = lexer.Analayze();
            var parser = new TokenParser(tokens.ToList());
            var rootNode = parser.Expression();
            var interpreter = new Interpreter(rootNode);
            var result = interpreter.Interpret();
            Assert.Equal(15, result);
        }

        [Fact]
        public void SubtractionTest()
        {
            var code = "7-5";
            var lexer = new LexicalAnalyzer(code);
            var tokens = lexer.Analayze();
            var parser = new TokenParser(tokens.ToList());
            var rootNode = parser.Expression();
            var interpreter = new Interpreter(rootNode);
            var result = interpreter.Interpret();
            Assert.Equal(2, result);
        }
    }
}