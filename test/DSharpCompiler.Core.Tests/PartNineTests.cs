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
        }

        [Fact]
        public void SimpleProgramTest()
        {
            var code = "BEGIN a := 2 END.";
            var lexer = new LexicalAnalyzer(code);
            var tokens = lexer.Analayze();
            var parser = new TokenParser(tokens.ToList());
            var rootNode = parser.Program();
            var interpreter = new Interpreter(rootNode);
            var result = interpreter.Interpret().FirstOrDefault().Value;
            Assert.Equal(2, result);
        }

        [Fact]
        public void ComplexProgramTest()
        {
            var code = @"
                BEGIN
                    BEGIN
                        number := 2;
                        a := number;
                        b := 10 * a + 10 * number / 4;
                        c := a - - b;
                    END;
                    x := 11
                END.";
            var lexer = new LexicalAnalyzer(code);
            var tokens = lexer.Analayze();
            var parser = new TokenParser(tokens.ToList());
            var rootNode = parser.Program();
            var interpreter = new Interpreter(rootNode);
            var dictionary = interpreter.Interpret();
            var number = dictionary["number"];
            var a = dictionary["a"];
            var b = dictionary["b"];
            var c = dictionary["c"];
            var x = dictionary["x"];
            Assert.Equal(2, number);
            Assert.Equal(2, a);
            Assert.Equal(25, b);
            Assert.Equal(27, c);
            Assert.Equal(11, x);
        }
    }
}