using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Rotativa.AspNetCore;
using Stripe;
using Stripe.Checkout;
using TP2.Models;
using TP2.Models.Repositories;
using System.Text.Json;
using TP2.Models;

namespace TP2.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private readonly IOrderRepository orderRepository;
        private readonly ICategoryRepository categRepository;
        private readonly IProductRepository productRepository;
        private readonly UserManager<IdentityUser> userManager;

        public OrderController(IOrderRepository orderRepository,
                               ICategoryRepository categRepository,
                               IProductRepository productRepository,
                               UserManager<IdentityUser> userManager)
        {
            this.orderRepository = orderRepository;
            this.categRepository = categRepository;
            this.productRepository = productRepository;
            this.userManager = userManager;
        }

        // =========================================================================
        // 1. FLUX DE PAIEMENT STRIPE & CHECKOUT
        // =========================================================================

        // GET: /Order/Checkout (Étape 1 : Récapitulatif et saisie d'adresse)
        [HttpGet]
        public async Task<IActionResult> Checkout()
        {
            ViewData["Categories"] = categRepository.GetAll();

            var cart = HttpContext.Session.GetObject<List<PanierItem>>("Panier");
            if (cart == null || !cart.Any())
            {
                return RedirectToAction("Index", "Panier");
            }

            var user = await userManager.GetUserAsync(User);
            ViewBag.CustomerEmail = user?.Email ?? "";
            ViewBag.CartTotal = cart.Sum(i => (decimal)(i.Quantity * i.Product.Price));

            return View(cart);
        }

        // POST: /Order/CreatePaymentSession (Étape 2 : Enregistrement de la commande + Redirection Stripe)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePaymentSession(string address, string phone)
        {
            var cart = HttpContext.Session.GetObject<List<PanierItem>>("Panier");
            if (cart == null || !cart.Any())
            {
                return RedirectToAction("Index", "Panier");
            }

            var user = await userManager.GetUserAsync(User);

            // Construction sécurisée de la commande (évite les valeurs NULL qui provoquent la DbUpdateException)
            var order = new Order
            {
                UserId = user?.Id ?? "",
                CustomerName = user?.UserName ?? user?.Email ?? "Client",
                Address = string.IsNullOrWhiteSpace(address) ? "Adresse non spécifiée" : address,
                Phone = string.IsNullOrWhiteSpace(phone) ? "Non renseigné" : phone,
                OrderDate = DateTime.Now,
                TotalAmount = cart.Sum(i => i.Quantity * (decimal)i.Product.Price),
                PaymentStatus = "En attente de paiement",
                PaymentMethod = "Carte Bancaire (Stripe)",
                StripeSessionId = "", // Valeur par défaut
                OrderItems = cart.Select(i => new OrderItem
                {
                    ProductName = i.Product?.Name ?? "Produit",
                    Quantity = i.Quantity,
                    Price = (float)(i.Product?.Price ?? 0)
                }).ToList()
            };

            // Enregistrement de la commande initiale
            orderRepository.Add(order);

            // Préparation des articles pour l'interface sécurisée Stripe Checkout
            var domain = $"{Request.Scheme}://{Request.Host}";
            // Exemple avec un taux fixe : 1 EUR = 3.3 DT
            decimal tauxConversion = 3.3m;

            var lineItems = cart.Select(item => new SessionLineItemOptions
            {
                PriceData = new SessionLineItemPriceDataOptions
                {
                    // Conversion du prix DT en EUR, puis passage en centimes (* 100)
                    UnitAmount = (long)((item.Product.Price / (double)tauxConversion) * 100),
                    Currency = "eur",
                    ProductData = new SessionLineItemPriceDataProductDataOptions
                    {
                        Name = item.Product.Name ?? "Article"
                    }
                },
                Quantity = item.Quantity
            }).ToList();

            var options = new SessionCreateOptions
            {
                AllowedPaymentMethodTypes = new List<string> { "card" },
                LineItems = lineItems,
                Mode = "payment",
                CustomerEmail = user?.Email,
                SuccessUrl = $"{domain}/Order/PaymentSuccess?sessionId={{CHECKOUT_SESSION_ID}}&orderId={order.Id}",
                CancelUrl = $"{domain}/Order/PaymentCancel?orderId={order.Id}"
            };

            var service = new SessionService();
            Session session = await service.CreateAsync(options);

            // Mise à jour de la commande avec l'identifiant de la session Stripe
            order.StripeSessionId = session.Id;
            orderRepository.Update(order);

            // Redirection de l'utilisateur vers Stripe
            return Redirect(session.Url);
        }

        // GET: /Order/PaymentSuccess (Étape 3 : Validation après paiement réussi)
        [HttpGet]
        public IActionResult PaymentSuccess(string sessionId, int orderId)
        {
            ViewData["Categories"] = categRepository.GetAll();

            var service = new SessionService();
            Session session = service.Get(sessionId);

            if (session.PaymentStatus == "paid")
            {
                var order = orderRepository.GetById(orderId);
                if (order != null && order.PaymentStatus != "Payée")
                {
                    // Validation du statut
                    order.PaymentStatus = "Payée";
                    orderRepository.Update(order);

                    // Décrémentation du stock des produits
                    var cart = HttpContext.Session.GetObject<List<PanierItem>>("Panier");
                    if (cart != null)
                    {
                        foreach (var item in cart)
                        {
                            var prod = productRepository.GetById(item.Product.ProductId);
                            if (prod != null)
                            {
                                prod.QteStock -= item.Quantity;
                                if (prod.QteStock < 0) prod.QteStock = 0;
                                productRepository.Update(prod);
                            }
                        }
                    }

                    // Vider le panier
                    HttpContext.Session.Remove("Panier");
                }

                return View(order);
            }

            return RedirectToAction("PaymentCancel", new { orderId });
        }

        // GET: /Order/PaymentCancel (Paiement annulé ou interrompu)
        [HttpGet]
        public IActionResult PaymentCancel(int orderId)
        {
            ViewData["Categories"] = categRepository.GetAll();

            var order = orderRepository.GetById(orderId);
            if (order != null && order.PaymentStatus == "En attente de paiement")
            {
                order.PaymentStatus = "Annulée";
                orderRepository.Update(order);
            }
            return View();
        }

        // =========================================================================
        // 2. CONSULTATION ET GESTION DES COMMANDES
        // =========================================================================

        // GET: /Order/MesCommandes (Historique du client connecté)
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

    public static class SessionExtensions
    {
        public static void SetObject<T>(this ISession session, string key, T value)
        {
            session.SetString(key, JsonSerializer.Serialize(value));
        }

        public static T? GetObject<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            return value == null ? default : JsonSerializer.Deserialize<T>(value);
        }
    }
}

