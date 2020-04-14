using System;
using System.Threading.Tasks;
using EasyCiteLib.Interface.Helpers;
using EasyCiteLib.Interface.Projects;
using EasyCiteLib.Models;
using EasyCiteLib.Repository;
using EasyCiteLib.Repository.EasyCite;
using Microsoft.EntityFrameworkCore;

namespace EasyCiteLib.Implementation.Projects
{
    public class DeleteProjectProcessor : IDeleteProjectProcessor
    {
        private readonly IGetCurrentUserProcessor _getCurrentUserProcessor;
        private readonly IGenericDataContextAsync<Project> _projectDataContext;

        public DeleteProjectProcessor(
            IGetCurrentUserProcessor getCurrentUserProcessor,
            IGenericDataContextAsync<Project> projectDataContext)
        {
            _getCurrentUserProcessor = getCurrentUserProcessor;
            _projectDataContext = projectDataContext;
        }
        public async Task<Results<bool>> DeleteAsync(int projectId)
        {
            var results = new Results<bool>();

            var project = await _projectDataContext.DataSet.FirstOrDefaultAsync(p => p.Id == projectId);

            try
            {
                var userId = await _getCurrentUserProcessor.GetUserIdAsync();
                
                if(project == null || project.UserId != userId)
                    throw new Exception();
                
                _projectDataContext.DataSet.Remove(project);
                await _projectDataContext.SaveChangesAsync();

                results.Data = true;
            }
            catch (System.Exception e)
            {
                results.AddException(new Exception("Could not delete this project", e));
            }

            return results;
        }
    }
}