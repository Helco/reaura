using System;
using System.Collections.Generic;
using System.Linq;

namespace Aura
{
    public static class EnumerableExtension
    {
        public static IEnumerable<T> Except<T>(this IEnumerable<T> set, params T[] excepts) => set.Except(excepts);
    }
}
