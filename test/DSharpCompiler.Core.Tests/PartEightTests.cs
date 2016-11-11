using DSharpCompiler.Core.Common;
using Xunit;

namespace DSharpCompiler.Core.Tests
{
    public class PartEightTests
    {
        [Fact]
        public void UnaryTest()
        {
            var code = "BEGIN a := 2 + + - 2 END.";
            var interpreter = Interpreter.GetPascalInterpreter();
            var dictionary = interpreter.Interpret(code);
            var result = dictionary.GetValue<int>("a");
            Assert.Equal(0, result);
        }

        [Fact]
        public void NegativeUnaryTest()
        {
            var code = "BEGIN a := 5 - - - 2 END.";
            var interpreter = Interpreter.GetPascalInterpreter();
            var dictionary = interpreter.Interpret(code);
            var result = dictionary.GetValue<int>("a");
            Assert.Equal(3, result);
        }

        [Fact]
        public void UnaryAndPrecedenceTest()
        {
            var code = "BEGIN a := 5 - - - + - (3 + 4 * 2) - +2 END.";
            var interpreter = Interpreter.GetPascalInterpreter();
            var dictionary = interpreter.Interpret(code);
            var result = dictionary.GetValue<int>("a");
            Assert.Equal(14, result);
        }
    }
}