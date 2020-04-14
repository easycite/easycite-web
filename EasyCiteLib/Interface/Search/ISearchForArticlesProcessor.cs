using System.Threading.Tasks;
using EasyCiteLib.Models;
using EasyCiteLib.Models.Search;

namespace EasyCiteLib.Interface.Search
{
    public interface ISearchForArticlesProcessor
    {
        Task<Results<SearchResultsVm>> SearchAsync(int projectId, SearchData searchData);
    }
}