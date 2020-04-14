using System.Threading.Tasks;
using EasyCiteLib.Interface.Helpers;
using EasyCiteLib.Interface.Search;
using EasyCiteLib.Models;
using EasyCiteLib.Models.Search;

namespace EasyCiteLib.Implementation.Search
{
    public class MockSearchForArticlesProcessor : ISearchForArticlesProcessor
    {
        private readonly IGetRandomDataProcessor _getRandomDataProcessor;

        public MockSearchForArticlesProcessor(IGetRandomDataProcessor getRandomDataProcessor)
        {
            _getRandomDataProcessor = getRandomDataProcessor;
        }
        public async Task<Results<SearchResultsVm>> SearchAsync(int projectId, SearchData searchData)
        {
            var results = new Results<SearchResultsVm>
            {
                Data = new SearchResultsVm()
            };

            results.Data.NumberOfPages = 5;

            for (var i = 0; i < 10; i++)
                results.Data.Results.Add(new ResultVm
                {
                    Id = _getRandomDataProcessor.GetInt().ToString(),
                    AuthorName = _getRandomDataProcessor.GetName(),
                    Conference = _getRandomDataProcessor.GetText(maxWords: 4),
                    Title = _getRandomDataProcessor.GetText(maxWords: 10),
                    PublishDate = _getRandomDataProcessor.GetDate().ToString("d"),
                    Abstract = _getRandomDataProcessor.GetParagraph()
                });

            return results;
        }
    }
}