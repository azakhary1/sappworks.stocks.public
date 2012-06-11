using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Stocks.Common
{
    public static class CollectionHelpers
    {
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> value)
        {
            if (value == null || value.Count() == 0)
            {
                return true;
            }

            return false;
        }
    }
}
