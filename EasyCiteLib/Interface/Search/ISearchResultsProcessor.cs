using System.Collections.Generic;
using System.Threading.Tasks;
using EasyCiteLib.Models.Search;

namespace EasyCiteLib.Interface.Search
{
    public interface ISearchResultsProcessor
    {
        Task<IReadOnlyList<string>> SearchAsync(SearchData searchData);
    }
}