﻿using System.Collections.Generic;

namespace DSharpCompiler.Core.Common.Models
{
    public class Node
    {
        public NodeType Type { get; set; }
    }

    public class CompoundNode : Node
    {
        public string Name { get; set; }
        public IEnumerable<Node> Children { get; set; }

        public CompoundNode(IEnumerable<Node> children)
        {
            Children = children;
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
        public RoutineNode(string routineName)
        {
            RoutineName = routineName;
            Type = NodeType.Routine;
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
        None, BinaryOp, UnaryOp, Numeric, String, Compound, Variable,
        Assignment, Routine, Empty,
    }
}