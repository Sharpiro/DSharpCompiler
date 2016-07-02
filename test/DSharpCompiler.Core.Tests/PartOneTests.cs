using System;
using System.Linq;
using Xunit;

namespace DSharpCompiler.Core.Tests
{
    public class PartOneTests
    {
        [Fact]
        public void BasicAdditionTest()
        {
            var code = "3+5";
            //var code = "\"hello\" public 555 temp.Do() class temp.Do()";
            var lexer = new LexicalAnalyzer(code);
            var tokens = lexer.Analayze();
            var parser = new TokenParser(tokens.ToList());
            var result = parser.Expression();
            Assert.Equal(8, result);
        }

        [Fact]
        public void MultiDigitTest()
        {
            var code = "12+3";
            var lexer = new LexicalAnalyzer(code);
            var tokens = lexer.Analayze();
            var parser = new TokenParser(tokens.ToList());
            var result = parser.Expression();
            Assert.Equal(15, result);
        }

        [Fact]
        public void WhiteSpaceTest()
        {
            var code = " 12 + 3";
            var lexer = new LexicalAnalyzer(code);
            var tokens = lexer.Analayze();
            var parser = new TokenParser(tokens.ToList());
            var result = parser.Expression();
            Assert.Equal(15, result);
        }

        [Fact]
        public void SubtractionTest()
        {
            var code = "7-5";
            var lexer = new LexicalAnalyzer(code);
            var tokens = lexer.Analayze();
            var parser = new TokenParser(tokens.ToList());
            var result = parser.Expression();
            Assert.Equal(2, result);
        }

        [Fact]
        public void InvalidTest()
        {
            var code = "11";
            var lexer = new LexicalAnalyzer(code);
            var tokens = lexer.Analayze();
            var parser = new TokenParser(tokens.ToList());
            Action action = () => parser.Expression();
            Assert.Throws(typeof(IndexOutOfRangeException), action);
        }
    }
}