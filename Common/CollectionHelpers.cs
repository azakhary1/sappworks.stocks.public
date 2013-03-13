
namespace Stocks.Common
{
    using System.Collections.Generic;
    using System.Linq;

    public static class CollectionHelpers
    {
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> value)
        {
            return (value == null || !value.Any());
        }
    }
}
