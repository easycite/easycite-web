using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EasyCiteLib.Interface.Documents;
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
        private readonly IGetDocumentProcessor _getDocumentProcessor;

        public SearchForArticlesProcessor(ISearchResultsCacheManager searchResultsCacheManager, IGenericDataContextAsync<ProjectHiddenResult> hiddenResultContext, IGetDocumentProcessor getDocumentProcessor)
        {
            _searchResultsCacheManager = searchResultsCacheManager;
            _hiddenResultContext = hiddenResultContext;
            _getDocumentProcessor = getDocumentProcessor;
        }

        public async Task<Results<SearchResultsVm>> SearchAsync(int projectId, SearchData searchData)
        {
            int offset = Math.Max(searchData.PageNumber, 0) * searchData.ItemsPerPage;

            IReadOnlyList<Document> documents;
            int totalCount;

            try
            {
                var hiddenResults = await GetHiddenResultsAsync(projectId);

                var allResultIds = (await _searchResultsCacheManager.GetSearchResultsAsync(projectId, searchData)).Except(hiddenResults).ToList();

                documents = await _getDocumentProcessor.GetDocumentsAsync(allResultIds.Skip(offset).Take(searchData.ItemsPerPage));
                totalCount = allResultIds.Count;
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
                        PublishDate = d.PublishDate,
                        Conference = d.PublicationTitle,
                        Abstract = d.Abstract,
                        AuthorName = string.Join(", ", d.Authors.Select(a => a.Name)),
                        CiteCount = d.CiteCount
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