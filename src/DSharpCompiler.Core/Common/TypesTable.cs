using DSharpCompiler.Core.Common.Models;
using System;
using System.Collections.Generic;

namespace DSharpCompiler.Core.Common
{
    public class TypesTable
    {
        public Type this[string key] { get { return _typeDictionary.Get(key); } }

        private Dictionary<string, Type> _typeDictionary = new Dictionary<string, Type>
        {
            ["string"] = typeof(string),
            ["int"] = typeof(int),
            ["void"] = typeof(void),
            ["dSharpFunctions"] = typeof(DSharpFunctions)
        };

        public void Add(string name, Type type)
        {
            _typeDictionary.Add(name, type);
        }

        public void GetSubroutine(string routineName)
        {

        }
    }
}
