using System.Diagnostics;
using System.Threading.Tasks;
using EasyCite.Models;
using EasyCiteLib.Interface;
using Microsoft.AspNetCore.Mvc;

namespace EasyCite.Controllers
{
    public class HomeController : Controller
    {
        readonly IGetExampleDataProcessor _getExampleDataProcessor;

        public HomeController(IGetExampleDataProcessor getExampleDataProcessor)
        {
            _getExampleDataProcessor = getExampleDataProcessor;
        }
        public async Task<ActionResult> Example(int id = 1)
        {
            return View(await _getExampleDataProcessor.GetAsync(id));
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
