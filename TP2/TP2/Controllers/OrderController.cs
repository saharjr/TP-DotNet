using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TP2.Models.Repositories;
using Rotativa.AspNetCore;

namespace TP2.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private readonly IOrderRepository orderRepository;
        private readonly ICategoryRepository categRepository;
        private readonly UserManager<IdentityUser> userManager;

        public OrderController(IOrderRepository orderRepository,
                               ICategoryRepository categRepository,
                               UserManager<IdentityUser> userManager)
        {
            this.orderRepository = orderRepository;
            this.categRepository = categRepository;
            this.userManager = userManager;
        }

        // GET: /Order/MesCommandes (Historique client connecté)
        [HttpGet]
        public async Task<IActionResult> MesCommandes()
        {
            ViewData["Categories"] = categRepository.GetAll();

            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var orders = orderRepository.GetOrdersByUserId(user.Id);
            return View(orders);
        }

        // GET: /Order/Details/5 (Détail d'une commande)
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            ViewData["Categories"] = categRepository.GetAll();

            var order = orderRepository.GetById(id);
            if (order == null)
            {
                return NotFound();
            }

            var user = await userManager.GetUserAsync(User);
            var isAdminOrManager = User.IsInRole("Admin") || User.IsInRole("Manager");

            // Sécurité : Vérifier que la commande appartient bien à l'utilisateur ou à l'Admin
            if (order.UserId != user?.Id && !isAdminOrManager)
            {
                return Forbid();
            }

            return View(order);
        }

        // GET: /Order/AllOrders (Toutes les commandes - Admin/Manager)
        [Authorize(Roles = "Admin, Manager")]
        [HttpGet]
        public IActionResult AllOrders()
        {
            ViewData["Categories"] = categRepository.GetAll();
            var orders = orderRepository.GetAllOrders();
            return View(orders);
        }

        // GET: /Order/DownloadInvoice/5
        [Authorize]
        public async Task<IActionResult> DownloadInvoice(int id)
        {
            var order = orderRepository.GetById(id);
            if (order == null) return NotFound();

            // Sécurité : Seul le client propriétaire de la commande ou un Admin/Manager peut télécharger la facture
            var currentUser = await userManager.GetUserAsync(User);
            if (!User.IsInRole("Admin") && !User.IsInRole("Manager") && order.UserId != currentUser?.Id)
            {
                return Forbid();
            }

            return new ViewAsPdf("InvoicePdf", order)
            {
                FileName = $"Facture_Commande_{order.Id}.pdf",
                PageSize = Rotativa.AspNetCore.Options.Size.A4,
                PageOrientation = Rotativa.AspNetCore.Options.Orientation.Portrait
            };
        }


        // GET: /Order/Invoice/5
        [Authorize]
        public async Task<IActionResult> Invoice(int id)
        {
            var order = orderRepository.GetById(id);
            if (order == null) return NotFound();

            var currentUser = await userManager.GetUserAsync(User);
            if (!User.IsInRole("Admin") && !User.IsInRole("Manager") && order.UserId != currentUser?.Id)
            {
                return Forbid();
            }

            return View("InvoicePdf", order);
        }
    }
}