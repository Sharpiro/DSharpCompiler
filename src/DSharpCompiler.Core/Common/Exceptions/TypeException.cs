using System;

namespace DSharpCompiler.Core.Common.Exceptions
{
    public class TypeMismatchException : Exception
    {
        public TypeMismatchException(string message) : base(message) { }

        public TypeMismatchException(string message, Exception innerException) : base(message, innerException) { }
    }

    public class TypeNotFoundException : Exception
    {
        public TypeNotFoundException(string message) : base(message) { }

        public TypeNotFoundException(string message, Exception innerException) : base(message, innerException) { }
    }

    public class VariableNotFoundException : Exception
    {
        public VariableNotFoundException(string message) : base(message) { }

        public VariableNotFoundException(string message, Exception innerException) : base(message, innerException) { }
    }
}