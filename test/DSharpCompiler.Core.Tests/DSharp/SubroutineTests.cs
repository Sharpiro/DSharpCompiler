using DSharpCompiler.Core.Common;
using Xunit;

namespace DSharpCompiler.Core.Tests
{
    public class SubroutineTests
    {
        [Fact]
        public void SimplePrintTest()
        {
            var code = @"
                func int add(int x, int y)
                {
                    return x + y;
                };
                let b = add(2, 6);
                dConsole.printInt(b);";
            var interpreter = Interpreter.GetDsharpInterpreter();
            var result = interpreter.Interpret(code);
            var b = result.SymbolsTable.GetValue<int>("b");
            Assert.Equal(8, b);
            Assert.Equal("8\r\n", result.ConsoleOutput);
        }

        [Fact]
        public void PrintTest()
        {
            var code = @"
                dConsole.printString(""1"");
                dConsole.printString(""2"");
                dConsole.printString(""3"");
                dConsole.printString(""4"");
                dConsole.printString(""5"");";
            var interpreter = Interpreter.GetDsharpInterpreter();
            var interpretResult = interpreter.Interpret(code);
            const string result = "1\r\n2\r\n3\r\n4\r\n5\r\n";
            Assert.Equal(result, interpretResult.ConsoleOutput);
        }

        [Fact]
        public void ConsoleAndFunctionsLibTest()
        {
            var code = @"
                let x = dFunctions.add(2, 3);
                dConsole.printInt(x);";
            var interpreter = Interpreter.GetDsharpInterpreter();
            var interpretResult = interpreter.Interpret(code);
            const string result = "5\r\n";
            Assert.Equal(result, interpretResult.ConsoleOutput);
        }
    }
}