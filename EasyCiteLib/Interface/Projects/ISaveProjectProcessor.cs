using System.Threading.Tasks;
using EasyCiteLib.Models;
using EasyCiteLib.Models.Projects;

namespace EasyCiteLib.Interface.Projects
{
    public interface ISaveProjectProcessor
    {
        Task<Results<ProjectVm>> SaveAsync(ProjectSaveData saveData);
    }
}