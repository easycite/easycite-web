using System.Collections.Generic;
using System.Threading.Tasks;
using EasyCiteLib.Models;
using EasyCiteLib.Models.Projects;

namespace EasyCiteLib.Interface.Projects
{
    public interface IGetProjectProcessor
    {
        Task<Results<ProjectVm>> GetAsync(int projectId);
        Task<Results<List<ProjectVm>>> GetAllAsync();
    }
}