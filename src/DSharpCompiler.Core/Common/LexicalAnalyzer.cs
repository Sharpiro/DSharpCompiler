using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using DSharpCompiler.Core.Common.Models;

namespace DSharpCompiler.Core.Common
{
    public class LexicalAnalyzer
    {
        private ITokenDefinitions _tokenDefinitions;
        private string _source;

        public LexicalAnalyzer(ITokenDefinitions tokenDefinitions)
        {
            _tokenDefinitions = tokenDefinitions;
        }

        public IEnumerable<Token> Analayze(string source)
        {
            if (string.IsNullOrEmpty(source))
                throw new ArgumentNullException(nameof(source));
            _source = source;

            _source.Split(new string[] { " " }, StringSplitOptions.None);
            RemoveNewLines();
            var foundTokens = new List<Token>();
            while (HasData())
            {
                var token = Advance();
                if (token != null)
                    foundTokens.Add(token);
            }
            return foundTokens;
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
            if (_tokenDefinitions.IsSymbol($"{firstLetter}{Peek()}"))
            {
                token = _tokenDefinitions.GetTokenBySymbol($"{firstLetter}{Peek()}");
            }
            else if (_tokenDefinitions.IsSymbol(firstLetter))
            {
                token = _tokenDefinitions.GetTokenBySymbol(firstLetter);
                if (_tokenDefinitions.IsQuote(firstLetter))
                {
                    _source = _source.Substring(1);
                    var endIndex = _source.IndexOf("\"");
                    token = new Token { Value = _source.Substring(0, endIndex), Type = TokenType.StringConstant };
                    _source = _source.Remove(endIndex, 1);
                }
            }
            else if (_tokenDefinitions.IsNumeric(firstWord))
            {
                token = new Token { Value = firstWord, Type = TokenType.NumericConstant };
            }
            else if (_tokenDefinitions.IsKeyword(firstWord))
            {
                token = new Token { Value = firstWord, Type = TokenType.Keyword };
            }
            else if (_tokenDefinitions.IsIdentifier(firstWord))
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
