using System.Collections.Generic;
using System.Threading.Tasks;
using EasyCiteLib.Models.Search;

namespace EasyCiteLib.Interface.Search
{
    public interface ISearchResultsCacheManager
    {
        Task<IReadOnlyList<string>> GetSearchResultsAsync(int projectId, SearchData searchData);
    }
}