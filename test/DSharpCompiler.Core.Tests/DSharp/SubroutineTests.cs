using DSharpCompiler.Core.Common;
using Xunit;

namespace DSharpCompiler.Core.Tests
{
    public class SubroutineTests
    {
        [Fact]
        public void InvalidReturnTest()
        {
            var code = @"
                func int add(int x, int y)
                {
                    return x + y;
                };
                let b = add(2, 6);
                dSharpFunctions.print(b);";
            var interpreter = Interpreter.GetDsharpInterpreter();
            var symbols = interpreter.Interpret(code);
            var b = symbols.GetValue<int>("b");
            Assert.Equal(8, b);
        }
    }
}