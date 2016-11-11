using DSharpCompiler.Core.Common;
using Xunit;
using DSharpCompiler.Core.Common.Exceptions;

namespace DSharpCompiler.Core.Tests
{
    public class ConditionalTests
    {
        [Fact]
        public void SimpleIsTrueTest()
        {
            var code = @"
                func int doWork()
                {
                    if (2 eq 2)
                    {
                        return 1;
                    };
                    return 2;
                };
                let b = doWork();";
            var interpreter = Interpreter.GetDsharpInterpreter();
            var dictionary = interpreter.Interpret(code);
            var b = dictionary.GetValue<object>("b");
            Assert.Equal(1, b);
        }

        [Fact]
        public void SimpleIsFalseTest()
        {
            var code = @"
                func int doWork()
                {
                    if (2 eq 3)
                    {
                        return 1;
                    };
                    return 0;
                };
                let b = doWork();";
            var interpreter = Interpreter.GetDsharpInterpreter();
            var dictionary = interpreter.Interpret(code);
            var b = dictionary.GetValue<int>("b");
            Assert.Equal(0, b);
        }

        [Fact]
        public void VariableTest()
        {
            var code = @"
                func int doWork(int n)
                {
                    if (n eq 2)
                    {
                        return 1;
                    };
                    return 0;
                };
                let b = doWork(2);";
            var interpreter = Interpreter.GetDsharpInterpreter();
            var dictionary = interpreter.Interpret(code);
            var b = dictionary.GetValue<int>("b");
            Assert.Equal(1, b);
        }

        [Fact]
        public void RecursionTest()
        {
            var code = @"
                func int fib(int n)
                {
                    if (n eq 0)
                    {
                        return 0;
                    };
                    if (n eq 1)
                    {
                        return 1;
                    };
                    return fib(n - 2) + fib(n - 1);
                };
                let b = fib(25);";
            var interpreter = Interpreter.GetDsharpInterpreter();
            var dictionary = interpreter.Interpret(code);
            var b = dictionary.GetValue<int>("b");
            Assert.Equal(75025, b);
        }

        [Fact]
        public void BlockInTest()
        {
            var code = @"
                func int test(int n)
                {
                    if (n eq 9)
                    {
                        return n;
                    };
                    return 0;
                };
                let b = test(9);";
            var interpreter = Interpreter.GetDsharpInterpreter();
            var dictionary = interpreter.Interpret(code);
            var b = dictionary.GetValue<int?>("b");
            Assert.Equal(9, b);
        }

        [Fact]
        public void BlockOutTest()
        {
            var code = @"
                func int test(int n)
                {
                    if (n eq 9)
                    {
                        let x = 3;
                    };
                    return x;
                };
                let b = test(9);";
            var interpreter = Interpreter.GetDsharpInterpreter();
            var expectedMessage = "tried to use a variable that was null";
            var exception = Assert.Throws<VariableNotFoundException>(() => interpreter.Interpret(code));
            Assert.Equal(expectedMessage, exception.Message);
        }
    }
}