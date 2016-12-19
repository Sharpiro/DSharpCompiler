using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DSharpCodeAnalysis.Syntax
{
    public class DSyntaxList<T> : IEnumerable<T> where T : DSyntaxNode
    {
        private List<T> _list = new List<T>();

        public DSyntaxList(IEnumerable<T> list)
        {
            _list = list.ToList();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _list.OfType<T>().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void SetParent(DSyntaxNode node)
        {
            _list.ForEach(t => t.Parent = node);
        }

        public void Add(T node)
        {
            _list.Add(node);
        }
    }

    public class DSeparatedSyntaxList<T> : IEnumerable<T> where T : DSyntaxNode
    {
        private List<T> _nodes = new List<T>();
        private List<DSyntaxToken> _seperators = new List<DSyntaxToken>();

        public DSeparatedSyntaxList(IEnumerable<T> nodes, IEnumerable<DSyntaxToken> seperators)
        {
            _nodes = nodes.ToList();
            _seperators = seperators.ToList();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _nodes.OfType<T>().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerable<IDSyntax> GetNodesAndSeperators()
        {
            var @switch = true;
            var nodeEnumerator = _nodes.GetEnumerator();
            var seperatorEnumerator = _seperators.GetEnumerator();
            for (var i = 0; true; i++)
            {
                IDSyntax syntax = null;
                bool hasNext;
                if (@switch)
                {
                    hasNext = nodeEnumerator.MoveNext();
                    if (!hasNext) yield break;
                    syntax = nodeEnumerator.Current;
                }
                else
                {
                    hasNext = seperatorEnumerator.MoveNext();
                    if (!hasNext) yield break;
                    syntax = seperatorEnumerator.Current;
                }
                @switch = !@switch;
                yield return syntax;
            }
        }

        public void SetParent(DSyntaxNode node)
        {
            _nodes.ForEach(t => t.Parent = node);
            _seperators.ForEach(t => t.Parent = node);
        }

        public void AddSeperator(DSyntaxToken seperatorToken)
        {
            _seperators.Add(seperatorToken);
        }

        public void Add(T node)
        {
            _nodes.Add(node);
        }
    }

    public class DSyntaxTokenList : IEnumerable<DSyntaxToken>
    {
        private IEnumerable<DSyntaxToken> _tokens = Enumerable.Empty<DSyntaxToken>();

        public DSyntaxTokenList()
        {
            _tokens = Enumerable.Empty<DSyntaxToken>();
        }

        public DSyntaxTokenList(IEnumerable<DSyntaxToken> tokens)
        {
            if (tokens == null) throw new ArgumentNullException(nameof(tokens));

            _tokens = tokens;
        }

        public IEnumerator<DSyntaxToken> GetEnumerator()
        {
            return _tokens.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void SetParent(DSyntaxNode node)
        {
            _tokens = _tokens.ForEach(t => t.Parent = node);
        }
    }
}
