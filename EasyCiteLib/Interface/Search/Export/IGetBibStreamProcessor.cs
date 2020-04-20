using System.Threading.Tasks;
using EasyCiteLib.Models;

namespace EasyCiteLib.Interface.Search.Export
{
    public interface IGetBibStreamProcessor
    {
        Task<Results<FileData>> GetAsync(int projectId);
    }
}