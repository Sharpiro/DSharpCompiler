using System;
using DSharpCompiler.Core.Models;

namespace DSharpCompiler.Core
{
    public class Interpreter
    {
        private readonly Node _root;

        public Interpreter(Node root)
        {
            _root = root;
        }

        public int Interpret()
        {
            var result = Visit(_root);
            return result;
        }

        public int Visit(Node node)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));
            if (node.Type == NodeType.BinaryOp)
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
            else
                throw new InvalidOperationException();
        }

        public int VisitBinaryOpNode(Node node)
        {
            var numericNode = node as BinaryNode;
            if (numericNode.Token.Value == "+")
            {
                return Visit(numericNode.Left) + Visit(numericNode.Right);
            }
            else if (numericNode.Token.Value == "-")
            {
                return Visit(numericNode.Left) - Visit(numericNode.Right);
            }
            else if (numericNode.Token.Value == "*")
            {
                return Visit(numericNode.Left) * Visit(numericNode.Right);
            }
            else if (numericNode.Token.Value == "/")
            {
                return Visit(numericNode.Left) / Visit(numericNode.Right);
            }
            else
                throw new InvalidOperationException();
        }
        public int VisitUnaryOpNode(Node node)
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

        public int VisitNumericNode(Node node)
        {
            var numericNode = node as NumericNode;
            return numericNode.Value;
        }
    }
}