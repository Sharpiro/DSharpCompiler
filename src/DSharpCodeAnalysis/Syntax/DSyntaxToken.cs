using DSharpCodeAnalysis.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DSharpCodeAnalysis.Syntax
{
    public interface IDSyntax
    {
        DSyntaxKind SyntaxKind { get; }
        Span FullSpan { get; }
        int Position { get; set; }
        int Width { get; }
        SyntaxHierarchyModel DescendantHierarchy();
    }

    public class DSyntaxToken : IDSyntax
    {
        public DSyntaxKind SyntaxKind { get; }
        public IEnumerable<Trivia> LeadingTrivia { get; set; } = Enumerable.Empty<Trivia>();
        public IEnumerable<Trivia> TrailingTrivia { get; set; } = Enumerable.Empty<Trivia>();
        public IEnumerable<Trivia> AllTrivia => LeadingTrivia.Concat(TrailingTrivia);
        public bool HasLeadingTrivia => LeadingTrivia.Any();
        public bool HasTrailingTrivia => TrailingTrivia.Any();
        public bool HasAnyTrivia => AllTrivia.Any();
        public string ValueText { get; set; }
        public Span FullSpan => new Span(Position, Width);
        public int Position { get; set; }
        public int Width => ValueText.Length;

        public DSyntaxToken(DSyntaxKind syntaxKind)
        {
            SyntaxKind = syntaxKind;
        }

        public DSyntaxToken(DSyntaxKind syntaxKind, int position)
        {
            SyntaxKind = syntaxKind;
            Position = position;
        }

        public override string ToString()
        {
            return ValueText;
        }

        public SyntaxHierarchyModel DescendantHierarchy()
        {
            return new SyntaxHierarchyModel
            {
                Children = AllTrivia.Select(t => new SyntaxHierarchyModel
                {
                    SyntaxKind = t.SyntaxKind.ToString(),
                    SyntaxType = nameof(Trivia)
                }).ToList()
            };
        }
    }

    public class Trivia : IDSyntax
    {
        public DSyntaxToken Token { get; set; }
        public Span Span { get; set; }
        public DSyntaxKind SyntaxKind { get; set; }
        public Span FullSpan { get; set; }
        public string FullText { get; set; }
        public int Position { get; set; }
        public int Width { get; set; }

        public Trivia(DSyntaxKind syntaxKind, string triviaText)
        {
            SyntaxKind = syntaxKind;
            FullText = triviaText;
            FullSpan = new Span(0, triviaText.Length);

        }

        public SyntaxHierarchyModel DescendantHierarchy()
        {
            throw new NotImplementedException();
        }
    }

    public class Span
    {
        public Span(int start, int length)
        {
            Start = start;
            Length = length;
        }

        public int Start { get; }
        public int End => Start + Length;
        public int Length { get; }
        public bool IsEmpty => this.Length == 0;
    }

    public class DSyntaxTokenList : IEnumerable<DSyntaxToken>
    {
        public IEnumerator<DSyntaxToken> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}