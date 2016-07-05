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

        public int Expression()
        {
            var result = Term();

            while (_currentToken != null && _currentToken.Value.In("+", "-"))
            {
                if (_currentToken.Value == "+")
                {
                    EatToken(TokenType.Symbol);
                    result += Term();
                }
                else if (_currentToken.Value == "-")
                {
                    EatToken(TokenType.Symbol);
                    result -= Term();
                }
                else
                    throw new InvalidOperationException();
            }
            return result;
        }

        private int Term()
        {
            var result = Factor();
            while (_currentToken != null && _currentToken.Value.In("*", "/"))
            {
                if (_currentToken.Value == "*")
                {
                    EatToken(TokenType.Symbol);
                    result *= Factor();
                }
                else if (_currentToken.Value == "/")
                {
                    EatToken(TokenType.Symbol);
                    result /= Factor();
                }
            }
            return result;
        }

        private int Factor()
        {
            var token = _currentToken;
            var result = 0;
            if (token.Type == TokenType.Symbol)
            {
                EatToken(TokenType.Symbol);
                result = Expression();
                EatToken(TokenType.Symbol);
            }
            else if (token.Type == TokenType.NumericConstant)
            {
                EatToken(TokenType.NumericConstant);
                result = int.Parse(token.Value);
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