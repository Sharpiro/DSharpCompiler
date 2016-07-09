using System.Collections.Generic;
using DSharpCompiler.Core.Common.Models;

namespace DSharpCompiler.Core.Common
{
    public interface ILanguageTokens
    {
        bool Contains(string value);
        IEnumerator<Token> GetEnumerator();
        Token GetTokenBySymbol(string symbol);
        bool IsIdentifier(string value);
        bool IsKeyword(string value);
        bool IsNumeric(string value);
        bool IsQuote(string value);
        bool IsSymbol(string value);
    }
}