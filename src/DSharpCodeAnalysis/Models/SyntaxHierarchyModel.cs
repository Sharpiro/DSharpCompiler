using DSharpCodeAnalysis.Syntax;
using System.Collections.Generic;

namespace DSharpCodeAnalysis.Models
{
    public class SyntaxHierarchyModel
    {
        public string SyntaxKind { get; set; }
        public string SyntaxType { get; set; }
        public IList<SyntaxHierarchyModel> Children { get; set; } = new List<SyntaxHierarchyModel>();
        public int Width { get; set; }
        public int FullWidth { get; set; }
        //public Span Span { get; set; }
        //public Span FullSpan { get; set; }
    }
}