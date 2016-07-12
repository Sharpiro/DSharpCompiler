using System;

namespace DSharpCompiler.Core.Common
{
    public static class Functions
    {
        public static int? Add(this object number, object otherNumber)
        {
            if (number == null)
                return (int)otherNumber;
            var result = (int)number + (int)otherNumber;
            return result;
        }

        public static int? Subtract(this object number, object otherNumber)
        {
            if (number == null)
                return (int)otherNumber;
            var result = (int)number - (int)otherNumber;
            return result;
        }

        public static int? Multiply(this object number, object otherNumber)
        {
            if (number == null)
                return (int)otherNumber;
            var result = (int)number * (int)otherNumber;
            return result;
        }

        public static int? Divide(this object number, object otherNumber)
        {
            if (number == null)
                return (int)otherNumber;
            var result = (int)number / (int)otherNumber;
            return result;
        }

        public static int? UnaryMinus(this object number)
        {
            var result = -((int)number);
            return result;
        }
    }
}
