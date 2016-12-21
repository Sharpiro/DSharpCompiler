using System;
using System.Collections.Generic;
using System.Linq;

namespace DSharpCodeAnalysis.Syntax
{
    public enum DSyntaxKind
    {
        Null,

        FunctionKeyword = 5000,

        OpenParenToken = 8200,
        CloseParenToken = 8201,
        PlusToken = 8203,
        EqualsToken = 8204,
        OpenBraceToken = 8205,
        CloseBraceToken = 8206,
        SemicolonToken = 8212,
        CommaToken = 8216,
        DotToken = 8218,
        IntKeyword = 8309,
        VoidKeyword = 8318,
        ReturnKeyword = 8341,
        StaticKeyword = 8347,
        UsingKeyword = 8373,
        ClassKeyword = 8374,

        EndOfFileToken = 8496,
        IdentifierToken = 8508,
        NumericLiteralToken = 8509,
        CharacterLiteralToken = 8510,
        StringLiteralToken = 8511,
        EndOfLineTrivia = 8539,
        WhitespaceTrivia = 8540,
        IdentifierName = 8616,
        PredefinedType = 8621,
        InvocationExpression = 8634,
        ArgumentList = 8636,
        Argument = 8638,
        AddExpression = 8668,
        SimpleMemberAccessExpression = 8689,
        NumericLiteralExpression = 8749,
        StringLiteralExpression = 8750,
        Block = 8792,
        LocalDeclarationStatement = 8793,
        VariableDeclaration = 8794,
        VariableDeclarator = 8795,
        EqualsValueClause = 8796,
        ExpressionStatement = 8797,
        ReturnStatement = 8805,
        CompilationUnit = 8840,
        GlobalStatement = 8841,
        UsingDirective = 8843,
        ClassDeclaration = 8855,
        FieldDeclaration = 8873,
        MethodDeclaration = 8875,
        ParameterList = 8906,
        Parameter = 8908,
    }

    public class DSyntaxEntry
    {
        public int Id => (int)SyntaxKind;
        public string KeywordText { get; }
        public DSyntaxKind SyntaxKind { get; }

        public DSyntaxEntry(string keywordText, DSyntaxKind syntaxKind)
        {
            if (syntaxKind == DSyntaxKind.Null) throw new ArgumentNullException(nameof(SyntaxKind));

            KeywordText = keywordText;
            SyntaxKind = syntaxKind;
        }
    }

    public class DSyntaxCache
    {
        private readonly IEnumerable<DSyntaxEntry> _entires = new List<DSyntaxEntry>
        {
            new DSyntaxEntry("func", DSyntaxKind.FunctionKeyword),

            new DSyntaxEntry("(", DSyntaxKind.OpenParenToken),
            new DSyntaxEntry(")", DSyntaxKind.CloseParenToken),
            new DSyntaxEntry("+", DSyntaxKind.PlusToken),
            new DSyntaxEntry("=", DSyntaxKind.EqualsToken),
            new DSyntaxEntry("{", DSyntaxKind.OpenBraceToken),
            new DSyntaxEntry("}", DSyntaxKind.CloseBraceToken),
            new DSyntaxEntry(";", DSyntaxKind.SemicolonToken),
            new DSyntaxEntry(",", DSyntaxKind.CommaToken),
            new DSyntaxEntry(".", DSyntaxKind.DotToken),
            new DSyntaxEntry("int", DSyntaxKind.IntKeyword),
            new DSyntaxEntry("void", DSyntaxKind.VoidKeyword),
            new DSyntaxEntry("return", DSyntaxKind.ReturnKeyword),
            new DSyntaxEntry("static", DSyntaxKind.StaticKeyword),
            new DSyntaxEntry("using", DSyntaxKind.UsingKeyword),
            new DSyntaxEntry("type", DSyntaxKind.ClassKeyword),
            new DSyntaxEntry("", DSyntaxKind.EndOfFileToken),
            new DSyntaxEntry("Identifier", DSyntaxKind.IdentifierToken),
            new DSyntaxEntry("0", DSyntaxKind.NumericLiteralToken),
        };
        private readonly Dictionary<DSyntaxKind, string> _kindToSyntax;
        private readonly Dictionary<string, DSyntaxKind> _syntaxToKind;

        public DSyntaxCache()
        {
            _kindToSyntax = _entires.ToDictionary(e => e.SyntaxKind, e => e.KeywordText);
            _syntaxToKind = _entires.ToDictionary(e => e.KeywordText, e => e.SyntaxKind);
        }

        public string Get(DSyntaxKind syntaxKind)
        {
            if (!_kindToSyntax.ContainsKey(syntaxKind))
                throw new KeyNotFoundException($"Could not find the value for the key ${syntaxKind}");

            return _kindToSyntax[syntaxKind];
        }

        public DSyntaxKind Get(string syntaxText)
        {
            if (!_syntaxToKind.ContainsKey(syntaxText))
                return DSyntaxKind.Null;

            return _syntaxToKind[syntaxText];
        }

        public static DSyntaxKind GetLiteralExpression(DSyntaxKind kind)
        {
            switch (kind)
            {
                case DSyntaxKind.NumericLiteralToken:
                    return DSyntaxKind.NumericLiteralExpression;
                default:
                    return DSyntaxKind.Null;
            }
        }

        public static bool IsPredefinedType(DSyntaxKind kind)
        {
            switch (kind)
            {
                case DSyntaxKind.IntKeyword:
                case DSyntaxKind.VoidKeyword:
                    return true;
                default: return false;
            }
        }
    }
}