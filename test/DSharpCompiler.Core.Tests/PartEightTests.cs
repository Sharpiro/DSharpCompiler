using System.Linq;
using Xunit;

namespace DSharpCompiler.Core.Tests
{
    public class PartEightTests
    {
        [Fact]
        public void UnaryTest()
        {
            var code = "BEGIN a := 2 + + - 2 END.";
            var lexer = new LexicalAnalyzer(code);
            var tokens = lexer.Analayze();
            var parser = new TokenParser(tokens.ToList());
            var rootNode = parser.Program();
            var interpreter = new Interpreter(rootNode);
            var dictionary = interpreter.Interpret();
            var result = dictionary.FirstOrDefault().Value;
            Assert.Equal(0, result);
        }

        [Fact]
        public void NegativeUnaryTest()
        {
            var code = "BEGIN a := 5 - - - 2 END.";
            var lexer = new LexicalAnalyzer(code);
            var tokens = lexer.Analayze();
            var parser = new TokenParser(tokens.ToList());
            var rootNode = parser.Program();
            var interpreter = new Interpreter(rootNode);
            var dictionary = interpreter.Interpret();
            var result = dictionary.FirstOrDefault().Value;
            Assert.Equal(3, result);
        }

        [Fact]
        public void UnaryAndPrecedenceTest()
        {
            var code = "BEGIN a := 5 - - - + - (3 + 4) - +2 END.";
            var lexer = new LexicalAnalyzer(code);
            var tokens = lexer.Analayze();
            var parser = new TokenParser(tokens.ToList());
            var rootNode = parser.Program();
            var interpreter = new Interpreter(rootNode);
            var dictionary = interpreter.Interpret();
            var result = dictionary.FirstOrDefault().Value;
            Assert.Equal(10, result);
        }

        
    }
}