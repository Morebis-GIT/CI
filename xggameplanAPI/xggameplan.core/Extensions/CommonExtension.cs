using System;
using System.Collections.Generic;
using System.Linq;

namespace xggameplan.Extensions
{
    public static class Extensions
    {
        /// <summary>
        /// Dispose the all value to avoid the memory leak
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="set"></param>
        public static void DisposeAll<T>(this IEnumerable<T> set)
        {
            set?.ToList<T>().ForEach(obj =>
            {
                var disp = obj as IDisposable;
                disp?.Dispose();
            });
        }

        public static void DisposeAll<T>(this IDictionary<T, T> dict)
        {
            dict.Clear();
        }
    }
}
