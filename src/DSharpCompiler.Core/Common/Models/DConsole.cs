namespace DSharpCompiler.Core.Common.Models
{
    public class DConsole
    {
        public static string PrintInt(int x)
        {
            return x.ToString();
        }

        public static string PrintString(string x)
        {
            return x.ToString();
        }
    }

    public class DFunctions
    {
        public static int Add(int x, int y)
        {
            return x + y;
        }
    }
}