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
            else if (node.Type == NodeType.Number)
            {
                return int.Parse(node.Token.Value);
            }
            else
                throw new InvalidOperationException();
        }

        public int VisitBinaryOpNode(Node node)
        {
            if (node.Token.Value == "+")
            {
                return Visit(node.Left) + Visit(node.Right);
            }
            else if (node.Token.Value == "-")
            {
                return Visit(node.Left) - Visit(node.Right);
            }
            else if (node.Token.Value == "*")
            {
                return Visit(node.Left) * Visit(node.Right);
            }
            else if (node.Token.Value == "/")
            {
                return Visit(node.Left) / Visit(node.Right);
            }
            else
                throw new InvalidOperationException();
        }
        public int VisitUnaryOpNode(Node node)
        {
            if (node.Token.Value == "+")
            {
                return +Visit(node.Right);
            }
            else if (node.Token.Value == "-")
            {
                return -Visit(node.Right);
            }
            else
                throw new InvalidOperationException();
        }
    }
}