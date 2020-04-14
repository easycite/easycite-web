using System.Collections.Generic;
using System.Threading.Tasks;
using EasyCiteLib.Models;
using EasyCiteLib.Models.Search;

namespace EasyCiteLib.Interface.Search
{
    public interface IProjectReferencesProcessor
    {
        Task<Results<List<string>>> AddAsync(int projectId, IEnumerable<string> documentIds);
        Task<Results<bool>> RemoveAsync(int projectId, string documentId);
        Task<Results<List<ReferenceVm>>> GetAllAsync(int projectId);

        Task<Results<List<ReferenceVm>>> GetCompletedScrapesAsync(int projectId, IEnumerable<string> documentIds);
    }
}