using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EasyCiteLib.Interface.Documents;
using EasyCiteLib.Interface.Helpers;
using EasyCiteLib.Interface.Projects;
using EasyCiteLib.Interface.Queue;
using EasyCiteLib.Interface.Search;
using EasyCiteLib.Models;
using EasyCiteLib.Models.Search;
using EasyCiteLib.Repository;
using EasyCiteLib.Repository.EasyCite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Configuration;

namespace EasyCiteLib.Implementation.Search
{
    public class ProjectReferencesProcessor : IProjectReferencesProcessor
    {
        private readonly IGetDocumentProcessor _getDocumentProcessor;
        private readonly IGenericDataContextAsync<Project> _projectContext;
        private readonly IGetCurrentUserProcessor _getCurrentUserProcessor;
        private readonly IGetProjectForUserProcessor _getProjectForUserProcessor;
        private readonly IQueueManager _queueManager;
        private readonly int _scrapeDepth;

        public ProjectReferencesProcessor(
            IGetDocumentProcessor getDocumentProcessor,
            IGenericDataContextAsync<Project> projectContext,
            IGetCurrentUserProcessor getCurrentUserProcessor,
            IGetProjectForUserProcessor getProjectForUserProcessor,
            IQueueManager queueManager,
            IConfiguration configuration)
        {
            _getDocumentProcessor = getDocumentProcessor;
            _projectContext = projectContext;
            _getCurrentUserProcessor = getCurrentUserProcessor;
            _getProjectForUserProcessor = getProjectForUserProcessor;
            _queueManager = queueManager;
            _scrapeDepth = int.Parse(configuration["ScrapeDepth"]);
        }
        public async Task<Results<List<string>>> AddAsync(int projectId, IEnumerable<string> documentIds)
        {
            var documentIdList = documentIds.ToList();

            var results = new Results<List<string>>();

            try
            {
                var projectResults = await _getProjectForUserProcessor.GetAsync(projectId);
                if (results.HasProblem)
                {
                    results.Merge(projectResults);
                    return results;
                }

                List<string> documentsToAdd = documentIdList.Except(projectResults.Data.ProjectReferences.Select(pr => pr.ReferenceId)).ToList();

                projectResults.Data.ProjectReferences.AddRange(documentsToAdd.Select(d => new ProjectReference
                {
                    ReferenceId = d
                }));

                await _projectContext.SaveChangesAsync();

                foreach (var d in documentsToAdd)
                    _queueManager.QueueArticleScrape(d, _scrapeDepth);

                results.Data = documentsToAdd;
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
                var projectResults = await _getProjectForUserProcessor.GetAsync(projectId);

                if (projectResults.HasProblem)
                {
                    results.Merge(projectResults);
                    return results;
                }

                foreach (ProjectReference pendingReferences in projectResults.Data.ProjectReferences.Where(pr => pr.IsPending))
                    _queueManager.QueueArticleScrape(pendingReferences.ReferenceId, _scrapeDepth);

                var documents = await _getDocumentProcessor.GetDocumentsAsync(projectResults.Data.ProjectReferences
                    .Select(pr => pr.ReferenceId));

                results.Data = projectResults.Data.ProjectReferences.Join(documents, l => l.ReferenceId, r => r.Id, (l, r) => new ReferenceVm
                {
                    Id = l.ReferenceId,
                    Title = r.Title,
                    Abstract = r.Abstract,
                    IsPending = l.IsPending
                }).ToList();
            }
            catch (System.Exception e)
            {
                results.AddException(new System.Exception("Failed to get your references", e));
            }

            return results;
        }

        public async Task<Results<List<ReferenceVm>>> GetCompletedScrapesAsync(int projectId, IEnumerable<string> documentIds)
        {
            var results = new Results<List<ReferenceVm>>();

            try
            {
                var projectResults = await _getProjectForUserProcessor.GetAsync(projectId);

                if (projectResults.HasProblem)
                {
                    results.Merge(projectResults);
                    return results;
                }

                var completedIds = documentIds.Where(d => projectResults.Data.ProjectReferences.Any(pr => pr.ReferenceId == d && !pr.IsPending));
                var documents = await _getDocumentProcessor.GetDocumentsAsync(completedIds);

                results.Data = documents.Select(d => new ReferenceVm
                {
                    Id = d.Id,
                    Title = d.Title,
                    Abstract = d.Abstract,
                    IsPending = false
                }).ToList();
            }
            catch (System.Exception e)
            {
                results.AddException(new System.Exception("Failed to check for completed scrapes.", e));
            }

            return results;
        }

        public async Task<Results<bool>> HideResultAsync(int projectId, string documentId)
        {
            var results = new Results<bool>();

            try
            {
                var projectResults = await _getProjectForUserProcessor.GetAsync(projectId);
                if (results.HasProblem)
                {
                    results.Merge(projectResults);
                    return results;
                }

                if (projectResults.Data.ProjectHiddenResults.All(ph => ph.ReferenceId != documentId))
                {
                    projectResults.Data.ProjectHiddenResults.Add(new ProjectHiddenResult
                    {
                        ReferenceId = documentId
                    });

                    await _projectContext.SaveChangesAsync();
                }

                results.Data = true;
            }
            catch (System.Exception e)
            {
                results.AddException(new System.Exception("Failed to hide result.", e));
            }

            return results;
        }

        public async Task<Results<bool>> RemoveAsync(int projectId, string documentId)
        {
            var results = new Results<bool>();

            try
            {
                var projectResults = await _getProjectForUserProcessor.GetAsync(projectId);
                if (results.HasProblem)
                {
                    results.Merge(projectResults);
                    return results;
                }

                var toRemove = projectResults.Data.ProjectReferences
                    .FirstOrDefault(pr => pr.ReferenceId == documentId);

                if (toRemove == null)
                    results.AddError("Your project does not have that reference");
                if (results.HasProblem) return results;

                projectResults.Data.ProjectReferences.Remove(toRemove);

                await _projectContext.SaveChangesAsync();

                results.Data = true;
            }
            catch (System.Exception e)
            {
                results.AddException(new System.Exception("Failed to add this reference", e));
            }

            return results;
        }
    }
}