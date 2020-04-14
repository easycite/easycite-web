using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EasyCiteLib.Interface.Search;
using EasyCiteLib.Models.Search;
using EasyCiteLib.Repository;

namespace EasyCiteLib.Implementation.Search
{
    public class SearchResultsProcessor : ISearchResultsProcessor
    {
        private readonly DocumentContext _documentContext;
        
        public SearchResultsProcessor(DocumentContext documentContext)
        {
            _documentContext = documentContext;
        }

        public async Task<IReadOnlyList<Document>> SearchAsync(SearchData searchData)
        {
            IList<string> resultIds = await _documentContext.SearchDocumentsAsync(searchData.SearchByIds, searchData.SearchSortType);

            List<Document> documents = await _documentContext.GetDocumentsAsync(resultIds);

            if (searchData.SearchTags.Count == 0)
                return documents;
            
            return documents.Where(d => d.Keywords.Any(searchData.SearchTags.Contains)).ToList();
        }
    }
}