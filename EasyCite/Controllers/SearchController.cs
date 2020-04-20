using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using EasyCiteLib.Interface.Documents;
using EasyCiteLib.Interface.Search;
using EasyCiteLib.Interface.Search.Export;
using EasyCiteLib.Models.Search;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EasyCite.Controllers
{
    [Authorize]
    public class SearchController : Controller
    {
        private readonly IProjectReferencesProcessor _projectReferencesProcessor;
        private readonly ISearchForArticlesProcessor _searchForArticlesProcessor;
        private readonly ILoadSearchProcessor _loadSearchProcessor;
        private readonly IDocumentSearchProcessor _documentSearchProcessor;
        private readonly IBibFileProcessor _bibFileProcessor;
        private readonly IGetBibStreamProcessor _getBibStreamProcessor;
        private readonly IGetSearchExportDataProcessor _getSearchExportDataProcessor;

        public SearchController(
            IProjectReferencesProcessor projectReferencesProcessor,
            ISearchForArticlesProcessor searchForArticlesProcessor,
            ILoadSearchProcessor loadSearchProcessor,
            IDocumentSearchProcessor documentSearchProcessor,
            IBibFileProcessor bibFileProcessor,
            IGetBibStreamProcessor getBibStreamProcessor,
            IGetSearchExportDataProcessor getSearchExportDataProcessor)
        {
            _projectReferencesProcessor = projectReferencesProcessor;
            _searchForArticlesProcessor = searchForArticlesProcessor;
            _loadSearchProcessor = loadSearchProcessor;
            _documentSearchProcessor = documentSearchProcessor;
            _bibFileProcessor = bibFileProcessor;
            _getBibStreamProcessor = getBibStreamProcessor;
            _getSearchExportDataProcessor = getSearchExportDataProcessor;
        }

        public async Task<IActionResult> Index(int? projectId)
        {
            if (projectId == null)
                return RedirectToAction("Index", "Projects");

            return View(await _loadSearchProcessor.LoadAsync(projectId.Value));
        }

        public async Task<JsonResult> GetReferences(int projectId)
        {
            return Json(await _projectReferencesProcessor.GetAllAsync(projectId));
        }

        [HttpPost]
        public async Task<JsonResult> AddReference(int projectId, string[] documentIds)
        {
            var results = await _projectReferencesProcessor.AddAsync(projectId, documentIds);

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

        public async Task<JsonResult> HideResult(int projectId, string documentId)
        {
            var results = await _projectReferencesProcessor.HideResultAsync(projectId, documentId);

            return Json(results);
        }

        #region Export
        public async Task<IActionResult> DownloadBibFile(int projectId)
        {
            var results = await _getBibStreamProcessor.GetAsync(projectId);
            return File(results.Data.Content, "text/plain", results.Data.Filename);
        }

        public async Task<JsonResult> GetExportData(int projectId)
        {
            return Json(await _getSearchExportDataProcessor.GetAsync(projectId));
        }
        #endregion
    }

}