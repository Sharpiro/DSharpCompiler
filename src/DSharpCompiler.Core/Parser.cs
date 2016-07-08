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

        public Node Expression()
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
                throw new InvalidOperationException();
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