using DSharpCompiler.Core.Common;
using Xunit;

namespace DSharpCompiler.Core.Tests
{
    public class CustomTypesTests
    {
        [Fact]
        public void SimpleCustomTypeTest()
        {
            var code = @"
                type functions
                {
                    func int add(int x, int y)
                    {
                        return x + y;
                    };
                };
                let b = functions.add(2, 6);
                dConsole.printInt(b);";
            var interpreter = Interpreter.GetDsharpInterpreter();
            var result = interpreter.Interpret(code);
            var b = result.SymbolsTable.GetValue<int>("b");
            Assert.Equal(8, b);
            Assert.Equal("8\r\n", result.ConsoleOutput);
        }

        [Fact]
        public void ComplexCustomTypeTest()
        {
            var code = @"
                type functions
                {
                    func int add(int x, int y)
                    {
                        return x + y;
                    };

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
                        return functions.fib(n - 2) + functions.fib(n - 1);
                    };
                };
                type moreFunctions
                {
                    func int subtract(int x, int y)
                    {
                        return x - y;
                    };
                };
                let b = functions.add(2, 6);
                let c = moreFunctions.subtract(2, 6);
                let d = b - c;
                let e = functions.fib(18);
                dConsole.printInt(b);
                dConsole.printInt(c);
                dConsole.printInt(d);
                dConsole.printInt(e);";
            var interpreter = Interpreter.GetDsharpInterpreter();
            var result = interpreter.Interpret(code);
            var b = result.SymbolsTable.GetValue<int>("b");
            var c = result.SymbolsTable.GetValue<int>("c");
            var d = result.SymbolsTable.GetValue<int>("d");
            var e = result.SymbolsTable.GetValue<int>("e");
            Assert.Equal(8, b);
            Assert.Equal(-4, c);
            Assert.Equal(12, d);
            Assert.Equal(2584, e);
            Assert.Equal("8\r\n-4\r\n12\r\n2584\r\n", result.ConsoleOutput);
        }
    }
}