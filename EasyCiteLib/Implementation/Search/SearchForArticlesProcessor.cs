using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EasyCiteLib.Interface.Search;
using EasyCiteLib.Models;
using EasyCiteLib.Models.Search;
using EasyCiteLib.Repository;
using EasyCiteLib.Repository.EasyCite;
using Microsoft.EntityFrameworkCore;

namespace EasyCiteLib.Implementation.Search
{
    public class SearchForArticlesProcessor : ISearchForArticlesProcessor
    {
        private readonly ISearchResultsCacheManager _searchResultsCacheManager;
        private readonly IGenericDataContextAsync<ProjectHiddenResult> _hiddenResultContext;

        public SearchForArticlesProcessor(ISearchResultsCacheManager searchResultsCacheManager, IGenericDataContextAsync<ProjectHiddenResult> hiddenResultContext)
        {
            _searchResultsCacheManager = searchResultsCacheManager;
            _hiddenResultContext = hiddenResultContext;
        }

        public async Task<Results<SearchResultsVm>> SearchAsync(int projectId, SearchData searchData)
        {
            int offset = Math.Max(searchData.PageNumber, 0) * searchData.ItemsPerPage;

            IList<Document> documents;
            int totalCount;

            try
            {
                var hiddenResults = await GetHiddenResultsAsync(projectId);

                List<Document> allResults = (await _searchResultsCacheManager.GetSearchResultsAsync(projectId, searchData)).Where(d => !hiddenResults.Contains(d.Id)).ToList();

                documents = allResults.Skip(offset).Take(searchData.ItemsPerPage).ToList();
                totalCount = allResults.Count;
            }
            catch (Exception e)
            {
                return new Results<SearchResultsVm> { Exceptions = { e } };
            }

            var vm = new SearchResultsVm
            {
                Results = documents.Select(d => new ResultVm
                    {
                        Id = d.Id,
                        Title = d.Title,
                        PublishDate = d.PublishDate.ToString("M/d/yyyy"),
                        Conference = d.PublicationTitle,
                        Abstract = d.Abstract,
                        AuthorName = string.Join(", ", d.Authors.Select(a => a.Name))
                    })
                    .ToList(),
                NumberOfPages = (int)Math.Ceiling((double)totalCount / searchData.ItemsPerPage)
            };

            return new Results<SearchResultsVm>
            {
                Data = vm
            };
        }

        Task<List<string>> GetHiddenResultsAsync(int projectId)
        {
            return _hiddenResultContext.DataSet.Where(h => h.ProjectId == projectId).Select(h => h.ReferenceId).ToListAsync();
        }
    }
}