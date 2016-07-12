using DSharpCompiler.Core.Common;
using DSharpCompiler.Core.DSharp;
using Xunit;

namespace DSharpCompiler.Core.Tests
{
    public class DSharpTests
    {
        [Fact]
        public void SimpleProgramTest()
        {
            var code = @"
                func doWork
                {
                    let a = 2;
                };
                doWork;";
            var pascalTokens = new DSharpTokens();
            var lexer = new LexicalAnalyzer(pascalTokens);
            var parser = new DSharpParser();
            var interpreter = new NodeVisitor();
            var wrapper = new Interpreter(lexer, parser, interpreter);
            var dictionary = wrapper.Interpret(code);
            var a = dictionary.Get("a");
            Assert.Equal(2, a);
        }

        [Fact]
        public void ReplTest()
        {
            var code = @"
                let a = 1;
                func doWork
                {
                    let b = 2;
                };
                let c = 3;";
            var pascalTokens = new DSharpTokens();
            var lexer = new LexicalAnalyzer(pascalTokens);
            var parser = new DSharpParser();
            var interpreter = new NodeVisitor();
            var wrapper = new Interpreter(lexer, parser, interpreter);
            var dictionary = wrapper.Interpret(code);
            var a = dictionary.Get("a");
            var b = dictionary.Get("b");
            var c = dictionary.Get("c");
            Assert.Equal(1, a);
            Assert.Equal(null, b);
            Assert.Equal(3, c);
        }

        [Fact]
        public void FunctionCallTest()
        {
            var code = @"
                let a = 1;
                func doWork
                {
                    let b = 2;
                };
                func doMoreWork
                {
                    let c = 3;
                };
                let d = 4;
                doWork;
                doMoreWork;";
            var pascalTokens = new DSharpTokens();
            var lexer = new LexicalAnalyzer(pascalTokens);
            var parser = new DSharpParser();
            var interpreter = new NodeVisitor();
            var wrapper = new Interpreter(lexer, parser, interpreter);
            var dictionary = wrapper.Interpret(code);
            var a = dictionary.Get("a");
            var b = dictionary.Get("b");
            var c = dictionary.Get("c");
            var d = dictionary.Get("d");
            Assert.Equal(1, a);
            Assert.Equal(2, b);
            Assert.Equal(3, c);
            Assert.Equal(4, d);
        }

        [Fact]
        public void AssignmentStringSimpleTest()
        {
            var code = @"let a = ""hello world"";";
            var pascalTokens = new DSharpTokens();
            var lexer = new LexicalAnalyzer(pascalTokens);
            var parser = new DSharpParser();
            var interpreter = new NodeVisitor();
            var wrapper = new Interpreter(lexer, parser, interpreter);
            var dictionary = wrapper.Interpret(code);
            var a = dictionary.Get("a");
            Assert.Equal("hello world", a);
        }

        [Fact]
        public void AssignmentStringComplexTest()
        {
            var code = @"
                let a = 1;
                func doWork
                {
                    let b = 2;
                };
                func doMoreWork
                {
                    let c = ""hello world"";
                };
                let d = 4;
                doWork;
                doMoreWork;";
            var pascalTokens = new DSharpTokens();
            var lexer = new LexicalAnalyzer(pascalTokens);
            var parser = new DSharpParser();
            var interpreter = new NodeVisitor();
            var wrapper = new Interpreter(lexer, parser, interpreter);
            var dictionary = wrapper.Interpret(code);
            var a = dictionary.Get("a");
            var b = dictionary.Get("b");
            var c = dictionary.Get("c");
            var d = dictionary.Get("d");
            Assert.Equal(1, a);
            Assert.Equal(2, b);
            Assert.Equal("hello world", c);
            Assert.Equal(4, d);
        }
    }
}