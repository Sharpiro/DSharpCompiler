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
                    SyntaxType = nameof(DTrivia)
                }).ToList()
            };
        }

        public DSyntaxToken WithLeadingTrivia(IEnumerable<DTrivia> leadingTrivia)
        {
            LeadingTrivia = leadingTrivia;
            return this;
        }

        public DSyntaxToken WithTrailingTrivia(IEnumerable<DTrivia> trailingTrivia)
        {
            TrailingTrivia = trailingTrivia;
            return this;
        }

        private string GetDebuggerDisplay()
        {
            return GetType().Name + " " + SyntaxKind.ToString() + " " + ToString();
        }
    }

    public class DTrivia : IDSyntax
    {
        public DSyntaxToken Token { get; set; }
        public Span Span { get; set; }
        public DSyntaxKind SyntaxKind { get; set; }
        public Span FullSpan { get; set; }
        public string FullText { get; set; }
        public int Position { get; set; }
        public int Width { get; set; }

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

        public DTrivia(DSyntaxKind syntaxKind, string triviaText)
        {
            SyntaxKind = syntaxKind;
            FullText = triviaText;
            FullSpan = new Span(0, triviaText.Length);

        }

        public SyntaxHierarchyModel DescendantHierarchy()
        {
            throw new NotImplementedException();
        }

        public static DTrivia Create(DSyntaxKind syntaxKind, string text)
        {
            return new DTrivia(syntaxKind, text);
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