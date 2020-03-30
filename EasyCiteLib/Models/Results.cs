using System;
using System.Collections.Generic;
using System.Linq;

namespace EasyCiteLib.Models
{
    public class Results<T>
    {
        public T Data { get; set; }
        public List<Exception> Exceptions { get; } = new List<Exception>();
        public bool HasException => Exceptions.Any();
        
        public void MergeExceptions<U>(Results<U> results)
        {
            Exceptions.AddRange(results.Exceptions);
        }
    }
}