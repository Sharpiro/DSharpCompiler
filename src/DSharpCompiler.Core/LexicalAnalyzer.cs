using DSharpCompiler.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace DSharpCompiler.Core
{
    public class LexicalAnalyzer
    {
        private Tokens _syntaxTokens;
        private string _source;
        private List<Token> _foundTokens = new List<Token>();

        public LexicalAnalyzer(string source)
        {
            _syntaxTokens = new Tokens();
            _source = source;
        }

        public IEnumerable<Token> Analayze()
        {
            _source.Split(new string[] { " " }, StringSplitOptions.None);
            RemoveNewLines();
            while (HasData())
            {
                var token = Advance();
                if (token != null)
                    _foundTokens.Add(token);
            }
            return _foundTokens;
        }

        private bool HasData()
        {
            RemoveWhiteSpace();
            return _source.Length > 0;
        }

        private Token Advance()
        {
            var firstLetter = _source.FirstOrDefault().ToString();
            var firstWord = GetFirstWord(_source);

            Token token = null;
            if (_syntaxTokens.IsSymbol($"{firstLetter}{Peek()}"))
            {
                token = _syntaxTokens.GetTokenBySymbol($"{firstLetter}{Peek()}");
            }
            else if (_syntaxTokens.IsSymbol(firstLetter))
            {
                token = _syntaxTokens.GetTokenBySymbol(firstLetter);
                if (_syntaxTokens.IsQuote(firstLetter))
                {
                    _source = _source.Substring(1);
                    var endIndex = _source.IndexOf("\"");
                    token = new Token { Value = _source.Substring(0, endIndex), Type = TokenType.StringConstant };
                    _source = _source.Remove(endIndex, 1);
                }
            }
            else if (_syntaxTokens.IsNumeric(firstWord))
            {
                token = new Token { Value = firstWord, Type = TokenType.NumericConstant };
            }
            else if (_syntaxTokens.IsKeyword(firstWord))
            {
                token = new Token { Value = firstWord, Type = TokenType.Keyword };
            }
            else if (_syntaxTokens.IsIdentifier(firstWord))
            {
                token = new Token { Value = firstWord, Type = TokenType.Identifier };
            }
            else
            {
                throw new InvalidOperationException();
            }
            _source = _source.Substring(token.Value.Length);
            return token;
        }

        private string Peek()
        {
            try
            {
                return _source.Skip(1)?.FirstOrDefault().ToString();
            }
            catch (NullReferenceException) { }
            return null;
        }

        private string GetFirstWord(string subSource)
        {
            var match = Regex.Match(_source, @"[_A-Za-z0-9][_A-Za-z0-9]*").ToString();
            if (string.IsNullOrEmpty(match))
                return null;
            return match;
        }

        private void RemoveNewLines()
        {
            _source = Regex.Replace(_source, @"\s+", " ");
        }

        private void RemoveWhiteSpace()
        {
            var regex = new Regex(@"\s?");
            _source = regex.Replace(_source, "", 1);
        }
    }
}
