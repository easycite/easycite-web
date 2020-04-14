using System.Threading.Tasks;
using EasyCiteLib.Models;

namespace EasyCiteLib.Interface.Projects
{
    public interface IDeleteProjectProcessor
    {
        Task<Results<bool>> DeleteAsync(int projectId);
    }
}