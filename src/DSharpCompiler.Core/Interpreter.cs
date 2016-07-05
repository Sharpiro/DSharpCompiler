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
                throw new InvalidOperationException();
            if (node.Token.Type == TokenType.Symbol)
            {
                if (node.Token.Value == "+")
                {
                    return Visit(node.Left) + Visit(node.Right);
                }
                if (node.Token.Value == "-")
                {
                    return Visit(node.Left) - Visit(node.Right);
                }
                if (node.Token.Value == "*")
                {
                    return Visit(node.Left) * Visit(node.Right);
                }
                if (node.Token.Value == "/")
                {
                    return Visit(node.Left) / Visit(node.Right);
                }
                else
                    throw new InvalidOperationException();
            }
            else if (node.Token.Type == TokenType.NumericConstant)
            {
                return int.Parse(node.Token.Value);
            }
            else
                throw new InvalidOperationException();
        }
    }
}