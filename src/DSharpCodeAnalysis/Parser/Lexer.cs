using Microsoft.CodeAnalysis.CSharp;
using System.Linq;

namespace DSharpCodeAnalysis.Parser
{
    public class Lexer
    {
        private SlidingTextWindow _textWindow;

        public Lexer(string source)
        {
            _textWindow = source.ToArray();
        }

        public void Lex()
        {
            var character = _textWindow.PeekChar();
            switch (character)
            {
                default:
                    break;
            }
        }
    }

    public class SlidingTextWindow
    {
        private int _offset = 0;
        private char[] _characterWindow;

        public char[] CharacterWindow => _characterWindow;

        public void AdvanceChar()
        {
            _offset++;
        }

        public char PeekChar()
        {
            return _characterWindow[_offset++];
        }
    }
}