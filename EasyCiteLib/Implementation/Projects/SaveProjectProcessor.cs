using System;
using System.Linq;
using System.Security;
using System.Threading.Tasks;
using EasyCiteLib.Interface.Helpers;
using EasyCiteLib.Interface.Projects;
using EasyCiteLib.Models;
using EasyCiteLib.Models.Projects;
using EasyCiteLib.Repository;
using EasyCiteLib.Repository.EasyCite;
using Microsoft.EntityFrameworkCore;

namespace EasyCiteLib.Implementation.Projects
{
    public class SaveProjectProcessor : ISaveProjectProcessor
    {
        private readonly IGenericDataContextAsync<User> _userContext;
        private readonly IGetCurrentUserProcessor _getCurrentUserProcessor;

        public SaveProjectProcessor(
            IGenericDataContextAsync<User> userContext,
            IGetCurrentUserProcessor getCurrentUserProcessor)
        {
            _userContext = userContext;
            _getCurrentUserProcessor = getCurrentUserProcessor;
        }
        public async Task<Results<ProjectVm>> SaveAsync(ProjectSaveData saveData)
        {
            var results = new Results<ProjectVm>();

            try
            {
                var userId = await _getCurrentUserProcessor.GetUserIdAsync();
                var user = await _userContext.DataSet
                    .Include(u => u.Projects)
                    .FirstAsync(u => u.Id == userId);
                    
                var project = user
                    .Projects
                    .FirstOrDefault(p => p.Id == saveData.Id);

                if (saveData.Id == null)
                {
                    project = new Project()
                    {
                        UserId = userId
                    };
                    user.Projects.Add(project);
                }
                else if (project == null || project.UserId != userId)
                    results.AddError("This project could not be found or you do not own this project");

                if(results.HasError) return results;

                project.Name = saveData.Name;
                await _userContext.SaveChangesAsync();
                results.Data.Id = project.Id;
                results.Data.Name = project.Name;
                results.Data.NumberOfReferences = project.ProjectReferences.Count();
            }
            catch (Exception e)
            {
                results.AddException(new Exception("Could not save this project", e));
            }

            return results;
        }
    }
}