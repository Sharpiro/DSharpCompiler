using System;

namespace DSharpCodeAnalysis.Exceptions
{
    public class UnexpectedTokenException : Exception
    {
        public UnexpectedTokenException(string expected, string actual)
            : base($"Expected: {expected}, but was actually: {actual}")
        {
        }
    }
}