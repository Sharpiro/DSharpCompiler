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
            var left = _currentToken;
            EatToken(TokenType.NumericConstant);
            var op = _currentToken;
            EatToken(TokenType.Symbol);
            var right = _currentToken;
            EatToken(TokenType.NumericConstant);
            int result = 0;
            if (op.Value == "+")
                result = int.Parse(left.Value) + int.Parse(right.Value);
            else if (op.Value == "-")
                result = int.Parse(left.Value) - int.Parse(right.Value);
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