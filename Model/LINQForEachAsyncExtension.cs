using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealtorObjects.Model
{
    static class LINQForEachAsyncExtension
    {
        public static async Task ForEachAsync<T>(this List<T> list, Func<T, Task> func) {
            foreach (var value in list) {
                await func(value);
            }
        }
    }
}
