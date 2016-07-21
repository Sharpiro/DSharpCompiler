using DSharpCompiler.Core.Common;
using DSharpCompiler.Core.DSharp;
using Xunit;

namespace DSharpCompiler.Core.Tests
{
    public class ParenthesisTests
    {
        [Fact]
        public void SimpleProgramTest()
        {
            var code = @"
                func doWork(a)
                {
                    return a;
                };
                let a = doWork(2);";
            var pascalTokens = new DSharpTokens();
            var lexer = new LexicalAnalyzer(pascalTokens);
            var parser = new DSharpParser();
            var interpreter = new NodeVisitor();
            var wrapper = new Interpreter(lexer, parser, interpreter);
            var dictionary = wrapper.Interpret(code);
            var a = dictionary.GetValue<int>("a");
            Assert.Equal(2, a);
        }

        [Fact]
        public void AdditionTest()
        {
            var code = @"
                func add(e, f)
                {
                    return e + f;
                };
                let e = add(2, 4);";
            var pascalTokens = new DSharpTokens();
            var lexer = new LexicalAnalyzer(pascalTokens);
            var parser = new DSharpParser();
            var interpreter = new NodeVisitor();
            var wrapper = new Interpreter(lexer, parser, interpreter);
            var dictionary = wrapper.Interpret(code);
            var e = dictionary.GetValue<int>("e");
            Assert.Equal(6, e);
        }

        [Fact]
        public void ScopeTest()
        {
            var code = @"
                func add(e, f)
                {
                    return e + f;
                };
                let g = add(2, 4);";
            var pascalTokens = new DSharpTokens();
            var lexer = new LexicalAnalyzer(pascalTokens);
            var parser = new DSharpParser();
            var interpreter = new NodeVisitor();
            var wrapper = new Interpreter(lexer, parser, interpreter);
            var dictionary = wrapper.Interpret(code);
            var e = dictionary.GetValue<int?>("e");
            var f = dictionary.GetValue<int?>("f");
            var g = dictionary.GetValue<int>("g");
            Assert.Equal(null, e);
            Assert.Equal(null, f);
            Assert.Equal(6, g);
        }
    }
}