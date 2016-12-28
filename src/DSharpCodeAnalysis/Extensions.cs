using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace DSharpCodeAnalysis
{
    public static class Extensions
    {
        public static List<T> ForEach<T>(this IEnumerable<T> list, Action<T> action)
        {
            var enumerated = list as List<T> ?? list.ToList();
            enumerated.ForEach(t => action(t));
            return enumerated;
        }

        //public static List<T> AddMany<T>(IEnumerable<T> list, params int[] @params)
        //{
        //    var list = new 
        //    var enumerated = list as List<T> ?? list.ToList();
        //    enumerated.ForEach(t => action(t));
        //    return enumerated;
        //}

        public static bool IsNullOrEmpty<T>(this IEnumerable<T> list)
        {
            if (list == null || !list.Any())
                return true;
            return false;
        }

        public static Script<T> WithDefaultOptions<T>(this Script<T> script)
        {
            var consoleAssembly = typeof(System.Console).GetTypeInfo().Assembly;
            var scriptOptions = ScriptOptions.Default;
            scriptOptions = scriptOptions.WithImports("System");
            scriptOptions = scriptOptions.WithReferences(consoleAssembly);
            script = script.WithOptions(scriptOptions);
            return script;
        }
    }
}