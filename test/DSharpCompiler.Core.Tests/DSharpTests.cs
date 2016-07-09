using DSharpCompiler.Core.Common;
using DSharpCompiler.Core.DSharp;
using System.Linq;
using Xunit;

namespace DSharpCompiler.Core.Tests
{
    public class DSharpTests
    {
        [Fact]
        public void SimpleProgramTest()
        {
            var code = @"
                func main
                {
                    a = 2;
                }";
            var pascalTokens = new DSharpTokens();
            var lexer = new LexicalAnalyzer(pascalTokens);
            var parser = new DSharpParser();
            var interpreter = new NodeVisitor();
            var wrapper = new Interpreter(lexer, parser, interpreter);
            var result = wrapper.Interpret(code).FirstOrDefault().Value;
            Assert.Equal(2, result);
        }

        [Fact]
        public void ComplexProgramTest()
        {
            var code = @"
                func main
                {
                    func action
                    {
                        number = 2;
                        a = number;
                        b = 10 * a + 10 * number / 4;
                        c = a - - b;
                    };
                    x = 11
                }";
            var pascalTokens = new DSharpTokens();
            var lexer = new LexicalAnalyzer(pascalTokens);
            var parser = new DSharpParser();
            var interpreter = new NodeVisitor();
            var wrapper = new Interpreter(lexer, parser, interpreter);
            var dictionary = wrapper.Interpret(code);
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

        [Fact]
        public void DoubleFunctionTest()
        {
            var code = @"
                func main
                {
                    c = 12;
                    func action
                    {
                        number = 2;
                    };

                    func actionTwo
                    {
                        b = 10 * number;
                    };
                };";
            var pascalTokens = new DSharpTokens();
            var lexer = new LexicalAnalyzer(pascalTokens);
            var parser = new DSharpParser();
            var interpreter = new NodeVisitor();
            var wrapper = new Interpreter(lexer, parser, interpreter);
            var dictionary = wrapper.Interpret(code);
            var number = dictionary["number"];
            var b = dictionary["b"];
            var c = dictionary["c"];
            Assert.Equal(2, number);
            Assert.Equal(20, b);
            Assert.Equal(12, c);

        }
    }
}