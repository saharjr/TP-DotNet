using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using TP2.Models;
using TP2.Models.Repositories;

namespace TP2.Controllers
{
    public class HomeController : Controller
    {
        private readonly ICategoryRepository _categRepository;

        // Injection du repository des catégories
        public HomeController(ICategoryRepository categRepository)
        {
            _categRepository = categRepository;
        }

        public IActionResult Index()
        {
            // Charger les catégories pour le menu latéral
            ViewData["Categories"] = _categRepository.GetAll();
            return View();
        }

        public IActionResult Privacy()
        {
            // Charger les catégories pour le menu latéral
            ViewData["Categories"] = _categRepository.GetAll();
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            ViewData["Categories"] = _categRepository.GetAll();
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}