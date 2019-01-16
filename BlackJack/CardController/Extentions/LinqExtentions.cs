using System;
using System.Collections.Generic;
using System.Linq;

namespace CardController.Extentions
{
    public static class LinqExtentions
    {
        public static IEnumerable<T> RandItems<T>(this IEnumerable<T> items)
        {
            return items.OrderBy(x => Guid.NewGuid());
        }
    }
}
