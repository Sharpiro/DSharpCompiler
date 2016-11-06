using System;
using System.Collections.Generic;

namespace DSharpCompiler.Core.Common
{
    public static class Extensions
    {
        public static bool ContainsAny<T>(this IEnumerable<T> list, params T[] otherList)
        {
            foreach (var item in list)
            {
                foreach (var item2 in otherList)
                {
                    if (item.Equals(item2))
                        return true;
                }
            }
            return false;
        }

        public static bool In<T>(this string value, params T[] list)
        {
            foreach (var item in list)
            {
                if (item.Equals(value))
                    return true;
            }
            return false;
        }

        public static TValue Get<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key) where TValue : class
        {
            if (dictionary.ContainsKey(key))
                return dictionary[key];
            return null;
        }
    }
}
