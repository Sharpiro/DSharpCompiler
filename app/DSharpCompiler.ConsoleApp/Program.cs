using DSharpCompiler.Core;
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
                var analyzer = new LexicalAnalyzer(source);
                var tokens = analyzer.Analayze();
                var parser = new TokenParser(tokens.ToList());
                var rootNode = parser.Program();
                var interpreter = new Interpreter(rootNode);
                var result = interpreter.Interpret();
                Console.WriteLine(result);
            }
        }
    }
}