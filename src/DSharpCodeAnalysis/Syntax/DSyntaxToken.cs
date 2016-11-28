using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DSharpCodeAnalysis.Syntax
{
    public class DSyntaxToken
    {
        public DSyntaxKind SyntaxKind { get; }
        public IEnumerable<Trivia> LeadingTrivia { get; set; } = Enumerable.Empty<Trivia>();
        public IEnumerable<Trivia> TrailingTrivia { get; set; } = Enumerable.Empty<Trivia>();
        public IEnumerable<Trivia> AllTrivia => LeadingTrivia.Concat(TrailingTrivia);
        public bool HasLeadingTrivia => LeadingTrivia.Any();
        public bool HasTrailingTrivia => TrailingTrivia.Any();
        public bool HasAnyTrivia => AllTrivia.Any();
        public string ValueText { get; set; }

        public DSyntaxToken(DSyntaxKind syntaxKind)
        {
            SyntaxKind = syntaxKind;
        }

        public override string ToString()
        {
            return ValueText;
        }
    }

    public class Trivia
    {
        public DSyntaxToken Token { get; set; }
        public Span Span { get; set; }
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