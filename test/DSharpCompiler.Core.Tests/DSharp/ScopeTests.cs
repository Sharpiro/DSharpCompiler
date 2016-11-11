using DSharpCompiler.Core.Common;
using Xunit;

namespace DSharpCompiler.Core.Tests
{
    public class ScopeTests
    {
        [Fact]
        public void InScopeTest()
        {
            var code = @"
                let a = 2;
                func int doWork()
                {
                    let b = ""data"";
                    let c = a + 1;
                    return c;
                };
                let d = doWork();
                let a = 5;
                let e = doWork();";
            var interpreter = Interpreter.GetDsharpInterpreter();
            var dictionary = interpreter.Interpret(code);
            var a = dictionary.GetValue<int>("a");
            var b = dictionary.GetValue<string>("b");
            var c = dictionary.GetValue<int?>("c");
            var d = dictionary.GetValue<int>("d");
            var e = dictionary.GetValue<int>("e");
            Assert.Equal(5, a);
            Assert.Equal(null, b);
            Assert.Equal(null, c);
            Assert.Equal(3, d);
            Assert.Equal(6, e);
        }
    }
}