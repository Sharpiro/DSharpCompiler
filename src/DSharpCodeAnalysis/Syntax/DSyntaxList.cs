using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DSharpCodeAnalysis.Syntax
{
    public class DSyntaxList<T> : IEnumerable<T> where T : DSyntaxNode
    {
        private List<T> _list = new List<T>();

        public DSyntaxList(List<T> list)
        {
            _list = list;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _list.OfType<T>().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public class DSeparatedSyntaxList<T> : IEnumerable<T> where T : DSyntaxNode
    {
        private IEnumerable<T> _nodes = Enumerable.Empty<T>();
        private IEnumerable<DSyntaxToken> _seperators = Enumerable.Empty<DSyntaxToken>();

        public DSeparatedSyntaxList(IEnumerable<T> nodes, IEnumerable<DSyntaxToken> seperators)
        {
            _nodes = nodes;
            _seperators = seperators;
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
                    syntax = nodeEnumerator.Current;
                }
                else
                {
                    hasNext = seperatorEnumerator.MoveNext();
                    syntax = seperatorEnumerator.Current;
                }
                @switch = !@switch;
                if (!hasNext) yield break;
                yield return syntax;
            }
        }
    }
}
