using DSharpCompiler.Core.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

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
            ["dConsole"] = typeof(DConsole),
            ["dFunctions"] = typeof(DFunctions)
        };

        public void Add(string name, Type type)
        {
            _typeDictionary.Add(name, type);
        }

        public Delegate GetSubroutine(string routineName)
        {
            var data = routineName.Split('.');
            if (data.Length == 1) return null;
            var typeName = data.FirstOrDefault();
            if (string.IsNullOrEmpty(typeName)) return null;

            var type = _typeDictionary.Get(typeName);
            if (type == null) return null;

            var methodName = data.LastOrDefault();
            if (string.IsNullOrEmpty(methodName)) throw new ArgumentException("Invalid sub routine");

            var method = type.GetRuntimeMethods().SingleOrDefault(m => m.Name.Equals(methodName, StringComparison.OrdinalIgnoreCase));
            if (method == null) throw new ArgumentException($"Sub routine not found on type {typeName}");

            var parameters = method.GetParameters()
                           .Select(p => Expression.Parameter(p.ParameterType, p.Name))
                           .ToArray();
            var call = Expression.Call(null, method, parameters);
            var lambdaDelegate = Expression.Lambda(call, parameters).Compile();
            return lambdaDelegate;
        }

        public Type GetCallingType(string routineName)
        {
            var data = routineName.Split('.');
            if (data.Length == 1) return null;
            var typeName = data.FirstOrDefault();
            if (string.IsNullOrEmpty(typeName)) return null;

            var type = _typeDictionary.Get(typeName);
            return type;
        }

        public Type GetX(string routineName)
        {
            var mainFunc = _typeDictionary.Get($"main.{routineName}");
            if (mainFunc != null)
                return mainFunc;

            return this[routineName];
        }
    }
}
