using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EasyCiteLib.Interface.Documents;
using EasyCiteLib.Interface.Helpers;
using EasyCiteLib.Interface.Search;
using EasyCiteLib.Models;
using EasyCiteLib.Models.Search;
using EasyCiteLib.Repository;
using EasyCiteLib.Repository.EasyCite;
using Microsoft.EntityFrameworkCore;

namespace EasyCiteLib.Implementation.Search
{
    public class ProjectReferencesProcessor : IProjectReferencesProcessor
    {
        private readonly IGetDocumentProcessor _getDocumentProcessor;
        private readonly IGenericDataContextAsync<Project> _projectContext;
        private readonly IGetCurrentUserProcessor _getCurrentUserProcessor;

        public ProjectReferencesProcessor(
            IGetDocumentProcessor getDocumentProcessor,
            IGenericDataContextAsync<Project> projectContext,
            IGetCurrentUserProcessor getCurrentUserProcessor)
        {
            _getDocumentProcessor = getDocumentProcessor;
            _projectContext = projectContext;
            _getCurrentUserProcessor = getCurrentUserProcessor;
        }
        public async Task<Results<bool>> AddAsync(int projectId, string documentId)
        {
            var results = new Results<bool>();

            try
            {
                var projectResults = await GetProjectIfBelongsToUserAsync(projectId);
                if (projectResults.HasError)
                    results.Merge(projectResults);
                if (results.HasProblem) return results;

                if (projectResults.Data.ProjectReferences.Any(pr => pr.ReferenceId == documentId))
                    results.AddError("Your project already contains that reference");
                if (results.HasProblem) return results;

                projectResults.Data.ProjectReferences.Add(new ProjectReference
                {
                    ReferenceId = documentId
                });

                await _projectContext.SaveChangesAsync();
            }
            catch (System.Exception e)
            {
                results.AddException(new System.Exception("Failed to add this reference", e));
            }

            return results;
        }

        public async Task<Results<List<ReferenceVm>>> GetAllAsync(int projectId)
        {
            var results = new Results<List<ReferenceVm>>();

            try
            {
                var projectResults = await GetProjectIfBelongsToUserAsync(projectId);

                if (projectResults.HasError)
                {
                    results.Merge(projectResults);
                    return results;
                }

                var documentIds = projectResults.Data.ProjectReferences
                    .Select(pr => pr.ReferenceId);
                var documents = await _getDocumentProcessor.GetDocumentsAsync(documentIds);

                results.Data = documents.Select(d => new ReferenceVm
                {
                    Id = d.Id,
                    Title = d.Title,
                    Abstract = d.Abstract
                }).ToList();
            }
            catch (System.Exception e)
            {
                results.AddException(new System.Exception("Failed to get your references", e));
            }

            return results;
        }
        public async Task<Results<bool>> RemoveAsync(int projectId, string documentId)
        {
            var results = new Results<bool>();
            
            try
            {
                var projectResults = await GetProjectIfBelongsToUserAsync(projectId);
                if (projectResults.HasError)
                    results.Merge(projectResults);
                if (results.HasProblem) return results;

                var toRemove = projectResults.Data.ProjectReferences
                    .FirstOrDefault(pr => pr.ReferenceId == documentId);

                if (toRemove == null)
                    results.AddError("Your project does not have that reference");
                if (results.HasProblem) return results;

                projectResults.Data.ProjectReferences.Remove(toRemove);

                await _projectContext.SaveChangesAsync();
            }
            catch (System.Exception e)
            {
                results.AddException(new System.Exception("Failed to add this reference", e));
            }

            return results;
        }

        private async Task<Results<Project>> GetProjectIfBelongsToUserAsync(int projectId)
        {
            Results<Project> results = new Results<Project>();

            var userId = await _getCurrentUserProcessor.GetUserIdAsync();

            results.Data = await _projectContext.DataSet
                .Include(p => p.ProjectReferences)
                .FirstOrDefaultAsync(p => p.Id == projectId && p.UserId == userId);

            if (results.Data == null)
                results.AddError("Could not find this project");

            return results;
        }
    }
}