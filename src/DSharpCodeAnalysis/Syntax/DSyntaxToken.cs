using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DSharpCodeAnalysis.Syntax
{
    public class DSyntaxToken
    {
        public DSyntaxToken(DSyntaxKind syntaxKind)
        {
            SyntaxKind = syntaxKind;
        }

        public DSyntaxKind SyntaxKind { get;}
        public IEnumerable<Trivia> LeadingTrivia { get; set; }
        public IEnumerable<Trivia> TrailingTrivia { get; set; }
        public bool HasLeadingTrivia => LeadingTrivia.Count() > 0;
        public bool HasTrailingTrivia => TrailingTrivia.Count() > 0;
        public string ValueText { get; set; }
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