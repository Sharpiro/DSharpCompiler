using DSharpCompiler.Core.Common;
using Xunit;

namespace DSharpCompiler.Core.Tests
{
    public class ParenthesisTests
    {
        [Fact]
        public void SimpleProgramTest()
        {
            var code = @"
                func int doWork(int a)
                {
                    return a;
                };
                let a = doWork(2);";
            var interpreter = Interpreter.GetDsharpInterpreter();
            var dictionary = interpreter.Interpret(code);
            var a = dictionary.SymbolsTable.GetValue<int>("a");
            Assert.Equal(2, a);
        }

        [Fact]
        public void AdditionTest()
        {
            var code = @"
                func int add(int e, int f)
                {
                    return e + f;
                };
                let e = add(2, 4);";
            var interpreter = Interpreter.GetDsharpInterpreter();
            var dictionary = interpreter.Interpret(code);
            var e = dictionary.SymbolsTable.GetValue<int>("e");
            Assert.Equal(6, e);
        }

        [Fact]
        public void ScopeTest()
        {
            var code = @"
                func int add(int e, int f)
                {
                    return e + f;
                };
                let g = add(2, 4);";
            var interpreter = Interpreter.GetDsharpInterpreter();
            var dictionary = interpreter.Interpret(code);
            var e = dictionary.SymbolsTable.GetValue<int?>("e");
            var f = dictionary.SymbolsTable.GetValue<int?>("f");
            var g = dictionary.SymbolsTable.GetValue<int>("g");
            Assert.Equal(null, e);
            Assert.Equal(null, f);
            Assert.Equal(6, g);
        }
    }
}