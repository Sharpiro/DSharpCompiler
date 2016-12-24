using System;

namespace DSharpCodeAnalysis.Exceptions
{
    public class TokenException : Exception
    {
        public TokenException(string message)
            : base(message)
        {
        }
    }

    public class UnexpectedTokenException : TokenException
    {
        public UnexpectedTokenException(string expected, string actual)
            : base($"Expected: {expected}, but was actually: {actual}")
        {
        }
    }

    public class TranspilationError : Exception
    {
        private const string error = 
            "An error occurred transpiling from dsharp to csharp, " + 
            "the output is not valid csharp";

        public TranspilationError(): base(error)
        {
        }
    }
}