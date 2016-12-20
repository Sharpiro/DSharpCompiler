using DSharpCompiler.Core.Common;
using DSharpCompiler.Core.Common.Models;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DSharpCompiler.Core.DSharp
{
    public class DSharpTokenDefinitions : IEnumerable, IEnumerable<Token>, ITokenDefinitions
    {
        private IEnumerable<Token> _tokens;

        public DSharpTokenDefinitions()
        {
            _tokens = new List<Token>
            {
                new Token { Value = DsharpConstants.Keywords.Func, Type = TokenType.Keyword },
                new Token { Value = DsharpConstants.Keywords.Let , Type = TokenType.Keyword },
                new Token { Value = DsharpConstants.Keywords.Return , Type = TokenType.Keyword },
                new Token { Value = DsharpConstants.Keywords.If , Type = TokenType.Keyword },
                new Token { Value = DsharpConstants.Keywords.True , Type = TokenType.Keyword },
                new Token { Value = DsharpConstants.Keywords.False , Type = TokenType.Keyword },
                new Token { Value = DsharpConstants.Keywords.ConditionalEquals , Type = TokenType.Keyword },
                new Token { Value = DsharpConstants.Keywords.Type , Type = TokenType.Keyword },
                new Token { Value = DsharpConstants.Symbols.Quote , Type = TokenType.Symbol },
                new Token { Value = DsharpConstants.Symbols.LeftCurlyBrace , Type = TokenType.Symbol },
                new Token { Value = DsharpConstants.Symbols.RighCurlyBrace , Type = TokenType.Symbol },
                new Token { Value = DsharpConstants.Symbols.LeftParenthesis, Type = TokenType.Symbol },
                new Token { Value = DsharpConstants.Symbols.RightParenthesis , Type = TokenType.Symbol },
                new Token { Value = DsharpConstants.Symbols.SemiColan , Type = TokenType.Symbol },
                new Token { Value = DsharpConstants.Symbols.Period, Type = TokenType.Symbol },
                new Token { Value = DsharpConstants.Symbols.Comma , Type = TokenType.Symbol },
                new Token { Value = DsharpConstants.Symbols.Plus , Type = TokenType.Symbol },
                new Token { Value = DsharpConstants.Symbols.Minus , Type = TokenType.Symbol },
                new Token { Value = DsharpConstants.Symbols.Multiply , Type = TokenType.Symbol },
                new Token { Value = DsharpConstants.Symbols.Divide , Type = TokenType.Symbol },
                new Token { Value = DsharpConstants.Symbols.Equal , Type = TokenType.Symbol }
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
            var quoteToken = _tokens.Where(t => t.Value == DsharpConstants.Symbols.Quote).FirstOrDefault();
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
