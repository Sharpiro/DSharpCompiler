using DSharpCodeAnalysis.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
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

    [DebuggerDisplay("{GetDebuggerDisplay(), nq}")]
    public class DSyntaxToken : IDSyntax
    {
        public DSyntaxKind SyntaxKind { get; }
        public DSyntaxNode Parent { get; set; }
        public IEnumerable<Trivia> LeadingTrivia { get; set; } = Enumerable.Empty<Trivia>();
        public IEnumerable<Trivia> TrailingTrivia { get; set; } = Enumerable.Empty<Trivia>();
        public IEnumerable<Trivia> AllTrivia => LeadingTrivia.Concat(TrailingTrivia);
        public bool HasLeadingTrivia => LeadingTrivia.Any();
        public bool HasTrailingTrivia => TrailingTrivia.Any();
        public bool HasAnyTrivia => AllTrivia.Any();
        public string ValueText => Value.ToString();
        public object Value { get; set; }
        public Span FullSpan => new Span(Position, Width);
        public int Position { get; set; }
        public int Width => ValueText.Length;

        public DSyntaxToken(DSyntaxKind syntaxKind)
        {
            SyntaxKind = syntaxKind;
            Value = DSyntaxFactory.SyntaxString(syntaxKind);
        }

        public DSyntaxToken(DSyntaxKind syntaxKind, object value)
        {
            SyntaxKind = syntaxKind;
            Value = value;
        }

        public override string ToString()
        {
            var leadingText = string.Join(string.Empty, LeadingTrivia.Select(l => l.ToString()));
            var trailingText = string.Join(string.Empty, TrailingTrivia.Select(l => l.ToString()));
            var tokenString = $"{leadingText}{ValueText}{trailingText}";
            return tokenString;
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

        public DSyntaxToken WithLeadingTrivia(IEnumerable<Trivia> leadingTrivia)
        {
            LeadingTrivia = leadingTrivia;
            return this;
        }

        public DSyntaxToken WithTrailingTrivia(IEnumerable<Trivia> trailingTrivia)
        {
            TrailingTrivia = trailingTrivia;
            return this;
        }

        private string GetDebuggerDisplay()
        {
            return GetType().Name + " " + SyntaxKind.ToString() + " " + ToString();
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

        public static Trivia Create(DSyntaxKind syntaxKind, string text)
        {
            return new Trivia(syntaxKind, text);
        }

        public override string ToString()
        {
            return FullText;
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
}