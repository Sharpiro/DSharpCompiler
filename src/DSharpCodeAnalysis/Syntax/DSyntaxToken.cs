using DSharpCodeAnalysis.Models;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;

namespace DSharpCodeAnalysis.Syntax
{
    public interface IDSyntax
    {
        DSyntaxKind SyntaxKind { get; }
        Span Span { get; }
        Span FullSpan { get; }
        int Position { get; set; }
        int Width { get; }
        DSyntaxNode Parent { get; set; }

        SyntaxHierarchyModel DescendantHierarchy();
    }

    [DebuggerDisplay("{GetDebuggerDisplay(), nq}")]
    public class DSyntaxToken : IDSyntax
    {
        public DSyntaxKind SyntaxKind { get; }
        public DSyntaxNode Parent { get; set; }
        public IEnumerable<DTrivia> LeadingTrivia { get; set; } = Enumerable.Empty<DTrivia>();
        public IEnumerable<DTrivia> TrailingTrivia { get; set; } = Enumerable.Empty<DTrivia>();
        public IEnumerable<DTrivia> AllTrivia => LeadingTrivia.Concat(TrailingTrivia);
        public bool HasLeadingTrivia => LeadingTrivia.Any();
        public bool HasTrailingTrivia => TrailingTrivia.Any();
        public bool HasAnyTrivia => AllTrivia.Any();
        public string ValueText => Value.ToString();
        public object Value { get; set; }
        public Span Span => new Span(Position + LeadingTrivia.Sum(l => l.FullSpan.Length), Width);
        public Span FullSpan => new Span(Position, FullWidth);
        public int Position { get; set; }
        public int Width => ValueText.Length;
        public int FullWidth
        {
            get
            {
                var leadingWidth = LeadingTrivia.Sum(l => l.Width);
                var trailingWidth = TrailingTrivia.Sum(l => l.Width);
                return leadingWidth + Width + trailingWidth;
            }
        }

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
                    SyntaxType = nameof(DTrivia),
                    Span = t.Span,
                    FullSpan = t.FullSpan
                }).ToList()
            };
        }

        public DSyntaxToken WithLeadingTrivia(IEnumerable<DTrivia> leadingTrivia)
        {
            var newLeading = ImmutableList.CreateRange(leadingTrivia);
            LeadingTrivia = newLeading;
            return this;
        }

        public DSyntaxToken WithTrailingTrivia(IEnumerable<DTrivia> trailingTrivia)
        {
            var newTrailing = ImmutableList.CreateRange(trailingTrivia);
            TrailingTrivia = newTrailing;
            return this;
        }

        public DSyntaxToken Clone()
        {
            var clone = (DSyntaxToken)MemberwiseClone();
            return clone;
        }

        private string GetDebuggerDisplay()
        {
            return GetType().Name + " " + SyntaxKind.ToString() + " " + ToString();
        }
    }

    public class DTrivia : IDSyntax
    {
        public DSyntaxToken Token { get; set; }
        public DSyntaxKind SyntaxKind { get; set; }
        public Span Span => new Span(Position, Width);
        public Span FullSpan => new Span(Position, Width);
        public string FullText { get; set; }
        public int Position { get; set; }
        public int Width => FullText.Length;

        public DSyntaxNode Parent
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public DTrivia(DSyntaxKind syntaxKind, string triviaText, int position = 0)
        {
            SyntaxKind = syntaxKind;
            FullText = triviaText;
            Position = position;
        }

        public SyntaxHierarchyModel DescendantHierarchy()
        {
            throw new NotImplementedException();
        }

        public static DTrivia Create(DSyntaxKind syntaxKind, string text, int position = 0)
        {
            return new DTrivia(syntaxKind, text, position);
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