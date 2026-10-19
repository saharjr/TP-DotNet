using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TP2.Models;
using TP2.Models.Repositories;
using TP2.ViewModels;

namespace TP2.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly UserManager<IdentityUser> userManager;
        private readonly AppDbContext context;
        private readonly ICategoryRepository categRepository;

        // Injection du Contexte DB et du Repository Catégories
        public AdminController(RoleManager<IdentityRole> roleManager,
                               UserManager<IdentityUser> userManager,
                               AppDbContext context,
                               ICategoryRepository categRepository)
        {
            this.roleManager = roleManager;
            this.userManager = userManager;
            this.context = context;
            this.categRepository = categRepository;
        }

        // --- DASHBOARD ANALYTICS ADMIN ---
        [HttpGet]
        public IActionResult Dashboard()
        {
            ViewData["Categories"] = categRepository.GetAll();

            // 1. Calcul des KPIs
            var totalRevenue = context.Orders.Sum(o => (float?)o.TotalAmount) ?? 0f;
            var totalOrders = context.Orders.Count();
            var totalProducts = context.Products.Count();
            var totalCategories = context.Categories.Count();

            // 2. Ventes réparties par catégorie
            var categoryData = context.OrderItems
                .Join(context.Products, item => item.ProductName, prod => prod.Name, (item, prod) => new { item, prod })
                .Join(context.Categories, combined => combined.prod.CategoryId, cat => cat.CategoryId, (combined, cat) => new {
                    CategoryName = cat.CategoryName,
                    Total = combined.item.Price * combined.item.Quantity
                })
                .GroupBy(x => x.CategoryName)
                .Select(g => new {
                    Category = g.Key,
                    TotalSales = g.Sum(x => x.Total)
                }).ToList();

            // 3. Ventes des 7 derniers jours
            var last7Days = Enumerable.Range(0, 7)
                .Select(i => DateTime.Now.Date.AddDays(-i))
                .OrderBy(d => d)
                .ToList();

            var recentOrders = context.Orders
                .Where(o => o.OrderDate >= DateTime.Now.Date.AddDays(-6))
                .ToList();

            var dailyData = last7Days.Select(date => new {
                Date = date.ToString("dd/MM"),
                Total = recentOrders.Where(o => o.OrderDate.Date == date).Sum(o => o.TotalAmount)
            }).ToList();

            var viewModel = new DashboardViewModel
            {
                TotalRevenue = totalRevenue,
                TotalOrders = totalOrders,
                TotalProducts = totalProducts,
                TotalCategories = totalCategories,
                CategoryNames = categoryData.Select(c => c.Category).ToList(),
                CategorySales = categoryData.Select(c => c.TotalSales).ToList(),
                DateLabels = dailyData.Select(d => d.Date).ToList(),
                DailyRevenue = dailyData.Select(d => d.Total).ToList()
            };

            return View(viewModel);
        }

        // --- CREER UN ROLE ---
        [HttpGet]
        public IActionResult CreateRole()
        {
            ViewData["Categories"] = categRepository.GetAll();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateRole(CreateRoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                IdentityRole role = new IdentityRole { Name = model.RoleName };
                IdentityResult result = await roleManager.CreateAsync(role);

                if (result.Succeeded)
                {
                    return RedirectToAction("ListRoles");
                }

                foreach (IdentityError error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            ViewData["Categories"] = categRepository.GetAll();
            return View(model);
        }

        // --- LISTER LES ROLES ---
        [HttpGet]
        public IActionResult ListRoles()
        {
            ViewData["Categories"] = categRepository.GetAll();
            var roles = roleManager.Roles;
            return View(roles);
        }

        // --- MODIFIER UN ROLE ---
        [HttpGet]
        public async Task<IActionResult> EditRole(string id)
        {
            ViewData["Categories"] = categRepository.GetAll();
            var role = await roleManager.FindByIdAsync(id);
            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id = {id} cannot be found";
                return View("NotFound");
            }

            var model = new EditRoleViewModel
            {
                Id = role.Id,
                RoleName = role.Name
            };

            foreach (var user in userManager.Users.ToList())
            {
                if (await userManager.IsInRoleAsync(user, role.Name))
                {
                    model.Users.Add(user.UserName);
                }
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditRole(EditRoleViewModel model)
        {
            var role = await roleManager.FindByIdAsync(model.Id);

            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id = {model.Id} cannot be found";
                return View("NotFound");
            }
            else
            {
                role.Name = model.RoleName;
                var result = await roleManager.UpdateAsync(role);

                if (result.Succeeded)
                {
                    return RedirectToAction("ListRoles");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

                ViewData["Categories"] = categRepository.GetAll();
                return View(model);
            }
        }

        // --- SUPPRIMER UN ROLE ---
        [HttpPost]
        public async Task<IActionResult> DeleteRole(string id)
        {
            var role = await roleManager.FindByIdAsync(id);

            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id = {id} cannot be found";
                return View("NotFound");
            }

            var result = await roleManager.DeleteAsync(role);

            if (result.Succeeded)
            {
                return RedirectToAction("ListRoles");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            ViewData["Categories"] = categRepository.GetAll();
            return View("ListRoles");
        }

        // --- GERER LES UTILISATEURS D'UN ROLE ---
        [HttpGet]
        public async Task<IActionResult> EditUsersInRole(string roleId)
        {
            ViewData["Categories"] = categRepository.GetAll();
            ViewBag.roleId = roleId;

            var role = await roleManager.FindByIdAsync(roleId);
            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id = {roleId} cannot be found";
                return View("NotFound");
            }

            var model = new List<UserRoleViewModel>();

            foreach (var user in userManager.Users.ToList())
            {
                var userRoleViewModel = new UserRoleViewModel
                {
                    UserId = user.Id,
                    UserName = user.UserName
                };

                if (await userManager.IsInRoleAsync(user, role.Name))
                {
                    userRoleViewModel.IsSelected = true;
                }
                else
                {
                    userRoleViewModel.IsSelected = false;
                }

                model.Add(userRoleViewModel);
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditUsersInRole(List<UserRoleViewModel> model, string roleId)
        {
            var role = await roleManager.FindByIdAsync(roleId);

            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id = {roleId} cannot be found";
                return View("NotFound");
            }

            for (int i = 0; i < model.Count; i++)
            {
                var user = await userManager.FindByIdAsync(model[i].UserId);
                IdentityResult result = null;

                if (model[i].IsSelected && !(await userManager.IsInRoleAsync(user, role.Name)))
                {
                    result = await userManager.AddToRoleAsync(user, role.Name);
                }
                else if (!model[i].IsSelected && await userManager.IsInRoleAsync(user, role.Name))
                {
                    result = await userManager.RemoveFromRoleAsync(user, role.Name);
                }
                else
                {
                    continue;
                }

                if (result.Succeeded)
                {
                    if (i < (model.Count - 1))
                        continue;
                    else
                        return RedirectToAction("EditRole", new { Id = roleId });
                }
            }

            return RedirectToAction("EditRole", new { Id = roleId });
        }
    }
}