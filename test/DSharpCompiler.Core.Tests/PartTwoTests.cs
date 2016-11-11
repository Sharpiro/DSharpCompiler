using DSharpCompiler.Core.Common;
using Xunit;

namespace DSharpCompiler.Core.Tests
{
    public class PartTwoTests
    {
        [Fact]
        public void AddtionTest()
        {
            var code = "BEGIN a := 1+1+1 END.";
            var interpreter = Interpreter.GetPascalInterpreter();
            var dictionary = interpreter.Interpret(code);
            var result = dictionary.GetValue<int>("a");
            Assert.Equal(3, result);
        }

        [Fact]
        public void MultiplicationTest()
        {
            var code = "BEGIN a := 5*3 END.";
            var interpreter = Interpreter.GetPascalInterpreter();
            var dictionary = interpreter.Interpret(code);
            var result = dictionary.GetValue<int>("a");
            Assert.Equal(15, result);
        }

        [Fact]
        public void DivisionTest()
        {
            var code = "BEGIN a := 12/3 END.";
            var interpreter = Interpreter.GetPascalInterpreter();
            var dictionary = interpreter.Interpret(code);
            var result = dictionary.GetValue<int>("a");
            Assert.Equal(4, result);
        }

        [Fact]
        public void ContinuousOperationsTest()
        {
            var code = "BEGIN a := 9 - 5 + 3 + 11 END.";
            var interpreter = Interpreter.GetPascalInterpreter();
            var dictionary = interpreter.Interpret(code);
            var result = dictionary.GetValue<int>("a");
            Assert.Equal(18, result);
        }

        [Fact]
        public void MultiplicationAndDivisionTest()
        {
            var code = "BEGIN a := 7 * 4 / 2 * 3 END.";
            var interpreter = Interpreter.GetPascalInterpreter();
            var dictionary = interpreter.Interpret(code);
            var result = dictionary.GetValue<int>("a");
            Assert.Equal(42, result);
        }
    }
}