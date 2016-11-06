using System;
using System.Collections.Generic;

namespace DSharpCompiler.Core.Common.Models
{
    public class Node
    {
        public NodeType NodeType { get; set; }
        public Node ParentNode { get; set; }
        public Type ValueType { get; set; }

        public TChild Cast<TChild>() where TChild : Node
        {
            try
            {
                var cast = (TChild)this;
                return cast;
            }
            catch (InvalidCastException)
            {
                return null;
            }
        }
    }

    public abstract class ParentNode : Node
    {
        public string Name { get; set; }
        public IEnumerable<Node> Children { get; set; }
    }

    public class CompoundNode : ParentNode
    {
        public IEnumerable<Node> Parameters { get; set; }

        public CompoundNode(IEnumerable<Node> children, IEnumerable<Node> parameters)
        {
            Children = children;
            NodeType = NodeType.Compound;
            Parameters = parameters;
        }
    }

    public class RoutineNode : Node
    {
        public string Name { get; set; }

        public IEnumerable<Node> Arguments { get; set; }

        public RoutineNode(string name, IEnumerable<Node> arguments)
        {
            Name = name;
            NodeType = NodeType.Routine;
            Arguments = arguments;
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
            NodeType = NodeType.BinaryOp;
        }
    }

    public class UnaryNode : Node
    {
        public Token Token { get; set; }
        public Node Expression { get; set; }

        public UnaryNode(Token operater, Node expression)
        {
            NodeType = NodeType.UnaryOp;
            Token = operater;
            Expression = expression;
        }
    }

    public class VariableNode : Node
    {
        public Token Token { get; set; }
        public object Value { get; private set; }

        public VariableNode(Token token, NodeType nodeType)
        {
            NodeType = nodeType;
            Token = token;
            Value = Token.Value;
        }
    }

    public class EmptyNode : Node
    {
        public EmptyNode()
        {
            NodeType = NodeType.Empty;
        }
    }

    public enum NodeType
    {
        None, BinaryOp, UnaryOp, Numeric, String, Boolean, Compound, Conditional, Variable,
        Assignment, Return, Routine, Empty,
    }
}