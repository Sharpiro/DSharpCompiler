using System.Linq;
using Xunit;

namespace DSharpCompiler.Core.Tests
{
    public class PartOneTests
    {
        [Fact]
        public void BasicAdditionTest()
        {
            var code = "BEGIN a := 3 + 2; END.";
            var lexer = new LexicalAnalyzer(code);
            var tokens = lexer.Analayze();
            var parser = new TokenParser(tokens.ToList());
            var rootNode = parser.Program();
            var interpreter = new Interpreter(rootNode);
            var dictionary = interpreter.Interpret();
            var result = dictionary.FirstOrDefault().Value;
            Assert.Equal(5, result);
        }

        [Fact]
        public void MultiDigitTest()
        {
            var code = "BEGIN a := 12+3; END.";
            var lexer = new LexicalAnalyzer(code);
            var tokens = lexer.Analayze();
            var parser = new TokenParser(tokens.ToList());
            var rootNode = parser.Program();
            var interpreter = new Interpreter(rootNode);
            var dictionary = interpreter.Interpret();
            var result = dictionary.FirstOrDefault().Value;
            Assert.Equal(15, result);
        }

        [Fact]
        public void WhiteSpaceTest()
        {
            var code = "BEGIN a :=  12 + 3; END.";
            var lexer = new LexicalAnalyzer(code);
            var tokens = lexer.Analayze();
            var parser = new TokenParser(tokens.ToList());
            var rootNode = parser.Program();
            var interpreter = new Interpreter(rootNode);
            var dictionary = interpreter.Interpret();
            var result = dictionary.FirstOrDefault().Value;
            Assert.Equal(15, result);
        }

        [Fact]
        public void SubtractionTest()
        {
            var code = "BEGIN a := 7-5 END.";
            var lexer = new LexicalAnalyzer(code);
            var tokens = lexer.Analayze();
            var parser = new TokenParser(tokens.ToList());
            var rootNode = parser.Program();
            var interpreter = new Interpreter(rootNode);
            var dictionary = interpreter.Interpret();
            var result = dictionary.FirstOrDefault().Value;
            Assert.Equal(2, result);
        }
    }
}