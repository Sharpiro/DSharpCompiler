using DSharpCompiler.Core.Common;
using System;
using Xunit;

namespace DSharpCompiler.Core.Tests
{
    public class ReturnTests
    {
        [Fact]
        public void AssignFunctionTest()
        {
            var code = @"
                func int doWork()
                {
                    return 2;
                };
                let b = doWork();";
            var interpreter = Interpreter.GetDsharpInterpreter();
            var dictionary = interpreter.Interpret(code);
            var b = dictionary.GetValue<int>("b");
            Assert.Equal(2, b);
        }

        [Fact]
        public void ReturnBreakTest()
        {
            var code = @"
                func int doWork()
                {
                    return 2;
                    return 4;
                };
                let b = doWork();";
            var interpreter = Interpreter.GetDsharpInterpreter();
            Assert.Throws(typeof(InvalidOperationException), () => interpreter.Interpret(code));
        }

        [Fact]
        public void ComplexAssignTest()
        {
            var code = @"
                func int doWork()
                {
                    return 2 * 2 + - 3;
                };
                func int doMoreWork()
                {
                    return 2 / (2 + - 4);
                };
                let a = doWork;
                let b = doMoreWork;
                let c = doWork + doMoreWork;";
            var interpreter = Interpreter.GetDsharpInterpreter();
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