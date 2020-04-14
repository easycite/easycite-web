using System;
using System.Collections.Generic;
using System.Linq;
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
    public class GetProjectProcessor : IGetProjectProcessor
    {
        private readonly IGetCurrentUserProcessor _getCurrentUserProcessor;
        private readonly IGenericDataContextAsync<Project> _projectDataContext;

        public GetProjectProcessor(
            IGetCurrentUserProcessor getCurrentUserProcessor,
            IGenericDataContextAsync<Project> projectDataContext)
        {
            _getCurrentUserProcessor = getCurrentUserProcessor;
            _projectDataContext = projectDataContext;
        }
        public async Task<Results<List<ProjectVm>>> GetAllAsync()
        {
            var results = new Results<List<ProjectVm>>();

            var userId = await _getCurrentUserProcessor.GetUserIdAsync();
            results.Data.AddRange(_projectDataContext.DataSet
                .Where(p => p.UserId == userId)
                .Select(p => new ProjectVm {
                    Id = p.Id,
                    Name = p.Name,
                    NumberOfReferences = p.ProjectReferences.Count()
                })
            );

            return results;
        }

        public async Task<Results<ProjectVm>> GetAsync(int projectId)
        {
            var results = new Results<ProjectVm>();

            try
            {
                var project = await _projectDataContext.DataSet
                    .Include(p => p.ProjectReferences)
                    .FirstAsync(p => p.Id == projectId);

                if(project.UserId != await _getCurrentUserProcessor.GetUserIdAsync())
                    throw new Exception();

                results.Data.Id = project.Id;
                results.Data.Name = project.Name;
                results.Data.NumberOfReferences = project.ProjectReferences.Count();
            }
            catch (Exception e)
            {
                results.AddException(new Exception("Could not load the project", e));
            }

            return results;
        }
    }
}