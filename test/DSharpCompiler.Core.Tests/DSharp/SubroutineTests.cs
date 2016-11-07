using DSharpCompiler.Core.Common;
using DSharpCompiler.Core.DSharp;
using Xunit;

namespace DSharpCompiler.Core.Tests
{
    public class SubroutineTests
    {
        [Fact]
        public void InvalidReturnTest()
        {
            var code = @"
                func int add(int x, int y)
                {
                    return x + y;
                };
                let b = add(2, 6);
                dSharpFunctions.print(b);";
            var pascalTokens = new DSharpTokens();
            var lexer = new LexicalAnalyzer(pascalTokens);
            var parser = new DSharpParser();
            var visitor = new NodeVisitor();
            var interpreter = new Interpreter(lexer, parser, visitor);
            var symbols = interpreter.Interpret(code);
            var b = symbols.GetValue<int>("b");
            Assert.Equal(8, b);
        }
    }
}