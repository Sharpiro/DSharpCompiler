﻿using DSharpCodeAnalysis.Syntax;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace DSharpCodeAnalysis.Parser
{
    public class DLexer
    {
        private SlidingTextWindow _textWindow;
        private DSyntaxCache _syntaxCache;
        private List<DTrivia> _leadingTrivia = new List<DTrivia>();
        private List<DTrivia> _trailingTrivia = new List<DTrivia>();
        private List<DSyntaxToken> _lexedTokens = new List<DSyntaxToken>();

        public DLexer(string source)
        {
            _textWindow = new SlidingTextWindow(source.ToArray());
            _syntaxCache = new DSyntaxCache();
        }

        public List<DSyntaxToken> Lex()
        {
            while (_textWindow.PeekChar() != SlidingTextWindow.InvalidCharacter)
            {
                _lexedTokens.Add(LexSyntaxToken());
            }
            return _lexedTokens;
        }

        private DSyntaxToken LexSyntaxToken()
        {
            var tokenInfo = new DTokenInfo();
            _leadingTrivia.Clear();
            var position = _textWindow.Offset;
            LexSyntaxTrivia(isTrailing: false, triviaList: ref _leadingTrivia);

            ScanSyntaxToken(ref tokenInfo);

            _trailingTrivia.Clear();
            LexSyntaxTrivia(isTrailing: true, triviaList: ref _trailingTrivia);

            return CreateToken(ref tokenInfo, _leadingTrivia, _trailingTrivia, position);
        }

        private DSyntaxToken CreateToken(ref DTokenInfo tokenInfo, IEnumerable<DTrivia> leading, IEnumerable<DTrivia> trailing, int position)
        {
            if (tokenInfo.SyntaxKind == DSyntaxKind.Null) throw new ArgumentNullException(nameof(tokenInfo.SyntaxKind));

            if (tokenInfo.SyntaxKind == DSyntaxKind.IdentifierToken)
                return DSyntaxFactory.Identifier(leading, tokenInfo.Text, trailing, position);
            if (tokenInfo.SyntaxKind == DSyntaxKind.NumericLiteralToken)
                return DSyntaxFactory.Literal(leading, Convert.ToInt32(tokenInfo.Text), trailing, position);
            return DSyntaxFactory.Token(leading, tokenInfo.SyntaxKind, trailing, position);
        }

        private void LexSyntaxTrivia(bool isTrailing, ref List<DTrivia> triviaList)
        {
            top:
            var character = _textWindow.PeekChar();
            switch (character)
            {
                case ' ':
                    triviaList.Add(ScanWhiteSpace());
                    goto top;
                case '\r':
                case '\n':
                    triviaList.Add(ScanEndOfLine());
                    if (isTrailing) return;
                    goto top;
                case '/':
                    throw new NotImplementedException();
                default:
                    break;
            }
        }

        private DTrivia ScanEndOfLine()
        {
            var character = _textWindow.PeekChar();
            switch (character)
            {
                case '\r':
                    _textWindow.AdvanceChar();
                    return DSyntaxFactory.EndOfLine("\r", _textWindow.Offset);
                case '\n':
                    _textWindow.AdvanceChar();
                    return DSyntaxFactory.EndOfLine("\n", _textWindow.Offset);
                default:
                    throw new ArgumentOutOfRangeException(nameof(character), "Invalid newline character");
            }
        }

        private DTrivia ScanWhiteSpace()
        {
            var length = 0;
            top:
            var character = _textWindow.PeekChar();
            switch (character)
            {
                case ' ':
                    length++;
                    _textWindow.AdvanceChar();
                    goto top;
                default:
                    break;
            }
            var whiteSpace = DSyntaxFactory.Whitespace(string.Join(string.Empty, Enumerable.Repeat(" ", length)), _textWindow.Offset);
            return whiteSpace;
        }

        private void ScanNumericLiteral(ref DTokenInfo tokenInfo)
        {
            var characterWindow = _textWindow.CharacterWindow;
            var currentOffset = _textWindow.Offset;
            var startOffset = _textWindow.Offset;
            char currentCharacter;
            while ((currentCharacter = characterWindow[currentOffset]) != SlidingTextWindow.InvalidCharacter)
            {
                var identifierMatch = Regex.Match(new string(currentCharacter, 1), "[0-9]");
                if (!identifierMatch.Success) break;
                currentOffset++;
            }
            var length = currentOffset - startOffset;
            tokenInfo.Text = _textWindow.GetRange(startOffset, length);
            tokenInfo.SyntaxKind = DSyntaxKind.NumericLiteralToken;
            _textWindow.AdvanceChar(length);
        }

        private void ScanSyntaxToken(ref DTokenInfo tokenInfo)
        {
            var character = _textWindow.PeekChar();
            var identifierMatch = Regex.Match(new string(character, 1), "[a-zA-Z]");
            if (identifierMatch.Success)
            {
                ScanIdentifierOrKeyword(ref tokenInfo);
                return;
            }

            var numericMatch = Regex.Match(new string(character, 1), "[0-9]");
            if (numericMatch.Success)
            {
                ScanNumericLiteral(ref tokenInfo);
                return;
            }

            switch (character)
            {
                case '{':
                    tokenInfo.SyntaxKind = DSyntaxKind.OpenBraceToken;
                    break;
                case '}':
                    tokenInfo.SyntaxKind = DSyntaxKind.CloseBraceToken;
                    break;
                case '(':
                    tokenInfo.SyntaxKind = DSyntaxKind.OpenParenToken;
                    break;
                case ')':
                    tokenInfo.SyntaxKind = DSyntaxKind.CloseParenToken;
                    break;
                case ',':
                    tokenInfo.SyntaxKind = DSyntaxKind.CommaToken;
                    break;
                case '.':
                    tokenInfo.SyntaxKind = DSyntaxKind.DotToken;
                    break;
                case '+':
                    tokenInfo.SyntaxKind = DSyntaxKind.PlusToken;
                    break;
                case '-':
                    tokenInfo.SyntaxKind = DSyntaxKind.MinusToken;
                    break;
                case '=':
                    tokenInfo.SyntaxKind = DSyntaxKind.EqualsToken;
                    break;
                case ';':
                    tokenInfo.SyntaxKind = DSyntaxKind.SemicolonToken;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(character), "Error @ ScanSyntaxToken");
            }
            _textWindow.AdvanceChar();
        }

        private void ScanIdentifierOrKeyword(ref DTokenInfo tokenInfo)
        {
            ScanIdentifier(ref tokenInfo);

            DSyntaxKind kind;
            var isKeyword = TryGetKeywordKind(tokenInfo.Text, out kind);
            if (!isKeyword) kind = DSyntaxKind.IdentifierToken;
            tokenInfo.SyntaxKind = kind;
        }

        private void ScanIdentifier(ref DTokenInfo token)
        {
            var characterWindow = _textWindow.CharacterWindow;
            var currentOffset = _textWindow.Offset;
            var startOffset = _textWindow.Offset;
            char currentCharacter;
            while ((currentCharacter = characterWindow[currentOffset]) != SlidingTextWindow.InvalidCharacter)
            {
                var identifierMatch = Regex.Match(new string(currentCharacter, 1), "[a-zA-Z]");
                if (!identifierMatch.Success) break;
                currentOffset++;
            }
            var length = currentOffset - startOffset;
            token.Text = _textWindow.GetRange(startOffset, length);
            _textWindow.AdvanceChar(length);
        }

        private bool TryGetKeywordKind(string tokenText, out DSyntaxKind syntaxKind)
        {
            syntaxKind = _syntaxCache.Get(tokenText);
            return syntaxKind != DSyntaxKind.Null;
        }
    }

    public class SlidingTextWindow
    {
        public const char InvalidCharacter = char.MaxValue;

        private int _offset = 0;
        private char[] _characterWindow;

        public char[] CharacterWindow => _characterWindow;
        public int Length => _characterWindow.Length;
        public int Offset => _offset;

        public SlidingTextWindow(char[] characters)
        {
            _characterWindow = characters;
        }

        public void AdvanceChar(int length = 1)
        {
            _offset += length;
        }

        public string GetRange(int startIndex, int length)
        {
            var builder = new StringBuilder();
            for (var i = startIndex; i < startIndex + length; i++)
            {
                builder.Append(_characterWindow[i]);
            }
            return builder.ToString();
        }

        public char PeekChar()
        {
            if (_offset >= Length)
                return InvalidCharacter;
            return _characterWindow[_offset];
        }
    }
}