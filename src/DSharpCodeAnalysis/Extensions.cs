using System;
using System.Collections.Generic;
using System.Linq;
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
    }
}