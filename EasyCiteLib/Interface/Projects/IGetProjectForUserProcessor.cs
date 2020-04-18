using System.Threading.Tasks;
using EasyCiteLib.Models;
using EasyCiteLib.Repository.EasyCite;

namespace EasyCiteLib.Interface.Projects
{
    public interface IGetProjectForUserProcessor
    {
        Task<Results<Project>> GetAsync(int projectId, int? userId = null);
    }
}