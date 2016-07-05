namespace DSharpCompiler.Core.Models
{
    public class Node
    {
        public Node Left { get; set; }
        public Token Token { get; set; }
        public Node Right { get; set; }
    }
}
