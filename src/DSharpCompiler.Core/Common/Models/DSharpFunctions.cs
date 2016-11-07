using System;

namespace DSharpCompiler.Core.Common.Models
{
    public class DSharpFunctions
    {
        public static int Add(int x, int y)
        {
            return x + y;
        }

        public static string Print(int x)
        {
            return x.ToString();
        }
    }
}