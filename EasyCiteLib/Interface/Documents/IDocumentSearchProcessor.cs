using System.Collections.Generic;
using System.Threading.Tasks;
using EasyCiteLib.Models.Search;

namespace EasyCiteLib.Interface.Documents
{
    public interface IDocumentSearchProcessor
    {
        IAsyncEnumerable<DocumentSearchData> SearchByNameAsync(string query);
        Task<DocumentSearchData> GetByNameExactAsync(string name);
    }
}