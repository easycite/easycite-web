using System;
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
            int depth = searchData.SearchDepth switch
            {
                SearchDepth.Low => 1,
                SearchDepth.Medium => 2,
                SearchDepth.High => 3,
                _ => throw new ArgumentOutOfRangeException(nameof(searchData.SearchDepth), searchData.SearchDepth, null)
            };
            
            IList<string> resultIds = await _documentContext.SearchDocumentsAsync(searchData.SearchByIds, searchData.SearchSortType, depth);

            List<Document> documents = await _documentContext.GetDocumentsAsync(resultIds);

            if (searchData.AnyTags.Count == 0 && searchData.AllTags.Count == 0)
                return documents;
            
            return documents.Where(d => d.Keywords?.Count > 0 && searchData.AllTags.All(k => d.Keywords.Contains(k)) && (searchData.AnyTags.Count == 0 || searchData.AnyTags.Any(k => d.Keywords.Contains(k)))).ToList();
        }
    }
}