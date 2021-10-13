using System.Collections.Generic;

namespace Raytracer.Extensions
{
    public static class HashSetExtensions
    {
        public static void AddRange<T>(this HashSet<T> extends, IEnumerable<T> items)
        {
            foreach (T item in items)
                extends.Add(item);
        }
    }
}
