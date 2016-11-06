namespace DSharpCompiler.Core.Common.Models
{
    public class DSharpObject { }

    public class DSharpAdder : DSharpObject
    {
        public int Add(int x, int y)
        {
            return x + y;
        }
    }
}