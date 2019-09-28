using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DiscordTopRPG.Models;
using Microsoft.AspNetCore.Authorization;
using System.IO;

namespace DiscordTopRPG.Controllers
{
    public class HomeController : Controller
    {
        
        public IActionResult Index()
        {
            return View();
        }
        [Authorize]
        public IActionResult Privacy()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult Corebook()
        {
            if (!System.IO.File.Exists(Path.Combine(Directory.GetCurrentDirectory(), "Data", "Manual.txt")))
            {
                return Content("TODO");
            }
            var Manual = System.IO.File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(),"Data","Manual.txt"));

            var chapters = Manual.Split("----");
            var model = new Dictionary<string, string>();

            foreach (var x in chapters)
            {
                var y = x.Split("||");
                model.Add(y[0], y[1]);
            }
            return View(model);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
