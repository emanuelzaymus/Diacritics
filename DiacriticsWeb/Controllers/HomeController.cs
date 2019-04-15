using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using DiacriticsWeb.Models;
using Diacritics;

namespace DiacriticsWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly Reconstructor reconstructor;

        public HomeController(Reconstructor reconstructor)
        {
            this.reconstructor = reconstructor;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(DiacriticsText model)
        {
            model.ReconstructedText = reconstructor.Reconstruct(model.OriginalText);

            return View(model);
        }

        public IActionResult About()
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
