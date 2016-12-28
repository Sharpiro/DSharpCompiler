using System;
using System.Linq;

namespace DSharpCodeAnalysis.Parser
{
    public static class ParserFunctions
    {
        public static Tuple<int,int> GetLocationFromLength(string source, int position)
        {
            var subSource = source.Substring(0, position);
            var newLineCount = subSource.Count(c => c == '\n');
            var columnNumber = subSource.Length % newLineCount;
            var lineNumber = columnNumber == 0 ? newLineCount : newLineCount + 1;

            return new Tuple<int, int>(lineNumber, columnNumber);
        }
    }
}