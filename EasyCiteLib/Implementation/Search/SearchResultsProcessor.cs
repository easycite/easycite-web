using System;
using System.Collections.Generic;
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

        public async Task<IReadOnlyList<string>> SearchAsync(SearchData searchData)
        {
            int depth = searchData.SearchDepth switch
            {
                SearchDepth.Low => 1,
                SearchDepth.Medium => 2,
                SearchDepth.High => 3,
                _ => throw new ArgumentOutOfRangeException(nameof(searchData.SearchDepth), searchData.SearchDepth, null)
            };
            
            return await _documentContext.SearchDocumentsAsync(searchData.SearchByIds, searchData.SearchSortType, depth, searchData.AnyTags, searchData.AllTags);
        }
    }
}