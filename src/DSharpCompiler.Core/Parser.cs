using DSharpCompiler.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DSharpCompiler.Core
{
    public class TokenParser
    {
        private readonly IList<Token> _tokens;
        private int _currentIndex;
        private Dictionary<string, string> _variables;
        private Token _currentToken;

        public TokenParser(IList<Token> tokens)
        {
            _tokens = tokens;
            _variables = new Dictionary<string, string>();
            _currentToken = _tokens.FirstOrDefault();
        }

        public Node Program()
        {
            var node = CompoundStatement();
            EatToken(TokenType.Symbol);
            return node;
        }

        private Node CompoundStatement()
        {
            EatToken(TokenType.Keyword);
            var children = StatementList();
            var node = new CompoundNode(children);
            EatToken(TokenType.Keyword);
            return node;
        }

        private IEnumerable<Node> StatementList()
        {
            var node = Statement();

            var nodes = new List<Node> { node };
            while (_currentToken.Value == ";")
            {
                EatToken(TokenType.Symbol);
                nodes.Add(Statement());
            }
            return nodes;
        }

        private Node Statement()
        {
            Node node = null;
            if (_currentToken.Value.ToLower() == "begin")
            {
                node = CompoundStatement();
            }
            else if (_currentToken.Type == TokenType.Identifier)
            {
                node = AssignmentStatement();
            }
            else
                node = Empty();
            return node;
        }

        private Node AssignmentStatement()
        {
            var variable = Variable();
            var token = _currentToken;
            EatToken(TokenType.Symbol);
            var node = new BinaryNode(variable, token, Expression()) { Type = NodeType.Assignment };
            return node;
        }

        private Node Empty()
        {
            var node = new EmptyNode();
            return node;
        }

        private Node Expression()
        {
            var node = Term();

            while (_currentToken != null && _currentToken.Value.In("+", "-"))
            {
                var token = _currentToken;
                EatToken(TokenType.Symbol);
                node = new BinaryNode(node, token, Term());
            }
            return node;
        }

        private Node Term()
        {
            var node = Factor();
            while (_currentToken != null && _currentToken.Value.In("*", "/"))
            {
                var token = _currentToken;
                EatToken(TokenType.Symbol);
                node = new BinaryNode(node, token, Factor());
            }
            return node;
        }

        private Node Factor()
        {
            var token = _currentToken;
            Node node = null;
            if (token.Type == TokenType.NumericConstant)
            {
                EatToken(TokenType.NumericConstant);
                node = new NumericNode(token);
            }
            else if (token.Value.In("+", "-"))
            {
                EatToken(TokenType.Symbol);
                node = new UnaryNode(token, Factor());
            }
            else if (token.Value == "(")
            {
                EatToken(TokenType.Symbol);
                node = Expression();
                EatToken(TokenType.Symbol);
            }
            else
                node = Variable();
            return node;
        }

        private Node Variable()
        {
            var node = new VariableNode(_currentToken);
            EatToken(TokenType.Identifier);
            return node;
        }

        private void EatToken(TokenType type)
        {
            if (_currentToken == null || _currentToken.Type != type)
                throw new IndexOutOfRangeException();
            _currentToken = NextToken();
        }

        private Token PeekToken()
        {
            try
            {
                return _tokens[_currentIndex + 1];
            }
            catch (ArgumentOutOfRangeException) { }
            return null;
        }

        private Token NextToken()
        {
            try
            {
                _currentIndex++;
                return _tokens[_currentIndex];
            }
            catch (ArgumentOutOfRangeException) { }
            return null;
        }
    }
}