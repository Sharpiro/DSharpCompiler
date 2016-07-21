using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DSharpCompiler.Core.Common.Models;

namespace DSharpCompiler.Core.Common
{
    public class SymbolsTable : IEnumerable, IEnumerable<KeyValuePair<string, Symbol>>
    {
        private Dictionary<string, Symbol> _globalData;
        private int _currentScopeId;

        public SymbolsTable()
        {
            _globalData = new Dictionary<string, Symbol>();
            _currentScopeId = 0;
        }

        public void Add(string key, Symbol value)
        {
            value.ScopeId = _currentScopeId;
            _globalData[key] = value;
        }

        public void AddNewScope()
        {
            _currentScopeId++;
        }

        public void RemoveCurrentScope()
        {
            if (_currentScopeId > 1)
                _globalData = _globalData
                    .Where(pair => pair.Value.ScopeId != _currentScopeId)
                    .ToDictionary(pair => pair.Key, pair => pair.Value);
            _currentScopeId--;
        }

        public void Remove(string key)
        {
            _globalData.Remove(key);
        }

        public Symbol Get(string index)
        {
            Symbol symbol = null;
            _globalData.TryGetValue(index, out symbol);
            return symbol;
        }

        public T GetValue<T>(string index)
        {
            Symbol symbol = null;
            _globalData.TryGetValue(index, out symbol);
            T value = (T)(symbol?.Value);
            return value;
        }

        public IEnumerator GetEnumerator()
        {
            return _globalData.GetEnumerator();
        }

        IEnumerator<KeyValuePair<string, Symbol>> IEnumerable<KeyValuePair<string, Symbol>>.GetEnumerator()
        {
            return _globalData.GetEnumerator();
        }

        public void AddNodes(IEnumerable<Node> paramsEnumerable, IEnumerable<object> argsEnumerable)
        {
            var parameters = paramsEnumerable.Select(p => p as VariableNode).Where(p => p != null).ToList();
            var arguments = argsEnumerable.Where(a => a != null).ToList();
            if (parameters.Count != arguments.Count)
                throw new InvalidOperationException("mismatch of # of parameters and arguments");
            for (var i = 0; i < parameters.Count(); i++)
            {
                Add(parameters[i].Value, new Symbol(arguments[i]));
            }
        }
    }

    public class Symbol
    {
        public object Value { get; private set; }
        public Type Type { get; private set; }
        public int ScopeId { get; set; }

        public Symbol(object value)
        {
            Value = value;
            Type = Value.GetType();
        }
    }
}
