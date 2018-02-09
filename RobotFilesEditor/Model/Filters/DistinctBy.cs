using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotFilesEditor
{
    public static class CollectionExtension
    {
        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            HashSet<TKey> seenKeys = new HashSet<TKey>();
            foreach (TSource element in source)
            {
                if (seenKeys.Add(keySelector(element)))
                {
                    yield return element;
                }
            }
        }

        //public static Dictionary<int, string> ConvertToFilterDictionary<TSource, TKey>(this Dictionary<int, TSource> source, Func<TSource, TKey> keySelector)
        //{
        //    Dictionary<int, string> result = new Dictionary<int, string>();        

        //    foreach (var element in source)
        //    {
        //        yield return (element.Key, keySelector(element.Value).ToString());               
        //    }            
        //}
    }
}
