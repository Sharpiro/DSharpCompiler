using DSharpCompiler.Core.Common.Models;
using System;
using System.Collections.Generic;

namespace DSharpCompiler.Core.Common
{
    public class NodeVisitor
    {
        private Dictionary<string, object> _globalData;

        public Dictionary<string, object> VisitNodes(Node root)
        {
            _globalData = new Dictionary<string, object>();
            Visit(root);
            return _globalData;
        }

        private object Visit(Node node)
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
            else if (node.Type == NodeType.String)
            {
                return VisitStringNode(node);
            }
            else if (node.Type == NodeType.Routine)
            {
                return VisitRoutineNode(node);
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
            if (compoundNode.Name == "main" || compoundNode.Name == null)
            {
                var guid = Guid.NewGuid().ToString();
                compoundNode.Name = guid;
                _globalData[compoundNode.Name] = compoundNode;
                var mainRoutineNode = new RoutineNode(compoundNode.Name);
                VisitRoutineNode(mainRoutineNode);
            }
            else
                _globalData[compoundNode.Name] = compoundNode;
            return null;
        }

        private int? VisitAssignmentNode(Node node)
        {
            var assignmentNode = node as BinaryNode;
            var left = assignmentNode.Left as VariableNode;
            var variableName = left.Value;
            _globalData[variableName] = Visit(assignmentNode.Right);
            return null;
        }

        private object VisitVariableNode(Node node)
        {
            var variableNode = node as VariableNode;
            var value = _globalData[variableNode.Value];
            if (value == null)
                throw new NullReferenceException("tried to use a variable that was null");
            if (value.GetType() == typeof(int))
            {
                return value;
            }
            else if (value.GetType() == typeof(string))
            {
                return value;
            }
            else
            {
                var valueNode = (CompoundNode)value;
                var routineNode = new RoutineNode(valueNode.Name);
                return VisitRoutineNode(routineNode);
            }
        }

        private int? VisitEmptyNode(Node node)
        {
            return null;
        }

        private int? VisitBinaryOpNode(Node node)
        {
            var binaryNode = node as BinaryNode;
            if (binaryNode.Token.Value == "+")
            {
                return Visit(binaryNode.Left).Add(Visit(binaryNode.Right));
            }
            else if (binaryNode.Token.Value == "-")
            {
                return Visit(binaryNode.Left).Subtract(Visit(binaryNode.Right));
            }
            else if (binaryNode.Token.Value == "*")
            {
                return Visit(binaryNode.Left).Multiply(Visit(binaryNode.Right));
            }
            else if (binaryNode.Token.Value == "/")
            {
                return Visit(binaryNode.Left).Divide(Visit(binaryNode.Right));
            }
            else
                throw new InvalidOperationException();
        }
        private int? VisitUnaryOpNode(Node node)
        {
            var unaryNode = node as UnaryNode;
            if (unaryNode.Token.Value == "+")
            {
                return (int)Visit(unaryNode.Expression);
            }
            else if (unaryNode.Token.Value == "-")
            {
                return Visit(unaryNode.Expression).UnaryMinus();
            }
            else if (unaryNode.Token.Value == "return")
            {
                return (int)Visit(unaryNode.Expression);
            }
            else
                throw new InvalidOperationException();
        }

        private int? VisitNumericNode(Node node)
        {
            var valueNode = node as NumericNode;
            return valueNode.Value;
        }

        private string VisitStringNode(Node node)
        {
            var valueNode = node as StringNode;
            return valueNode.Value;
        }

        private object VisitRoutineNode(Node node)
        {
            var routineNode = node as RoutineNode;
            var compoundNode = _globalData[routineNode.RoutineName] as CompoundNode;
            object returnValue = null;
            foreach (var child in compoundNode.Children)
            {
                returnValue = returnValue.Add(Visit(child));
            }
            return returnValue;
        }
    }
}