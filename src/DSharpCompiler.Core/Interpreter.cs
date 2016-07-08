using System;
using DSharpCompiler.Core.Models;
using System.Collections.Generic;

namespace DSharpCompiler.Core
{
    public class Interpreter
    {
        private readonly Node _root;
        private readonly Dictionary<string, object> _globalData;

        public Interpreter(Node root)
        {
            _root = root;
            _globalData = new Dictionary<string, object>();
        }

        public Dictionary<string, object> Interpret()
        {
            Visit(_root);
            var result = _globalData;
            return result;
        }

        public int? Visit(Node node)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            if (node.Type == NodeType.Compound)
            {
                return VisitCompoundNode(node);
            }
            else if (node.Type == NodeType.Assignment)
            {
                return VisitAssignmentNode(node);
            }
            else if (node.Type == NodeType.Variable)
            {
                return VisitVariableNode(node);
            }
            else if (node.Type == NodeType.BinaryOp)
            {
                return VisitBinaryOpNode(node);
            }
            else if (node.Type == NodeType.UnaryOp)
            {
                return VisitUnaryOpNode(node);
            }
            else if (node.Type == NodeType.Numeric)
            {
                return VisitNumericNode(node);
            }
            else if (node.Type == NodeType.Empty)
            {
                return VisitEmptyNode(node);
            }
            else
                throw new InvalidOperationException();
        }

        private int? VisitCompoundNode(Node node)
        {
            var compoundNode = node as CompoundNode;
            foreach (var child in compoundNode.Children)
            {
                Visit(child);
            }
            return null;
        }

        private int? VisitAssignmentNode(Node node)
        {
            var assignmentNode = node as BinaryNode;
            var left = assignmentNode.Left as VariableNode;
            var variableName = left.Value;
            _globalData[variableName] = Visit(assignmentNode.Right).Value;
            return null;
        }

        private int? VisitVariableNode(Node node)
        {
            var variableNode = node as VariableNode;
            var value = _globalData[variableNode.Value];
            if (value == null)
                throw new NullReferenceException("tried to use a variable that was null");
            return (int)value;
        }

        private int? VisitEmptyNode(Node node)
        {
            return null;
        }

        public int? VisitBinaryOpNode(Node node)
        {
            var numericNode = node as BinaryNode;
            if (numericNode.Token.Value == "+")
            {
                return Visit(numericNode.Left).Add(Visit(numericNode.Right));
            }
            else if (numericNode.Token.Value == "-")
            {
                return Visit(numericNode.Left).Subtract(Visit(numericNode.Right));
            }
            else if (numericNode.Token.Value == "*")
            {
                return Visit(numericNode.Left).Multiply(Visit(numericNode.Right));
            }
            else if (numericNode.Token.Value == "/")
            {
                return Visit(numericNode.Left).Divide(Visit(numericNode.Right));
            }
            else
                throw new InvalidOperationException();
        }
        public int? VisitUnaryOpNode(Node node)
        {
            var unaryNode = node as UnaryNode;
            if (unaryNode.Token.Value == "+")
            {
                return +Visit(unaryNode.Expression);
            }
            else if (unaryNode.Token.Value == "-")
            {
                return -Visit(unaryNode.Expression);
            }
            else
                throw new InvalidOperationException();
        }

        public int? VisitNumericNode(Node node)
        {
            var valueNode = node as NumericNode;
            return valueNode.Value;
        }
    }
}