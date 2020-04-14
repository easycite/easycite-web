using System.Collections.Generic;
using System.Threading.Tasks;
using EasyCiteLib.Models.Search;
using EasyCiteLib.Repository;

namespace EasyCiteLib.Interface.Search
{
    public interface ISearchResultsProcessor
    {
        Task<IReadOnlyList<Document>> SearchAsync(SearchData searchData);
    }
}