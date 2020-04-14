using System;
using System.Threading.Tasks;

namespace EasyCiteLib.Implementation.Helpers
{
    public static class RunExtension
    {
        public static Task<T> Run<T>(Func<Task<T>> func) => func();
    }
}