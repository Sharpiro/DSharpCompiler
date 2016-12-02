using System.Collections.Generic;

namespace DSharpCodeAnalysis.Syntax
{
    public enum DSyntaxKind
    {
        Null,
        OpenParenToken = 8200,
        CloseParenToken = 8201,
        EqualsToken = 8204,
        OpenBraceToken = 8205,
        CloseBraceToken = 8206,
        SemicolonToken = 8212,
        VoidKeyword = 8318,
        UsingKeyword = 8373,
        ClassKeyword = 8374,

        IdentifierToken = 8508,
        NumericLiteralToken = 8509,
        CharacterLiteralToken = 8510,
        StringLiteralToken = 8511,
        EndOfLineTrivia = 8539,
        WhitespaceTrivia = 8540,
        IdentifierName = 8616,
        PredefinedType = 8621,
        NumericLiteralExpression = 8749,
        StringLiteralExpression = 8750,
        Block = 8792,
        LocalDeclarationStatement = 8793,
        VariableDeclaration = 8794,
        VariableDeclarator = 8795,
        EqualsValueClause = 8796,
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
            [8204] = "=",
            [8205] = "{",
            [8206] = "}",
            [8212] = ";",
            [8318] = "void",
            [8373] = "using",
            [8374] = "class",
            [8508] = null
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