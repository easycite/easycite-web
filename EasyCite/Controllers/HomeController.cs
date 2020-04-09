using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using EasyCite.Models;
using EasyCiteLib.Repository;
using EasyCiteLib.Repository.EasyCite;

namespace EasyCite.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IGenericDataContextAsync<User> _userContext;

        public HomeController(ILogger<HomeController> logger,
                              IGenericDataContextAsync<User> userContext)
        {
            _logger = logger;
            _userContext = userContext;
        }

        public IActionResult Index()
        {
            var test = _userContext.DataSet;
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public async Task<IActionResult> Test([FromServices] DocumentContext documentContext)
        {
            var docs = await documentContext.GetDocumentsAsync(new[] { "1", "2" });

            return Ok(docs);
        }
    }
}
