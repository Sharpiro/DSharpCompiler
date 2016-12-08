using DSharpCodeAnalysis.Syntax;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace DSharpCodeAnalysis.Parser
{
    public class DLexer
    {
        private SlidingTextWindow _textWindow;

        public DLexer(string source)
        {
            _textWindow = new SlidingTextWindow(source.ToArray());
        }

        public void Lex()
        {
            while (_textWindow.PeekChar() != SlidingTextWindow.InvalidCharacter)
            {
                ScanSyntaxToken();
            }
        }

        private void ScanSyntaxToken()
        {
            var character = _textWindow.PeekChar();
            switch (character)
            {
                case 'a':
                case 'b':
                case 'c':
                    ScanIdentifierOrKeyword();
                    break;
                default:
                    break;
            }
        }

        private void ScanIdentifierOrKeyword()
        {
            var token = new DTokenInfo();
            ScanIdentifier(ref token);
        }

        private void ScanIdentifier(ref DTokenInfo token)
        {
            var characterWindow = _textWindow.CharacterWindow;
            var currentOffset = _textWindow.Offset;
            var startOffset = _textWindow.Offset;
            while (true)
            {
                char currentCharacter = _textWindow.PeekChar();
                //Regex.Match("a", "b").Success
                var identifierMatch = Regex.Match(new string(currentCharacter, 1), "[a-zA-Z]");
                if (identifierMatch.Success) break;
                currentOffset++;
                _textWindow.AdvanceChar();
                //switch (currentCharacter)
                //{

                //}
            }
            var length = currentOffset - startOffset;
            token.Text = _textWindow.GetRange(startOffset, length);
            _textWindow.AdvanceChar(length);

            DSyntaxKind kind;
            var isKeyword = TryGetKeywordKind(token.Text, out kind);
            if (!isKeyword) kind = DSyntaxKind.IdentifierToken;
            token.SyntaxKind = kind;
        }

        private bool TryGetKeywordKind(string tokenText, out DSyntaxKind syntaxKind)
        {
            syntaxKind = 0;
            var result = (DSyntaxKind)Enum.Parse(typeof(DSyntaxKind), tokenText);
            throw new NotImplementedException();
        }

        private DSyntaxToken Create(ref DTokenInfo tokenInfo)
        {


            throw new NotImplementedException();
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
            for (var i = startIndex; i < length; i++)
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