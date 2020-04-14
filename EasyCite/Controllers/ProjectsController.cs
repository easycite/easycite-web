using System.Threading.Tasks;
using EasyCiteLib.Interface.Projects;
using EasyCiteLib.Models.Projects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EasyCite.Controllers
{
    [Authorize]
    public class ProjectsController : Controller
    {
        private readonly IGetProjectProcessor _getProjectProcessor;
        private readonly ISaveProjectProcessor _saveProjectProcessor;
        private readonly IDeleteProjectProcessor _deleteProjectProcessor;

        public ProjectsController(
            IGetProjectProcessor getProjectProcessor,
            ISaveProjectProcessor saveProjectProcessor,
            IDeleteProjectProcessor deleteProjectProcessor)
        {
            _getProjectProcessor = getProjectProcessor;
            _saveProjectProcessor = saveProjectProcessor;
            _deleteProjectProcessor = deleteProjectProcessor;
        }

        public async Task<IActionResult> Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<JsonResult> Load()
        {
            return Json(await _getProjectProcessor.GetAllAsync());
        }

        [HttpPost]
        public async Task<JsonResult> Save(ProjectSaveData saveData)
        {
            // Saves or inserts
            return Json(await _saveProjectProcessor.SaveAsync(saveData));
        }

        [HttpPost]
        public async Task<JsonResult> Delete(int projectId)
        {
            return Json(await _deleteProjectProcessor.DeleteAsync(projectId));
        }
    }
}