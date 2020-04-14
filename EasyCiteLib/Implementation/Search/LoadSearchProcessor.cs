using System;
using System.Threading.Tasks;
using EasyCiteLib.Interface.Helpers;
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

        public LoadSearchProcessor(
            IGenericDataContextAsync<Project> projectDataContext,
            IGetCurrentUserProcessor getCurrentUserProcessor)
        {
            _projectDataContext = projectDataContext;
            _getCurrentUserProcessor = getCurrentUserProcessor;
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

            var userId = await _getCurrentUserProcessor.GetUserIdAsync();
            var project = await _projectDataContext.DataSet
                .FirstOrDefaultAsync(p => p.Id == projectId && p.UserId == userId);

            if (project == null)
            {
                results.AddError("Cannot find your project");
                return results;
            }

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