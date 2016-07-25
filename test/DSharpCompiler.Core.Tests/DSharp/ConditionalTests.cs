using DSharpCompiler.Core.Common;
using DSharpCompiler.Core.DSharp;
using Xunit;

namespace DSharpCompiler.Core.Tests
{
    public class ConditionalTests
    {
        [Fact]
        public void SimpleIsTrueTest()
        {
            var code = @"
                func doWork()
                {
                    if (2 eq 2)
                    {
                        return 1;
                    };
                    return 0;
                };
                let b = doWork();";
            var pascalTokens = new DSharpTokens();
            var lexer = new LexicalAnalyzer(pascalTokens);
            var parser = new DSharpParser();
            var visitor = new NodeVisitor();
            var interpreter = new Interpreter(lexer, parser, visitor);
            var dictionary = interpreter.Interpret(code);
            var b = dictionary.GetValue<int>("b");
            Assert.Equal(1, b);
        }

        [Fact]
        public void SimpleIsFalseTest()
        {
            var code = @"
                func doWork()
                {
                    if (2 eq 3)
                    {
                        return 1;
                    };
                    return 0;
                };
                let b = doWork();";
            var pascalTokens = new DSharpTokens();
            var lexer = new LexicalAnalyzer(pascalTokens);
            var parser = new DSharpParser();
            var visitor = new NodeVisitor();
            var interpreter = new Interpreter(lexer, parser, visitor);
            var dictionary = interpreter.Interpret(code);
            var b = dictionary.GetValue<int>("b");
            Assert.Equal(0, b);
        }

        [Fact]
        public void VariableTest()
        {
            var code = @"
                func doWork(n)
                {
                    if (n eq 2)
                    {
                        return 1;
                    };
                    return 0;
                };
                let b = doWork(2);";
            var pascalTokens = new DSharpTokens();
            var lexer = new LexicalAnalyzer(pascalTokens);
            var parser = new DSharpParser();
            var visitor = new NodeVisitor();
            var interpreter = new Interpreter(lexer, parser, visitor);
            var dictionary = interpreter.Interpret(code);
            var b = dictionary.GetValue<int>("b");
            Assert.Equal(1, b);
        }

        [Fact]
        public void RecursionTest()
        {
            var code = @"
                func fib(n)
                {
                    if (n eq 0)
                    {
                        return n;
                    };
                    if (n eq 1)
                    {
                        return n;
                    };
                    return fib(n - 2) + fib(n - 1);
                };
                let b = fib(2);";
            var pascalTokens = new DSharpTokens();
            var lexer = new LexicalAnalyzer(pascalTokens);
            var parser = new DSharpParser();
            var visitor = new NodeVisitor();
            var interpreter = new Interpreter(lexer, parser, visitor);
            var dictionary = interpreter.Interpret(code);
            var b = dictionary.GetValue<int>("b");
            Assert.Equal(1, b);
        }
    }
}