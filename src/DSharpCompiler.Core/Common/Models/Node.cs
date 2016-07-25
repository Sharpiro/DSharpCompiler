using System.Collections.Generic;

namespace DSharpCompiler.Core.Common.Models
{
    public class Node
    {
        public NodeType Type { get; set; }
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
            Type = NodeType.Compound;
            Parameters = parameters;
        }
    }

    //public class ConditionalNode : ParentNode
    //{
    //    //public Node ExpressionOne { get; set; }
    //    //public Node ExpressionTwo { get; set; }
    //    public IEnumerable<Node> Parameters { get; set; }

    //    public ConditionalNode(IEnumerable<Node> children)
    //    {
    //        Children = children;
    //        Type = NodeType.Conditional;
    //    }
    //}

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

    public class NumericNode : Node
    {
        public Token Token { get; set; }
        public int Value { get; private set; }

        public NumericNode(Token token)
        {
            Type = NodeType.Numeric;
            Token = token;
            Value = int.Parse(Token.Value);
        }
    }

    public class StringNode : Node
    {
        public Token Token { get; set; }
        public string Value { get; private set; }

        public StringNode(Token token)
        {
            Type = NodeType.String;
            Token = token;
            Value = Token.Value;
        }
    }

    public class BooleanNode : Node
    {
        public Token Token { get; set; }
        public bool Value { get; private set; }

        public BooleanNode(Token token)
        {
            Type = NodeType.Boolean;
            Token = token;
            Value = bool.Parse(token.Value);
        }
    }

    public class VariableNode : Node
    {
        public Token Token { get; set; }
        public string Value { get; private set; }

        public VariableNode(Token token)
        {
            Type = NodeType.Variable;
            Token = token;
            Value = Token.Value;
        }
    }

    public class RoutineNode : Node
    {
        public string RoutineName { get; set; }
        public IEnumerable<Node> Arguments { get; set; }
        public RoutineNode(string routineName, IEnumerable<Node> arguments)
        {
            RoutineName = routineName;
            Type = NodeType.Routine;
            Arguments = arguments;
        }
    }

    public class EmptyNode : Node
    {
        public EmptyNode()
        {
            Type = NodeType.Empty;
        }
    }

    public enum NodeType
    {
        None, BinaryOp, UnaryOp, Numeric, String, Boolean, Compound, Conditional, Variable,
        Assignment, Return, Routine, Empty,
    }
}