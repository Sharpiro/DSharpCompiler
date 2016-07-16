using DSharpCompiler.Core.Common;
using DSharpCompiler.Core.DSharp;
using Xunit;

namespace DSharpCompiler.Core.Tests
{
    public class ScopeTests
    {
        [Fact]
        public void InScopeTest()
        {
            var code = @"
                let x = 2;
                func doWork
                {
                    let a = ""should be null"";
                    let y = x + 1;
                    return y;
                };
                let b = doWork;
                let x = 5;
                let c = doWork;";
            var pascalTokens = new DSharpTokens();
            var lexer = new LexicalAnalyzer(pascalTokens);
            var parser = new DSharpParser();
            var visitor = new NodeVisitor();
            var interpreter = new Interpreter(lexer, parser, visitor);
            var dictionary = interpreter.Interpret(code);
            var a = dictionary.Get("a");
            var b = dictionary.Get("b");
            Assert.Equal(null, a);
            Assert.Equal(2, b);
        }
    }
}