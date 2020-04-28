using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EasyCiteLib.Interface.Search;
using EasyCiteLib.Models.Search;
using Microsoft.Extensions.Caching.Memory;

namespace EasyCiteLib.Implementation.Search
{
    public class InMemorySearchResultsCacheManager : ISearchResultsCacheManager
    {
        private readonly ISearchResultsProcessor _searchResultsProcessor;
        private readonly IMemoryCache _memoryCache;

        public InMemorySearchResultsCacheManager(ISearchResultsProcessor searchResultsProcessor, IMemoryCache memoryCache)
        {
            _searchResultsProcessor = searchResultsProcessor;
            _memoryCache = memoryCache;
        }

        public async Task<IReadOnlyList<string>> GetSearchResultsAsync(int projectId, SearchData searchData)
        {
            string cacheKey = LocalCacheKeys.ProjectSearchResults(projectId);
            
            if (searchData.ForceNoCache)
                _memoryCache.Remove(cacheKey);

            return await _memoryCache.GetOrCreateAsync(cacheKey,
                entry =>
                {
                    entry.SetSlidingExpiration(TimeSpan.FromHours(6));
                    return _searchResultsProcessor.SearchAsync(searchData);
                });
        }
    }
}