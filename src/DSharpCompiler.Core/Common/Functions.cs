using System;

namespace DSharpCompiler.Core.Common
{
    public static class Functions
    {
        public static int? Add(this int? number, int? otherNumber)
        {
            if (number == null)
                return otherNumber;
            var result = number + otherNumber;
            return result;
        }

        public static int? Subtract(this int? number, int? otherNumber)
        {
            if (number == null)
                return otherNumber;
            var result = number - otherNumber;
            return result;
        }

        public static int? Multiply(this int? number, int? otherNumber)
        {
            if (number == null)
                return otherNumber;
            var result = number * otherNumber;
            return result;
        }

        public static int? Divide(this int? number, int? otherNumber)
        {
            if (number == null)
                return otherNumber;
            var result = number / otherNumber;
            return result;
        }
    }
}
