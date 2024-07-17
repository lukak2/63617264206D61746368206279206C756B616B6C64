using System.Collections.Generic;

using Random = UnityEngine.Random;

namespace Runtime.Utilities
{
    public static class Extensions
    {
        public static T GetRandomElement<T>(this IList<T> list)
        {
            return list[Random.Range(0, list.Count)];
        }
    }
}