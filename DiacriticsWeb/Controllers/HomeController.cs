using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using DiacriticsWeb.Models;
using Diacritics;

namespace DiacriticsWeb.Controllers
{
    public class HomeController : Controller
    {

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(DiacriticsText model)
        {
            if (model.OriginalText.Length > 10000)
            {
                model.OriginalText = model.OriginalText.Substring(0, 10000);
            }

            var reconstructor = new Reconstructor(Startup.BinaryFilePath, Startup.PositionTriePath);
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
