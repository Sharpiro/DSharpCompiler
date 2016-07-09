using DSharpCompiler.Core;
using DSharpCompiler.Core.Common;
using DSharpCompiler.Core.DSharp;
using DSharpCompiler.Core.Pascal;
using System;
using System.Linq;

namespace DSharpCompiler.ConsoleApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var isRunning = true;
            while (isRunning)
            {
                Console.WriteLine("Enter source");
                var source = Console.ReadLine();
                var pascalTokens = new PascalTokens();
                var lexer = new LexicalAnalyzer(pascalTokens);
                var parser = new DSharpParser();
                var interpreter = new NodeVisitor();
                var wrapper = new Interpreter(lexer, parser, interpreter);
                var result = wrapper.Interpret(source);
                Console.WriteLine(result);
            }
        }
    }
}