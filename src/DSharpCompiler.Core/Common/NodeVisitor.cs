using DSharpCompiler.Core.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DSharpCompiler.Core.Common
{
    public class NodeVisitor
    {
        private SymbolsTable _symbols;

        public SymbolsTable VisitNodes(Node root)
        {
            _symbols = new SymbolsTable();
            Visit(root);
            return _symbols;
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
                _symbols.Add(compoundNode.Name, new Symbol(compoundNode));
                var mainRoutineNode = new RoutineNode(compoundNode.Name, new List<Node>());
                VisitRoutineNode(mainRoutineNode);
            }
            else
                _symbols.Add(compoundNode.Name, new Symbol(compoundNode));
            return null;
        }

        private int? VisitAssignmentNode(Node node)
        {
            var assignmentNode = node as BinaryNode;
            var left = assignmentNode.Left as VariableNode;
            var variableName = left.Value;
            _symbols.Add(variableName, new Symbol(Visit(assignmentNode.Right)));
            return null;
        }

        private object VisitVariableNode(Node node)
        {
            var variableNode = node as VariableNode;
            var symbol = _symbols.Get(variableNode.Value);
            if (symbol == null)
                throw new NullReferenceException("tried to use a variable that was null");
            if (symbol.Type == typeof(int))
            {
                return symbol.Value;
            }
            else if (symbol.Type == typeof(string))
            {
                return symbol.Value;
            }
            else
            {
                var valueNode = (CompoundNode)symbol.Value;
                var routineNode = new RoutineNode(valueNode.Name, new List<Node>());
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
        private object VisitUnaryOpNode(Node node)
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
                return Visit(unaryNode.Expression);
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

        private IEnumerable<object> VisitArgumentNodes(IEnumerable<Node> nodes)
        {
            var args = new List<object>();
            foreach (var node in nodes)
            {
                args.Add(Visit(node));
            }
            return args;
        }

        private object VisitRoutineNode(Node node)
        {
            var routineNode = node as RoutineNode;
            var symbol = _symbols.Get(routineNode.RoutineName);
            var compoundNode = symbol.Value as CompoundNode;
            object returnValue = null;
            _symbols.AddNewScope();
            _symbols.AddNodes(compoundNode.Parameters, VisitArgumentNodes(routineNode.Arguments));
            foreach (var child in compoundNode.Children.Where(c => c.Type != NodeType.Empty))
            {
                returnValue = Visit(child);
            }
            _symbols.RemoveCurrentScope();
            return returnValue;
        }
    }
}