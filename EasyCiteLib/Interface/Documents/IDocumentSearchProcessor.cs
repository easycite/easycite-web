using System.Threading.Tasks;
using EasyCiteLib.Models.Search;

namespace EasyCiteLib.Interface.Documents
{
    public interface IDocumentSearchProcessor
    {
        Task<DocumentSearchResults> SearchByNameAsync(string query, int itemsPerPage = 10, int page = 1);
        Task<DocumentSearchResults.Article> GetByNameExactAsync(string name);
    }
}