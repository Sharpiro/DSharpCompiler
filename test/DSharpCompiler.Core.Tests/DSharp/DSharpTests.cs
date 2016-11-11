using DSharpCompiler.Core.Common;
using Xunit;

namespace DSharpCompiler.Core.Tests
{
    public class DSharpTests
    {
        [Fact]
        public void SimpleProgramTest()
        {
            var code = @"
                func void doWork()
                {
                    let a = 2;
                    return a;
                };
                let a = doWork();";
            var interpreter = Interpreter.GetDsharpInterpreter();
            var dictionary = interpreter.Interpret(code);
            var a = dictionary.GetValue<int>("a");
            Assert.Equal(2, a);
        }

        [Fact]
        public void ReplTest()
        {
            var code = @"
                let a = 1;
                func void doWork()
                {
                    let b = 2;
                };
                let c = 3;";
            var interpreter = Interpreter.GetDsharpInterpreter();
            var dictionary = interpreter.Interpret(code);
            var a = dictionary.GetValue<int>("a");
            var b = dictionary.GetValue<object>("b");
            var c = dictionary.GetValue<int>("c");
            Assert.Equal(1, a);
            Assert.Equal(null, b);
            Assert.Equal(3, c);
        }

        [Fact]
        public void FunctionCallTest()
        {
            var code = @"
                let a = 1;
                func void doWork()
                {
                    let b = 2;
                };
                func void doMoreWork()
                {
                    let c = 3;
                };
                let d = 4;
                doWork();
                doMoreWork();";
            var interpreter = Interpreter.GetDsharpInterpreter();
            var dictionary = interpreter.Interpret(code);
            var a = dictionary.GetValue<int>("a");
            var b = dictionary.GetValue<int?>("b");
            var c = dictionary.GetValue<int?>("c");
            var d = dictionary.GetValue<int>("d");
            Assert.Equal(1, a);
            Assert.Equal(null, b);
            Assert.Equal(null, c);
            Assert.Equal(4, d);
        }

        [Fact]
        public void AssignmentStringSimpleTest()
        {
            var code = @"let a = ""hello world"";";
            var interpreter = Interpreter.GetDsharpInterpreter();
            var dictionary = interpreter.Interpret(code);
            var a = dictionary.GetValue<string>("a");
            Assert.Equal("hello world", a);
        }

        [Fact]
        public void AssignmentStringComplexTest()
        {
            var code = @"
                let a = 1;
                func void doWork()
                {
                    let b = 2;
                };
                func string doMoreWork()
                {
                    let f = ""hello world"";
                    return f;
                };
                let d = 4;
                doWork();
                let c = doMoreWork();";
            var interpreter = Interpreter.GetDsharpInterpreter();
            var dictionary = interpreter.Interpret(code);
            var a = dictionary.GetValue<int>("a");
            var b = dictionary.GetValue<int?>("b");
            var c = dictionary.GetValue<string>("c");
            var d = dictionary.GetValue<int>("d");
            Assert.Equal(1, a);
            Assert.Equal(null, b);
            Assert.Equal("hello world", c);
            Assert.Equal(4, d);
        }

        [Fact]
        public void StringVariableUseTest()
        {
            var code = @"
                let c = ""hello world"";
                let d = c";
            var interpreter = Interpreter.GetDsharpInterpreter();
            var dictionary = interpreter.Interpret(code);
            var c = dictionary.GetValue<string>("c");
            var d = dictionary.GetValue<string>("d");
            Assert.Equal("hello world", c);
            Assert.Equal(c, d);
        }
    }
}