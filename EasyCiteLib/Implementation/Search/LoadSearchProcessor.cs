using System;
using System.Linq;
using System.Threading.Tasks;
using EasyCiteLib.Interface.Documents;
using EasyCiteLib.Interface.Helpers;
using EasyCiteLib.Interface.Projects;
using EasyCiteLib.Interface.Search;
using EasyCiteLib.Models;
using EasyCiteLib.Models.Search;
using EasyCiteLib.Repository;
using EasyCiteLib.Repository.EasyCite;
using Microsoft.EntityFrameworkCore;
using static EasyCiteLib.Models.Search.SearchData;

namespace EasyCiteLib.Implementation.Search
{
    public class LoadSearchProcessor : ILoadSearchProcessor
    {
        private readonly IGenericDataContextAsync<Project> _projectDataContext;
        private readonly IGetCurrentUserProcessor _getCurrentUserProcessor;
        private readonly IGetProjectForUserProcessor _getProjectForUserProcessor;
        private readonly IGetDocumentProcessor _getDocumentProcessor;

        public LoadSearchProcessor(
            IGenericDataContextAsync<Project> projectDataContext,
            IGetCurrentUserProcessor getCurrentUserProcessor,
            IGetProjectForUserProcessor getProjectForUserProcessor,
            IGetDocumentProcessor getDocumentProcessor)
        {
            _projectDataContext = projectDataContext;
            _getCurrentUserProcessor = getCurrentUserProcessor;
            _getProjectForUserProcessor = getProjectForUserProcessor;
            _getDocumentProcessor = getDocumentProcessor;
        }
        public async Task<Results<SearchVm>> LoadAsync(int projectId)
        {
            var results = new Results<SearchVm>
            {
                Data =
                {
                    ProjectId = projectId
                }
            };

            var projectResults = await _getProjectForUserProcessor.GetAsync(projectId);

            if (projectResults.HasProblem)
            {
                results.Merge(projectResults);
                return results;
            }

            // Add loaded references, then pending
            var loadedDocuments = await _getDocumentProcessor.GetDocumentsAsync(projectResults.Data.ProjectReferences
                .Where(pr => pr.IsPending == false)
                .Select(pr => pr.ReferenceId));

            results.Data.References.AddRange(loadedDocuments.Select(d => new ReferenceVm
            {
                Id = d.Id,
                Title = d.Title,
                IsPending = false,
                Abstract = d.Abstract
            }));

            results.Data.References.AddRange(projectResults.Data.ProjectReferences
                .Where(pr => pr.IsPending == true)
                .Select(pr => new ReferenceVm{
                    Id = pr.ReferenceId,
                    IsPending = true
                }));

            // Sort options
            results.Data.DefaultSortOption = SearchSortType.PageRank;
            foreach (SearchSortType sortType in Enum.GetValues(typeof(SearchSortType)))
                results.Data.SortOptions.Add(new DropdownOption<SearchSortType>
                {
                    Text = sortType.GetDescription(),
                    Value = sortType
                });

            // Search Depth
            results.Data.DefaultSearchDepth = SearchDepth.Medium;
            foreach (SearchDepth searchDepth in Enum.GetValues(typeof(SearchDepth)))
                results.Data.SearchDepths.Add(new DropdownOption<SearchDepth>
                {
                    Text = searchDepth.GetDescription(),
                    Value = searchDepth
                });

            return results;
        }
    }
}