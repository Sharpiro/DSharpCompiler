using System.Collections.Generic;
using DSharpCompiler.Core.Common.Models;

namespace DSharpCompiler.Core.Common
{
    public interface ITokenParser
    {
        Node Program(IList<Token> tokens);
    }
}