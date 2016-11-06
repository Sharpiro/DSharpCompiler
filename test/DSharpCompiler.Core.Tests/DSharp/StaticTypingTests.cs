using DSharpCompiler.Core.Common;
using DSharpCompiler.Core.Common.Exceptions;
using DSharpCompiler.Core.DSharp;
using System;
using Xunit;

namespace DSharpCompiler.Core.Tests
{
    public class StaticTypingTests
    {
        [Fact]
        public void InvalidReturnTest()
        {
            var code = @"
                func int doWork(int x)
                {
                    if (x eq 2)
                    {
                        return ""def"";
                    };
                    return ""abc"";
                };
                let b = doWork(2);";
            var pascalTokens = new DSharpTokens();
            var lexer = new LexicalAnalyzer(pascalTokens);
            var parser = new DSharpParser();
            var visitor = new NodeVisitor();
            var interpreter = new Interpreter(lexer, parser, visitor);
            Assert.Throws(typeof(TypeMismatchException), () => interpreter.Interpret(code));
        }

        [Fact]
        public void InvalidReturnTest2()
        {
            var code = @"
                func int doWork()
                {
                    if (2 eq 2)
                    {
                        return ""abc"";
                    };
                    return 123;
                };
                let b = doWork();";
            var pascalTokens = new DSharpTokens();
            var lexer = new LexicalAnalyzer(pascalTokens);
            var parser = new DSharpParser();
            var visitor = new NodeVisitor();
            var interpreter = new Interpreter(lexer, parser, visitor);
            Assert.Throws(typeof(TypeMismatchException), () => interpreter.Interpret(code));
        }

        [Fact]
        public void InvalidReturnTest3()
        {
            var code = @"
                func int doWork()
                {
                    if (2 eq 2)
                    {
                        return 123;
                    };
                    return ""abc"";
                };
                let b = doWork();";
            var pascalTokens = new DSharpTokens();
            var lexer = new LexicalAnalyzer(pascalTokens);
            var parser = new DSharpParser();
            var visitor = new NodeVisitor();
            var interpreter = new Interpreter(lexer, parser, visitor);
            Assert.Throws(typeof(TypeMismatchException), () => interpreter.Interpret(code));
        }

        [Fact]
        public void ValidReturnTest()
        {
            var code = @"
                func int doWork(int x)
                {
                    if (x eq 2)
                    {
                        return 123;
                    };
                    return 456;
                };
                let b = doWork(2);";
            var pascalTokens = new DSharpTokens();
            var lexer = new LexicalAnalyzer(pascalTokens);
            var parser = new DSharpParser();
            var visitor = new NodeVisitor();
            var interpreter = new Interpreter(lexer, parser, visitor);
            var dictionary = interpreter.Interpret(code);
            var b = dictionary.GetValue<int>("b");
            Assert.Equal(123, b);
        }

        [Fact]
        public void ValidReturnTest2()
        {
            var code = @"
                func int doWork()
                {
                    if (2 eq 1)
                    {
                        return 123;
                    };
                    return 456;
                };
                let b = doWork();";
            var pascalTokens = new DSharpTokens();
            var lexer = new LexicalAnalyzer(pascalTokens);
            var parser = new DSharpParser();
            var visitor = new NodeVisitor();
            var interpreter = new Interpreter(lexer, parser, visitor);
            var dictionary = interpreter.Interpret(code);
            var b = dictionary.GetValue<int>("b");
            Assert.Equal(456, b);
        }

        [Fact]
        public void BinaryNodeTypeMismatchTest()
        {
            var code = @"
                func int add(int e, int f)
                {
                    return e + ""abcd"";
                };
                let e = add(2, 4);";
            var pascalTokens = new DSharpTokens();
            var lexer = new LexicalAnalyzer(pascalTokens);
            var parser = new DSharpParser();
            var nodeVisitor = new NodeVisitor();
            var interpreter = new Interpreter(lexer, parser, nodeVisitor);
            Assert.Throws(typeof(TypeMismatchException), () => interpreter.Interpret(code));
        }
    }
}