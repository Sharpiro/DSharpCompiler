using DSharpCompiler.Core.Common.Exceptions;
using DSharpCompiler.Core.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DSharpCompiler.Core.Common
{
    public class NodeVisitor
    {
        private SymbolsTable _symbols;
        private readonly TypesTable _typesTable;
        private readonly StringBuilder _console = new StringBuilder();

        public NodeVisitor(TypesTable typesTable)
        {
            _typesTable = typesTable;
        }

        public InterpretResult VisitNodes(Node root)
        {
            _symbols = new SymbolsTable();
            Visit(root);
            var interpretResult = new InterpretResult
            {
                SymbolsTable = _symbols,
                ConsoleOutput = _console.ToString()
            };
            return interpretResult;
        }

        private object Visit(Node node)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            if (node.NodeType == NodeType.Compound)
            {
                return VisitCompoundNode(node);
            }
            else if (node.NodeType == NodeType.CustomType)
            {
                return VisitCustomTypeNode(node);
            }
            else if (node.NodeType == NodeType.Conditional)
            {
                return VisitConditionalNode(node);
            }
            else if (node.NodeType == NodeType.Assignment)
            {
                return VisitAssignmentNode(node);
            }
            else if (node.NodeType == NodeType.Variable)
            {
                return VisitVariableNode(node);
            }
            else if (node.NodeType == NodeType.BinaryOp)
            {
                return VisitBinaryOpNode(node);
            }
            else if (node.NodeType == NodeType.UnaryOp || node.NodeType == NodeType.Return)
            {
                return VisitUnaryOpNode(node);
            }
            else if (node.NodeType == NodeType.Numeric)
            {
                return VisitNumericNode(node);
            }
            else if (node.NodeType == NodeType.String)
            {
                return VisitStringNode(node);
            }
            else if (node.NodeType == NodeType.Routine)
            {
                return VisitRoutineNode(node);
            }
            else if (node.NodeType == NodeType.Empty)
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

        private int? VisitCustomTypeNode(Node node)
        {
            var customTypeNode = node.Cast<CustomTypeNode>();
            foreach (CompoundNode compoundNode in customTypeNode.Children)
            {
                VisitCompoundNode(compoundNode);
            }
            return null;
        }

        private object VisitConditionalNode(Node node)
        {
            var compoundNode = node as CompoundNode;
            var exp1 = Visit(compoundNode.Parameters.FirstOrDefault());
            var exp2 = Visit(compoundNode.Parameters.LastOrDefault());
            object returnValue = null;
            if ((int)exp1 == (int)exp2)
            {
                var guid = Guid.NewGuid().ToString();
                compoundNode.Name = guid;
                _symbols.Add(compoundNode.Name, new Symbol(compoundNode));
                var mainRoutineNode = new RoutineNode(compoundNode.Name, new List<Node>());
                returnValue = VisitRoutineNode(mainRoutineNode);
            }
            return returnValue;
        }

        private int? VisitAssignmentNode(Node node)
        {
            var assignmentNode = node as BinaryNode;
            var left = assignmentNode.Left as VariableNode;
            var variableName = (string)left.Value;
            _symbols.Add(variableName, new Symbol(Visit(assignmentNode.Right)));
            return null;
        }

        private object VisitVariableNode(Node node)
        {
            var variableNode = node as VariableNode;
            var symbol = _symbols.Get((string)variableNode.Value);
            if (symbol == null)
                throw new VariableNotFoundException("tried to use a variable that was null");
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
            var valueNode = node as VariableNode;
            return Convert.ToInt32(valueNode.Value);
        }

        private string VisitStringNode(Node node)
        {
            var valueNode = node as VariableNode;
            return (string)valueNode.Value;
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
            var callingType = _typesTable.GetCallingType(routineNode.Name);
            return callingType == null || callingType == typeof(object)
                ? VisitDSharpRoutineNode(routineNode)
                : VisitLibraryRoutineNode(routineNode, callingType);
        }

        private object VisitDSharpRoutineNode(RoutineNode node)
        {
            var symbol = _symbols.Get(node.Name);
            if (symbol == null)
                throw new TypeNotFoundException(node.Name);
            var compoundNode = symbol.Value as CompoundNode;
            object returnValue = null;
            var argumentNodes = VisitArgumentNodes(node.Arguments);
            _symbols.AddNewScope();
            _symbols.AddNodes(compoundNode.Parameters, argumentNodes);
            if (compoundNode.NodeType == NodeType.Conditional)
                _symbols.AddParentScopes();
            foreach (var child in compoundNode.Children.Where(c => c.NodeType != NodeType.Empty))
            {
                returnValue = Visit(child);
                if (returnValue != null)
                    break;
            }
            _symbols.RemoveCurrentScope();
            return returnValue;
        }

        private object VisitLibraryRoutineNode(RoutineNode node, Type callingType)
        {
            var method = _typesTable.GetSubroutine(node.Name);
            var argumentValues = node.Arguments.Select(a => Visit(a)).ToArray();
            var result = method.DynamicInvoke(argumentValues);
            if (callingType == typeof(DConsole))
            {
                _console.AppendLine(result.ToString());
                return null;
            }
            return result;
        }
    }
}