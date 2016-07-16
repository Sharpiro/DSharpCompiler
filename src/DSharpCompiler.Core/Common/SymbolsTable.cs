using System;
using System.Collections.Generic;

namespace DSharpCompiler.Core.Common
{
    public class SymbolsTable
    {
        private Dictionary<string, Symbol> _globalData;

        public SymbolsTable()
        {
            _globalData = new Dictionary<string, Symbol>();
        }
    }

    public class Symbol
    {
        public object Value { get; private set; }
        public Type Type { get; private set; }

        public Symbol(object value)
        {
            Value = value;
            Type = Value.GetType();
        }
    }
}
