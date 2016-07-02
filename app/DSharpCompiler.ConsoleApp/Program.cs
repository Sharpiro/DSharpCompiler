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
                var result = parser.Expression();
                Console.WriteLine(result);
            }
        }
    }
}
