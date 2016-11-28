using System.Collections.Generic;

namespace DSharpCodeAnalysis.Syntax
{
    public enum DSyntaxKind
    {
        Null,
        OpenParenToken = 8200,
        CloseParenToken = 8201,
        OpenBraceToken = 8205,
        CloseBraceToken = 8206,
        VoidKeyword = 8318,
        UsingKeyword = 8373,
        ClassKeyword = 8374,

        IdentifierToken = 8508,
        PredefinedType = 8621,
        Block = 8792,
        UsingDirective = 8843,
        ClassDeclaration = 8855,
        MethodDeclaration = 8875,
        ParameterList = 8906
    }

    public static class DSyntaxStrings
    {
        private static readonly Dictionary<int, string> _strings = new Dictionary<int, string>
        {
            [8200] = "(",
            [8201] = ")",
            [8205] = "{",
            [8206] = "}",
            [8318] = "void",
            [8373] = "using",
            [8374] = "class"
        };

        public static string Get(DSyntaxKind syntaxKind)
        {
            var index = (int)syntaxKind;
            if (!_strings.ContainsKey(index))
                throw new KeyNotFoundException($"Could not find the value for the key ${syntaxKind}");

            return _strings[index];
        }
    }
}