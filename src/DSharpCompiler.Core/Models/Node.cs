namespace DSharpCompiler.Core.Models
{
    public class Node
    {
        public Node Left { get; set; }
        public Token Token { get; set; }
        public Node Right { get; set; }
        public NodeType Type { get; set; }
    }

    public enum NodeType { None, BinaryOp, UnaryOp, Number }
}
