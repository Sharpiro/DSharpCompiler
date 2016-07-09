using DSharpCompiler.Core.Common;
using DSharpCompiler.Core.Common.Models;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DSharpCompiler.Core.DSharp
{
    public class DSharpTokens : IEnumerable, IEnumerable<Token>, ILanguageTokens
    {
        private IEnumerable<Token> _tokens;

        public DSharpTokens()
        {
            _tokens = new List<Token>
            {
                new Token { Value = "func" , Type = TokenType.Keyword },
                new Token { Value = "\"" , Type = TokenType.Symbol },
                new Token { Value = "{" , Type = TokenType.Symbol },
                new Token { Value = "}" , Type = TokenType.Symbol },
                new Token { Value = "(" , Type = TokenType.Symbol },
                new Token { Value = ")" , Type = TokenType.Symbol },
                new Token { Value = ";" , Type = TokenType.Symbol },
                new Token { Value = "." , Type = TokenType.Symbol },
                new Token { Value = "+" , Type = TokenType.Symbol },
                new Token { Value = "-" , Type = TokenType.Symbol },
                new Token { Value = "*" , Type = TokenType.Symbol },
                new Token { Value = "/" , Type = TokenType.Symbol },
                new Token { Value = ";" , Type = TokenType.Symbol },
                new Token { Value = "=" , Type = TokenType.Symbol }
            };
        }

        public Token GetTokenBySymbol(string symbol)
        {
            var token = _tokens.FirstOrDefault(t => t.Value == symbol);
            if (token == null)
                throw new KeyNotFoundException();
            return token;
        }

        public bool IsSymbol(string value)
        {
            if (string.IsNullOrEmpty(value))
                return false;
            var symbols = _tokens.Where(t => t.Type == TokenType.Symbol)
                .Select(t => t.Value);
            var isSymbol = symbols.Contains(value);
            return isSymbol;
        }

        public bool IsQuote(string value)
        {
            var quoteToken = _tokens.Where(t => t.Value == "\"").FirstOrDefault();
            var isQuote = quoteToken.Value == value;
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
            if (string.IsNullOrEmpty(value))
                return false;
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
