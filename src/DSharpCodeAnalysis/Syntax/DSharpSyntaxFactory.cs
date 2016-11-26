using System;

namespace DSharpCodeAnalysis.Syntax
{
    public static class DSharpSyntaxFactory
    {
        public static DSharpClassDeclarationSyntax ClassDeclaration(string identifier)
        {
            return new DSharpClassDeclarationSyntax();
        }

        public static DSharpClassDeclarationSyntax ClassDeclarationX(string identifier)
        {
            return new DSharpClassDeclarationSyntax();
        }
    }
}