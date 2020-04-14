using System.Collections.Generic;
using System.Threading.Tasks;
using EasyCiteLib.Models.Search;
using EasyCiteLib.Repository;

namespace EasyCiteLib.Interface.Search
{
    public interface ISearchResultsCacheManager
    {
        Task<IReadOnlyList<Document>> GetSearchResultsAsync(int projectId, SearchData searchData);
    }
}