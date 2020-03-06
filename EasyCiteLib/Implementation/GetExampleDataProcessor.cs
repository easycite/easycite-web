using System;
using System.Threading.Tasks;
using EasyCiteLib.Interface;
using EasyCiteLib.Models;

namespace EasyCiteLib.Implementation
{
    public class GetExampleDataProcessor : IGetExampleDataProcessor
    {
        public Task<ExampleData> GetAsync(int id)
        {
            var results = new ExampleData
            {
                FirstName = "Andrew",
                LastName = "Schmid",
                Id = id,
                Guid = Guid.NewGuid()
            };

            return Task.FromResult(results);
        }
    }
}