﻿using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using EasyCite.Models;
using EasyCiteLib.Repository;
using Microsoft.AspNetCore.Authorization;

namespace EasyCite.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public async Task<IActionResult> Index() => View();

        [Authorize]
        public IActionResult TestAuthentication()
        {
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
