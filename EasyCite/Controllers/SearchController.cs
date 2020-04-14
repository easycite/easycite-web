using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using EasyCiteLib.Interface.Documents;
using EasyCiteLib.Interface.Queue;
using EasyCiteLib.Interface.Search;
using EasyCiteLib.Models.Search;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace EasyCite.Controllers
{
    public class SearchController : Controller
    {
        private readonly IProjectReferencesProcessor _projectReferencesProcessor;
        private readonly ISearchForArticlesProcessor _searchForArticlesProcessor;
        private readonly IDocumentSearchProcessor _documentSearchProcessor;
        private readonly IBibFileProcessor _bibFileProcessor;
        private readonly IQueueManager _queueManager;

        private readonly int _scrapeDepth;

        public SearchController(
            IProjectReferencesProcessor projectReferencesProcessor,
            ISearchForArticlesProcessor searchForArticlesProcessor,
            IDocumentSearchProcessor documentSearchProcessor,
            IBibFileProcessor bibFileProcessor,
            IQueueManager queueManager,
            IConfiguration configuration)
        {
            _projectReferencesProcessor = projectReferencesProcessor;
            _searchForArticlesProcessor = searchForArticlesProcessor;
            _documentSearchProcessor = documentSearchProcessor;
            _bibFileProcessor = bibFileProcessor;
            _queueManager = queueManager;
            _scrapeDepth = int.Parse(configuration["ScrapeDepth"]);
        }
        public IActionResult Index(int projectId) => View(projectId);

        public async Task<JsonResult> GetReferences(int projectId)
        {
            return Json(await _projectReferencesProcessor.GetAllAsync(projectId));
        }

        [HttpPost]
        public async Task<JsonResult> AddReference(int projectId, string[] documentIds)
        {
            var results = await _projectReferencesProcessor.AddAsync(projectId, documentIds);
            
            foreach (var addedReference in results.Data)
                _queueManager.QueueArticleScrape(addedReference, _scrapeDepth);

            return Json(results);
        }

        [HttpPost]
        public async Task<JsonResult> RemoveReference(int projectId, string documentId)
        {
            var results = await _projectReferencesProcessor.RemoveAsync(projectId, documentId);

            return Json(results);
        }

        public async Task<JsonResult> PendingReferencesStatus(int projectId, string[] documentIds)
        {
            var results = await _projectReferencesProcessor.GetCompletedScrapesAsync(projectId, documentIds);

            return Json(results);
        }

        [HttpPost]
        public async Task<JsonResult> Search(int projectId, SearchData searchData)
        {
            searchData.SearchTags?.RemoveAll(string.IsNullOrWhiteSpace);
            return Json(await _searchForArticlesProcessor.SearchAsync(projectId, searchData));
        }

        public async Task<IActionResult> SearchByName(string term)
        {
            if (string.IsNullOrEmpty(term))
                return BadRequest();
            
            var results = await _documentSearchProcessor.SearchByNameAsync(term);
            return Json(results.Results);
        }

        public async Task<JsonResult> UploadAndSearchByBibFile(IFormFile file)
        {
            string fileContent;
            using (var streamReader = new StreamReader(file.OpenReadStream()))
                fileContent = await streamReader.ReadToEndAsync();

            IEnumerable<string> titles = await _bibFileProcessor.GetTitlesAsync(fileContent);
            IEnumerable<Task<DocumentSearchResults.Article>> tasks = titles.Select(t => _documentSearchProcessor.GetByNameExactAsync(t));
            List<DocumentSearchResults.Article> results = (await Task.WhenAll(tasks)).Where(r => r != null).ToList();

            return Json(results);
        }
    }
}