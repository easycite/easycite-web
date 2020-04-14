using System.Collections.Generic;
using System.Threading.Tasks;

namespace EasyCiteLib.Implementation.Helpers
{
    public static class Extensions
    {
        public static async Task<List<T>> ToListAsync<T>(this IAsyncEnumerable<T> source)
        {
            var list = new List<T>();
            
            await foreach (T item in source)
                list.Add(item);

            return list;
        }
    }
}