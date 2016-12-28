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
        public UnexpectedTokenException(string expected, string actual, int line, int column, int width)
            : base($"Expected: {expected}, but was actually: {actual} at line: {line}, column: {column}, width: {width}")
        {
        }
    }

    public class TranspilationError : Exception
    {
        private const string error =
            "An error occurred transpiling from dsharp to csharp, " +
            "the output is not valid csharp";

        public TranspilationError() : base(error)
        {
        }
    }
}