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
                //if (_currentToken.Value == "+")
                //{
                //    result += Term();
                //}
                //else if (_currentToken.Value == "-")
                //{
                //    EatToken(TokenType.Symbol);
                //    result -= Term();
                //}
                //else
                //    throw new InvalidOperationException();
                EatToken(TokenType.Symbol);
                node = new Node { Left = node, Token = token, Right = Term() };
            }
            return node;
        }

        private Node Term()
        {
            var node = Factor();
            while (_currentToken != null && _currentToken.Value.In("*", "/"))
            {
                var token = _currentToken;
                //if (_currentToken.Value.In("*", "/"))
                //{
                //}
                EatToken(TokenType.Symbol);
                node = new Node { Left = node, Token = token, Right = Factor() };
            }
            return node;
        }

        private Node Factor()
        {
            var token = _currentToken;
            Node result = null;
            if (token.Type == TokenType.Symbol)
            {
                EatToken(TokenType.Symbol);
                result = Expression();
                EatToken(TokenType.Symbol);
            }
            else if (token.Type == TokenType.NumericConstant)
            {
                EatToken(TokenType.NumericConstant);
                result = new Node { Token = token };
            }
            else
                throw new InvalidOperationException();
            return result;
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