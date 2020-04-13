using System.Collections.Generic;
using System.Threading.Tasks;
using EasyCiteLib.Interface.Documents;
using EasyCiteLib.Models.Search;

namespace EasyCiteLib.Implementation.Documents
{
    public class MockDocumentSearchProcessor : IDocumentSearchProcessor
    {
        public async IAsyncEnumerable<DocumentSearchData> SearchByNameAsync(string query)
        {
            await Task.Yield();
            
            yield return new DocumentSearchData
            {
                Id = "12229",
                Title = "Location estimation and uncertainty analysis for mobile robots"
            };
            yield return new DocumentSearchData
            {
                Id = "1676196",
                Title = "Spatial Planning: A Configuration Space Approach"
            };
            yield return new DocumentSearchData
            {
                Id = "976357",
                Title = "General solution for linearized systematic error propagation in vehicle odometry"
            };
            yield return new DocumentSearchData
            {
                Id = "1622527",
                Title = "An experimental system for processing movement information of vehicle"
            };
        }

        public Task<DocumentSearchData> GetByNameExactAsync(string name)
        {
            return Task.FromResult(new DocumentSearchData
            {
                Id = "1622527",
                Title = "An experimental system for processing movement information of vehicle"
            });
        }
    }
}