using System.Collections.Generic;

namespace DSharpCompiler.Core.Models
{
    public class Node
    {
        public NodeType Type { get; set; }
    }

    public class CompoundNode : Node
    {
        public IEnumerable<Node> Children { get; set; }

        public CompoundNode()
        {
            Type = NodeType.Compound;
        }
    }

    public class BinaryNode : Node
    {
        public Node Left { get; set; }
        public Token Token { get; set; }
        public Node Right { get; set; }

        public BinaryNode(Node left, Token token, Node right)
        {
            Left = left;
            Token = token;
            Right = right;
            Type = NodeType.BinaryOp;
        }
    }

    public class UnaryNode : Node
    {
        public Token Token { get; set; }
        public Node Expression { get; set; }

        public UnaryNode(Token operater, Node expression)
        {
            Type = NodeType.UnaryOp;
            Token = operater;
            Expression = expression;
        }
    }

    public class ValueNode : Node
    {
        public Token Token { get; set; }
        public string Value { get; private set; }

        public ValueNode(Token token)
        {
            Type = NodeType.Numeric;
            Token = token;
            Value = Token.Value;
        }
    }

    public enum NodeType { None, BinaryOp, UnaryOp, Numeric, Compound }
}