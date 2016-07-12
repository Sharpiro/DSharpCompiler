using DSharpCompiler.Core.Common;
using DSharpCompiler.Core.Pascal;
using System.Linq;
using Xunit;

namespace DSharpCompiler.Core.Tests
{
    public class PartNineTests
    {
        [Fact]
        public void SimpleProgramTest()
        {
            var code = "BEGIN a := 2 END.";
            var pascalTokens = new PascalTokens();
            var lexer = new LexicalAnalyzer(pascalTokens);
            var parser = new PascalParser();
            var visitor = new NodeVisitor();
            var interpreter = new Interpreter(lexer, parser, visitor);
            var dictionary = interpreter.Interpret(code);
            var result = dictionary.Get("a");
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
            var pascalTokens = new PascalTokens();
            var lexer = new LexicalAnalyzer(pascalTokens);
            var parser = new PascalParser();
            var visitor = new NodeVisitor();
            var interpreter = new Interpreter(lexer, parser, visitor);
            var dictionary = interpreter.Interpret(code);
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