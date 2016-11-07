using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DSharpCompiler.Core.Common.Models;

namespace DSharpCompiler.Core.Common
{
    public class SymbolsTable : IEnumerable<KeyValuePair<string, Symbol>>
    {
        private Dictionary<string, Symbol> _globalData;
        private Dictionary<string, Symbol> _scopeData;
        private int _currentScopeId;

        public SymbolsTable()
        {
            _globalData = new Dictionary<string, Symbol>();
            _scopeData = new Dictionary<string, Symbol>();
            _currentScopeId = 0;
        }

        public void Add(string key, Symbol value)
        {
            if (_currentScopeId < 2)
                _globalData[key] = value;
            else
            {
                key = $"{key}-{_currentScopeId}";
                value.ScopeId = _currentScopeId;
                _scopeData[key] = value;
            }
        }

        public void RemoveCurrentScope()
        {
            _scopeData = _scopeData
                .Where(pair => pair.Value.ScopeId != _currentScopeId)
                .ToDictionary(pair => pair.Key, pair => pair.Value);
            _currentScopeId--;
        }

        public void Remove(string key)
        {
            key = $"{key}-{_currentScopeId}";
            _globalData.Remove(key);
        }

        public Symbol Get(string index)
        {
            Symbol symbol = null;
            var result = _globalData.TryGetValue(index, out symbol);
            if (result)
                return symbol;
            var key = $"{index}-{_currentScopeId}";
            _scopeData.TryGetValue(key, out symbol);
            return symbol;
        }

        public T GetValue<T>(string index)
        {
            Symbol symbol = null;
            var key = $"{index}-{_currentScopeId}";
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

        public void AddNewScope()
        {
            _currentScopeId++;
        }

        public void AddParentScopes()
        {
            var parentScopes = _scopeData.Where(kvp => kvp.Value.ScopeId == _currentScopeId - 1);
            var newDict = new Dictionary<string, Symbol>();
            foreach (var item in parentScopes)
            {
                var key = StripScope(item.Key);
                key = $"{key}-{_currentScopeId}";
                newDict[key] = item.Value;
            }
            _scopeData = _scopeData.Concat(newDict).ToDictionary(d => d.Key, d => d.Value);
        }

        public void AddNodes(IEnumerable<Node> paramsEnumerable, IEnumerable<object> argsEnumerable)
        {
            var parameters = paramsEnumerable.Select(p => p as VariableNode).Where(p => p != null).ToList();
            var arguments = argsEnumerable.Where(a => a != null).ToList();
            if (parameters.Count != arguments.Count)
                return;
            for (var i = 0; i < parameters.Count(); i++)
            {
                Add((string)parameters[i].Value, new Symbol(arguments[i]));
            }
        }

        private string StripScope(string scopedKey)
        {
            return scopedKey.Substring(0, scopedKey.Length - 2);
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