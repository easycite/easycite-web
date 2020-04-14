using System.Threading.Tasks;
using EasyCiteLib.Models;
using EasyCiteLib.Models.Search;

namespace EasyCiteLib.Interface.Search
{
    public interface ILoadSearchProcessor
    {
        Task<Results<SearchVm>> LoadAsync(int projectId);
    }
}