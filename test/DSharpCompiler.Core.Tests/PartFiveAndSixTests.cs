using System.Linq;
using Xunit;

namespace DSharpCompiler.Core.Tests
{
    public class PartFiveAndSixTests
    {
        [Fact]
        public void PrecedenceTest()
        {
            var code = "14 + 2 * 3 - 6 / 2";
            var lexer = new LexicalAnalyzer(code);
            var tokens = lexer.Analayze();
            var parser = new TokenParser(tokens.ToList());
            var result = parser.Expression();
            Assert.Equal(17, result);
        }

        [Fact]
        public void ShallowNestingTest()
        {
            var code = "2 * (7 + 3)";
            var lexer = new LexicalAnalyzer(code);
            var tokens = lexer.Analayze();
            var parser = new TokenParser(tokens.ToList());
            var result = parser.Expression();
            Assert.Equal(20, result);
        }

        [Fact]
        public void DeepNestingTest()
        {
            var code = "7 + 3 * (10 / (12 / (3 + 1) - 1))";
            var lexer = new LexicalAnalyzer(code);
            var tokens = lexer.Analayze();
            var parser = new TokenParser(tokens.ToList());
            var result = parser.Expression();
            Assert.Equal(22, result);
        }
    }
}