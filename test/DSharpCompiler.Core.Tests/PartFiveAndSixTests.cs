using DSharpCompiler.Core.Common;
using Xunit;

namespace DSharpCompiler.Core.Tests
{
    public class PartFiveAndSixTests
    {
        [Fact]
        public void PrecedenceTest()
        {
            var code = "BEGIN a := 14 + 2 * 3 - 6 / 2 END.";
            var interpreter = Interpreter.GetPascalInterpreter();
            var dictionary = interpreter.Interpret(code);
            var result = dictionary.SymbolsTable.GetValue<int>("a");
            Assert.Equal(17, result);
        }

        [Fact]
        public void ShallowNestingTest()
        {
            var code = "BEGIN a := 2 * (7 + 3); END.";
            var interpreter = Interpreter.GetPascalInterpreter();
            var dictionary = interpreter.Interpret(code);
            var result = dictionary.SymbolsTable.GetValue<int>("a");
            Assert.Equal(20, result);
        }

        [Fact]
        public void DeepNestingTest()
        {
            var code = "BEGIN a := 7 + 3 * (10 / (12 / (3 + 1) - 1)); END.";
            var interpreter = Interpreter.GetPascalInterpreter();
            var dictionary = interpreter.Interpret(code);
            var result = dictionary.SymbolsTable.GetValue<int>("a");
            Assert.Equal(22, result);
        }
        [Fact]
        public void DeepDeepNestingTest()
        {
            var code = "BEGIN a := 7 + 3 * (10 / (12 / (3 + 1) - 1)) / (2 + 3) - 5 - 3 + (8) END.";
            var interpreter = Interpreter.GetPascalInterpreter();
            var dictionary = interpreter.Interpret(code);
            var result = dictionary.SymbolsTable.GetValue<int>("a");
            Assert.Equal(10, result);
        }
    }
}