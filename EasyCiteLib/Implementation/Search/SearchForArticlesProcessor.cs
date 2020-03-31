using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EasyCiteLib.Interface.Search;
using EasyCiteLib.Models;
using EasyCiteLib.Models.Search;
using EasyCiteLib.Repository;

namespace EasyCiteLib.Implementation.Search
{
    public class SearchForArticlesProcessor : ISearchForArticlesProcessor
    {
        private readonly DocumentContext _documentContext;

        public SearchForArticlesProcessor(DocumentContext documentContext)
        {
            _documentContext = documentContext;
        }

        public async Task<Results<SearchResultsVm>> SearchAsync(SearchData searchData)
        {
            int offset = Math.Max(searchData.PageNumber, 0) * searchData.ItemsPerPage;
            
            IList<Document> documents;
            int totalCount;

            try
            {
                (documents, totalCount) = await _documentContext.SearchDocumentsAsync(searchData.SearchByIds, searchData.SearchTags, offset, searchData.ItemsPerPage);
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

            return new Results<SearchResultsVm> { Data = vm };
        }
    }
}