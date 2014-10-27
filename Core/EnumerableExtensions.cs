
namespace Sappworks.Stocks
{
    using System.Collections.Generic;
    using System.Linq;

    public static class EnumerableExtensions
    {
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> enumerable)
        {
            return enumerable == null || !enumerable.Any();
        }

        public static void AddIfNotExists<TKey, TValue>(this IDictionary<TKey, TValue> enumerable, TKey key, TValue value)
        {
            if (!enumerable.ContainsKey(key))
            {
                enumerable.Add(key, value);
            }
        }
    }
}
