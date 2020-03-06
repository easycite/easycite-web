using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using EasyCite.Models;
using EasyCiteLib.Interface;

namespace EasyCite.Controllers
{
    public class HomeController : Controller
    {
        private readonly IGetExampleDataProcessor _getExampleDataProcessor;

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

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

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
