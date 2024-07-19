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
        
        public static void Shuffle<T>(this IList<T> list)
        {
            for (int i = list.Count - 1; i > 0; i--)
            {
                int k = Random.Range(0, i + 1);
                (list[k], list[i]) = (list[i], list[k]);
            }
        }
    }
}