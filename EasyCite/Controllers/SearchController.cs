using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using EasyCiteLib.Interface.Search;
using EasyCiteLib.Models.Search;

namespace EasyCite.Controllers
{
    public class SearchController : Controller
    {
        private readonly IProjectReferencesProcessor _projectReferencesProcessor;
        private readonly ISearchForArticlesProcessor _searchForArticlesProcessor;

        public SearchController(
            IProjectReferencesProcessor projectReferencesProcessor,
            ISearchForArticlesProcessor searchForArticlesProcessor)
        {
            _projectReferencesProcessor = projectReferencesProcessor;
            _searchForArticlesProcessor = searchForArticlesProcessor;
        }
        public async Task<IActionResult> Index(int projectId)
        {
            return View(projectId);
        }

        public async Task<JsonResult> GetReferences(int projectId)
        {
            return Json(await _projectReferencesProcessor.GetAllAsync(projectId));
        }

        [HttpPost]
        public async Task<JsonResult> AddReference(int projectId, string documentId)
        {
            var addResults = await _projectReferencesProcessor.AddAsync(projectId, documentId);
            var results = await _projectReferencesProcessor.GetAllAsync(projectId);
            
            results.Merge(addResults);

            return Json(results);
        }

        [HttpPost]
        public async Task<JsonResult> RemoveReference(int projectId, string documentId)
        {
            var removeResults = await _projectReferencesProcessor.RemoveAsync(projectId, documentId);
            var results = await _projectReferencesProcessor.GetAllAsync(projectId);
            
            results.Merge(removeResults);

            return Json(results);
        }

        [HttpPost]
        public async Task<JsonResult> Search(int projectId, SearchData searchData)
        {
            searchData.SearchTags?.RemoveAll(string.IsNullOrWhiteSpace);
            return Json(await _searchForArticlesProcessor.SearchAsync(projectId, searchData));
        }
    }
}