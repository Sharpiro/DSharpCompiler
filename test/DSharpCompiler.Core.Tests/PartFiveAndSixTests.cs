using System.Linq;
using Xunit;

namespace DSharpCompiler.Core.Tests
{
    public class PartFiveAndSixTests
    {
        [Fact]
        public void PrecedenceTest()
        {
            var code = "BEGIN a := 14 + 2 * 3 - 6 / 2 END.";
            var lexer = new LexicalAnalyzer(code);
            var tokens = lexer.Analayze();
            var parser = new TokenParser(tokens.ToList());
            var rootNode = parser.Program();
            var interpreter = new Interpreter(rootNode);
            var result = interpreter.Interpret().FirstOrDefault().Value;
            Assert.Equal(17, result);
        }

        [Fact]
        public void ShallowNestingTest()
        {
            var code = "BEGIN a := 2 * (7 + 3); END.";
            var lexer = new LexicalAnalyzer(code);
            var tokens = lexer.Analayze();
            var parser = new TokenParser(tokens.ToList());
            var rootNode = parser.Program();
            var interpreter = new Interpreter(rootNode);
            var result = interpreter.Interpret().FirstOrDefault().Value;
            Assert.Equal(20, result);
        }

        [Fact]
        public void DeepNestingTest()
        {
            var code = "BEGIN a := 7 + 3 * (10 / (12 / (3 + 1) - 1)); END.";
            var lexer = new LexicalAnalyzer(code);
            var tokens = lexer.Analayze();
            var parser = new TokenParser(tokens.ToList());
            var rootNode = parser.Program();
            var interpreter = new Interpreter(rootNode);
            var result = interpreter.Interpret().FirstOrDefault().Value;
            Assert.Equal(22, result);
        }
        [Fact]
        public void DeepDeepNestingTest()
        {
            var code = "BEGIN a := 7 + 3 * (10 / (12 / (3 + 1) - 1)) / (2 + 3) - 5 - 3 + (8) END.";
            var lexer = new LexicalAnalyzer(code);
            var tokens = lexer.Analayze();
            var parser = new TokenParser(tokens.ToList());
            var rootNode = parser.Program();
            var interpreter = new Interpreter(rootNode);
            var result = interpreter.Interpret().FirstOrDefault().Value;
            Assert.Equal(10, result);
        }
    }
}