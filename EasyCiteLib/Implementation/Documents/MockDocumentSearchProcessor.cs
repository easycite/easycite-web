using System.Collections.Generic;
using System.Threading.Tasks;
using EasyCiteLib.Interface.Documents;
using EasyCiteLib.Models.Search;

namespace EasyCiteLib.Implementation.Documents
{
    public class MockDocumentSearchProcessor : IDocumentSearchProcessor
    {
        public Task<DocumentSearchResults> SearchByNameAsync(string query, int itemsPerPage = 10, int page = 1)
        {
            var results = new DocumentSearchResults
            {
                PageCount = 1,
                Results =
                {
                    new DocumentSearchResults.Article
                    {
                        Id = "12229",
                        Title = "Location estimation and uncertainty analysis for mobile robots"
                    },
                    new DocumentSearchResults.Article
                    {
                        Id = "1676196",
                        Title = "Spatial Planning: A Configuration Space Approach"
                    },
                    new DocumentSearchResults.Article
                    {
                        Id = "976357",
                        Title = "General solution for linearized systematic error propagation in vehicle odometry"
                    },
                    new DocumentSearchResults.Article
                    {
                        Id = "1622527",
                        Title = "An experimental system for processing movement information of vehicle"
                    }
                }
            };

            return Task.FromResult(results);
        }

        public Task<DocumentSearchResults.Article> GetByNameExactAsync(string name)
        {
            return Task.FromResult(new DocumentSearchResults.Article
            {
                Id = "1622527",
                Title = "An experimental system for processing movement information of vehicle"
            });
        }
    }
}