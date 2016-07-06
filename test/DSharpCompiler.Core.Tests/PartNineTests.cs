using System.Linq;
using Xunit;

namespace DSharpCompiler.Core.Tests
{
    public class PartNineTests
    {
        [Fact]
        public void PeekTest()
        {
            var code = "BEGIN a := 2; END.";
            var lexer = new LexicalAnalyzer(code);
            var tokens = lexer.Analayze();
            Assert.Equal(7, tokens.Count());
            //var parser = new TokenParser(tokens.ToList());
            //var rootNode = parser.Expression();
            //var interpreter = new Interpreter(rootNode);
            //var result = interpreter.Interpret();
        }
    }
}