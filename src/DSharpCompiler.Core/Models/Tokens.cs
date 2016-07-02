using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DSharpCompiler.Core
{
    public class Tokens : IEnumerable, IEnumerable<Token>
    {
        private IEnumerable<Token> _tokens;

        public Tokens()
        {
            _tokens = new List<Token>
            {
                new Token { Value = "public" , Type = TokenType.Keyword},
                new Token { Value = "class" , Type = TokenType.Keyword},
                new Token { Value = "void" , Type = TokenType.Keyword},
                new Token { Value = "\"" , Type = TokenType.Symbol},
                new Token { Value = "{" , Type = TokenType.Symbol},
                new Token { Value = "}" , Type = TokenType.Symbol},
                new Token { Value = "(" , Type = TokenType.Symbol},
                new Token { Value = ")" , Type = TokenType.Symbol},
                new Token { Value = ";" , Type = TokenType.Symbol},
                new Token { Value = "." , Type = TokenType.Symbol},
                new Token { Value = "+" , Type = TokenType.Symbol},
                new Token { Value = "-" , Type = TokenType.Symbol}
            };
        }

        public Token GetTokenBySymbol(char symbol)
        {
            var token = _tokens.FirstOrDefault(t => t.Value == symbol.ToString());
            if (token == null)
                throw new KeyNotFoundException();
            return token;
        }

        public bool IsSymbol(char value)
        {
            var symbols = _tokens.Where(t => t.Type == TokenType.Symbol)
                .Select(t => Convert.ToChar(t.Value));
            var isSymbol = symbols.Contains(value);
            return isSymbol;
        }

        public bool IsQuote(char value)
        {
            var quoteToken = _tokens.Where(t => t.Value == "\"").FirstOrDefault();
            var isQuote = quoteToken.Value == value.ToString();
            return isQuote;
        }

        public bool IsNumeric(string value)
        {
            int integer;
            var isInt = int.TryParse(value, out integer);
            return isInt;
        }

        public bool IsKeyword(string value)
        {
            var keyTokens = _tokens.Where(t => t.Type == TokenType.Keyword)
                .Select(t => t.Value);
            var isKeyword = keyTokens.Contains(value);
            return isKeyword;
        }

        public bool IsIdentifier(string value)
        {
            var keyTokens = _tokens.Where(t => t.Type == TokenType.Keyword)
                .Select(t => t.Value);
            var isKeyword = !keyTokens.Contains(value);
            return isKeyword;
        }

        public bool Contains(string value)
        {
            return _tokens.Select(t => t.Value).Contains(value);
        }

        public IEnumerator<Token> GetEnumerator()
        {
            return _tokens.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _tokens.GetEnumerator();
        }
    }
}
