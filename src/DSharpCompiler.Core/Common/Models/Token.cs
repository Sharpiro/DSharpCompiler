using System;

namespace DSharpCompiler.Core.Common.Models
{
    public class Token : IEquatable<Token>
    {
        public string Value { get; set; }
        public TokenType Type { get; set; }

        public bool Equals(Token other)
        {
            return Value == other.Value;
        }
    }
    public enum TokenType
    {
        None,
        Symbol,
        NumericConstant,
        CharacterConstant,
        StringConstant,
        Keyword,
        Identifier
    }
}