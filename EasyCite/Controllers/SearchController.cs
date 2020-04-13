using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using EasyCiteLib.Implementation.Helpers;
using EasyCiteLib.Interface.Documents;
using Microsoft.AspNetCore.Mvc;
using EasyCiteLib.Interface.Search;
using EasyCiteLib.Models;
using EasyCiteLib.Models.Search;
using Microsoft.AspNetCore.Http;

namespace EasyCite.Controllers
{
    public class SearchController : Controller
    {
        private readonly IProjectReferencesProcessor _projectReferencesProcessor;
        private readonly ISearchForArticlesProcessor _searchForArticlesProcessor;
        private readonly IDocumentSearchProcessor _documentSearchProcessor;
        private readonly IBibFileProcessor _bibFileProcessor;

        public SearchController(
            IProjectReferencesProcessor projectReferencesProcessor,
            ISearchForArticlesProcessor searchForArticlesProcessor,
            IDocumentSearchProcessor documentSearchProcessor,
            IBibFileProcessor bibFileProcessor)
        {
            _projectReferencesProcessor = projectReferencesProcessor;
            _searchForArticlesProcessor = searchForArticlesProcessor;
            _documentSearchProcessor = documentSearchProcessor;
            _bibFileProcessor = bibFileProcessor;
        }
        public IActionResult Index(int projectId) => View(projectId);

        public async Task<JsonResult> GetReferences(int projectId)
        {
            return Json(await _projectReferencesProcessor.GetAllAsync(projectId));
        }

        [HttpPost]
        public async Task<JsonResult> AddReference(int projectId, string documentId)
        {
            var addResults = await _projectReferencesProcessor.AddAsync(projectId, documentId);
            var results = await _projectReferencesProcessor.GetAllAsync(projectId);
            
            results.MergeExceptions(addResults);

            return Json(results);
        }

        [HttpPost]
        public async Task<JsonResult> RemoveReference(int projectId, string documentId)
        {
            var removeResults = await _projectReferencesProcessor.RemoveAsync(projectId, documentId);
            var results = await _projectReferencesProcessor.GetAllAsync(projectId);
            
            results.MergeExceptions(removeResults);

            return Json(results);
        }

        public async Task<JsonResult> ImportReferences(int projectId, IEnumerable<string> documentIds)
        {
            IEnumerable<Task<Results<bool>>> tasks = documentIds.Select(d => _projectReferencesProcessor.AddAsync(projectId, d));
            Results<bool>[] allResults = await Task.WhenAll(tasks);

            Results<bool> totalResult = allResults.Aggregate((aggregateResult, nextResult) =>
            {
                aggregateResult.Data = aggregateResult.Data || nextResult.Data;
                aggregateResult.MergeExceptions(nextResult);
                return aggregateResult;
            });

            return Json(totalResult);
        }

        [HttpPost]
        public async Task<JsonResult> Search(SearchData searchData)
        {
            searchData.SearchTags?.RemoveAll(string.IsNullOrWhiteSpace);
            return Json(await _searchForArticlesProcessor.SearchAsync(searchData));
        }

        public async Task<JsonResult> SearchByName(string term)
        {
            var results = await _documentSearchProcessor.SearchByNameAsync(term).ToListAsync();
            return Json(results);
        }

        public async Task<JsonResult> UploadAndSearchByBibFile(IFormFile file)
        {
            string fileContent;
            using (var streamReader = new StreamReader(file.OpenReadStream()))
                fileContent = await streamReader.ReadToEndAsync();

            IEnumerable<string> titles = await _bibFileProcessor.GetTitlesAsync(fileContent);
            IEnumerable<Task<DocumentSearchData>> tasks = titles.Select(t => _documentSearchProcessor.GetByNameExactAsync(t));
            List<DocumentSearchData> results = (await Task.WhenAll(tasks)).Where(r => r != null).ToList();

            return Json(results);
        }
    }
}