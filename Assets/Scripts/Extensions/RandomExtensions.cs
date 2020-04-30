using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Extensions
{
    public static class RandomExtensions
    {
        public static T RandomElement<T>(this IEnumerable<T> enumerable, T defaultValue = default)
        {
            if (enumerable == null)
            {
                return defaultValue;
            }
            
            var e = enumerable as List<T> ?? enumerable.ToList();
            if (!e.Any())
            {
                return defaultValue;
            }

            var index = Random.Range(0, e.Count());
            return e.ElementAt(index);
        }
    }
}