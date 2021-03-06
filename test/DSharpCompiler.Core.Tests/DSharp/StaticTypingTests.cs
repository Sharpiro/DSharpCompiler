﻿using DSharpCompiler.Core.Common;
using DSharpCompiler.Core.Common.Exceptions;
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
            var interpreter = Interpreter.GetDsharpInterpreter();
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
            var interpreter = Interpreter.GetDsharpInterpreter();
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
            var interpreter = Interpreter.GetDsharpInterpreter();
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
            var interpreter = Interpreter.GetDsharpInterpreter();
            var dictionary = interpreter.Interpret(code);
            var b = dictionary.SymbolsTable.GetValue<int>("b");
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
            var interpreter = Interpreter.GetDsharpInterpreter();
            var dictionary = interpreter.Interpret(code);
            var b = dictionary.SymbolsTable.GetValue<int>("b");
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
            var interpreter = Interpreter.GetDsharpInterpreter();
            Assert.Throws(typeof(TypeMismatchException), () => interpreter.Interpret(code));
        }
    }
}