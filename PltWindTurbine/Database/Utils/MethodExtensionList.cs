using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PltWindTurbine.Database.Utils
{
    public static class MethodExtensionList
    {
        public static IEnumerable<T> FastReverse<T>(this IList<T> items)
        {
            for (int i = items.Count - 1; i >= 0; i--)
            {
                yield return items[i];
            }
        } 
        public static void Deconstruct<T>(this List<T> list, out T head, out List<T> tail)
        {
            head = list.FirstOrDefault();
            tail = new List<T>(list.Skip(1));
        }
        public static void Deconstruct<T>(this List<T> list, out T head, out T head2, out List<T> tail)
        {
            head = list.FirstOrDefault();
            head2 = list.Skip(1).FirstOrDefault();
            tail = new List<T>(list.Skip(2));
        }
    }
}
