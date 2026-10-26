using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TP2.Models;
using TP2.Models.Repositories;

namespace TP2.Controllers
{
    [Authorize(Roles = "Admin, Manager")]
    public class CategoryController : Controller
    {
        readonly ICategoryRepository CategRepository;

        // Injection du repository des catégories
        public CategoryController(ICategoryRepository categRepository)
        {
            CategRepository = categRepository;
        }

        // GET: CategoryController (Afficher la liste des catégories)
        [AllowAnonymous]
        public ActionResult Index()
        {
            ViewData["Categories"] = CategRepository.GetAll(); // Charger le menu latéral
            var categories = CategRepository.GetAll();
            return View(categories);
        }

        // GET: CategoryController/Details/5 (Détails d'une catégorie)
        [AllowAnonymous]
        public ActionResult Details(int id)
        {
            ViewData["Categories"] = CategRepository.GetAll(); // Charger le menu latéral
            var category = CategRepository.GetById(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        // GET: CategoryController/Create (Formulaire de création)
        public ActionResult Create()
        {
            ViewData["Categories"] = CategRepository.GetAll(); // Charger le menu latéral
            return View();
        }

        // POST: CategoryController/Create (Enregistrement de la nouvelle catégorie)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Category category)
        {
            if (ModelState.IsValid)
            {
                CategRepository.Add(category);
                return RedirectToAction(nameof(Index));
            }

            ViewData["Categories"] = CategRepository.GetAll(); // Charger si validation échoue
            return View(category);
        }

        // GET: CategoryController/Edit/5 (Formulaire de modification)
        public ActionResult Edit(int id)
        {
            ViewData["Categories"] = CategRepository.GetAll(); // Charger le menu latéral
            var category = CategRepository.GetById(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        // POST: CategoryController/Edit/5 (Enregistrement de la modification)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Category category)
        {
            if (ModelState.IsValid)
            {
                CategRepository.Update(category);
                return RedirectToAction(nameof(Index));
            }

            ViewData["Categories"] = CategRepository.GetAll(); // Charger si validation échoue
            return View(category);
        }

        // GET: CategoryController/Delete/5 (Page de confirmation de suppression)
        public ActionResult Delete(int id)
        {
            ViewData["Categories"] = CategRepository.GetAll(); // Charger le menu latéral
            var category = CategRepository.GetById(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        // POST: CategoryController/Delete/5 (Suppression effective)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CategRepository.Delete(id);
            return RedirectToAction(nameof(Index));
        }
    }
}