using DSharpCompiler.Core.Common;
using DSharpCompiler.Core.DSharp;
using Xunit;

namespace DSharpCompiler.Core.Tests
{
    public class ReturnTests
    {
        [Fact]
        public void AssignFunctionTest()
        {
            var code = @"
                func doWork()
                {
                    return 2;
                };
                let b = doWork();";
            var pascalTokens = new DSharpTokens();
            var lexer = new LexicalAnalyzer(pascalTokens);
            var parser = new DSharpParser();
            var visitor = new NodeVisitor();
            var interpreter = new Interpreter(lexer, parser, visitor);
            var dictionary = interpreter.Interpret(code);
            var b = dictionary.GetValue<int>("b");
            Assert.Equal(2, b);
        }

        [Fact]
        public void ReturnBreakTest()
        {
            var code = @"
                func doWork()
                {
                    return 2;
                    return 4;
                };
                let b = doWork();";
            var pascalTokens = new DSharpTokens();
            var lexer = new LexicalAnalyzer(pascalTokens);
            var parser = new DSharpParser();
            var visitor = new NodeVisitor();
            var interpreter = new Interpreter(lexer, parser, visitor);
            var dictionary = interpreter.Interpret(code);
            var b = dictionary.GetValue<int>("b");
            Assert.Equal(2, b);
        }

        [Fact]
        public void ComplexAssignTest()
        {
            var code = @"
                func doWork()
                {
                    return 2 * 2 + - 3;
                };
                func doMoreWork()
                {
                    return 2 / (2 + - 4);
                };
                let a = doWork;
                let b = doMoreWork;
                let c = doWork + doMoreWork;";
            var pascalTokens = new DSharpTokens();
            var lexer = new LexicalAnalyzer(pascalTokens);
            var parser = new DSharpParser();
            var visitor = new NodeVisitor();
            var interpreter = new Interpreter(lexer, parser, visitor);
            var dictionary = interpreter.Interpret(code);
            var a = dictionary.GetValue<int>("a");
            var b = dictionary.GetValue<int>("b");
            var c = dictionary.GetValue<int>("c");
            Assert.Equal(1, a);
            Assert.Equal(-1, b);
            Assert.Equal(0, c);
        }
    }
}