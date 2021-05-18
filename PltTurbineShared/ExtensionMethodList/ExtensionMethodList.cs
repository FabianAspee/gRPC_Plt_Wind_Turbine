using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PltTurbineShared.ExtensionMethodList
{
    public static class ExtensionMethodList
    {
        public static IEnumerable<T> FastReverse<T>(this IList<T> items)
        {
            for (int i = items.Count - 1; i >= 0; i--)
            {
                yield return items[i];
            }
        }
        public static (IList<T> first, IList<T> second) Partition<T>(this IList<T> items, Func<T, bool> func) => (items.Where(func).ToList(), items.Where(x => !func(x)).ToList());

        public static void ForEach<T>(this IEnumerable<T> sequence, Action<int, T> action)
        {
            // argument null checking omitted
            int i = 0;
            foreach (T item in sequence)
            {
                action(i, item);
                i++;
            }
        }
        public static void Deconstruct<T>(this IList<T> list, out T head, out IList<T> tail)
        {
            head = list.FirstOrDefault();
            tail = new List<T>(list.Skip(1));
        }
        public static void Deconstruct<T>(this IList<T> list, out T head, out T head2, out IList<T> tail)
        {
            head = list.FirstOrDefault();
            head2 = list.Skip(1).FirstOrDefault();
            tail = new List<T>(list.Skip(2));
        }
    }
}
