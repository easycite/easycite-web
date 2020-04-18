using System.Threading.Tasks;
using EasyCiteLib.Interface.Helpers;
using EasyCiteLib.Interface.Projects;
using EasyCiteLib.Models;
using EasyCiteLib.Repository;
using EasyCiteLib.Repository.EasyCite;
using Microsoft.EntityFrameworkCore;

namespace EasyCiteLib.Implementation.Projects
{
    public class GetProjectForUserProcessor : IGetProjectForUserProcessor
    {
        private readonly IGetCurrentUserProcessor _getCurrentUserProcessor;
        private readonly IGenericDataContextAsync<Project> _projectContext;

        public GetProjectForUserProcessor(
            IGetCurrentUserProcessor getCurrentUserProcessor,
            IGenericDataContextAsync<Project> projectContext)
        {
            _getCurrentUserProcessor = getCurrentUserProcessor;
            _projectContext = projectContext;
        }
        public async Task<Results<Project>> GetAsync(int projectId, int? forUserId = null)
        {
            var results = new Results<Project>();

            var userId = forUserId ?? await _getCurrentUserProcessor.GetUserIdAsync();

            results.Data = await _projectContext.DataSet
                .FirstOrDefaultAsync(p => p.Id == projectId && p.UserId == userId);

            if(results.Data == null)
                results.AddError($"Cannot find a project with the id {projectId}");

            return results;
        }
    }
}